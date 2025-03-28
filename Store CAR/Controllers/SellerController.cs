using Application.DTO;
using Domain.Model;
using Infrustruction.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Store_CAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {

        private readonly CARdbcontext _cARdbcontext;
        public SellerController(CARdbcontext cARdbcontext)
        {
            _cARdbcontext = cARdbcontext;
        }
        [HttpPost("Registerseller")]
        public async Task<IActionResult> Register(RegistersellerDTO registersellerDTO)
        {
            var seller = new Seller
            {
                Id = Guid.NewGuid(),
                Name = registersellerDTO.Name,
                Age = registersellerDTO.Age, // تبدیل `string` به `int`
                National_Code = registersellerDTO.National_Code,
                Password = registersellerDTO.Password,
                Phonenmber = registersellerDTO.Phonenmber,
                Role = registersellerDTO.Role/*.ToList()*/,
                Otp = null // مقدار پیش‌فرض (می‌توانید مقداردهی کنید)
            };
            //information.Id = Guid.NewGuid();
            await _cARdbcontext.sellers.AddAsync(seller);
            await _cARdbcontext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Loginselleronestage")]
        public async Task<IActionResult> Login([FromBody] LogingsellerDTO logingsellerDTO)
        {
            var user = await _cARdbcontext.sellers.FirstOrDefaultAsync(x => x.Password == logingsellerDTO.Password && x.Name == logingsellerDTO.Name);
            if (user == null)
            {
                return BadRequest("User not foun");
            }
            return Ok("Heloo");
        }

        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset(string nationalCode)
        {
            if (string.IsNullOrEmpty(nationalCode))
            {
                return BadRequest("کدملی نمی‌تواند خالی باشد");
            }

            var user = await _cARdbcontext.sellers
                .FirstOrDefaultAsync(i => i.National_Code == nationalCode);
            if (user == null)
            {
                return NotFound("کاربری با این کدملی پیدا نشد");
            }
            var random = new Random();
            string GenerateOtp = random.Next(100000, 999999).ToString();
            var otp = GenerateOtp;
            user.Otp = otp;
            user.OtpExpiry = DateTime.Now.AddMinutes(5);

            try
            {
                await _cARdbcontext.SaveChangesAsync();
                //await SendOtpToUser(user, otp);
                return Ok("کد موقت ارسال شد");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا: {ex.Message}");
            }
        }
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangepasswordDTo request)
        {
            if (request == null || string.IsNullOrEmpty(request.NationalCode) ||
                string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("کدملی، کد موقت و رمز جدید نمی‌توانند خالی باشند");
            }

            var user = await _cARdbcontext.sellers
                .FirstOrDefaultAsync(i => i.National_Code == request.NationalCode);
            if (user == null)
            {
                return NotFound("کاربری با این کدملی پیدا نشد");
            }

            if (user.Otp != request.Otp)
            {
                return BadRequest("کد موقت اشتباه است");
            }

            if (!user.OtpExpiry.HasValue || user.OtpExpiry < DateTime.Now)
            {
                return BadRequest("کد موقت منقضی شده است");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Otp = null;
            user.OtpExpiry = null;

            try
            {
                await _cARdbcontext.SaveChangesAsync();
                return Ok("رز عبور با یت تغییر کرد");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در تغییر رمز عبور: {ex.Message}");
            }
        }
    }
}
