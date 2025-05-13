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
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BookLendingDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private readonly Dictionary<Type, object> _readRepositories = new();
        private readonly Dictionary<Type, object> _writeRepositories = new();
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
        public IReadRepository<T> ReadRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (!_readRepositories.ContainsKey(type))
            {
                var repositoryInstance = new ReadRepository<T>(_context);
                _readRepositories[type] = repositoryInstance;
            }
            return (IReadRepository<T>)_readRepositories[type];
        }
        public IWriteRepository<T> WriteRepository<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (!_writeRepositories.ContainsKey(type))
            {
                var repositoryInstance = new WriteRepository<T>(_context);
                _writeRepositories[type] = repositoryInstance;
            }
            return (IWriteRepository<T>)_writeRepositories[type];
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
