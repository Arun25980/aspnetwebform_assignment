<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IssueTracker.Web.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Issue Tracker</title>
    <!-- Bootstrap 5 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />

    <style>
        .pagination-container td span {
            background-color: #0d6efd;
            color: white;
            padding: 6px 12px;
            border-radius: 4px;
            margin: 0 2px;
            font-weight: bold;
        }
        .pagination-container td a {
            color: #0d6efd;
            padding: 6px 12px;
            text-decoration: none;
            border: 1px solid #dee2e6;
            border-radius: 4px;
            margin: 0 2px;
        }
        .pagination-container td a:hover {
            background-color: #e9ecef;
        }
    </style>
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="container my-4">

            <!-- Header & Add Button -->
            <div class="d-flex justify-content-between align-items-center mb-4">
                <h3 class="fw-bold text-primary mb-0">Issue Tracker</h3>
                <button type="button" class="btn btn-primary" onclick="openModalForNew();">
                    + Add New Issue
                </button>
            </div>

            <!-- Search Section -->
            <div class="card shadow-sm mb-4">
                <div class="card-body">
                    <h5 class="card-title fw-bold mb-3">Search Issues</h5>
                    <div class="row g-2 align-items-center">
                        <div class="col-md-3">
                            <asp:DropDownList ID="ddlSearchBy" runat="server" CssClass="form-select">
                                <asp:ListItem Text="Search All Fields" Value="All" />
                                <asp:ListItem Text="Title" Value="Title" />
                                <asp:ListItem Text="Priority" Value="Priority" />
                                <asp:ListItem Text="Assigned To" Value="AssignedTo" />
                            </asp:DropDownList>
                        </div>
                        <div class="col-md-6">
                            <asp:TextBox ID="txtSearch" runat="server" CssClass="form-control" placeholder="Type keyword to search..."></asp:TextBox>
                        </div>
                        <div class="col-md-3 d-flex gap-2">
                            <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="btn btn-outline-primary w-100" OnClick="btnSearch_Click" />
                            <asp:Button ID="btnClear" runat="server" Text="Clear" CssClass="btn btn-outline-secondary w-100" OnClick="btnClear_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <!-- GridView -->
            <div class="card shadow-sm">
                <div class="card-header bg-dark text-white">
                    <h5 class="mb-0 fw-bold">Active Issues</h5>
                </div>
                <div class="card-body p-0">
                    <div class="table-responsive">
                        <asp:GridView ID="gvIssues" runat="server" AutoGenerateColumns="False"
                            CssClass="table table-striped table-hover align-middle mb-0"
                            DataKeyNames="IssueID"
                            OnRowCommand="gvIssues_RowCommand"
                           
                            AllowPaging="True"
                            PageSize="5"
                            OnPageIndexChanging="gvIssues_PageIndexChanging">

                            <PagerStyle CssClass="pagination-container my-2" HorizontalAlign="Center" />
                            <PagerSettings Mode="NumericFirstLast" FirstPageText="First" LastPageText="Last" PageButtonCount="5" />

                            <Columns>
                                <asp:BoundField DataField="IssueID" HeaderText="ID" />
                                <asp:BoundField DataField="Title" HeaderText="Title" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:BoundField DataField="Priority" HeaderText="Priority" />
                                <asp:BoundField DataField="AssignedTo" HeaderText="Assigned To" />
                                <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" />

                                <asp:TemplateField HeaderText="Actions">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="btnEdit" runat="server"
                                            CommandName="EditIssue"
                                            CommandArgument='<%# Eval("IssueID") %>'
                                            CssClass="btn btn-sm btn-outline-primary me-1">
                                            Edit
                                        </asp:LinkButton>
                                        <asp:LinkButton ID="btnDelete" runat="server"
                                            CommandName="DeleteIssue"
                                            CommandArgument='<%# Eval("IssueID") %>'
                                            CssClass="btn btn-sm btn-outline-danger"
                                            OnClientClick="return confirm('Are you sure you want to delete this issue?');">
                                            Delete
                                        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>

        <!-- Bootstrap 5 Modal Form -->
        <div class="modal fade" id="addEditModal" tabindex="-1" aria-labelledby="addEditModalLabel" aria-hidden="true">
            <div class="modal-dialog modal-dialog-centered modal-lg">
                <div class="modal-content">
                    <div class="modal-header bg-primary text-white">
                        <h5 class="modal-title fw-bold" id="addEditModalLabel">Manage Issue</h5>
                        <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body p-4">
                        <asp:HiddenField ID="hfIssueID" runat="server" />

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Title</label>
                            <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter title"></asp:TextBox>
                        </div>

                        <div class="mb-3">
                            <label class="form-label fw-semibold">Description</label>
                            <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Enter description"></asp:TextBox>
                        </div>

                        <div class="row">
                            <div class="col-md-6 mb-3">
                                <label class="form-label fw-semibold">Priority</label>
                                <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-select">
                                    <asp:ListItem Text="-- Select Priority --" Value="" />
                                    <asp:ListItem Text="Low" Value="Low" />
                                    <asp:ListItem Text="Medium" Value="Medium" />
                                    <asp:ListItem Text="High" Value="High" />
                                </asp:DropDownList>
                            </div>

                            <div class="col-md-6 mb-3">
                                <label class="form-label fw-semibold">Assigned To</label>
                                <asp:TextBox ID="txtAssignedTo" runat="server" CssClass="form-control" placeholder="Assigned person"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancel</button>
                        <asp:Button ID="btnSave" runat="server" Text="Save Issue" CssClass="btn btn-primary" OnClick="btnSave_Click" />
                    </div>
                </div>
            </div>
        </div>

    </form>

    <!-- Bootstrap 5 JS -->
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/js/bootstrap.bundle.min.js"></script>
    <script type="text/javascript">
        function openModal() {
            var myModal = new bootstrap.Modal(document.getElementById('addEditModal'));
            myModal.show();
        }

        function openModalForNew() {
            clearForm();
            openModal();
        }

        function clearForm() {
            document.getElementById('<%= hfIssueID.ClientID %>').value = '';
            document.getElementById('<%= txtTitle.ClientID %>').value = '';
            document.getElementById('<%= txtDescription.ClientID %>').value = '';
            document.getElementById('<%= ddlPriority.ClientID %>').value = '';
            document.getElementById('<%= txtAssignedTo.ClientID %>').value = '';
        }
    </script>
</body>
</html>