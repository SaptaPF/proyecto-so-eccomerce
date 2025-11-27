using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models; // Para ErrorViewModel
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public HomeController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: /Home/Categorias
        public async Task<IActionResult> Categorias()
        {
            // 1. Obtener todas las categorías
            // Incluimos "ProductoCategorias" para poder contar cuántos productos hay en cada una
            var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync(
                includeProperties: "ProductoCategorias"
            );

            // 2. Mapear a DTO
            var dtos = _mapper.Map<IEnumerable<Ecommerce.Dtos.CategoriaDto>>(categorias);

            return View(dtos);
        }

        // GET: /Home/Index
        public async Task<IActionResult> Index()
        {
            var homeViewModel = new HomeViewModel();

            // --- 1. Lógica para "Producto del Mes" (Hero) ---
            // ESTRATEGIA: El más vendido (contando cantidad en DetallesPedido)

            // Obtenemos productos con sus detalles de pedido y reseñas
            var todosProductos = await _unitOfWork.ProductoRepository.GetAllAsync(
                includeProperties: "DetallesPedido,Resenas,ProductoCategorias.Categoria"
            );

            // Ordenamos en memoria por la suma de cantidades vendidas
            var productoMesEntidad = todosProductos
                .OrderByDescending(p => p.DetallesPedido.Sum(dp => dp.Cantidad))
                .FirstOrDefault();

            // Fallback: Si no hay ventas aún, mostramos el producto más caro (Premium)
            if (productoMesEntidad == null || !productoMesEntidad.DetallesPedido.Any())
            {
                productoMesEntidad = todosProductos.OrderByDescending(p => p.Precio).FirstOrDefault();
            }

            // Mapeamos si encontramos algo
            if (productoMesEntidad != null)
            {
                homeViewModel.ProductoDelMes = _mapper.Map<ProductoDto>(productoMesEntidad);
            }


            // --- 2. Lógica para "Productos Destacados" ---
            // Los 4 productos con más reseñas (populares)
            var productosDestacadosEntidades = await _unitOfWork.ProductoRepository.GetAllAsync(
                orderBy: q => q.OrderByDescending(p => p.Resenas.Count),
                includeProperties: "ProductoCategorias.Categoria,Resenas"
            );
            homeViewModel.ProductosDestacados = _mapper.Map<IEnumerable<ProductoDto>>(productosDestacadosEntidades.Take(4));


            // --- 3. Lógica para "Categorías Populares" ---
            // Las 6 categorías con más productos
            var categoriasPopularesEntidades = await _unitOfWork.CategoriaRepository.GetAllAsync(
                orderBy: q => q.OrderByDescending(p => p.ProductoCategorias.Count),
                includeProperties: "ProductoCategorias"
            );
            homeViewModel.CategoriasPopulares = _mapper.Map<IEnumerable<CategoriaDto>>(categoriasPopularesEntidades.Take(6));


            // --- 4. Lógica para "Reseñas Recientes" ---
            // Las 3 reseñas más recientes con 4 o más estrellas
            var resenasRecientesEntidades = await _unitOfWork.ResenaRepository.GetAllAsync(
                filter: r => r.Rating >= 4,
                orderBy: q => q.OrderByDescending(r => r.Fecha),
                includeProperties: "Usuario" // ¡IMPORTANTE: Incluir Usuario para el nombre/avatar!
            );
            homeViewModel.ResenasRecientes = _mapper.Map<IEnumerable<ResenaDto>>(resenasRecientesEntidades.Take(3));


            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}