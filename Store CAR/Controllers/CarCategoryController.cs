
using Application.DTO;
using Domain.Model;
using Infrustruction.Context;
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

        public CarCategoryController(CARdbcontext context)
        {
            _context = context;
        }

        // ✅ دریافت لیست دسته‌بندی‌ها
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarCategory>>> GetCategories()
        {
            return await _context.CarCategories.ToListAsync();
        }

        // ✅ دریافت یک دسته‌بندی خاص
        [HttpGet("{id}")]
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

            _context.CarCategories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
        }

        // ✅ حذف دسته‌بندی
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.CarCategories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found" });
            }

            _context.CarCategories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
