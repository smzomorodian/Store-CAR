using Domain.Model.File;
using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrustructure.Repository
{
    public class FileRepository : IFileRepository
    {
        private readonly CARdbcontext _context;

        public FileRepository(CARdbcontext context)
        {
            _context = context;
        }

        // دریافت یک فایل بر اساس شناسه
        public async Task<FileBase?> GetFileByIdAsync(Guid id)
        {
            return await _context.FileBase
                                 .FirstOrDefaultAsync(f => f.Id == id);
        }

        // دریافت تمام فایل‌ها
        public async Task<List<FileBase>> GetAllAsync()
        {
            return await _context.FileBase
                                 .ToListAsync();
        }

        // اضافه کردن یک فایل جدید
        public async Task AddFileAsync(FileBase file)
        {
            await _context.FileBase.AddAsync(file);
        }

        // حذف یک فایل
        public async Task DeleteFileAsync(FileBase file)
        {
            _context.FileBase.Remove(file);
        }

        // ذخیره‌سازی تغییرات در پایگاه داده
        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
