using Domain.Model;
using Domain.Model.File;
using Domain.Model.ReportNotifModel;
using Domain.Model.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository.IRepository
{
    public interface IFileRepository
    {
        Task<FileBase?> GetFileByIdAsync(Guid id);
        Task<List<FileBase>> GetAllAsync();
        Task AddFileAsync(FileBase file);
        Task DeleteFileAsync(FileBase file);
        Task SaveAsync();
    }
}
