using System.Data.Entity;
using IssueTracker.Core.Entities;

namespace IssueTracker.Data
{
    public class ArchiveIssueEntities : DbContext
    {
        // Pass the EDMX connection string name
        public ArchiveIssueEntities() : base("name=ArchiveIssueEntities")
        {
            Database.SetInitializer<ArchiveIssueEntities>(null);
        }

        public DbSet<Issue> Issues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Explicitly map your shared POCO entity class to the table in EDMX
            modelBuilder.Entity<Issue>().ToTable("Issues");
        }
    }
}