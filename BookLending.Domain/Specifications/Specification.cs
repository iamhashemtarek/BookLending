using BookLending.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BookLending.Domain.Specifications
{
    public class Specification<T> : ISpecification<T> where T : BaseEntity
    {
        public Expression<Func<T, bool>> Criteria { get; set; }
        public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; }
        public Expression<Func<T, object>> OrderByDesc { get; set; }

        public int Take { get; set; }
        public int Skip { get; set; }

        public Specification()
        {

        }

        public Specification(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }
        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }
        public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
        {
            OrderByDesc = orderByDescExpression;
        }
        public void AddCriteria(Expression<Func<T, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }
        public void AddInclude(Expression<Func<T, object>> includeExpression)
        {
            Includes.Add(includeExpression);
        }
        public void AddPaging(int pageNumber, int pageSize)
        {
            Skip = (pageNumber - 1) * pageSize;
            Take = pageSize;
        }

        //gpt ****************
        public void AddSorting(string sortBy, bool isSortAscending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
                return;

            var parameter = Expression.Parameter(typeof(T), "x"); //defines the parameter x =>
            var property = Expression.PropertyOrField(parameter, sortBy); // gets the property value dynamically
            var converted = Expression.Convert(property, typeof(object)); // box value type if needed
            var expression = Expression.Lambda<Func<T, object>>(converted, parameter); //wraps it into expression

            if (isSortAscending)
            {
                AddOrderBy(expression);
            }
            else
            {
                AddOrderByDesc(expression);
            }
        }
    }
}
