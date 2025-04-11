using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTO.ReportDTO;

namespace Application.Services
{
    public interface IReportService
    {
        Task<List<PopularCarModelDto>> GetPopularCarModelsAsync();
        Task<List<LoyalCustomerDto>> GetLoyalCustomersAsync();
        Task<List<TopCustomerByAmountDto>> GetTopCustomersByAmountAsync();
        Task<List<NewCustomerDto>> GetNewCustomersAsync(int days = 30);

    }
}
