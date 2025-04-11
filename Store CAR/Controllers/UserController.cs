using Application.Command.User.Command;
using Application.DTO;
using Azure.Core;
using Carproject.DTO;
using Carproject.Model;
using Domain.Model;
using Infrustructure.Context;
using Infrustructure.Repository;
using Infrustructure.Repository.IRepository;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
    public class UserController : ControllerBase
    {
        //private readonly CARdbcontext _cARdbcontext;
        //private readonly IRepositoryBuyer _repositoryBuyer;
        //private readonly IUserInfoRepository<Buyer> _userInfoRepository;
        //private readonly IRepository<Buyer> _genericRepository;
        private string secretKey;
        private readonly IMediator _mediator;
        public UserController(IMediator mediator, IConfiguration configuration, IUserInfoRepository<Buyer> userInfoRepository, IRepository<Buyer> genericRepository)
        {
            // _cARdbcontext = cARdbcontext;
            //_repositoryBuyer = repositoryBuyer;
            //_userInfoRepository = userInfoRepository;
            //_genericRepository = genericRepository;
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mediator = mediator;

        }

        [HttpPost("register/{userType}")]
        //[Authorize(Roles = "buyer, moder, seller")]
        public async Task<IActionResult> Register(string userType, [FromBody]RegisterbuyerDTO registerbuyerDTO)
        {
            Type userClassType = null;
            string userTypeClean = userType?.Trim().ToLower();
            Guid result;

            switch (userTypeClean)
            {
                case "buyer":
                    var commandbuyer = new UserRegisterCommand<Buyer>
                    {
                        Name = registerbuyerDTO.Name,
                        Age = registerbuyerDTO.Age,
                        National_Code = registerbuyerDTO.National_Code,
                        Password = BCrypt.Net.BCrypt.HashPassword(registerbuyerDTO.Password),
                        Phonenmber = registerbuyerDTO.Phonenmber,
                        Email = registerbuyerDTO.Email,
                        Role = registerbuyerDTO.Role

                    };
                    result = await _mediator.Send(commandbuyer);
                    break;
                case "seller":
                    var commandseller = new UserRegisterCommand<Seller>
                    {
                        Name = registerbuyerDTO.Name,
                        Age = registerbuyerDTO.Age,
                        National_Code = registerbuyerDTO.National_Code,
                        Password = registerbuyerDTO.Password,
                        Phonenmber = registerbuyerDTO.Phonenmber,
                        Email = registerbuyerDTO.Email,
                        Role = registerbuyerDTO.Role

                    };
                    result = await _mediator.Send(commandseller);
                    break;
                case "moder":
                    var commandmoder = new UserRegisterCommand<Seller>
                    {
                        Name = registerbuyerDTO.Name,
                        Age = registerbuyerDTO.Age,
                        National_Code = registerbuyerDTO.National_Code,
                        Password = registerbuyerDTO.Password,
                        Phonenmber = registerbuyerDTO.Phonenmber,
                        Email = registerbuyerDTO.Email,
                        Role = registerbuyerDTO.Role
                    };
                    result = await _mediator.Send(commandmoder);
                    break;
                default:
                    return BadRequest("نوع کاربر نامعتبر است.");
            }
            //var commandType = typeof(UserRegisterCommand<>).MakeGenericType(userClassType);
            //var command = Activator.CreateInstance(commandType, registerbuyerDTO.Name, registerbuyerDTO.Age, registerbuyerDTO.National_Code,
            //    registerbuyerDTO.Password, registerbuyerDTO.Phonenmber, registerbuyerDTO.Role);
            //var result = await _mediator.Send((IRequest<Guid>)command);
            return Ok(new { Id = result });
        }

        [HttpPost("Loginonestage/{userType}")]
      //[Authorize(Roles = "buyer, moder, seller")]
        public async Task<IActionResult> Login(string userType,[FromBody] LogingbuyersDTO logingbuyersDTO)
        {
            string Token;
            string userTypeclean = userType;
            switch (userTypeclean)
            {
                case "buyer":
                    var commandbuyer = new UserLoginCommand<Buyer>
                    {
                        Nationalcode = logingbuyersDTO.Nationalcode,
                        Password =logingbuyersDTO.Password
                    };
                    Token = await _mediator.Send(commandbuyer);
                    break;

                case "seller":
                    var commandseller = new UserLoginCommand<Seller>
                    {
                        Nationalcode = logingbuyersDTO.Nationalcode,
                        Password = logingbuyersDTO.Password
                    };
                    Token = await _mediator.Send(commandseller);
                    break;
                case "moder":
                    var commandmoder = new UserLoginCommand<Moder>
                    {
                        Nationalcode = logingbuyersDTO.Nationalcode,
                        Password = logingbuyersDTO.Password
                    };
                    Token = await _mediator.Send(commandmoder);
                    break;
                default:
                    return BadRequest("نوع کاربر نامعتبر است.");
            }
            return Ok(new { token = Token });
        }

        [HttpPost("RequestPasswordReset")]
        public async Task<IActionResult> RequestPasswordReset(string nationalCode)
        {
            var command = new RequestPasswordResetCommand<User> 
            { nationalCode = nationalCode };
            var user =await _mediator.Send(command);
            return Ok(user);
        }


        //[HttpPost("ResetPassword")]
        //public async Task<IActionResult> ResetPassword([FromBody] ChangepasswordDTo request)
        //{
        //    if (request == null || string.IsNullOrEmpty(request.NationalCode) ||
        //        string.IsNullOrEmpty(request.Otp) || string.IsNullOrEmpty(request.NewPassword))
        //    {
        //        return BadRequest("کدملی، کد موقت و رمز جدید نمی‌توانند خالی باشند");
        //    }

        //    var user = await _userInfoRepository.getnationalcode(request.NationalCode);
        //    if (user == null)
        //    {
        //        return NotFound("کاربری با این کدملی پیدا نشد");
        //    }

        //    if (user.Otp != request.Otp)
        //    {
        //        return BadRequest("کد موقت اشتباه است");
        //    }

        //    if (!user.OtpExpiry.HasValue || user.OtpExpiry < DateTime.Now)
        //    {
        //        return BadRequest("کد موقت منقضی شده است");
        //    }

        //    user.password = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        //    user.Otp = null;
        //    user.OtpExpiry = null;

        //    try
        //    {
        //        await _genericRepository.SavechangeAsync();
        //        return Ok("رز عبور با یت تغییر کرد");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, $"خطا در تغییر رمز عبور: {ex.Message}");
        //    }
        //}













        //[HttpPost("request-otp")]
        //public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDTO request)
        //{
        //    var user = await _userInfoRepository.getphonenmber(request.Phonenumber);

        //    if (user == null)
        //    {
        //        return NotFound("کاربر یافت نشد");
        //    }
        //    // ایجاد OTP و ذخیره در دیتابیس
        //    var otp = new Random().Next(100000, 999999).ToString();
        //    user.Otp = otp;
        //    user.OtpExpiry = DateTime.UtcNow.AddMinutes(5); // اعتبار ۵ دقیقه

        //    await _genericRepository.SavechangeAsync();

        //    // اینجا باید OTP را از طریق SMS یا ایمیل ارسال کنید (مثلا با Twilio یا SendGrid)

        //    return Ok("OTP ارسال شد");
        //}

        //[HttpPost("verify-otp")]
        //public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO verifyRequest)
        //{
        //    var user = await _userInfoRepository.getphonenmber(verifyRequest.PhoneNumber);

        //    if (user == null)
        //        return NotFound("کاربر یافت نشد");

        //    if (user.Otp == null || user.OtpExpiry < DateTime.UtcNow)
        //        return BadRequest("OTP نامعتبر یا منقضی شده است");

        //    if (user.Otp != verifyRequest.otp)
        //        return BadRequest("OTP اشتباه است");

        //    // پاک کردن OTP بعد از تأیید موفقیت‌آمیز
        //    user.Otp = null;
        //    user.OtpExpiry = null;
        //    await _genericRepository.SavechangeAsync();

        //    return Ok("ورود موفقیت‌آمیز بود");
        //}

        //[HttpGet("check buyer")]
        //public async Task<IActionResult> checkbuyer(string nationalcode)
        //{
        //    if (string.IsNullOrWhiteSpace(nationalcode))
        //        return BadRequest(new { message = "National code is required." });

        //    var user = await _userInfoRepository.getnationalcode(nationalcode);
        //    if (user == null)
        //    {
        //        return NotFound(new { exists = false });

        //    }
        //    return Ok(new { exists = true });
        //}

        //[HttpPut("edit information buyer")]
        //[Authorize(Roles = "buyer")]
        //public async Task<IActionResult> editinformation(string nationalcode, [FromBody] RegisterbuyerDTO registerbuyerDTO)
        //{
        //    var user = await _userInfoRepository.getnationalcode(nationalcode);
        //    if (user == null)
        //    {
        //        return BadRequest("User Not Found");
        //    }
        //    user.Name = registerbuyerDTO.Name;
        //    user.phonenumber = registerbuyerDTO.Phonenmber;
        //    user.nationalcode = registerbuyerDTO.National_Code;
        //    user.Age = registerbuyerDTO.Age;
        //    user.password = registerbuyerDTO.Password;

        //    await _genericRepository.SavechangeAsync();
        //    return NoContent();

        //}

        //    [HttpGet("{id}/purchase-history")]
        //    public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetPurchaseHistory(int id)
        //    {
        //        var customer = await _context.Customers
        //            .Include(c => c.PurchaseHistories)
        //            .FirstOrDefaultAsync(c => c.CustomerId == id);

        //        if (customer == null)
        //            return NotFound("Customer not found.");

        //        // تبدیل اطلاعات مشتری به CustomerDto
        //        var customerDto = _mapper.Map<CustomerDto>(customer);

        //        // دریافت تاریخچه خرید
        //        var purchaseHistoryDtos = _mapper.Map<List<PurchaseHistoryDto>>(customer.PurchaseHistories);



        //        // برگرداندن اطلاعات مشتری و تاریخچه خرید
        //        return Ok(new
        //        {
        //            Customer = customerDto,
        //            PurchaseHistory = purchaseHistoryDtos
        //        });
        //    }



        //    [HttpPost("add-purchase-history")]
        //    public async Task<ActionResult<PurchaseHistoryDto>> AddPurchaseHistory([FromBody] PurchaseHistoryDto purchaseHistoryDto)
        //    {
        //        // بررسی اینکه مشتری وجود دارد یا خیر
        //        var customer = await _context.Customers.FindAsync(purchaseHistoryDto.CustomerId);
        //        if (customer == null)
        //            return NotFound("Customer not found.");

        //        var purchaseHistory = _mapper.Map<PurchaseHistory>(purchaseHistoryDto);


        //        // افزودن سابقه خرید به پایگاه داده
        //        _genericRepository.AddAsync(purchaseHistory);
        //        await _genericRepository.SavechangeAsync();

        //        // تبدیل PurchaseHistory به PurchaseHistoryDto و بازگشت داده‌ها
        //        var result = _mapper.Map<PurchaseHistoryDto>(purchaseHistory);

        //        return CreatedAtAction(nameof(GetPurchaseHistory), new { id = purchaseHistory.CustomerId }, result);
        //    }

        //    private LoyaltyStatus GetLoyaltyStatus(decimal points)
        //    {
        //        if (points < 100) return LoyaltyStatus.Bronze;
        //        if (points < 500) return LoyaltyStatus.Silver;
        //        if (points < 1000) return LoyaltyStatus.Gold;
        //        return LoyaltyStatus.Platinum;
        //    }


        //    [HttpPost("add-points")]
        //    public async Task<IActionResult> AddPointsToCustomer(int customerId, decimal purchaseAmount)
        //    {
        //        var customer = await _context.Customers.FindAsync(customerId);
        //        if (customer == null) return NotFound("Customer not found.");

        //        customer.Points += purchaseAmount / 1000;
        //        customer.LoyaltyStatus = GetLoyaltyStatus(customer.Points);

        //        await _context.SaveChangesAsync();
        //        return Ok(new { customer.Points, customer.LoyaltyStatus });
        //    }
        //}
    }
}

