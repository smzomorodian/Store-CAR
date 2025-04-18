using Domain.Model.ReportNotifModel;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrustructure.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly CARdbcontext _context;

        public SaleRepository(CARdbcontext context)
        {
            _context = context;
        }

        // پیاده‌سازی متد GetSaleByIdAsync
        public async Task<Sale?> GetSaleByIdAsync(Guid saleId)
        {
            return await _context.Sales
               .FirstOrDefaultAsync(s => s.Id == saleId);  // جستجو بر اساس saleId
        }                  

        // سایر متدهای Repository
        public async Task<List<Sale>> GetAllSalesAsync()
        {
            return await _context.Sales
                                 .ToListAsync();
        }

        public async Task<Sale> AddSaleAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
            return sale;
        }

        public async Task UpdateSaleAsync(Sale sale)
        {
            _context.Sales.Update(sale);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSaleAsync(Sale sale)
        {
            _context.Sales.Remove(sale);
            await _context.SaveChangesAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
