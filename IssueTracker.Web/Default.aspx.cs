using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
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
        public IUnitOfWork UnitOfWork { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            // Resolve UnitOfWork directly from Unity container if property injection is skipped
            var container = HttpContext.Current.Application.GetContainer();
            if (container != null && UnitOfWork == null)
            {
                UnitOfWork = container.Resolve<IUnitOfWork>();
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
            if (UnitOfWork == null) return;

            var issueRepo = UnitOfWork.GetRepository<Issue>();
            var query = issueRepo.Find(i => i.IsDeleted == 0).AsQueryable();

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

                upModal.Update();
                return;
            }

            // 3. Perform Insert OR Update and commit via UnitOfWork
            SaveOrUpdateIssue();

            // 4. Refresh GridView and update its UpdatePanel
            BindGrid();
            upGrid.Update();

            // 5. Close Modal and clean up backdrop
            ScriptManager.RegisterStartupScript(this, GetType(), "CloseModalScript", "closeModal();", true);
        }

        private void SaveOrUpdateIssue()
        {
            int issueID = 0;
            bool isEdit = int.TryParse(hfIssueID.Value, out issueID) && issueID > 0;
            var issueRepo = UnitOfWork.GetRepository<Issue>();

            if (isEdit)
            {
                var issue = issueRepo.GetById(issueID);
                if (issue != null)
                {
                    issue.Title = txtTitle.Text.Trim();
                    issue.Description = txtDescription.Text.Trim();
                    issue.Priority = ddlPriority.SelectedValue;
                    issue.AssignedTo = txtAssignedTo.Text.Trim();

                    issueRepo.Update(issue); // Staged in memory
                }
            }
            else
            {
                var newIssue = new Issue
                {
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Priority = ddlPriority.SelectedValue,
                    AssignedTo = txtAssignedTo.Text.Trim(),
                    CreatedDate = DateTime.Now,
                    IsDeleted = 0
                };

                issueRepo.Add(newIssue); // Staged in memory
            }

            // Commit all changes to DB in a single transaction
            UnitOfWork.Complete();
        }

        protected void gvIssues_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditIssue")
            {
                int issueID = Convert.ToInt32(e.CommandArgument);
                var issueRepo = UnitOfWork.GetRepository<Issue>();
                var issue = issueRepo.GetById(issueID);

                lblModalError.Visible = false;
                lblModalError.Text = string.Empty;

                if (issue != null)
                {
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

                    upModal.Update();

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
                PerformArchiveAndDelete(issueId);
            }
        }

        protected void gvIssues_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            int issueId = Convert.ToInt32(gvIssues.DataKeys[e.RowIndex].Value);
            PerformArchiveAndDelete(issueId);
        }

        private void PerformArchiveAndDelete(int issueId)
        {
            var issueRepo = UnitOfWork.GetRepository<Issue>();
            var archiveRepo = UnitOfWork.GetRepository<ArchiveIssue>();

            var activeIssue = issueRepo.GetById(issueId);

            if (activeIssue != null)
            {
                var archiveRecord = new ArchiveIssue
                {
                    OriginalIssueId = activeIssue.IssueID,
                    Title = activeIssue.Title,
                    Description = activeIssue.Description,
                    Status = activeIssue.Status,
                    Priority = activeIssue.Priority,
                    ArchivedDate = DateTime.Now
                };

                // Stage both operations
                archiveRepo.Add(archiveRecord);
                issueRepo.SoftDelete(issueId);

                // Execute transaction across both repositories
                UnitOfWork.Complete();

                ClearForm();
                BindGrid();
                upGrid.Update();
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