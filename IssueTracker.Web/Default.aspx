<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="IssueTracker.Web.Default" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Issue Tracker</title>
    <!-- Bootstrap 5 CSS -->
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="container my-4">
            
            <!-- Centered Form -->
            <div class="row justify-content-center mb-4">
                <div class="col-md-8 col-lg-6">
                    <div class="card shadow-sm border-0">
                        <div class="card-header bg-primary text-white text-center py-3">
                            <h4 class="mb-0 fw-bold">Issue Tracker</h4>
                        </div>
                        <div class="card-body p-4">
                            <asp:HiddenField ID="hfIssueID" runat="server" />

                            <div class="mb-3">
                                <label for="txtTitle" class="form-label fw-semibold">Title</label>
                                <asp:TextBox ID="txtTitle" runat="server" CssClass="form-control" placeholder="Enter title"></asp:TextBox>
                            </div>

                            <div class="mb-3">
                                <label for="txtDescription" class="form-label fw-semibold">Description</label>
                                <asp:TextBox ID="txtDescription" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Enter description"></asp:TextBox>
                            </div>

                            <div class="row">
                                <div class="col-md-6 mb-3">
                                    <label for="ddlPriority" class="form-label fw-semibold">Priority</label>
                                    <asp:DropDownList ID="ddlPriority" runat="server" CssClass="form-select">
                                        <asp:ListItem Text="-- Select Priority --" Value="" />
                                        <asp:ListItem Text="Low" Value="Low" />
                                        <asp:ListItem Text="Medium" Value="Medium" />
                                        <asp:ListItem Text="High" Value="High" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-md-6 mb-3">
                                    <label for="txtAssignedTo" class="form-label fw-semibold">Assigned To</label>
                                    <asp:TextBox ID="txtAssignedTo" runat="server" CssClass="form-control" placeholder="Assigned person"></asp:TextBox>
                                </div>
                            </div>

                            <div class="d-grid gap-2 mt-3">
                                <asp:Button ID="btnSave" runat="server" Text="Save Issue" CssClass="btn btn-primary btn-lg" OnClick="btnSave_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Search Section (Title, Priority, or Assigned To) -->
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
                        <!-- Cleaned GridView without OnRowCommand error -->
                        <asp:GridView ID="gvIssues" runat="server" AutoGenerateColumns="False" DataKeyNames="IssueID"
                            CssClass="table table-hover align-middle mb-0" GridLines="None">
                            <HeaderStyle CssClass="table-light" />
                            <Columns>
                                <asp:BoundField DataField="IssueID" HeaderText="ID" ItemStyle-Width="60px" />
                                <asp:BoundField DataField="Title" HeaderText="Title" />
                                <asp:BoundField DataField="Description" HeaderText="Description" />
                                <asp:BoundField DataField="Priority" HeaderText="Priority" ItemStyle-Width="100px" />
                                <asp:BoundField DataField="AssignedTo" HeaderText="Assigned To" ItemStyle-Width="150px" />
                                <asp:BoundField DataField="CreatedDate" HeaderText="Created Date" DataFormatString="{0:yyyy-MM-dd HH:mm}" ItemStyle-Width="160px" />
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>

        </div>
    </form>
</body>
</html>