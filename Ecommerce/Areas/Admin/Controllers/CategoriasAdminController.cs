using AutoMapper;
using Ecommerce.Dtos;
using Ecommerce.Models;
using Ecommerce.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Para DbUpdateException

namespace Ecommerce.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriasAdminController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CategoriasAdminController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        // GET: /Admin/CategoriasAdmin
        public async Task<IActionResult> Index()
        {
            var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();
            return View(categorias);
        }

        // GET: Upsert (Create / Edit)
        public async Task<IActionResult> Upsert(int? id)
        {
            var dto = new CategoriaUpsertDto();

            if (id == null || id == 0)
            {
                // Crear
                return View(dto);
            }
            else
            {
                // Editar
                var categoria = await _unitOfWork.CategoriaRepository.GetByIdAsync(id.Value);
                if (categoria == null) return NotFound();

                dto = _mapper.Map<CategoriaUpsertDto>(categoria);
                return View(dto);
            }
        }

        // POST: Upsert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(CategoriaUpsertDto dto)
        {
            if (ModelState.IsValid)
            {
                if (dto.CategoriaId == 0)
                {
                    // Crear
                    var categoria = _mapper.Map<Categoria>(dto);
                    await _unitOfWork.CategoriaRepository.AddAsync(categoria);
                }
                else
                {
                    // Actualizar
                    var categoria = await _unitOfWork.CategoriaRepository.GetByIdAsync(dto.CategoriaId);
                    if (categoria == null) return NotFound();

                    _mapper.Map(dto, categoria);
                    _unitOfWork.CategoriaRepository.Update(categoria);
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        // DELETE (API)
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var categoria = await _unitOfWork.CategoriaRepository.GetByIdAsync(id);
            if (categoria == null) return Json(new { success = false, message = "Categoría no encontrada." });

            try
            {
                _unitOfWork.CategoriaRepository.Remove(categoria);
                await _unitOfWork.SaveAsync();
                return Json(new { success = true, message = "Categoría eliminada correctamente." });
            }
            catch (DbUpdateException)
            {
                // Esto ocurre si hay productos usando esta categoría
                return Json(new { success = false, message = "No se puede eliminar: Hay productos asociados a esta categoría." });
            }
        }
    }
}