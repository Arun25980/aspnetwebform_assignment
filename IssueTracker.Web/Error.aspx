<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="IssueTracker.Web.Error" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Error - Issue Tracker</title>
    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.0/dist/css/bootstrap.min.css" rel="stylesheet" />
</head>
<body class="bg-light">
    <form id="form1" runat="server">
        <div class="container mt-5">
            <div class="row justify-content-center">
                <div class="col-md-8">
                    <div class="card shadow text-center p-5">
                        <h1 class="display-4 text-danger mb-3">Something Went Wrong</h1>
                        <p class="lead text-secondary">
                            An unexpected system error occurred while processing your request.
                        </p>
                        <hr class="my-4" />
                        <p class="text-muted">
                            Our technical team has been notified and the details have been logged.
                        </p>
                        <div class="mt-4">
                            <a href="Default.aspx" class="btn btn-primary btn-lg">Return to Dashboard</a>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </form>
</body>
</html>