using Application.DTO.CarDTO;
using Carproject.Model;
using Domain.Model;
using Infrustructure.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Store_CAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarSaleController : ControllerBase
    {
        private readonly ICarRepository _carRepository;
        private readonly ISaleRepository _saleRepository;
        private readonly IRepository<Buyer> _genericrepository;
        private readonly IUserInfoRepository<Buyer> _userInfoRepository;

        // کانستراکتور برای دریافت وابستگی‌ها
        public CarSaleController(ICarRepository carRepository, ISaleRepository saleRepository)
        {
            _carRepository = carRepository;
            _saleRepository = saleRepository;
        }

        //// فیلتر کردن خودروها بر اساس پارامترهای ورودی
        [HttpGet("filter")]
        public async Task<IActionResult> GetFilteredCars(
            [FromQuery] string brand,
            [FromQuery] string model,
            [FromQuery] int? minYear,
            [FromQuery] int? maxYear,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string color,
            [FromQuery] Car.CarStatus? status)
        {
            try
            {
                // فراخوانی متد repository برای دریافت خودروها بر اساس فیلترهای ورودی
                var cars = await _carRepository.GetFilteredCarsAsync(brand, model, minYear, maxYear, minPrice, maxPrice, color, status);

                // بررسی نتیجه و ارسال پاسخ مناسب
                if (cars == null || cars.Count == 0)
                {
                    return NotFound("هیچ خودرویی مطابق با فیلتر پیدا نشد.");
                }

                return Ok(cars);
            }
            catch (Exception ex)
            {
                // مدیریت استثناها و خطاها
                return StatusCode(500, $"خطا در دریافت داده‌ها: {ex.Message}");
            }
        }

        // دریافت تمامی خودروها
        [HttpGet]
        public async Task<IActionResult> GetAllCars()
        {
            try
            {
                var cars = await _carRepository.GetAllCarsAsync();
                if (cars == null || cars.Count == 0)
                {
                    return NotFound("هیچ خودرویی پیدا نشد.");
                }
                return Ok(cars);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در دریافت خودروها: {ex.Message}");
            }
        }

        // دریافت خودرو بر اساس شناسه
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
        // انتخاب خودرو برای خرید
        [HttpPost("Buy Car")]
        public async Task<IActionResult> BuyCar([FromBody] BuyCarDTO buyCarDto)
        {
            try
            {
                // ابتدا بررسی می‌کنیم که آیا خودرو وجود دارد و در وضعیت "فروخته شده" نیست
                var car = await _carRepository.GetCarByIdAsync(buyCarDto.CarId);
                if (car == null)
                {
                    return NotFound("خودرو با این شناسه پیدا نشد.");
                }

                if (car.Status == Car.CarStatus.Sold)
                {
                    return BadRequest("این خودرو قبلاً فروخته شده است.");
                }

                // بررسی خریدار
                var buyer = await _saleRepository.GetBuyerByIdAsync(buyCarDto.BuyerId);
                if (buyer == null)
                {
                    return NotFound("خریدار با این شناسه پیدا نشد.");
                }

                // ایجاد یک رکورد جدید در جدول فروش
                var sale = new Sale
                (
                    saleDate: DateTime.Now,
                    amount: car.Price,
                    buyerId: buyCarDto.BuyerId
                )
                {
                    CarId = buyCarDto.CarId,
                    Buyer = buyer,
                    Car = car
                };

                // ثبت خرید در پایگاه داده
                var createdSale = await _saleRepository.AddSaleAsync(sale);

                // به‌روزرسانی وضعیت خودرو به "فروخته شده"
                car.SetStatus(Car.CarStatus.Sold);
                await _carRepository.UpdateCarAsync(car);

                return Ok(new { Message = "خرید موفقیت‌آمیز بود.", Sale = createdSale });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در ثبت خرید: {ex.Message}");
            }
        }
    }
}

