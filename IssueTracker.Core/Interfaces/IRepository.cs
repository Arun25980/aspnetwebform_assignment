using System;
using System.Linq;
using System.Linq.Expressions;

namespace IssueTracker.Core.Interfaces
{
    /// <summary>
    /// Generic repository contract providing operations over IQueryable.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Returns all entities as an IQueryable.
        /// </summary>
        IQueryable<TEntity> GetAll();

        /// <summary>
        /// Finds entities matching the predicate as an IQueryable.
        /// </summary>
        /// <param name="predicate">Filter expression.</param>
        IQueryable<TEntity> Find(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Fetches a paged subset of entities using server-side database paging.
        /// </summary>
        IQueryable<TEntity> GetPaged<TKey>(
            Expression<Func<TEntity, bool>> predicate,
            Expression<Func<TEntity, TKey>> orderBy,
            bool descending,
            int pageIndex,
            int pageSize,
            out int totalCount);

        /// <summary>
        /// Finds an entity by primary key object ID.
        /// </summary>
        TEntity GetById(object id);

        /// <summary>
        /// Adds a new entity.
        /// </summary>
        void Add(TEntity entity);

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        void Update(TEntity entity);

        /// <summary>
        /// Removes an entity.
        /// </summary>
        void Remove(TEntity entity);

        /// <summary>
        /// Soft deletes an entity by setting its IsDeleted flag.
        /// </summary>
        void SoftDelete(object id);
    }
}