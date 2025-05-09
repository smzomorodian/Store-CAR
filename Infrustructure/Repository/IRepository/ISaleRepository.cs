﻿using Domain.Model;
using Domain.Model.ReportNotifModel;
using Domain.Model.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository.IRepository
{
    public interface ISaleRepository
    {
        Task<Sale?> GetSaleByIdAsync(Guid saleId); 
        Task<List<Sale>> GetAllSalesAsync();
        Task<Sale> AddSaleAsync(Sale sale);
        Task UpdateSaleAsync(Sale sale);
        Task DeleteSaleAsync(Sale sale);
        Task SaveAsync();
    }
}
