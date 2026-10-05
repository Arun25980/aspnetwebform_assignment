using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Text;
using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data.Factories; // Ensure this namespace is imported
using IssueTracker.Data.Repositories;

namespace IssueTracker.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly DbContext _primaryContext;
        private readonly DbContext _archiveContext;
        private readonly Dictionary<Type, object> _repositories = new Dictionary<Type, object>();

        public UnitOfWork()
        {
            // Use DbContextFactory to instantiate contexts cleanly!
            _primaryContext = DbContextFactory.CreateContext("primary");
            _archiveContext = DbContextFactory.CreateContext("archive");
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!_repositories.ContainsKey(type))
            {
                // Dynamic routing: ArchiveIssue uses Archive Context, all others use Primary Context
                DbContext contextToUse = (type == typeof(ArchiveIssue))
                    ? _archiveContext
                    : _primaryContext;

                var repositoryInstance = new Repository<TEntity>(contextToUse);
                _repositories.Add(type, repositoryInstance);
            }

            return (IRepository<TEntity>)_repositories[type];
        }

        public int Complete()
        {
            try
            {
                int rowsAffected = 0;
                rowsAffected += _primaryContext.SaveChanges();
                rowsAffected += _archiveContext.SaveChanges();
                return rowsAffected;
            }
            catch (DbEntityValidationException ex)
            {
                var sb = new StringBuilder();
                foreach (var failure in ex.EntityValidationErrors)
                {
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendLine($"Property: {error.PropertyName} - Error: {error.ErrorMessage}");
                    }
                }
                throw new Exception("Entity Validation Failed:\n" + sb.ToString(), ex);
            }
        }

        public void Dispose()
        {
            _primaryContext?.Dispose();
            _archiveContext?.Dispose();
        }
    }
}