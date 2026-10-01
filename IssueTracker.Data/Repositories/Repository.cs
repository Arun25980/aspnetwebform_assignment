using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using IssueTracker.Core.Interfaces;

namespace IssueTracker.Data.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(DbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            DbSet = Context.Set<TEntity>();
        }

        public virtual TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual IEnumerable<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Queryable.Where(DbSet, predicate).ToList();
        }

        public virtual void Add(TEntity entity)
        {
            DbSet.Add(entity);
            Context.SaveChanges(); // Persists instantly to SQL LocalDB
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Attach(entity);
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges(); // Persists updates instantly
        }

        public virtual void SoftDelete(object id)
        {
            var entity = GetById(id);
            if (entity == null) return;

            if (entity is IBaseEntity softDeletable)
            {
                softDeletable.IsDeleted = 1;
                Update(entity);
            }
            else
            {
                // Reflection fallback to check for IsDeleted property
                var prop = entity.GetType().GetProperty("IsDeleted");
                if (prop != null && prop.CanWrite)
                {
                    prop.SetValue(entity, 1);
                    Update(entity);
                }
                else
                {
                    // Fallback to hard delete if entity is not soft-deletable
                    DbSet.Remove(entity);
                }
            }
        }
    }
}