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
        public async Task AddAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
        }
        public async Task<IEnumerable<T>> GetallAsync()
        {
           return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<bool> SavechangeAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
        void IRepository<T>.Delete(T entity)
        {
            _context.Set<T>().Remove(entity);
        }

        void IRepository<T>.Update(T entity)
        {
            _context.Set<T>().Update(entity);
        }
    }
}
