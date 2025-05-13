using BookLending.Domain.Entities;
using BookLending.Domain.Specifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Interfaces
{
    public interface IWriteRepository<T> where T : BaseEntity
    {
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<IEnumerable<T>> GetAllAsyncTracked();
        Task<IReadOnlyList<T>> GetAllWithSpecAsyncTracked(ISpecification<T> spec);
        Task<T?> GetByIdAsyncTracked(int id);
        Task<T?> GetWithSpecAsyncTracked(ISpecification<T> spec);
    }
}
