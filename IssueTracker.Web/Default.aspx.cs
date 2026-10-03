using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Core.Validation;
using IssueTracker.Data;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.WebForms;

namespace IssueTracker.Web
{
    public partial class Default : Page
    {
        [Dependency]
        public IRepository<Issue> IssueRepository { get; set; }

        [Dependency("ArchiveRepository")]
        public IRepository<ArchiveIssue> ArchiveRepository { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            // Resolve directly from Unity container if property injection is skipped
          
            var container = HttpContext.Current.Application.GetContainer();
            if (container != null)
            {
                if (IssueRepository == null)
                    IssueRepository = container.Resolve<IRepository<Issue>>();

                if (ArchiveRepository == null)
                    ArchiveRepository = container.Resolve<IRepository<ArchiveIssue>>("ArchiveRepository");
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        private void BindGrid(string searchKeyword = "", string searchBy = "All")
        {
            if (IssueRepository == null) return;

            var query = IssueRepository.Find(i => i.IsDeleted == 0).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();

                switch (searchBy)
                {
                    case "Title":
                        query = query.Where(i => i.Title != null && i.Title.ToLower().Contains(searchKeyword));
                        break;
                    case "Priority":
                        query = query.Where(i => i.Priority != null && i.Priority.ToLower().Contains(searchKeyword));
                        break;
                    case "AssignedTo":
                        query = query.Where(i => i.AssignedTo != null && i.AssignedTo.ToLower().Contains(searchKeyword));
                        break;
                    default:
                        query = query.Where(i => (i.Title != null && i.Title.ToLower().Contains(searchKeyword)) ||
                                                 (i.Priority != null && i.Priority.ToLower().Contains(searchKeyword)) ||
                                                 (i.AssignedTo != null && i.AssignedTo.ToLower().Contains(searchKeyword)));
                        break;
                }
            }

            gvIssues.DataSource = query.OrderByDescending(i => i.IssueID).ToList();
            gvIssues.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            BindGrid(txtSearch.Text, ddlSearchBy.SelectedValue);
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlSearchBy.SelectedIndex = 0;
            BindGrid();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            // 1. Clear previous errors
            lblModalError.Visible = false;
            lblModalError.Text = string.Empty;

            // 2. Server-side Validation
            List<string> errors = new List<string>();

            if (string.IsNullOrWhiteSpace(txtTitle.Text))
                errors.Add("Title is required.");

            if (string.IsNullOrWhiteSpace(txtDescription.Text))
                errors.Add("Description is required.");

            if (string.IsNullOrWhiteSpace(ddlPriority.SelectedValue))
                errors.Add("Please select a priority.");

            if (string.IsNullOrWhiteSpace(txtAssignedTo.Text))
                errors.Add("Assigned person is required.");

            if (errors.Count > 0)
            {
                lblModalError.Text = "<strong>Please fix the following errors:</strong><ul class='mb-0 ps-3'>" +
                    string.Join("", errors.Select(err => "<li>" + err + "</li>")) + "</ul>";
                lblModalError.Visible = true;

                upModal.Update(); // Keeps data in textboxes and shows red alert box
                return;
            }

            // 3. Perform Insert OR Update based on hfIssueID
            SaveOrUpdateIssue();

            // 4. Refresh GridView and update its UpdatePanel
            BindGrid();
            upGrid.Update();

            // 5. Close Modal and clean up backdrop
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModalScript", "closeModal();", true);
        }

        private void SaveOrUpdateIssue()
        {
            // Determine if this is an Edit or Add New
            int issueID = 0;
            bool isEdit = int.TryParse(hfIssueID.Value, out issueID) && issueID > 0;

            if (isEdit)
            {
                // UPDATE existing issue
                var issue = IssueRepository.GetById(issueID);
                if (issue != null)
                {
                    issue.Title = txtTitle.Text.Trim();
                    issue.Description = txtDescription.Text.Trim();
                    issue.Priority = ddlPriority.SelectedValue;
                    issue.AssignedTo = txtAssignedTo.Text.Trim();

                    IssueRepository.Update(issue); // Call your repository/database Update method
                }
            }
            else
            {
                // INSERT new issue
                var newIssue = new Issue
                {
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Priority = ddlPriority.SelectedValue,
                    AssignedTo = txtAssignedTo.Text.Trim(),
                    CreatedDate = DateTime.Now
                };

                IssueRepository.Add(newIssue); // Call your repository/database Insert method
            }
        }

        protected void gvIssues_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditIssue")
            {
                int issueID = Convert.ToInt32(e.CommandArgument);

                // Fetch data using repository
                var issue = IssueRepository.GetById(issueID);

                // 1. Reset error label state
                lblModalError.Visible = false;
                lblModalError.Text = string.Empty;

                if (issue != null)
                {
                    // 2. Populate form controls
                    hfIssueID.Value = issue.IssueID.ToString();
                    txtTitle.Text = issue.Title;
                    txtDescription.Text = issue.Description;

                    if (ddlPriority.Items.FindByValue(issue.Priority) != null)
                    {
                        ddlPriority.SelectedValue = issue.Priority;
                    }
                    else
                    {
                        ddlPriority.SelectedIndex = 0;
                    }

                    txtAssignedTo.Text = issue.AssignedTo;

                    // 3. Force the UpdatePanel to sync and refresh its controls
                    upModal.Update();

                    // 4. Open Modal via ScriptManager (works with UpdatePanel)
                    ScriptManager.RegisterStartupScript(
                        this,
                        this.GetType(),
                        "OpenModal",
                        "openModal();",
                        true
                    );
                }
            }
            else if (e.CommandName == "DeleteIssue")
            {
                int issueId = Convert.ToInt32(e.CommandArgument);
                var issue = IssueRepository.GetById(issueId);

                if (issue != null)
                {
                    // 1. Map to Archive entity
                    var archiveRecord = new ArchiveIssue
                    {
                        OriginalIssueId = issue.IssueID,
                        Title = issue.Title,
                        Description = issue.Description,
                        Status = issue.Status,
                        Priority = issue.Priority,
                        ArchivedDate = DateTime.Now
                    };

                    // 2. Save record to Archive database
                    ArchiveRepository.Add(archiveRecord);

                    // 3. Perform Soft Delete in Primary database
                    IssueRepository.SoftDelete(issueId);
                    // Note: If SoftDelete inside your repository sets IsDeleted = 1 internally, 
                    // you can use: IssueRepository.SoftDelete(issueId);
                    // Otherwise keep:
                    // issue.IsDeleted = 1;
                    // IssueRepository.Update(issue);

                    ClearForm();
                    BindGrid();
                }
            }
        }
        protected void gvIssues_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int issueId = Convert.ToInt32(gvIssues.DataKeys[e.RowIndex].Value);

            // 1. Fetch record from Primary DB
            var activeIssue = IssueRepository.GetById(issueId);

            if (activeIssue != null)
            {
                // 2. Map active issue properties to Archive entity
                var archiveRecord = new ArchiveIssue
                {
                    OriginalIssueId = activeIssue.IssueID,
                    Title = activeIssue.Title,
                    Description = activeIssue.Description,
                    Status = activeIssue.Status,
                    Priority = activeIssue.Priority,
                    ArchivedDate = DateTime.Now
                };

                // 3. Save to Archive DB
                ArchiveRepository.Add(archiveRecord);

                // 4. Remove from Primary DB using SoftDelete
                IssueRepository.SoftDelete(issueId);

                // 5. Refresh Primary Grid
                BindGrid();
            }
        }

        protected void gvIssues_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvIssues.PageIndex = e.NewPageIndex;
            BindGrid(txtSearch.Text, ddlSearchBy.SelectedValue);
        }

        private void ClearForm()
        {
            hfIssueID.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlPriority.SelectedIndex = 0;
            txtAssignedTo.Text = string.Empty;
            btnSave.Text = "Save Issue";
        }
    }
}