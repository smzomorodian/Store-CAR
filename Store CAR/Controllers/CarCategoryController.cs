
using Application.DTO;
using Domain.Model;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Carproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarCategoryController : ControllerBase
    {
        private readonly CARdbcontext _context;
        private readonly IRepository<CarCategory> _genericRepository;
        

        public CarCategoryController(CARdbcontext context , IRepository<CarCategory> genericRepository)
        {
            _context = context;
            _genericRepository = genericRepository;
        }

        // ✅ دریافت لیست دسته‌بندی‌ها
        [HttpGet]
        [Authorize(Roles = "moder, seller")] // اضافه کردن محدودیت دسترسی
        public async Task<ActionResult<IEnumerable<CarCategory>>> GetCategories()
        {
            return await _context.CarCategories.ToListAsync();
        }

        // ✅ دریافت یک دسته‌بندی خاص
        [HttpGet("{id}")]
        [Authorize(Roles = "moder, seller")] // اضافه کردن محدودیت دسترسی
        public async Task<ActionResult<CarCategory>> GetCategory(int id)
        {
            var category = await _context.CarCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }
            return category;
        }

        // ✅ افزودن دسته‌بندی جدید
        [HttpPost]
        [Authorize(Roles = "moder, seller")] // اضافه کردن محدودیت دسترسی
        public async Task<ActionResult<CarCategory>> AddCategory([FromBody] CarCategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var category = new CarCategory
            {
                Name = categoryDto.Name
            };

            //_context.CarCategories.Add(category);
            _genericRepository.AddAsync(category);
            await _genericRepository.SavechangeAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // ✅ حذف دسته‌بندی
        [HttpDelete("{id}")]
        [Authorize(Roles = "moder, seller")] // اضافه کردن محدودیت دسترسی
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.CarCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            //_context.CarCategories.Remove(category);
            _genericRepository.Delete(category);
            await _genericRepository.SavechangeAsync();

            return NoContent();
        }
    }
}
