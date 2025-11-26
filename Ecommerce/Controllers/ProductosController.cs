using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Ecommerce.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    public class ProductosController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        public ProductosController(IUnitOfWork unitOfWork, IMapper mapper, IProductoService productoService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _productoService = productoService;
        }

        // GET: /Productos
        // GET: /Productos?busqueda=teclado
        // GET: /Productos?categoriaId=5
        public async Task<IActionResult> Index(string? busqueda, int? categoriaId)
        {
            // 1. Obtener TODOS los productos
            // Es vital incluir 'ProductoCategorias' para poder filtrar
            var productosEntidad = await _unitOfWork.ProductoRepository.GetAllAsync(
                includeProperties: "ProductoCategorias.Categoria,Resenas"
            );

            // 2. FILTRO: Por Búsqueda (Texto)
            if (!string.IsNullOrEmpty(busqueda))
            {
                productosEntidad = productosEntidad.Where(p =>
                    p.Nombre.Contains(busqueda, StringComparison.OrdinalIgnoreCase) ||
                    p.Descripcion.Contains(busqueda, StringComparison.OrdinalIgnoreCase)
                );
                ViewData["BusquedaActual"] = busqueda;
                ViewData["Title"] = $"Resultados para '{busqueda}'";
            }

            // 3. FILTRO: Por Categoría (ID)
            // ¡AQUÍ ES DONDE OCURRE LA MAGIA DEL FILTRO DE CATEGORÍA!
            if (categoriaId.HasValue && categoriaId.Value > 0)
            {
                productosEntidad = productosEntidad.Where(p =>
                    p.ProductoCategorias.Any(pc => pc.CategoriaId == categoriaId.Value)
                );
                ViewData["Title"] = "Productos Filtrados";
            }

            // 4. Si no hay filtros, título por defecto
            if (string.IsNullOrEmpty(busqueda) && (!categoriaId.HasValue || categoriaId.Value == 0))
            {
                ViewData["Title"] = "Catálogo Completo";
            }

            // 5. Mapear a DTO y enviar a la vista
            var listaProductos = _mapper.Map<IEnumerable<ProductoDto>>(productosEntidad);

            return View(listaProductos);
        }

        // GET: /Productos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // 1. Incluimos "Resenas.Usuario" para mostrar el nombre de quien comentó
            var producto = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(
                p => p.ProductoId == id,
                includeProperties: "ProductoCategorias.Categoria,Resenas.Usuario"
            );

            if (producto == null) return NotFound();

            var dto = _mapper.Map<ProductoDto>(producto);
            return View(dto);
        }
        // POST: /Productos/AddReview
        [HttpPost]
        [Authorize] // Solo usuarios logueados pueden comentar
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddReview(ResenaCreateDto reseñaDto)
        {
            if (!ModelState.IsValid)
            {
                // Si falla, volvemos a mostrar los detalles (con los errores)
                return RedirectToAction(nameof(Details), new { id = reseñaDto.ProductoId });
            }

            try
            {
                var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                await _productoService.AddResenaAsync(usuarioId, reseñaDto.ProductoId, reseñaDto.Rating, reseñaDto.Comentario);

                TempData["Success"] = "¡Gracias por tu opinión!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message; // Ej: "Ya has valorado este producto"
            }

            return RedirectToAction(nameof(Details), new { id = reseñaDto.ProductoId });
        }
    }
}