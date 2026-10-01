using System;
using IssueTracker.Core.Interfaces;

namespace IssueTracker.Core.Entities
{
    public class Issue : IBaseEntity
    {
        public int IssueID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string Priority { get; set; }
        public string AssignedTo { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int IsDeleted { get; set; }
    }
}
