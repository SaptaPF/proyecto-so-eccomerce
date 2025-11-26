using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models; // Para ErrorViewModel
using Ecommerce.Repository.Interfaces; // O Ecommerce.Repository (donde esté IUnitOfWork)
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;

namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        // 1. Inyectamos IUnitOfWork y IMapper
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

            // 2. Mapear a DTO (Reusamos el CategoriaDto que ya tenemos)
            var dtos = _mapper.Map<IEnumerable<Ecommerce.Dtos.CategoriaDto>>(categorias);

            return View(dtos);
        }
        // 2. La acción Index ahora es asíncrona y construye el ViewModel
        public async Task<IActionResult> Index()
        {
            // Creamos una sola instancia del ViewModel que vamos a llenar
            var homeViewModel = new HomeViewModel();

            // --- Lógica para "Producto del Mes" (Hero) ---
            // 1. Usamos GetAllAsync para poder ordenar
            var productosOrdenados = await _unitOfWork.ProductoRepository.GetAllAsync(
                filter: p => p.Resenas.Any(), // Solo productos que tengan reseñas
                orderBy: q => q.OrderByDescending(p => p.Resenas.Average(r => r.Rating)), // Ordenar por rating
                includeProperties: "ProductoCategorias.Categoria,Resenas"
            );

            // 2. Y luego tomamos el primero de la lista
            var productoMesEntidad = productosOrdenados.FirstOrDefault();

            // Fallback: Si ningún producto tiene reseñas, toma el primero que encuentre
            if (productoMesEntidad == null)
            {
                productoMesEntidad = await _unitOfWork.ProductoRepository.GetFirstOrDefaultAsync(
                    filter: p => true, // Cualquier producto
                    includeProperties: "ProductoCategorias.Categoria,Resenas"
                );
            }
            homeViewModel.ProductoDelMes = _mapper.Map<ProductoDto>(productoMesEntidad);


            // --- Lógica para "Productos Destacados" ---
            var productosDestacadosEntidades = await _unitOfWork.ProductoRepository.GetAllAsync(
                orderBy: q => q.OrderByDescending(p => p.Resenas.Count), // Ordenar por # de reseñas
                includeProperties: "ProductoCategorias.Categoria,Resenas"
            );
            homeViewModel.ProductosDestacados = _mapper.Map<IEnumerable<ProductoDto>>(productosDestacadosEntidades.Take(4));


            // --- Lógica para "Categorías Populares" ---
            var categoriasPopularesEntidades = await _unitOfWork.CategoriaRepository.GetAllAsync(
                orderBy: q => q.OrderByDescending(p => p.ProductoCategorias.Count), // Ordenar por # de productos
                includeProperties: "ProductoCategorias"
            );
            homeViewModel.CategoriasPopulares = _mapper.Map<IEnumerable<CategoriaDto>>(categoriasPopularesEntidades.Take(6));


            // --- Lógica para "Reseñas Recientes" (¡Corregida!) ---
            var resenasRecientesEntidades = await _unitOfWork.ResenaRepository.GetAllAsync(
                filter: r => r.Rating >= 4, // Solo reseñas buenas (4 o 5 estrellas)
                orderBy: q => q.OrderByDescending(r => r.Fecha), // Las más nuevas primero
                includeProperties: "Usuario" // <-- CRÍTICO: Para que ResenaProfile funcione
            );
            homeViewModel.ResenasRecientes = _mapper.Map<IEnumerable<ResenaDto>>(resenasRecientesEntidades.Take(3));


            // --- Devolvemos el ViewModel completo ---
            // (Se eliminó la lógica duplicada y el segundo 'return View')
            return View(homeViewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            // Asumiendo que tienes un ErrorViewModel en Ecommerce.Models
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}