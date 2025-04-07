using Application.DTO;
using Domain.Model;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Store_CAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {

        private readonly CARdbcontext _cARdbcontext;
        private readonly IUserInfoRepository<Seller> _userInfoRepository;
        private readonly IRepository<Seller> _genericRepository;
        private string secretKey;
        public SellerController(CARdbcontext cARdbcontext, IConfiguration configuration, IUserInfoRepository<Seller> userInfoRepository, IRepository<Seller> genericRepository)
        {
            _cARdbcontext = cARdbcontext;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userInfoRepository = userInfoRepository;
            _genericRepository = genericRepository;
        }
        [HttpPost("Registerseller")]
        //[Authorize(Roles = "seller")]
        public async Task<IActionResult> Register(RegistersellerDTO registersellerDTO)
        {
            var seller = new Seller
            {
                Id = Guid.NewGuid(),
                Name = registersellerDTO.Name,
                Age = registersellerDTO.Age,
                nationalcode = registersellerDTO.National_Code,
                password = BCrypt.Net.BCrypt.HashPassword(registersellerDTO.Password),
                phonenumber = registersellerDTO.Phonenmber,
                Role = registersellerDTO.Role,
                Otp = null 
            };
            //information.Id = Guid.NewGuid();
            await _genericRepository.AddAsync(seller);
            await _genericRepository.SavechangeAsync();
            return Ok();
        }
        [HttpPost("Loginselleronestage")]
        //[Authorize(Roles = "seller")]
        public async Task<IActionResult> Login([FromBody] LogingsellerDTO logingsellerDTO)
        {
            var user = await _userInfoRepository.getnationalcode(logingsellerDTO.Nationalcode);
            if (user == null)
            {
                return BadRequest("User not foun");
            }
            // بررسی اینکه آیا رمز ذخیره‌شده یک هش معتبر است
            if (!BCrypt.Net.BCrypt.Verify(logingsellerDTO.Password, user.password))
            {
                return BadRequest("نام کاربری یا رمز عبور اشتباه است");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            return Ok(new { Token = tokenString });
        }

        [HttpPost("RequestPasswordReset")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> RequestPasswordReset(string nationalCode)
        {
            if (string.IsNullOrEmpty(nationalCode))
            {
                return BadRequest("کدملی نمی‌تواند خالی باشد");
            }

            var user = await _userInfoRepository.getnationalcode(nationalCode);
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
                await _genericRepository.SavechangeAsync();
                //await SendOtpToUser(user, otp);
                return Ok("کد موقت ارسال شد");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا: {ex.Message}");
            }
        }
        [HttpPost("ResetPassword")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> ResetPassword([FromBody] ChangepasswordDTo request)
        {
            if (request == null || string.IsNullOrEmpty(request.NationalCode) ||
                string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
            {
                return BadRequest("کدملی، کد موقت و رمز جدید نمی‌توانند خالی باشند");
            }

            var user = await _userInfoRepository.getnationalcode(request.NationalCode);
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

            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.Otp = null;
            user.OtpExpiry = null;

            try
            {
                await _genericRepository.SavechangeAsync();
                return Ok("رز عبور با یت تغییر کرد");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"خطا در تغییر رمز عبور: {ex.Message}");
            }
        }
        [HttpPost("request-otp")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDTO request)
        {
            var user = await _userInfoRepository.getphonenmber(request.Phonenumber);

            if (user == null)
                return NotFound("کاربر یافت نشد");

            // ایجاد OTP و ذخیره در دیتابیس
            var otp = new Random().Next(100000, 999999).ToString();
            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // اعتبار ۵ دقیقه

            await _genericRepository.SavechangeAsync();

            // اینجا باید OTP را از طریق SMS یا ایمیل ارسال کنید (مثلا با Twilio یا SendGrid)

            return Ok("OTP ارسال شد");
        }

        [HttpPost("verify-otp")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO verifyRequest)
        {
            var user = await _userInfoRepository.getphonenmber(verifyRequest.PhoneNumber);

            if (user == null)
                return NotFound("کاربر یافت نشد");

            if (user.Otp == null || user.OtpExpiry < DateTime.UtcNow)
                return BadRequest("OTP نامعتبر یا منقضی شده است");

            if (user.Otp != verifyRequest.otp)
                return BadRequest("OTP اشتباه است");

            // پاک کردن OTP بعد از تأیید موفقیت‌آمیز
            user.Otp = null;
            user.OtpExpiry = null;
            await _genericRepository.SavechangeAsync();

            return Ok("ورود موفقیت‌آمیز بود");
        }
        [HttpGet("check Seller")]
        public async Task<IActionResult> checkbuyer(string nationalcode)
        {
            if (string.IsNullOrWhiteSpace(nationalcode))
                return BadRequest(new { message = "National code is required." });

            var user = await _userInfoRepository.getnationalcode(nationalcode);
            if (user == null)
            {
                return NotFound(new { exists = false });

            }
            return Ok(new { exists = true });
        }
        [HttpPut("edit information seller")]
        [Authorize(Roles = "seller")]
        public async Task<IActionResult> editinformation([FromBody] RegistersellerDTO registersellerDTO)
        {
            var user = await _userInfoRepository.getpassword(registersellerDTO.Password);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            user.Name = registersellerDTO.Name;
            user.phonenumber = registersellerDTO.Phonenmber;
            user.nationalcode = registersellerDTO.National_Code;
            user.Age = registersellerDTO.Age;
            user.password = registersellerDTO.Password;

            await _genericRepository.SavechangeAsync();
            return NoContent();

        }
    }
}
