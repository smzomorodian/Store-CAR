using Application.DTO.CarDTO;
using Domain.Model.CarModel;
using Domain.Model.File;
using Infrustructure.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Application.Command.Car;
using static Domain.Model.CarModel.Car;
using Infrustructure.Repository;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Carproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly IMapper _mapper;
        private readonly IFileRepository _fileRepository;
        public CarController(ICarRepository carRepository, IMapper mapper, IFileRepository fileRepository)
        {
            _carRepository = carRepository;
            _mapper = mapper;
            _fileRepository = fileRepository;
        }

        // اضافه کردن خودرو
        [HttpPost]
        public async Task<ActionResult<CarDto>> PostCar([FromBody] CommandCar commandCar)
        {
            // بررسی داده‌های ورودی
            if (commandCar == null)
                return BadRequest(new { message = "Invalid data." });

            // بررسی فرمت VIN با استفاده از Regex
            //string vinPattern = "^[A-HJ-NPR-Z0-9]{17}$";
            //if (!Regex.IsMatch(commandCar.VIN?.ToUpper() ?? "", vinPattern))
            //    return BadRequest(new { message = "VIN must be exactly 17 characters, using only A–Z (except I, O, Q) and digits." });


            if (commandCar.CategoryId == Guid.Empty)
            {
                var defaultCategory = await _carRepository.GetDefaultCategoryAsync();
                if (defaultCategory == null)
                    return BadRequest(new { message = "No categories available in the system." });

                commandCar.CategoryId = defaultCategory.Id;
            }

            // بررسی اینکه آیا دسته‌بندی معتبر است
            if (!await _carRepository.IsCategoryValidAsync(commandCar.CategoryId))
                return BadRequest(new { message = "Invalid CategoryId" });

            // نگاشت CommandCar به Car با استفاده از AutoMapper
            var car = _mapper.Map<Car>(commandCar);

            // تبدیل VIN به حروف بزرگ قبل از ذخیره
            car.AddVin(commandCar.VIN.ToUpper());

            // اضافه کردن خودرو به پایگاه داده از طریق ریپازیتوری
            await _carRepository.AddCarAsync(car);
            await _carRepository.SaveAsync();


            var savedCar = await _carRepository.GetCarByIdAsync(car.Id);

            // نگاشت خودرو به CarDto برای پاسخ‌دهی
            var carDto = _mapper.Map<CarDto>(savedCar);

            // برگرداندن خودرو جدید با استفاده از CreatedAtAction
            return CreatedAtAction(nameof(GetCar), new { id = savedCar.Id }, carDto);
        }


        // دریافت اطلاعات یک خودرو بر اساس شناسه
        [HttpGet("{id}")]
        public async Task<ActionResult<CarDto>> GetCar(Guid id)
        {
            var car = await _carRepository.GetCarByIdAsync(id);
            if (car == null)
                return NotFound(new { message = "Car not found." });

            return Ok(_mapper.Map<CarDto>(car));
        }


        // دریافت تمامی خودروها
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CarDto>>> GetAllCars()
        {
            var cars = await _carRepository.GetAllCarsAsync();
            return Ok(_mapper.Map<IEnumerable<CarDto>>(cars));
        }


        // فیلتر خودروها
        [HttpGet("filter1")]
        public async Task<ActionResult<IEnumerable<CarDto>>> FilterCars(
            [FromQuery] string brand,
            [FromQuery] string model,
            [FromQuery] int? minYear,
            [FromQuery] int? maxYear,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string color,
            [FromQuery] Car.CarStatus? status,
            [FromQuery] Guid categoryId)

        {
            var cars = await _carRepository.GetFilteredCarsAsync(brand, model, minYear, maxYear, minPrice, maxPrice, color, status, categoryId);
            return Ok(_mapper.Map<IEnumerable<CarDto>>(cars));
        }

        // ویرایش خودرو
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCar(Guid id, [FromBody] CarDto updateCarDto)
        {
            if (id == Guid.Empty || updateCarDto == null)
                return BadRequest(new { message = "Invalid data." });

            var existingCar = await _carRepository.GetCarByIdAsync(id);
            if (existingCar == null) return NotFound(new { message = "Car not found." });

            _mapper.Map(updateCarDto, existingCar);
            await _carRepository.UpdateCarAsync(existingCar);

            return Ok(_mapper.Map<CarDto>(existingCar));
        }

        // حذف خودرو
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCar(Guid id)
        {
            var success = await _carRepository.DeleteCarAsync(id);
            if (!success) return NotFound(new { message = "Car not found." });

            return Ok(new { message = "Car deleted successfully." });
        }


        // فیلتر خودروها
        [HttpGet("filter2")]
        public async Task<ActionResult<IEnumerable<CarDto>>> FilterCars([FromQuery] string? brand, [FromQuery] string? model, [FromQuery] int? year, [FromQuery] string? color, [FromQuery] decimal? minPrice, [FromQuery] decimal? maxPrice, [FromQuery] Guid? categoryId, [FromQuery] CarStatus? status)
        {
            var cars = await _carRepository.GetFilteredCarsAsync(
            brand,
            model,
            minYear: year,
            maxYear: year,
            minPrice,
            maxPrice,
            color,
            status,
            categoryId);
            return Ok(_mapper.Map<IEnumerable<CarDto>>(cars));
        }

        // آپلود فایل
        [HttpPost("{carId}/Upload")]
        public async Task<IActionResult> Upload(Guid carId, IFormFile file)
        {
            if (file.Length <= 0)
                return BadRequest("No file uploaded");

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

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(directoryPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            var carFile = new FileBase(uniqueFileName, filePath);
            await _fileRepository.AddFileAsync(carFile);
            await _fileRepository.SaveAsync();

            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
                return NotFound("Car not found.");

            car.AddFileId(carFile.Id); // افزودن ID فایل به خودرو
            await _carRepository.UpdateCarAsync(car);

            return Ok(new { Message = "File uploaded successfully", FileId = carFile.Id });
        }

        // دانلود فایل
        [HttpGet("Car/{carId}/DownloadFile/{fileId}")]
        public async Task<IActionResult> DownloadFile(Guid carId, Guid fileId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
                return NotFound("Car not found.");

            var file = await _fileRepository.GetFileByIdAsync(fileId);
            if (file == null)
                return NotFound("File not found.");

            if (!car.FilesIdsList.Contains(fileId.ToString()))
                return BadRequest("File does not belong to this car.");

            var fileBytes = await System.IO.File.ReadAllBytesAsync(file.FilePath);
            var fileExtension = Path.GetExtension(file.FileName).ToLower();
            var mimeType = fileExtension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"
            };

            return File(fileBytes, mimeType, file.FileName);
        }

        // حذف فایل
        [HttpDelete("Car/{carId}/DeleteFile/{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid carId, Guid fileId)
        {
            var car = await _carRepository.GetCarByIdAsync(carId);
            if (car == null)
                return NotFound("Car not found.");

            var file = await _fileRepository.GetFileByIdAsync(fileId);
            if (file == null)
                return NotFound("File not found.");

            if (!car.FilesIdsList.Contains(fileId.ToString()))
                return BadRequest("File does not belong to this car.");

            if (System.IO.File.Exists(file.FilePath))
            {
                System.IO.File.Delete(file.FilePath);  // حذف فایل از سیستم فایل
            }

            await _fileRepository.DeleteFileAsync(file);
            await _fileRepository.SaveAsync();

            car.FilesIdsList.Remove(fileId.ToString());  // حذف ID فایل از لیست
            car.SetFilesIds(car.FilesIdsList);
            await _carRepository.UpdateCarAsync(car);

            return Ok(new { Message = "File deleted successfully" });
        }



    }
}


