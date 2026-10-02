using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data;
using Microsoft.Practices.Unity;
using System;
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
            int issueId = 0;
            int.TryParse(hfIssueID.Value, out issueId);

            if (issueId == 0)
            {
                var newIssue = new Issue
                {
                    Title = txtTitle.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Priority = string.IsNullOrEmpty(ddlPriority.SelectedValue) ? "Low" : ddlPriority.SelectedValue,
                    AssignedTo = txtAssignedTo.Text.Trim(),
                    CreatedDate = DateTime.Now,
                    IsDeleted = 0
                };

                IssueRepository.Add(newIssue);
            }
            else
            {
                var existing = IssueRepository.GetById(issueId);
                if (existing != null)
                {
                    existing.Title = txtTitle.Text.Trim();
                    existing.Description = txtDescription.Text.Trim();
                    existing.Priority = ddlPriority.SelectedValue;
                    existing.AssignedTo = txtAssignedTo.Text.Trim();

                    IssueRepository.Update(existing);
                }
            }

            ClearForm();
            BindGrid();
        }

        protected void gvIssues_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "EditIssue")
            {
                int issueID = Convert.ToInt32(e.CommandArgument);

                // Fetch data using repository instead of raw SQL/DataTable
                var issue = IssueRepository.GetById(issueID);

                if (issue != null)
                {
                    // 1. Populate the form controls
                    hfIssueID.Value = issue.IssueID.ToString();
                    txtTitle.Text = issue.Title;
                    txtDescription.Text = issue.Description;

                    if (ddlPriority.Items.FindByValue(issue.Priority) != null)
                    {
                        ddlPriority.SelectedValue = issue.Priority;
                    }

                    txtAssignedTo.Text = issue.AssignedTo;

                    // 2. Open Modal via JavaScript
                    string script = "window.onload = function() { openModal(); };";
                    ClientScript.RegisterStartupScript(this.GetType(), "OpenModal", script, true);
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