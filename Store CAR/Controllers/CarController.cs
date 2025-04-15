using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Carproject.Model;
using Domain.Model;
using static Domain.Model.Car;
using Infrustructure.Context;
using Application.Command.Car;  
using Application.DTO.CarDTO;
using System.Formats.Tar;
using System.Text.RegularExpressions;

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


            // بررسی فرمت VIN با استفاده از Regex
            string vinPattern = "^[A-HJ-NPR-Z0-9]{17}$";
            if (!Regex.IsMatch(commandCar.VIN?.ToUpper() ?? "", vinPattern))
                return BadRequest(new { message = "VIN must be exactly 17 characters, using only A–Z (except I, O, Q) and digits." });

            // بررسی اینکه آیا دسته‌بندی معتبر است
            if (!await _context.CarCategories.AnyAsync(c => c.Id == commandCar.CategoryId))
                return BadRequest(new { message = "Invalid CategoryId" });

            // نگاشت CommandCar به Car با استفاده از AutoMapper
            var car = _mapper.Map<Car>(commandCar);

            // تبدیل VIN به حروف بزرگ قبل از ذخیره
            car.GetType().GetProperty("VIN")?.SetValue(car, commandCar.VIN.ToUpper());

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



        // فایل
        [HttpPost("{carId}/Upload")]
        public async Task<IActionResult> Upload(Guid carId, IFormFile file)
        {
            if (file.Length <= 0)
                return BadRequest("BadRequest");

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
                return BadRequest("Unsupported file format. Only JPG and PNG are allowed.");

            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
                return BadRequest("File is too large. Max allowed size is 5MB.");



            var directoryPath = $@"C:\Uploads\Cars";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var filePath = Path.Combine(directoryPath, file.FileName);

            //var carFile = new FileBase(file.FileName, filePath, carId);
            var carFile = new FileCar(file.FileName, filePath,carId);

            using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await file.CopyToAsync(stream);

            _context.FileBase.Add(carFile);
            await _context.SaveChangesAsync();

            return Ok(new { Id = carFile.Id, Message = "File uploaded successfully" });
        }

        [HttpGet("{id}/Download")]
        public async Task<IActionResult> Download(int id)
        {
            var carFile = await _context.FileBase.FindAsync(id);

            if (carFile == null)
                return NotFound("File not found");

            var fileBytes = System.IO.File.ReadAllBytes(carFile.FilePath);
            return File(fileBytes, "application/octet-stream", carFile.FileName);
        }


    }
}

