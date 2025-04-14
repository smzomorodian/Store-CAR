using Carproject.Model;
using Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository.IRepository
{
    public interface ISaleRepository
    {
        Task<Sale> AddSaleAsync(Sale sale);
        Task<Buyer> GetBuyerByIdAsync(Guid buyerId);
        Task<Sale> GetBSaleByIdAsync(Guid selerId);
    }
}
