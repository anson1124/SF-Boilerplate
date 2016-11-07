﻿using Microsoft.EntityFrameworkCore;
using SimpleFramework.Core.Abstraction.Data;
using SimpleFramework.Core.Abstraction.Data.UnitOfWork;
using SimpleFramework.Core.Abstraction.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleFramework.Core.Data
{
    public class RepositoryAsync<TEntity> : Repository<TEntity>, IRepositoryAsync<TEntity>
       where TEntity : BaseEntity
    {
        private readonly IUnitOfWorkAsync _unitOfWork;
        public RepositoryAsync(DbContext context, IUnitOfWorkAsync unitOfWork) : base(context, unitOfWork)
        {

        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.ToListAsync();
        }

        public virtual async Task<TEntity> FindAsync(params object[] keyValues) => await DbSet.FindAsync(keyValues);

        public virtual async Task<TEntity> FindAsync(CancellationToken cancellationToken, params object[] keyValues) => await DbSet.FindAsync(cancellationToken, keyValues);

        public virtual async Task<bool> DeleteAsync(params object[] keyValues) => await DeleteAsync(CancellationToken.None, keyValues);

        public virtual async Task<bool> DeleteAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            var entity = await FindAsync(cancellationToken, keyValues);

            if (entity == null)
            {
                return false;
            }

            Context.Entry(entity).State = EntityState.Deleted;
            Context.Set<TEntity>().Attach(entity);
            Context.SaveChanges();
            return true;
        }
        public virtual async Task<IEnumerable<TEntity>> FilterAsync(
                  Expression<Func<TEntity, bool>> predicate = null,
                  Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                  List<Expression<Func<TEntity, object>>> includes = null,
                  int? page = null,
                  int? pageSize = null)
        {
            return await Select(predicate, orderBy, includes, page, pageSize).ToListAsync();
        }

        public virtual Task<bool> AnyAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.AnyAsync();
        }

        public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.CountAsync();
        }
    }
}
