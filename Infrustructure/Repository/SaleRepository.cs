using Carproject.Model;
using Domain.Model;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository
{
    public class SaleRepository : ISaleRepository
    {
        private readonly CARdbcontext _context;

        public SaleRepository(CARdbcontext context)
        {
            _context = context;
        }
        public async Task<Sale> AddSaleAsync(Sale sale)
        {
            await _context.AddAsync(sale);
            return (sale);
        }

        public async Task<Sale> GetBSaleByIdAsync(Guid selerId)
        {
            var salepay = await _context.Sales.FindAsync(selerId);
            return (salepay);
        }

        public async Task<Buyer> GetBuyerByIdAsync(Guid buyerId)
        {
            var buyer = await _context.buyers.FindAsync(buyerId);
            return buyer;
        }

        public async Task<FileBaseSale> getimagesalefactor(Filesale filesale)
        {
            //await _context.FileBase.AddAsync(filesale);
            return (filesale);
        }

        //Task<FileBase> ISaleRepository.getimagesalefactor(Filesale filesale)
        //{
        //    //    await _context.FileBase.AddAsync(filesale);
        //    //    return (filesale);

        //}
    }
}
