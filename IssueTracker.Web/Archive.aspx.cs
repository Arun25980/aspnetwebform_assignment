using IssueTracker.Data; // Contains ArchiveIssue entity from EDMX
using IssueTracker.Core.Interfaces;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Unity.WebForms;

namespace IssueTracker.Web
{
    public partial class Archive : Page
    {
        [Dependency("ArchiveRepository")]
        public IRepository<ArchiveIssue> ArchiveRepository { get; set; }

        protected void Page_Init(object sender, EventArgs e)
        {
            var container = HttpContext.Current.Application.GetContainer();
            if (container != null && ArchiveRepository == null)
            {
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
            if (ArchiveRepository == null) return;

            var query = ArchiveRepository.GetAll().AsQueryable();

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
                    case "Status":
                        query = query.Where(i => i.Status != null && i.Status.ToLower().Contains(searchKeyword));
                        break;
                    default:
                        query = query.Where(i => (i.Title != null && i.Title.ToLower().Contains(searchKeyword)) ||
                                                 (i.Priority != null && i.Priority.ToLower().Contains(searchKeyword)) ||
                                                 (i.Status != null && i.Status.ToLower().Contains(searchKeyword)));
                        break;
                }
            }

            // Order by Archive ID or ArchivedDate
            gvArchive.DataSource = query.OrderByDescending(i => i.Id).ToList();
            gvArchive.DataBind();
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

        protected void gvArchive_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvArchive.PageIndex = e.NewPageIndex;
            BindGrid(txtSearch.Text, ddlSearchBy.SelectedValue);
        }
    }
}