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
              
                default:
                    throw new ArgumentException("Unknown DbContext key specified.");
            }
        }
    }
}