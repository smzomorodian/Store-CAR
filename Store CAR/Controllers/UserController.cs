using Application.Command.User.Command;
using Application.DTO;
using AutoMapper;
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
        private string secretKey;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        public UserController(IMediator mediator, IMapper mapper, IConfiguration configuration, IUserInfoRepository<Buyer> userInfoRepository, IRepository<Buyer> genericRepository)
        {
            secretKey = configuration.GetValue<string>("ApiSettings:Secret");
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("register/{userType}")]
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
                        Password = BCrypt.Net.BCrypt.HashPassword(registerbuyerDTO.Password),
                        Phonenmber = registerbuyerDTO.Phonenmber,
                        Email = registerbuyerDTO.Email,
                        Role = registerbuyerDTO.Role

                    };
                    result = await _mediator.Send(commandseller);
                    break;
                case "moder":
                    var commandmoder = new UserRegisterCommand<Moder>
                    {
                        Name = registerbuyerDTO.Name,
                        Age = registerbuyerDTO.Age,
                        National_Code = registerbuyerDTO.National_Code,
                        Password = BCrypt.Net.BCrypt.HashPassword(registerbuyerDTO.Password),
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
        public async Task<IActionResult> Login(string userType,[FromBody] LogingUserDTO logingUserDTO)
        {
            string Token;
            string userTypeclean = userType;
            switch (userTypeclean)
            {
                case "buyer":
                    var commandbuyer = new UserLoginCommand<Buyer>
                    {
                        Nationalcode = logingUserDTO.Nationalcode,
                        Password = logingUserDTO.Password
                    };
                    Token = await _mediator.Send(commandbuyer);
                    break;

                case "seller":
                    var commandseller = new UserLoginCommand<Seller>
                    {
                        Nationalcode = logingUserDTO.Nationalcode,
                        Password = logingUserDTO.Password
                    };
                    Token = await _mediator.Send(commandseller);
                    break;
                case "moder":
                    var commandmoder = new UserLoginCommand<Moder>
                    {
                        Nationalcode = logingUserDTO.Nationalcode,
                        Password = logingUserDTO.Password
                    };
                    Token = await _mediator.Send(commandmoder);
                    break;
                default:
                    return BadRequest("نوع کاربر نامعتبر است.");
            }
            return Ok(new { token = Token });
        }

        [HttpPost("RequestPasswordReset/{userType}")]
        public async Task<IActionResult> RequestPasswordReset(string userType, [FromQuery] string nationalCode)
        {
            string userTypeClean = userType.Trim().ToLower();
            string otp;

            switch (userTypeClean)
            {
                case "buyer":
                    var commandbuyer = new RequestPasswordResetCommand<Buyer> { nationalCode = nationalCode };
                    otp = await _mediator.Send(commandbuyer);
                    break;
                case "seller":
                    var commandseller = new RequestPasswordResetCommand<Seller> { nationalCode = nationalCode };
                    otp = await _mediator.Send(commandseller);
                    break;
                case "moder":
                    var commandmoder = new RequestPasswordResetCommand<Moder> { nationalCode = nationalCode };
                    otp = await _mediator.Send(commandmoder);
                    break;
                default:
                    return BadRequest("نوع کاربر نامعتبر است.");
            }

            return Ok("کد موقت ارسال شد");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string userType , [FromBody] ChangepasswordDTo request)
        {
            string userTypeClean = userType.Trim().ToLower();
            string result;

            switch (userTypeClean)
            {
                case "buyer":
                    var commandbuyer = new ChangePasswordcommand<Buyer>
                    {
                        NationalCode = request.NationalCode,
                        Otp = request.Otp,
                        NewPassword = request.NewPassword
                    };
                    result = await _mediator.Send(commandbuyer);
                    break;
                case "seller":
                    var commandseller = new ChangePasswordcommand<Seller>
                    {
                        NationalCode = request.NationalCode,
                        Otp = request.Otp,
                        NewPassword = request.NewPassword
                    };
                    result = await _mediator.Send(commandseller);
                    break;
                case "moder":
                    var commandmoder = new ChangePasswordcommand<Moder>
                    {
                        NationalCode = request.NationalCode,
                        Otp = request.Otp,
                        NewPassword = request.NewPassword
                    };
                    result = await _mediator.Send(commandmoder);
                    break;
                default:
                    return BadRequest("نوع کاربر نامعتبر است.");
            }
            return Ok("change passwod is succesfuly");

        }

        // Two-step authentication
        [HttpPost("request-otp")]
        public async Task<IActionResult> RequestOtp([FromBody] RequestOtpDTO request)
        {
            if (request.UserType.ToLower() == "buyer")
            {
                var command = new RequestOtpCommand<Buyer>(request.UserType, request.Phonenumber);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            else if (request.UserType.ToLower() == "seller")
            {
                var command = new RequestOtpCommand<Seller>(request.UserType, request.Phonenumber);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            return BadRequest("نوع کاربر نامعتبر است");
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDTO verifyRequest)
        {
            if (verifyRequest.UserType.ToLower() == "buyer")
            {
                var command = new VerifyOtpCommand<Buyer>(verifyRequest.UserType, verifyRequest.PhoneNumber , verifyRequest.otp);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            if (verifyRequest.UserType.ToLower() == "seller")
            {
                var command = new VerifyOtpCommand<Seller>(verifyRequest.UserType, verifyRequest.PhoneNumber, verifyRequest.otp);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            return BadRequest("نوع کاربر نامعتبر است");
        }
        //---------------------------------------------

        [HttpGet("checkUser")]
        public async Task<IActionResult> checkbuyer(string nationalcode , string UserType)
        {
            if (UserType.ToLower() == "buyer")
            {
                var command = new checkUserCommand<Buyer>(nationalcode, UserType);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            if (UserType.ToLower() == "seller")
            {
                var command = new checkUserCommand<Seller>(nationalcode, UserType);
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            return BadRequest("نوع کاربر نامعتبر است");
        }

        [HttpPut("EditInformation")]
        public async Task<IActionResult> editinformation(string nationalcode, string UserType, [FromBody] RegisterbuyerDTO registerbuyerDTO)
        {
            if(UserType.ToLower() == "buyer")
            {
                var command = new EditInformationCommand<Buyer>()
                {
                    Name = registerbuyerDTO.Name,
                    Age = registerbuyerDTO.Age,
                    National_Code = registerbuyerDTO.National_Code,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerbuyerDTO.Password),
                    Phonenmber = registerbuyerDTO.Phonenmber,
                    Email = registerbuyerDTO.Email,
                    Role = registerbuyerDTO.Role
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            if (UserType.ToLower() == "seller")
            {
                var command = new EditInformationCommand<Seller>()
                {
                    Name = registerbuyerDTO.Name,
                    Age = registerbuyerDTO.Age,
                    National_Code = registerbuyerDTO.National_Code,
                    Password = BCrypt.Net.BCrypt.HashPassword(registerbuyerDTO.Password),
                    Phonenmber = registerbuyerDTO.Phonenmber,
                    Email = registerbuyerDTO.Email,
                    Role = registerbuyerDTO.Role
                };
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            return BadRequest("نوع کاربر نامعتبر است");
        }


        //[HttpGet("{id}/purchase-history")]
        //public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetPurchaseHistory(int id)
        //{
        //    var customer = await _context.Customers
        //        .Include(c => c.PurchaseHistories)
        //        .FirstOrDefaultAsync(c => c.CustomerId == id);

        //    if (customer == null)
        //        return NotFound("Customer not found.");

        //    // تبدیل اطلاعات مشتری به CustomerDto
        //    var customerDto = _mapper.Map<CustomerDto>(customer);

        //    // دریافت تاریخچه خرید
        //    var purchaseHistoryDtos = _mapper.Map<List<PurchaseHistoryDto>>(customer.PurchaseHistories);

        //    // برگرداندن اطلاعات مشتری و تاریخچه خرید
        //    return Ok(new
        //    {
        //        Customer = customerDto,
        //        PurchaseHistory = purchaseHistoryDtos
        //    });
        //}



        //[HttpPost("add-purchase-history")]
        //public async Task<ActionResult<PurchaseHistoryDto>> AddPurchaseHistory([FromBody] PurchaseHistoryDto purchaseHistoryDto)
        //{
        //    // بررسی اینکه مشتری وجود دارد یا خیر
        //    var customer = await _context.Customers.FindAsync(purchaseHistoryDto.CustomerId);
        //    if (customer == null)
        //        return NotFound("Customer not found.");

        //    var purchaseHistory = _mapper.Map<PurchaseHistory>(purchaseHistoryDto);


        //    // افزودن سابقه خرید به پایگاه داده
        //    _genericRepository.AddAsync(purchaseHistory);
        //    await _genericRepository.SavechangeAsync();

        //    // تبدیل PurchaseHistory به PurchaseHistoryDto و بازگشت داده‌ها
        //    var result = _mapper.Map<PurchaseHistoryDto>(purchaseHistory);

        //    return CreatedAtAction(nameof(GetPurchaseHistory), new { id = purchaseHistory.CustomerId }, result);
        //}

        //private LoyaltyStatus GetLoyaltyStatus(decimal points)
        //{
        //    if (points < 100) return LoyaltyStatus.Bronze;
        //    if (points < 500) return LoyaltyStatus.Silver;
        //    if (points < 1000) return LoyaltyStatus.Gold;
        //    return LoyaltyStatus.Platinum;
        //}


        //[HttpPost("add-points")]
        //public async Task<IActionResult> AddPointsToCustomer(int customerId, decimal purchaseAmount)
        //{
        //    var customer = await _context.Customers.FindAsync(customerId);
        //    if (customer == null) return NotFound("Customer not found.");

        //    customer.Points += purchaseAmount / 1000;
        //    customer.LoyaltyStatus = GetLoyaltyStatus(customer.Points);

        //    await _context.SaveChangesAsync();
        //    return Ok(new { customer.Points, customer.LoyaltyStatus });
        //}
    }
}


