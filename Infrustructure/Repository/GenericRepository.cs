using Infrustructure.Context;
using Infrustructure.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrustructure.Repository
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly CARdbcontext _context;

        public GenericRepository(CARdbcontext context)
        {
            _context = context;
        }

        // اضافه کردن موجودیت به دیتابیس
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }

        // دریافت همه موجودیت‌ها از دیتابیس
        public async Task<IEnumerable<T>> GetallAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // دریافت موجودیت بر اساس id
        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        // ذخیره تغییرات در دیتابیس و برگرداندن موفقیت
        public async Task<bool> SavechangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        // حذف موجودیت از دیتابیس
        void IRepository<T>.Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        // به‌روزرسانی موجودیت در دیتابیس
        void IRepository<T>.Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
