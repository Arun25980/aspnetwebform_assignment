using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using IssueTracker.Core.Interfaces;

namespace IssueTracker.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected virtual DbSet<TEntity> DbSet => Context.Set<TEntity>();

        public virtual IQueryable<TEntity> GetAll()
        {
            return Context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetPaged<TKey>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TKey>> orderBy,
            bool descending,
            int pageIndex,
            int pageSize,
            out int totalCount)
        {
            // Use IQueryable throughout so the underlying EF provider can translate
            // the expression to SQL and the model/type mapping is validated by EF.
            var queryable = Context.Set<TEntity>().Where(predicate);

            totalCount = queryable.Count();

            var ordered = descending ? queryable.OrderByDescending(orderBy) : queryable.OrderBy(orderBy);

            return ordered.Skip(pageIndex * pageSize).Take(pageSize);
        }

        /// <summary>
        /// Finds entities matching the predicate without triggering EF Metadata Workspace errors.
        /// </summary>
        public virtual IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            // Return an IQueryable so EF can translate the expression tree to SQL.
            // Avoid compiling the predicate and pulling data into memory which
            // can mask model/CLR type mismatches and causes inefficient queries.
            return Context.Set<TEntity>().Where(predicate);
        }

        public virtual TEntity GetById(object id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public virtual void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
        }

        public virtual void Update(TEntity entity)
        {
            var entry = Context.Entry(entity);
            if (entry.State == EntityState.Detached)
            {
                Context.Set<TEntity>().Attach(entity);
                entry.State = EntityState.Modified;
            }
        }

        public virtual void Remove(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
        }

        public virtual void SoftDelete(object id)
        {
            var entity = GetById(id);
            if (entity != null)
            {
                var prop = entity.GetType().GetProperty("IsDeleted");
                if (prop != null && prop.CanWrite)
                {
                    var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;

                    if (targetType == typeof(bool))
                    {
                        prop.SetValue(entity, true);
                    }
                    else if (targetType == typeof(int) || targetType == typeof(byte) || targetType == typeof(short))
                    {
                        prop.SetValue(entity, Convert.ChangeType(1, targetType));
                    }

                    Update(entity);
                }
                else
                {
                    Remove(entity);
                }
            }
        }
    }
}