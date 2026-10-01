using System.Data.Entity;
using IssueTracker.Core.Entities;

namespace IssueTracker.Data
{
    public class ArchiveIssueEntities : DbContext
    {
        public ArchiveIssueEntities() : base("name=ArchiveIssueEntities")
        {
            Database.SetInitializer<ArchiveIssueEntities>(null);
        }

        public DbSet<ArchiveIssue> ArchiveIssues { get; set; }
    }
}