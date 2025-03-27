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
    public class RegisterController : ControllerBase
    {
        private readonly CARdbcontext _cARdbcontext;
        public RegisterController(CARdbcontext cARdbcontext)
        {
            _cARdbcontext = cARdbcontext;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] Information information)
        {
            information.Id = Guid.NewGuid();
            await _cARdbcontext.informations.AddAsync(information);
            await _cARdbcontext.SaveChangesAsync();
            return Ok();
        }
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LogingDTO logingDTO)
        {
            var user = await _cARdbcontext.informations.FirstOrDefaultAsync(x => x.Password == logingDTO.Password && x.Name == logingDTO.Name);
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

            var user = await _cARdbcontext.informations
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

            var user = await _cARdbcontext.informations
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
                return Ok("رمز عبور با موفقیت تغییر کرد");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در تغییر رمز عبور: {ex.Message}");
            }
        }
    }
}

