using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace IssueTracker.Web
{
    /// <summary>
    /// Code-behind for Default page managing active issues grid, creation, and soft deletion.
    /// </summary>
    public partial class Default : Page
    {
        private IUnitOfWork _unitOfWork;

        /// <summary>
        /// Gets or sets the UnitOfWork instance, falling back to PrimaryIssueEntities if DI property injection was bypassed.
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    // Explicit instantiation ensures PrimaryIssueEntities is used as the DbContext
                    _unitOfWork = new UnitOfWork(new PrimaryIssueEntities());
                }
                return _unitOfWork;
            }
            set => _unitOfWork = value;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGrid();
            }
        }

        /// <summary>
        /// Binds active issues (IsDeleted == 0) to the GridView with optional keyword search filtering.
        /// </summary>
        private void BindGrid()
        {
            var issueRepo = UnitOfWork.GetRepository<Issue>();

            string searchKeyword = ViewState["SearchKeyword"] as string ?? string.Empty;

            // Query active records directly without extra AsNoTracking() re-wrapping
            var query = issueRepo.Find(i => i.IsDeleted == 0);

            if (!string.IsNullOrWhiteSpace(searchKeyword))
            {
                searchKeyword = searchKeyword.Trim().ToLower();

                // Added null checks to avoid runtime execution failures
                query = query.Where(i => (i.Title != null && i.Title.ToLower().Contains(searchKeyword)) ||
                                         (i.AssignedTo != null && i.AssignedTo.ToLower().Contains(searchKeyword)));
            }

            gvIssues.DataSource = query.OrderByDescending(i => i.IssueID).ToList();
            gvIssues.DataBind();
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            ViewState["SearchKeyword"] = txtSearch.Text;
            ViewState["SearchBy"] = ddlSearchBy.SelectedValue;
            gvIssues.PageIndex = 0;
            BindGrid();
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            txtSearch.Text = string.Empty;
            ddlSearchBy.SelectedIndex = 0;
            ViewState["SearchKeyword"] = null;
            ViewState["SearchBy"] = null;
            gvIssues.PageIndex = 0;
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

            // 3. Perform Insert OR Update and commit via UnitOfWork with exception safety
            try
            {
                SaveOrUpdateIssue();
            }
            catch (DbEntityValidationException ex)
            {
                var validationErrors = ex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => x.ErrorMessage);

                lblModalError.Text = "<strong>Database Validation Error:</strong><ul class='mb-0 ps-3'>" +
                    string.Join("", validationErrors.Select(err => "<li>" + err + "</li>")) + "</ul>";
                lblModalError.Visible = true;
                upModal.Update();
                return;
            }
            catch (Exception ex)
            {
                lblModalError.Text = "<strong>Error:</strong> " + ex.Message;
                lblModalError.Visible = true;
                upModal.Update();
                return;
            }

            // 4. Reset search criteria when adding a new record so new item displays
            if (string.IsNullOrEmpty(hfIssueID.Value))
            {
                btnClear_Click(sender, e);
            }
            else
            {
                BindGrid();
            }

            // 5. Refresh Grid UpdatePanel & Close Modal
            upGrid.Update();
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

                    issueRepo.Update(issue);
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

                issueRepo.Add(newIssue);
            }

            // Single atomic transaction commit
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
                PerformSoftDelete(issueId);
            }
        }

        private void PerformSoftDelete(int issueId)
        {
            var issueRepo = UnitOfWork.GetRepository<Issue>();
            var activeIssue = issueRepo.GetById(issueId);

            if (activeIssue != null)
            {
                // Soft delete within the single database boundary
                issueRepo.SoftDelete(issueId);
                UnitOfWork.Complete();

                ClearForm();
                BindGrid();
                upGrid.Update();
            }
        }

        protected void gvIssues_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvIssues.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        private void ClearForm()
        {
            hfIssueID.Value = string.Empty;
            txtTitle.Text = string.Empty;
            txtDescription.Text = string.Empty;
            ddlPriority.SelectedIndex = 0;
            txtAssignedTo.Text = string.Empty;
            btnSave.Text = "Save Issue";
            lblModalError.Visible = false;
            lblModalError.Text = string.Empty;

            upModal.Update();
        }
    }
}