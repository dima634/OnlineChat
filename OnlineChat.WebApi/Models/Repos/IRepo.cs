using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public interface IRepo<T>
    {
        int Add(T entity);
        Task<int> AddAsync(T entity);
        int AddRange(IList<T> entities);
        Task<int> AddRangeAsync(IList<T> entities);
        int Save(T entity);
        int SaveRange(IList<T> entity);
        Task<int> SaveAsync(T entity);
        int Delete(T entity);
        Task<int> DeleteAsync(T entity);
        T GetOne(object id);
        List<T> GetAll();
    }
}
