using Application.DTO;
using AutoMapper;
using Carproject.DTO;
using Carproject.Model;
using Domain.Model;
using Infrustructure.Context;
using Infrustructure.Repository;
using Infrustructure.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace Store_CAR.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyerController : ControllerBase
    {
        private readonly CARdbcontext _cARdbcontext;
        //private readonly IRepositoryBuyer _repositoryBuyer;
        private readonly IUserInfoRepository<Buyer> _userInfoRepository;
        private readonly IRepository<Buyer> _genericRepository;
        private readonly IRepository<PurchaseHistory> _PurchaseHistory;


        private string secretKey;
        //مپر رو اضافه کردم aa
        private readonly IMapper _mapper;
        public BuyerController(/*IRepositoryBuyer repositoryBuyer,*/ IConfiguration configuration, IUserInfoRepository<Buyer> userInfoRepository, IRepository<Buyer> genericRepository, CARdbcontext cARdbcontext, IMapper mapper, IRepository<PurchaseHistory> purchaseHistory)
        {
            _cARdbcontext = cARdbcontext;
            //_repositoryBuyer = repositoryBuyer;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _userInfoRepository = userInfoRepository;
            _genericRepository = genericRepository;
            _mapper = mapper;
            _PurchaseHistory = purchaseHistory;
        }

        [HttpPost("RegisterBuyer")]
        public async Task<IActionResult> Register(RegisterbuyerDTO registerbuyerDTO)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(registerbuyerDTO.Password);
            var buyer = new Buyer()
            {
                Id = Guid.NewGuid(),
                Name = registerbuyerDTO.Name,
                Age = registerbuyerDTO.Age,
                nationalcode = registerbuyerDTO.National_Code,
                password = hashedPassword,
                phonenumber = registerbuyerDTO.Phonenmber,
                Role = registerbuyerDTO.Role,
                Otp = null
            };
            await _genericRepository.AddAsync(buyer);
            await _genericRepository.SavechangeAsync();
            return Ok();
        }

        [HttpPost("Loginbuyeronestage")]
        public async Task<IActionResult> Login([FromBody] LogingbuyersDTO logingbuyersDTO)
        {
            var user = await _userInfoRepository.getnationalcode(logingbuyersDTO.Nationalcode) ;
            if (user == null)
            {
                return BadRequest("User not foun");
            }
            // بررسی اینکه آیا رمز ذخیره‌شده یک هش معتبر است
            if (!BCrypt.Net.BCrypt.Verify(logingbuyersDTO.Password, user.password))
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
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDTO request)
        {
            var user = await _userInfoRepository.getphonenmber(request.Phonenumber);

            if (user == null)
            {
                return NotFound("کاربر یافت نشد");
            }
            // ایجاد OTP و ذخیره در دیتابیس
            var otp = new Random().Next(100000, 999999).ToString();
            user.Otp = otp;
            user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // اعتبار ۵ دقیقه

            await _genericRepository.SavechangeAsync();

            // اینجا باید OTP را از طریق SMS یا ایمیل ارسال کنید (مثلا با Twilio یا SendGrid)

            return Ok("OTP ارسال شد");
        }

        [HttpPost("verify-otp")]
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

        [HttpGet("check buyer")]
        public async Task<IActionResult> checkbuyer(string nationalcode)
        {
            if (string.IsNullOrWhiteSpace(nationalcode))
                return BadRequest(new { message = "National code is required." });

            var user = await _userInfoRepository.getnationalcode(nationalcode);
            if(user == null)
            {
                return NotFound(new { exists = false });
                
            }
            return Ok(new { exists = true });
        }

        [HttpPut("edit information buyer")]
        [Authorize(Roles = "buyer")]
        public async Task<IActionResult> editinformation(string nationalcode , [FromBody] RegisterbuyerDTO registerbuyerDTO)
        {
            var user = await _userInfoRepository.getnationalcode(nationalcode);
            if (user == null)
            {
                return BadRequest("User Not Found");
            }
            user.Name = registerbuyerDTO.Name;
            user.phonenumber = registerbuyerDTO.Phonenmber;
            user.nationalcode = registerbuyerDTO.National_Code;
            user.Age = registerbuyerDTO.Age;
            user.password = registerbuyerDTO.Password;

            await _genericRepository.SavechangeAsync();
            return NoContent();

        }

        [HttpGet("{id}/purchase-history")]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetPurchaseHistory(Guid id)
        {
            var customer = await _cARdbcontext.buyers
                .Include(c => c.PurchaseHistories)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
                return NotFound("Customer not found.");

            // تبدیل اطلاعات مشتری به CustomerDto
            var customerDto = _mapper.Map<CustomerDto>(customer);

            // دریافت تاریخچه خرید
            var purchaseHistoryDtos = _mapper.Map<List<PurchaseHistoryDto>>(customer.PurchaseHistories);



            // برگرداندن اطلاعات مشتری و تاریخچه خرید
            return Ok(new
            {
                Customer = customerDto,
                PurchaseHistory = purchaseHistoryDtos
            });
        }



        [HttpPost("add-purchase-history")]
        public async Task<ActionResult<PurchaseHistoryDto>> AddPurchaseHistory([FromBody] PurchaseHistoryDto purchaseHistoryDto)
        {
            // بررسی اینکه مشتری وجود دارد یا خیر
            var customer = await _cARdbcontext.buyers.FindAsync(purchaseHistoryDto.CustomerId);
            if (customer == null)
                return NotFound("Customer not found.");

            var purchaseHistory = _mapper.Map<PurchaseHistory>(purchaseHistoryDto);


            // افزودن سابقه خرید به پایگاه داده
            await _PurchaseHistory.AddAsync(purchaseHistory);
            await _PurchaseHistory.SavechangeAsync();

            // تبدیل PurchaseHistory به PurchaseHistoryDto و بازگشت داده‌ها
            var result = _mapper.Map<PurchaseHistoryDto>(purchaseHistory);

            return CreatedAtAction(nameof(GetPurchaseHistory), new { id = purchaseHistory.CustomerId }, result);
        }

        private LoyaltyStatus GetLoyaltyStatus(decimal points)
        {
            if (points < 100) return LoyaltyStatus.Bronze;
            if (points < 500) return LoyaltyStatus.Silver;
            if (points < 1000) return LoyaltyStatus.Gold;
            return LoyaltyStatus.Platinum;
        }


        [HttpPost("add-points")]
        public async Task<IActionResult> AddPointsToCustomer(int customerId, decimal purchaseAmount)
        {
            var customer = await _cARdbcontext.buyers.FindAsync(customerId);
            if (customer == null) return NotFound("Customer not found.");

            customer.Points += purchaseAmount / 1000;
            customer.LoyaltyStatus = GetLoyaltyStatus(customer.Points);

            await _cARdbcontext.SaveChangesAsync();
            return Ok(new { customer.Points, customer.LoyaltyStatus });
        }
    }
}

