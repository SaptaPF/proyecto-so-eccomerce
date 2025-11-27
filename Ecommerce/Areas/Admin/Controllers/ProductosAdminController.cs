using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Persistence; // <-- NECESARIO para ApplicationDbContext
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductosAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ApplicationDbContext _context; 

        public ProductosAdminController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IWebHostEnvironment hostEnvironment,
            ApplicationDbContext context) 
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
            _context = context;
        }

        // GET: /Admin/ProductosAdmin
        public async Task<IActionResult> Index()
        {
            var productos = await _unitOfWork.ProductoRepository.GetAllAsync(includeProperties: "ProductoCategorias.Categoria");
            return View(productos);
        }

        // GET: Upsert
        public async Task<IActionResult> Upsert(int? id)
        {
            var viewModel = new ProductoUpsertViewModel();
            var todasCategorias = await _unitOfWork.CategoriaRepository.GetAllAsync();

            if (id == null || id == 0)
            {
                // CREAR
                viewModel.CategoriasDisponibles = todasCategorias.Select(c => new CategoriaCheckboxDto
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    IsChecked = false
                }).ToList();
            }
            else
            {
                // EDITAR
                var producto = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(
                    p => p.ProductoId == id,
                    includeProperties: "ProductoCategorias"
                );

                if (producto == null) return NotFound();

                viewModel.Producto = _mapper.Map<ProductoUpsertDto>(producto);

                viewModel.CategoriasDisponibles = todasCategorias.Select(c => new CategoriaCheckboxDto
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    IsChecked = viewModel.Producto.CategoriaIds.Contains(c.CategoriaId)
                }).ToList();
            }

            return View(viewModel);
        }

        // POST: Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductoUpsertViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                string rutaPrincipal = _hostEnvironment.WebRootPath;
                var archivos = HttpContext.Request.Form.Files;

                var productoEntidad = _mapper.Map<Producto>(viewModel.Producto);

                // --- LOGICA DE EDICIÓN ---
                if (viewModel.Producto.ProductoId != 0)
                {
                    // Obtenemos el producto viejo de la BD
                    var productoBd = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(p => p.ProductoId == viewModel.Producto.ProductoId);

                    if (productoBd != null)
                    {
                        // Copiamos la URL vieja
                        productoEntidad.ImagenUrl = productoBd.ImagenUrl;

                        // --- ¡¡LA CORRECCIÓN QUE NECESITAS!! ---
                        // Le decimos a EF Core: "Deja de rastrear este objeto viejo".
                        // Así evitamos el conflicto de "instance already being tracked".
                        _context.Entry(productoBd).State = EntityState.Detached;
                        // ---------------------------------------
                    }
                }

                if (archivos.Count > 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString();
                    // Usamos Path.Combine para compatibilidad Windows/Linux
                    var subidas = Path.Combine(rutaPrincipal, "imagenes", "productos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    if (!Directory.Exists(subidas))
                    {
                        Directory.CreateDirectory(subidas);
                    }

                    // Borrar imagen anterior si existe
                    if (!string.IsNullOrEmpty(productoEntidad.ImagenUrl))
                    {
                        var rutaRelativa = productoEntidad.ImagenUrl.TrimStart('/', '\\');

                        // Normalizar separadores para el sistema operativo actual
                        rutaRelativa = rutaRelativa.Replace("/", Path.DirectorySeparatorChar.ToString())
                                                   .Replace("\\", Path.DirectorySeparatorChar.ToString());

                        var rutaImagenAnterior = Path.Combine(rutaPrincipal, rutaRelativa);

                        if (System.IO.File.Exists(rutaImagenAnterior))
                        {
                            System.IO.File.Delete(rutaImagenAnterior);
                        }
                    }

                    // Guardar la nueva imagen
                    using (var fileStreams = new FileStream(Path.Combine(subidas, nombreArchivo + extension), FileMode.Create))
                    {
                        archivos[0].CopyTo(fileStreams);
                    }

                    // Guardar URL en formato web (con /)
                    productoEntidad.ImagenUrl = "/imagenes/productos/" + nombreArchivo + extension;
                }

                // --- GUARDAR EN BD ---
                if (viewModel.Producto.ProductoId == 0)
                {
                    await _unitOfWork.ProductoRepository.AddAsync(productoEntidad);
                    await _unitOfWork.SaveAsync();
                    await UpdateProductoCategoriasAsync(productoEntidad.ProductoId, viewModel.Producto.CategoriaIds);
                }
                else
                {
                    // Ahora el Update funcionará porque ya soltamos (Detached) el objeto viejo
                    _unitOfWork.ProductoRepository.Update(productoEntidad);
                    await UpdateProductoCategoriasAsync(productoEntidad.ProductoId, viewModel.Producto.CategoriaIds);
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            // Si falla el modelo, recargamos las categorías
            var todasCategorias = await _unitOfWork.CategoriaRepository.GetAllAsync();
            viewModel.CategoriasDisponibles = todasCategorias.Select(c => new CategoriaCheckboxDto
            {
                CategoriaId = c.CategoriaId,
                Nombre = c.Nombre,
                IsChecked = viewModel.Producto.CategoriaIds.Contains(c.CategoriaId)
            }).ToList();

            return View(viewModel);
        }

        private async Task UpdateProductoCategoriasAsync(int productoId, List<int> categoriaIdsSeleccionadas)
        {
            var categoriasActuales = (await _unitOfWork.ProductoCategoriaRepository
                .GetAllAsync(pc => pc.ProductoId == productoId)).ToList();

            var borrar = categoriasActuales.Where(pc => !categoriaIdsSeleccionadas.Contains(pc.CategoriaId));
            _unitOfWork.ProductoCategoriaRepository.RemoveRange(borrar);

            var idsActuales = categoriasActuales.Select(pc => pc.CategoriaId);
            var anadir = categoriaIdsSeleccionadas.Where(id => !idsActuales.Contains(id));
            foreach (var id in anadir)
            {
                await _unitOfWork.ProductoCategoriaRepository.AddAsync(new ProductoCategoria { ProductoId = productoId, CategoriaId = id });
            }
        }

        // DELETE: /Admin/ProductosAdmin/Delete/5
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var producto = await _unitOfWork.ProductoRepository.GetByIdAsync(id);
                if (producto == null)
                {
                    return Json(new { success = false, message = "Error: Producto no encontrado." });
                }

                // Borrar imagen del servidor
                if (!string.IsNullOrEmpty(producto.ImagenUrl))
                {
                    string rutaPrincipal = _hostEnvironment.WebRootPath;
                    var rutaRelativa = producto.ImagenUrl.TrimStart('/', '\\');
                    rutaRelativa = rutaRelativa.Replace("/", Path.DirectorySeparatorChar.ToString())
                                               .Replace("\\", Path.DirectorySeparatorChar.ToString());

                    var rutaImagen = Path.Combine(rutaPrincipal, rutaRelativa);

                    if (System.IO.File.Exists(rutaImagen))
                    {
                        System.IO.File.Delete(rutaImagen);
                    }
                }

                var categorias = await _unitOfWork.ProductoCategoriaRepository.GetAllAsync(pc => pc.ProductoId == id);
                if (categorias != null)
                {
                    _unitOfWork.ProductoCategoriaRepository.RemoveRange(categorias);
                }

                _unitOfWork.ProductoRepository.Remove(producto);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Producto eliminado exitosamente." });
            }
            catch (DbUpdateException)
            {
                return Json(new { success = false, message = "No se puede eliminar: El producto es parte de pedidos existentes." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Error inesperado: " + ex.Message });
            }
        }
    }
}