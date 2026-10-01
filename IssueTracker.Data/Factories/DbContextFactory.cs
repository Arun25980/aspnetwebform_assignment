using System;
using System.Data.Entity;

namespace IssueTracker.Data.Factories
{
    public static class DbContextFactory
    {
        public static DbContext CreateContext(string keyName)
        {
            switch (keyName.ToLower())
            {
                case "primary":
                    return new PrimaryIssueEntities(); // Direct EDMX Context 1
                case "archive":
                    return new ArchiveIssueEntities(); // Direct EDMX Context 2
                default:
                    throw new ArgumentException("Unknown DbContext key specified.");
            }
        }
    }
}