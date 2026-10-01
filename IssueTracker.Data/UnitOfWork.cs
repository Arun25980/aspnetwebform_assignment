using System;
using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data.Repositories;

namespace IssueTracker.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly PrimaryIssueEntities _primaryContext;
        private readonly ArchiveIssueEntities _archiveContext;

        public UnitOfWork()
        {
            _primaryContext = new PrimaryIssueEntities();
            _archiveContext = new ArchiveIssueEntities();

            // Pass the primary context explicitly to the primary Issue repository
            Issues = new Repository<IssueTracker.Core.Entities.Issue>(_primaryContext);
            ArchiveIssues = new Repository<ArchiveIssue>(_archiveContext);
        }

        public IRepository<Issue> Issues { get; private set; }
        public IRepository<ArchiveIssue> ArchiveIssues { get; private set; }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : class
        {
            if (typeof(TEntity) == typeof(Issue))
            {
                return (IRepository<TEntity>)Issues;
            }

            if (typeof(TEntity) == typeof(ArchiveIssue))
            {
                return (IRepository<TEntity>)ArchiveIssues;
            }

            throw new ArgumentException($"No repository configured for entity type '{typeof(TEntity).Name}'.");
        }

        public int Complete()
        {
            return _primaryContext.SaveChanges() + _archiveContext.SaveChanges();
        }

        public void Dispose()
        {
            _primaryContext?.Dispose();
            _archiveContext?.Dispose();
        }
    }
}