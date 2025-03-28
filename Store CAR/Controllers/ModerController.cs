using Application.DTO;
using Domain.Model;
using Infrustruction.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Store_CAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ModerController : ControllerBase
    {
        private readonly CARdbcontext _cARdbcontext;
        public ModerController(CARdbcontext cARdbcontext)
        {
            _cARdbcontext = cARdbcontext;
        }
        [HttpPost("Registermoder")]
        public async Task<IActionResult> Register(RegistermoderDTO registermoderDTO)
        {
            var moder = new Moder
            {
                Id = Guid.NewGuid(),
                Name = registermoderDTO.Name,
                Age = registermoderDTO.Age, // تبدیل `string` به `int`
                National_Code = registermoderDTO.National_Code,
                Password = registermoderDTO.Password,
                Phonenmber = registermoderDTO.Phonenmber,
                Role = registermoderDTO.Role/*.ToList()*/,
                Otp = null // مقدار پیش‌فرض (می‌توانید مقداردهی کنید)
            };
            //information.Id = Guid.NewGuid();
            await _cARdbcontext.moders.AddAsync(moder);
            await _cARdbcontext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Loginmoderonestage")]
        public async Task<IActionResult> Login([FromBody] LogingmoderDTO logingmoderDTO)
        {
            var user = await _cARdbcontext.moders.FirstOrDefaultAsync(x => x.Password == logingmoderDTO.Password && x.Name == logingmoderDTO.Name);
            if (user == null)
            {
                return BadRequest("User not foun");
            }
            return Ok("Heloo");
        }
        //[HttpPatch("change_Password")]
        //public async Task<IActionResult> Changepassword(string nationalcode,[FromBody] JsonPatchDocument<Information> change)
        //{
        //    var informations = await _cARdbcontext.informations.FindAsync(nationalcode);
        //    if (informations == null)
        //    {
        //        return BadRequest("user Not Found");
        //    }
        //    change.ApplyTo(informations, ModelState);
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }
        //    await _cARdbcontext.SaveChangesAsync();
        //    return NoContent();

        //    // return HttpRequestOptionsKey()
        //}
        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset(string nationalCode)
        {
            if (string.IsNullOrEmpty(nationalCode))
            {
                return BadRequest("کدملی نمی‌تواند خالی باشد");
            }

            var user = await _cARdbcontext.moders
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

            var user = await _cARdbcontext.moders
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

