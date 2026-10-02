using System;
using System.Web;
using NLog;

namespace IssueTracker.Web
{
    public class Global : HttpApplication
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected void Application_Start(object sender, EventArgs e)
        {
            Logger.Info("IssueTracker Application Started successfully.");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception lastError = Server.GetLastError();

            if (lastError != null)
            {
                Exception actualException = lastError.GetBaseException();

                // Log the actual exception details
                Logger.Error(actualException, "Unhandled Exception caught in Application_Error");

                // Check current URL to prevent infinite loops if Error.aspx has an issue
                string currentUrl = Request.Url.AbsolutePath.ToLower();
                if (!currentUrl.EndsWith("error.aspx"))
                {
                    Server.ClearError();
                    Response.Redirect("~/Error.aspx");
                }
            }
        }
    }
}