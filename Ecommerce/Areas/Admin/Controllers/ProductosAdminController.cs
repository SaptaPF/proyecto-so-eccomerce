using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting; // Necesario para IWebHostEnvironment
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO; // Necesario para manejar archivos
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
        private readonly IWebHostEnvironment _hostEnvironment; // <-- Para saber dónde guardar las imágenes

        public ProductosAdminController(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment hostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hostEnvironment = hostEnvironment;
        }

        // GET: /Admin/ProductosAdmin
        public async Task<IActionResult> Index()
        {
            // Incluimos las categorías para mostrarlas en la tabla si quisieras
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
                // --- CREAR ---
                viewModel.CategoriasDisponibles = todasCategorias.Select(c => new CategoriaCheckboxDto
                {
                    CategoriaId = c.CategoriaId,
                    Nombre = c.Nombre,
                    IsChecked = false
                }).ToList();
            }
            else
            {
                // --- EDITAR ---
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
                    var productoBd = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(p => p.ProductoId == viewModel.Producto.ProductoId);
                    if (productoBd != null)
                    {
                        productoEntidad.ImagenUrl = productoBd.ImagenUrl;
                    }
                }

                if (archivos.Count > 0)
                {
                    string nombreArchivo = Guid.NewGuid().ToString();

                    // CAMBIO 1: Usar Path.Combine con argumentos separados para que funcione en Linux y Windows
                    var subidas = Path.Combine(rutaPrincipal, "imagenes", "productos");
                    var extension = Path.GetExtension(archivos[0].FileName);

                    if (!Directory.Exists(subidas))
                    {
                        Directory.CreateDirectory(subidas);
                    }

                    // Borrar imagen anterior
                    if (!string.IsNullOrEmpty(productoEntidad.ImagenUrl))
                    {
                        // CAMBIO 2: Limpiar la ruta para eliminar barras invertidas viejas si existen
                        var rutaImagenAnterior = Path.Combine(rutaPrincipal, productoEntidad.ImagenUrl.TrimStart('/', '\\').Replace("/", Path.DirectorySeparatorChar.ToString()));

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

                    // CAMBIO 3: Guardar la URL con formato WEB (Barras normales /)
                    // Esto asegura que el navegador la entienda siempre.
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
                    _unitOfWork.ProductoRepository.Update(productoEntidad);
                    await UpdateProductoCategoriasAsync(productoEntidad.ProductoId, viewModel.Producto.CategoriaIds);
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }

            // ... (Resto del código de recarga si falla el modelo) ...
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

            // Borrar las que ya no están
            var borrar = categoriasActuales.Where(pc => !categoriaIdsSeleccionadas.Contains(pc.CategoriaId));
            _unitOfWork.ProductoCategoriaRepository.RemoveRange(borrar);

            // Añadir las nuevas
            var idsActuales = categoriasActuales.Select(pc => pc.CategoriaId);
            var anadir = categoriaIdsSeleccionadas.Where(id => !idsActuales.Contains(id));
            foreach (var id in anadir)
            {
                await _unitOfWork.ProductoCategoriaRepository.AddAsync(new ProductoCategoria { ProductoId = productoId, CategoriaId = id });
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var producto = await _unitOfWork.ProductoRepository.GetByIdAsync(id);
            if (producto == null)
            {
                return Json(new { success = false, message = "Error al borrar." });
            }

            // Borrar imagen del servidor si existe
            if (!string.IsNullOrEmpty(producto.ImagenUrl))
            {
                string rutaPrincipal = _hostEnvironment.WebRootPath;
                var rutaImagen = Path.Combine(rutaPrincipal, producto.ImagenUrl.TrimStart('\\'));
                if (System.IO.File.Exists(rutaImagen))
                {
                    System.IO.File.Delete(rutaImagen);
                }
            }

            // Borrar relaciones de categorías
            var categorias = await _unitOfWork.ProductoCategoriaRepository.GetAllAsync(pc => pc.ProductoId == id);
            _unitOfWork.ProductoCategoriaRepository.RemoveRange(categorias);

            _unitOfWork.ProductoRepository.Remove(producto);
            await _unitOfWork.SaveAsync();

            return Json(new { success = true, message = "Producto eliminado." });
        }
    }
}