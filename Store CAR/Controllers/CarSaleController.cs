using Application.DTO.CarDTO;
using Domain.Model.CarModel;
using Domain.Model.File;
using Domain.Model.ReportNotifModel;
using Domain.Model.UserModel;
using Infrustructure.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace Store_CAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSaleController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IFileRepository _fileRepository;
        private readonly IRepository<Buyer> _genericRepository;
        private readonly IUserInfoRepository<Buyer> _userInfoRepository;

        public CarSaleController(
            ICarRepository carRepository,
            ISaleRepository saleRepository,
            IRepository<Buyer> genericRepository,
            IUserInfoRepository<Buyer> userInfoRepository,
            IFileRepository fileRepository)
        {
            _carRepository = carRepository;
            _saleRepository = saleRepository;
            _genericRepository = genericRepository;
            _userInfoRepository = userInfoRepository;
            _fileRepository = fileRepository;
        }




        [HttpGet("{carId}")]
        public async Task<IActionResult> GetCarById(Guid carId)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(carId);
                if (car == null)
                {
                    return NotFound("خودرویی با این شناسه پیدا نشد.");
                }
                return Ok(car);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در دریافت خودرو: {ex.Message}");
            }
        }

        [HttpPost("BuyCar")]
        public async Task<IActionResult> BuyCar([FromBody] BuyCarDTO buyCarDto)
        {
            try
            {
                var car = await _carRepository.GetCarByIdAsync(buyCarDto.CarId);
                if (car == null)
                {
                    return NotFound("خودرو با این شناسه پیدا نشد.");
                }

                if (car.Status == Car.CarStatus.Sold)
                {
                    return BadRequest("این خودرو قبلاً فروخته شده است.");
                }

                var buyer = await _userInfoRepository.GetByIdAsync(buyCarDto.BuyerId);
                if (buyer == null)
                {
                    return NotFound("خریدار با این شناسه پیدا نشد.");
                }

                var sale = new Sale(DateTime.Now, car.Price, buyCarDto.BuyerId, buyCarDto.CarId, false);
                var createdSale = await _saleRepository.AddSaleAsync(sale);
                await _genericRepository.SavechangeAsync();

                car.SetStatus(Car.CarStatus.Sold);
                await _carRepository.UpdateCarAsync(car);

                return Ok(new { Message = "خرید موفقیت‌آمیز بود.", Sale = createdSale });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در ثبت خرید: {ex.Message}");
            }
        }

            [HttpPost("mark-as-pay")]
        public async Task<IActionResult> MarkAsPaid(Guid saleId)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(saleId);
            if (sale == null) return NotFound();

            sale.MarkAsPaid();
            await _genericRepository.SavechangeAsync();

            return Ok("سفارش شما تایید شد.");
        }

        [HttpPost("Sale/{saleId}/UploadFile")]
        public async Task<IActionResult> UploadFile(Guid saleId, IFormFile file)
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

            var directoryPath = $@"C:\Uploads\Sales";

            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);

            var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
            var filePath = Path.Combine(directoryPath, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await file.CopyToAsync(stream);
            }

            var saleFile = new FileBase(file.FileName, filePath);
            await _fileRepository.AddFileAsync(saleFile);
            await _fileRepository.SaveAsync();

            var sale = await _saleRepository.GetSaleByIdAsync(saleId);
            if (sale == null)
                return NotFound("Sale not found.");

            sale.AddFileId(saleFile.Id);
            await _saleRepository.UpdateSaleAsync(sale);

            return Ok(new { Id = saleFile.Id, Message = "File uploaded successfully" });
        }

        [HttpGet("Sale/{saleId}/DownloadFile/{fileId}")]
        public async Task<IActionResult> DownloadFile(Guid saleId, Guid fileId)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(saleId);
            if (sale == null)
                return NotFound("Sale not found.");

            var file = await _fileRepository.GetFileByIdAsync(fileId);
            if (file == null)
                return NotFound("File not found.");

            var fileIdsList = string.IsNullOrWhiteSpace(sale.FilesIds)
                ? new List<string>()
                : sale.FilesIds.Split(',').ToList();

            if (!fileIdsList.Contains(fileId.ToString()))
                return BadRequest("File does not belong to this sale.");

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

        [HttpDelete("Sale/{saleId}/DeleteFile/{fileId}")]
        public async Task<IActionResult> DeleteFile(Guid saleId, Guid fileId)
        {
            var sale = await _saleRepository.GetSaleByIdAsync(saleId);
            if (sale == null)
                return NotFound("Sale not found.");

            var file = await _fileRepository.GetFileByIdAsync(fileId);
            if (file == null)
                return NotFound("File not found.");

            var fileIdsList = string.IsNullOrWhiteSpace(sale.FilesIds)
                ? new List<string>()
                : sale.FilesIds.Split(',').ToList();

            if (!fileIdsList.Contains(fileId.ToString()))
                return BadRequest("File does not belong to this sale.");

            if (System.IO.File.Exists(file.FilePath))
            {
                System.IO.File.Delete(file.FilePath);
            }

            await _fileRepository.DeleteFileAsync(file);
            await _fileRepository.SaveAsync();

            sale.RemoveFileId(fileId);
            await _saleRepository.UpdateSaleAsync(sale);

            return Ok(new { Message = "File deleted successfully" });
        }
    }
}
