using BookLending.Domain.Entities;
using BookLending.Domain.Interfaces;
using BookLending.Infrastructure.Persistence;
using BookLending.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Infrastructure.UnitOfWork
{
    class UnitOfWork : IUnitOfWork
    {
        private readonly BookLendingDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        public UnitOfWork(BookLendingDbContext context)
        {
            _context = context;
        }
        public IGenericRepository<T> Repository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<T>(_context);
                _repositories[type] = repositoryInstance;
            }

            return (IGenericRepository<T>)_repositories[type];
        }
        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
