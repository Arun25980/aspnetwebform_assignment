using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IssueTracker.Core.Entities
{
    /// <summary>
    /// Represents an issue record within the Issue Tracker system.
    /// </summary>
    [Table("Issues")]
    public class Issue
    {
        /// <summary>
        /// Gets or sets the primary key identifier for the issue.
        /// </summary>
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IssueID { get; set; }

        /// <summary>
        /// Gets or sets the issue title.
        /// </summary>
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the detailed description of the issue.
        /// </summary>
        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the issue priority (e.g., Low, Medium, High).
        /// </summary>
        [Required(ErrorMessage = "Priority is required.")]
        [StringLength(20)]
        public string Priority { get; set; }

        /// <summary>
        /// Gets or sets the individual assigned to resolve the issue.
        /// </summary>
        [Required(ErrorMessage = "Assigned person is required.")]
        [StringLength(100)]
        public string AssignedTo { get; set; }

        /// <summary>
        /// Gets or sets the UTC creation timestamp.
        /// </summary>
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the soft-delete indicator flag (0 = Active, 1 = Archived/Deleted).
        /// </summary>
        public int IsDeleted { get; set; } = 0;
    }
}