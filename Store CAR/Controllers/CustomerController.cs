using AutoMapper;
using Carproject.Command;
using Carproject.DTO;
using Carproject.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Carproject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly CarStoreDbContext _context;
        private readonly IMapper _mapper;

        public CustomerController(CarStoreDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerDto>> GetCustomerById(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
                return NotFound("Customer not found.");

            // تبدیل به CustomerDto برای پاسخ
            var customerDto = _mapper.Map<CustomerDto>(customer);

            return Ok(customerDto);
        }






        [HttpPost("RegisterCustomer")]
        public async Task<ActionResult<Customer>> RegisterCustomer([FromBody] CommanCustomer model)
        {
            if (model == null)
                return BadRequest("Invalid data.");

            // بررسی اینکه آیا ایمیل یا شماره تلفن مشتری تکراری نیست
            if (await _context.Customers.AnyAsync(c => c.Email == model.Email || c.PhoneNumber == model.PhoneNumber))
                return BadRequest("Customer with this email or phone already exists.");

            // تبدیل مدل ورودی به مدل Customer
            var customer = _mapper.Map<Customer>(model);

            // افزودن مشتری جدید به پایگاه داده
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            // تبدیل مدل Customer به CustomerDto و بازگشت داده‌ها
            var customerDto = _mapper.Map<CustomerDto>(customer);

            // بازگشت پاسخ به کاربر
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerId }, customerDto);
        }



        [HttpPut("UpdateCustomer{id}")]
        public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerDto customerDto)
        {
            if (id != customerDto.CustomerId) return BadRequest();

            // تبدیل UpdateCustomerDto به Customer
            var customer = _mapper.Map<Customer>(customerDto);
            customer.CustomerId = id; // اطمینان از اینکه شناسه مشتری به درستی تنظیم می‌شود

            // تنظیم وضعیت entity به Modified
            _context.Entry(customer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Customers.Any(e => e.CustomerId == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();

            
        }


        [HttpGet("{id}/purchase-history")]
        public async Task<ActionResult<IEnumerable<PurchaseHistoryDto>>> GetPurchaseHistory(int id)
        {
            var customer = await _context.Customers
                .Include(c => c.PurchaseHistories)
                .FirstOrDefaultAsync(c => c.CustomerId == id);

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
            var customer = await _context.Customers.FindAsync(purchaseHistoryDto.CustomerId);
            if (customer == null)
                return NotFound("Customer not found.");

            var purchaseHistory = _mapper.Map<PurchaseHistory>(purchaseHistoryDto);


            // افزودن سابقه خرید به پایگاه داده
            _context.PurchaseHistories.Add(purchaseHistory);
            await _context.SaveChangesAsync();

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
            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null) return NotFound("Customer not found.");

            customer.Points += purchaseAmount / 1000;
            customer.LoyaltyStatus = GetLoyaltyStatus(customer.Points);

            await _context.SaveChangesAsync();
            return Ok(new { customer.Points, customer.LoyaltyStatus });
        }

    }

}
