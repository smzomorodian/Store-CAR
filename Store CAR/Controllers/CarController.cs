using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Carproject.Model;
using Domain.Model;
using static Domain.Model.Car;
using Infrustructure.Context;
using Application.Command.Car;
using Application.DTO.CarDTO;

namespace Carproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly CARdbcontext _context;
        private readonly IMapper _mapper;

        public CarController(CARdbcontext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // اضافه کردن خودرو
        [HttpPost]
        public async Task<ActionResult<CarDto>> PostCar([FromBody] CommandCar commandCar)
        {
            // بررسی داده‌های ورودی
            if (commandCar == null) return BadRequest(new { message = "Invalid data." });

            // بررسی اینکه آیا دسته‌بندی معتبر است
            if (!await _context.CarCategories.AnyAsync(c => c.Id == commandCar.CategoryId))
                return BadRequest(new { message = "Invalid CategoryId" });

            // نگاشت CommandCar به Car با استفاده از AutoMapper
            var car = _mapper.Map<Car>(commandCar);

            // اضافه کردن خودرو به پایگاه داده
            _context.Cars.Add(car);
            await _context.SaveChangesAsync();

            // لود کردن دسته‌بندی
            await _context.Entry(car).Reference(c => c.Category).LoadAsync();

            // نگاشت خودرو به CarDto برای پاسخ‌دهی
            var carDto = _mapper.Map<CarDto>(car);

            // برگرداندن خودرو جدید با استفاده از CreatedAtAction
            return CreatedAtAction(nameof(GetCar), new { id = car.Id }, carDto);
        }



        // ویرایش خودرو
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(int id, [FromBody] CarDto updateCarDto)
        {
            if (id <= 0 || updateCarDto == null)
                return BadRequest(new { message = "Invalid data." });

            var existingCar = await _context.Cars.FindAsync(id);
            if (existingCar == null) return NotFound(new { message = "Car not found." });

            _mapper.Map(updateCarDto, existingCar);
            await _context.SaveChangesAsync();

            return Ok(_mapper.Map<CarDto>(existingCar));
        }

        // دریافت خودرو بر اساس ID
        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(Guid id)
        {
            var car = await _context.Cars
                                    .Include(c => c.Category)
                                    .FirstOrDefaultAsync(c => c.Id == id);

            if (car == null) return NotFound();

            return Ok(_mapper.Map<CarDto>(car));
        }

        // حذف خودرو
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null) return NotFound();

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Delete successful" });
        }

        // فیلتر خودروها
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<CarDto>>> FilterCars(
            [FromQuery] string? brand,
            [FromQuery] string? model,
            [FromQuery] int? year,
            [FromQuery] string? color,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] int? categoryId,
            [FromQuery] CarStatus? status)
        {
            if (minPrice > maxPrice)
                return BadRequest(new { message = "حداقل قیمت نمی‌تواند بیشتر از حداکثر قیمت باشد." });

            var query = _context.Cars.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(brand))
                query = query.Where(c => c.Brand.Contains(brand));

            if (!string.IsNullOrEmpty(model))
                query = query.Where(c => c.Model.Contains(model));

            if (year.HasValue)
                query = query.Where(c => c.Year == year.Value);

            if (!string.IsNullOrEmpty(color))
                query = query.Where(c => c.Color.Contains(color));

            if (minPrice.HasValue)
                query = query.Where(c => c.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(c => c.Price <= maxPrice.Value);

            if (categoryId.HasValue)
                query = query.Where(c => c.CategoryId == categoryId.Value);

            if (status.HasValue)
                query = query.Where(c => c.Status == status.Value);

            var cars = await query.ToListAsync();
            return Ok(_mapper.Map<IEnumerable<CarDto>>(cars));
        }
    }
}

