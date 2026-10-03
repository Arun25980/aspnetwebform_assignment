using System.Data.Entity;
using IssueTracker.Core.Entities;

namespace IssueTracker.Data
{
    public class PrimaryIssueEntities : DbContext
    {
        public PrimaryIssueEntities() : base("name=PrimaryIssueEntities")
        {
            // Disables Code First migrations since table is already defined in LocalDB .mdf
            Database.SetInitializer<PrimaryIssueEntities>(null);
            this.Configuration.ValidateOnSaveEnabled = false;
        }

        public DbSet<Issue> Issues { get; set; }
    }
}