using System.Data.Entity;
using IssueTracker.Core.Entities; // Explicitly import Core Entities

namespace IssueTracker.Data
{
    public class PrimaryIssueEntities : DbContext
    {
        public PrimaryIssueEntities() : base("name=PrimaryIssueEntities")
        {
            Database.SetInitializer<PrimaryIssueEntities>(null);
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        // Must match IssueTracker.Core.Entities.Issue
        public DbSet<Issue> Issues { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Force explicit EF model registration for Issue entity
            modelBuilder.Entity<Issue>().ToTable("Issues");
        }
    }
}