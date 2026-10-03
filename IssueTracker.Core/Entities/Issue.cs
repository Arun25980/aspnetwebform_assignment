using System;
using System.ComponentModel.DataAnnotations;
using IssueTracker.Core.Interfaces;
using System.ComponentModel.DataAnnotations;
namespace IssueTracker.Core.Entities
{
    public class Issue : IBaseEntity
    {
        public int IssueID { get; set; }

        [Required(ErrorMessage = "Title is required.", AllowEmptyStrings = false)]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.", AllowEmptyStrings = false)]
        public string Description { get; set; }

        public string Status { get; set; }

        [Required(ErrorMessage = "Please select a priority.", AllowEmptyStrings = false)]
        public string Priority { get; set; }

        [Required(ErrorMessage = "Assigned person is required.", AllowEmptyStrings = false)]
        public string AssignedTo { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int IsDeleted { get; set; }
    }
}