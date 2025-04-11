//using Application.DTO;
//using Domain.Model;
//using Infrustructure.Context;
//using Infrustructure.Repository.IRepository;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Http.HttpResults;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.AspNetCore.JsonPatch;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Newtonsoft.Json.Linq;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;

//namespace Store_CAR.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ModerController : ControllerBase
//    {
//        private readonly CARdbcontext _cARdbcontext;
//        private readonly IUserInfoRepository<Moder> _userInfoRepository;
//        private readonly IRepository<Moder> _genericRepository;
//        private string secretKey;
//        public ModerController(CARdbcontext cARdbcontext, IConfiguration configuration, IUserInfoRepository<Moder> userInfoRepository, IRepository<Moder> genericRepository)
//        {
//            _cARdbcontext = cARdbcontext;
//            _userInfoRepository = userInfoRepository;
//            _genericRepository = genericRepository;
//            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
//        }
        
//        [HttpPost("Registermoder")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> Register(RegistermoderDTO registermoderDTO)
//        {
//            var moder = new Moder
//            {
//                Id = Guid.NewGuid(),
//                Name = registermoderDTO.Name,
//                Age = registermoderDTO.Age,
//                nationalcode = registermoderDTO.National_Code,
//                password = BCrypt.Net.BCrypt.HashPassword(registermoderDTO.Password),
//                phonenumber = registermoderDTO.Phonenmber,
//                Role = registermoderDTO.Role,
//                Otp = null // مقدار پیش‌فرض (می‌توانید مقداردهی کنید)
//            };
//            //information.Id = Guid.NewGuid();
//            await _genericRepository.AddAsync(moder);
//            await _genericRepository.SavechangeAsync();
//            return Ok();
//        }
//        //[HttpPost("Loginmoderonestage")]
//        //public async Task<IActionResult> Login([FromBody] LogingmoderDTO logingmoderDTO)
//        //{
//        //    var user = await _cARdbcontext.moders.FirstOrDefaultAsync(x => x.Password == logingmoderDTO.Password && x.Name == logingmoderDTO.Name);
//        //    if (user == null)
//        //    {
//        //        return BadRequest("User not foun");
//        //    }
//        //    var tokenHandeler = new JwtSecurityTokenHandler();
//        //    var key = Encoding.ASCII.GetBytes(secretKey);

//        //    var tokenDescriptor = new SecurityTokenDescriptor
//        //    {
//        //        Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
//        //        {
//        //            new Claim(ClaimTypes.Name, user.Id.ToString()),
//        //            new Claim(ClaimTypes.Role, user.Role)
//        //        }),
//        //        Expires = DateTime.UtcNow.AddDays(7),
//        //        SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//        //    };
//        //    var token = tokenHandeler.CreateToken(tokenDescriptor);
//        //    LoginresponsemoderDTO loginresponsemoderDTO = new LoginresponsemoderDTO()
//        //    {
//        //        token = tokenHandeler.WriteToken(token),
//        //        moder = user,
//        //    };
//        //    return Ok("Heloo");
//        //}
//        [HttpPost("Loginmoderonestage")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> Login([FromBody] LogingmoderDTO logingmoderDTO)
//        {
//            var user = await _userInfoRepository.getnationalcode(logingmoderDTO.Nationalcode);

//            if (user == null)
//            {
//                return BadRequest("نام کاربری یا رمز عبور اشتباه است");
//            }

//            // بررسی اینکه آیا رمز ذخیره‌شده یک هش معتبر است
//            if (!BCrypt.Net.BCrypt.Verify(logingmoderDTO.Password, user.password))
//            {
//                return BadRequest("نام کاربری یا رمز عبور اشتباه است");
//            }

//            var tokenHandler = new JwtSecurityTokenHandler();
//            var key = Encoding.ASCII.GetBytes(secretKey);

//            var tokenDescriptor = new SecurityTokenDescriptor
//            {
//                Subject = new ClaimsIdentity(new Claim[]
//                {
//            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
//            new Claim(ClaimTypes.Name, user.Name),
//            new Claim(ClaimTypes.Role, user.Role)
//                }),
//                Expires = DateTime.UtcNow.AddDays(7),
//                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
//            };

//            var token = tokenHandler.CreateToken(tokenDescriptor);
//            var tokenString = tokenHandler.WriteToken(token);

//            return Ok(new { Token = tokenString });
//        }

//        [HttpPost("RequestPasswordReset")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> RequestPasswordReset(string nationalCode)
//        {
//            if (string.IsNullOrEmpty(nationalCode))
//            {
//                return BadRequest("کدملی نمی‌تواند خالی باشد");
//            }

//            var user = await _cARdbcontext.moders
//                .FirstOrDefaultAsync(i => i.nationalcode == nationalCode);
//            if (user == null)
//            {
//                return NotFound("کاربری با این کدملی پیدا نشد");
//            }
//            var random = new Random();
//            string GenerateOtp = random.Next(100000, 999999).ToString();
//            var otp = GenerateOtp;
//            user.Otp = otp;
//            user.OtpExpiry = DateTime.Now.AddMinutes(5);

//            try
//            {
//                await _genericRepository.SavechangeAsync();
//                //await SendOtpToUser(user, otp);
//                return Ok("کد موقت ارسال شد");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"خطا: {ex.Message}");
//            }
//        }
//        [HttpPost("ResetPassword")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> ResetPassword([FromBody] ChangepasswordDTo request)
//        {
//            if (request == null || string.IsNullOrEmpty(request.NationalCode) ||
//                string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
//            {
//                return BadRequest("کدملی، کد موقت و رمز جدید نمی‌توانند خالی باشند");
//            }

//            var user = await _userInfoRepository.getnationalcode(request.NationalCode);
//            if (user == null)
//            {
//                return NotFound("کاربری با این کدملی پیدا نشد");
//            }

//            if (user.Otp != request.Otp)
//            {
//                return BadRequest("کد موقت اشتباه است");
//            }

//            if (!user.OtpExpiry.HasValue || user.OtpExpiry < DateTime.Now)
//            {
//                return BadRequest("کد موقت منقضی شده است");
//            }

//            user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
//            user.Otp = null;
//            user.OtpExpiry = null;

//            try
//            {
//                await _genericRepository.SavechangeAsync() ;
//                return Ok("رز عبور با یت تغییر کرد");
//            }
//            catch (Exception ex)
//            {
//                return StatusCode(500, $"خطا در تغییر رمز عبور: {ex.Message}");
//            }
//        }
//        [HttpPost("request-otp")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDTO request)
//        {
//            var user = await _userInfoRepository.getphonenmber(request.Phonenumber);

//            if (user == null)
//                return NotFound("کاربر یافت نشد");

//            // ایجاد OTP و ذخیره در دیتابیس
//            var otp = new Random().Next(100000, 999999).ToString();
//            user.Otp = otp;
//            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // اعتبار ۵ دقیقه

//            await _genericRepository.SavechangeAsync();

//            // اینجا باید OTP را از طریق SMS یا ایمیل ارسال کنید (مثلا با Twilio یا SendGrid)

//            return Ok("OTP ارسال شد");
//        }

//        [HttpPost("verify-otp")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO verifyRequest)
//        {
//            var user = await _userInfoRepository.getphonenmber(verifyRequest.PhoneNumber);

//            if (user == null)
//                return NotFound("کاربر یافت نشد");

//            if (user.Otp == null || user.OtpExpiry < DateTime.UtcNow)
//                return BadRequest("OTP نامعتبر یا منقضی شده است");

//            if (user.Otp != verifyRequest.otp)
//                return BadRequest("OTP اشتباه است");

//            // پاک کردن OTP بعد از تأیید موفقیت‌آمیز
//            user.Otp = null;
//            user.OtpExpiry = null;
//            await _genericRepository.SavechangeAsync();

//            return Ok("ورود موفقیت‌آمیز بود");
//        }
//        [HttpGet("check Moder")]
//        public async Task<IActionResult> checkbuyer(string nationalcode)
//        {
//            if (string.IsNullOrWhiteSpace(nationalcode))
//                return BadRequest(new { message = "National code is required." });

//            var user = await _userInfoRepository.getnationalcode(nationalcode);
//            if (user == null)
//            {
//                return NotFound(new { exists = false });

//            }
//            return Ok(new { exists = true });
//        }
//        [HttpPut("edit information moder")]
//        [Authorize(Roles = "moder")]
//        public async Task<IActionResult> editinformation([FromBody] RegistermoderDTO registermoderDTO)
//        {
//            var user = await _userInfoRepository.getpassword(registermoderDTO.Password);
//            if (user == null)
//            {
//                return BadRequest("User Not Found");
//            }
//            user.Name = registermoderDTO.Name;
//            user.phonenumber = registermoderDTO.Phonenmber;
//            user.nationalcode = registermoderDTO.National_Code;
//            user.Age = registermoderDTO.Age;
//            user.password = registermoderDTO.Password;

//            await _genericRepository.SavechangeAsync();
//            return NoContent();

//        }
//    }
//}

