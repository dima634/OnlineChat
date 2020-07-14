using Castle.DynamicProxy.Generators.Emitters;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace OnlineChat.WebApi.Models.Repos
{
    public class BaseRepo<T> : IDisposable, IRepo<T> where T : class, new()
    {
        public OnlineChatDatabaseContext Context { get; }

        protected DbSet<T> Table;

        public BaseRepo(OnlineChatDatabaseContext applicationDbContext)
        {
            Context = applicationDbContext;
        }

        virtual public T GetOne(object id) => Table.Find(id);
        virtual public List<T> GetAll() => Table.ToList();

        public int Add(T entity)
        {
            Table.Add(entity);
            return SaveChanges();
        }
        public Task<int> AddAsync(T entity)
        {
            Table.Add(entity);
            return SaveChangesAsync();
        }
        public int AddRange(IList<T> entities)
        {
            Table.AddRange(entities);
            return SaveChanges();
        }
        public Task<int> AddRangeAsync(IList<T> entities)
        {
            Table.AddRange(entities);
            return SaveChangesAsync();
        }

        bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                Context.Dispose();
                // Free any managed objects here.
                //
            }
            // Free any unmanaged objects here.
            //
            disposed = true;
        }

        public int Save(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChanges();
        }

        public Task<int> SaveAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return SaveChangesAsync();
        }

        public int Delete(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return SaveChanges();
        }
        public Task<int> DeleteAsync(T entity)
        {
            Context.Entry(entity).State = EntityState.Deleted;
            return SaveChangesAsync();
        }

        internal int SaveChanges()
        {
            return Context.SaveChanges();
        }

        internal async Task<int> SaveChangesAsync()
        {
            return await Context.SaveChangesAsync();
        }

        public int SaveRange(IList<T> entities)
        {
            foreach (var entity in entities)
            {
                Context.Entry(entity).State = EntityState.Modified;
            }
            
            return SaveChanges();
        }
    }
}
