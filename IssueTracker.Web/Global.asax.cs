using System;
using System.Data.Entity;
using System.Web;
using Autofac;
using Autofac.Integration.Web;
using NLog;
using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data;

namespace IssueTracker.Web
{
    /// <summary>
    /// Global application class configuring Autofac dependency injection, logging, and global unhandled exception management.
    /// </summary>
    public class Global : HttpApplication, IContainerProviderAccessor
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static IContainerProvider _containerProvider;

        /// <summary>
        /// Gets the Autofac container provider used across the ASP.NET application lifecycle.
        /// </summary>
        public IContainerProvider ContainerProvider => _containerProvider;

        /// <summary>
        /// Configures dependency injection registrations and initializes logging on application startup.
        /// </summary>
        protected void Application_Start(object sender, EventArgs e)
        {
            var builder = new ContainerBuilder();

            // 1. Register concrete PrimaryIssueEntities as DbContext and Self
            builder.RegisterType<PrimaryIssueEntities>()
                   .AsSelf()
                   .As<DbContext>()
                   .InstancePerLifetimeScope();

            // 2. Register UnitOfWork
            builder.RegisterType<UnitOfWork>()
                   .As<IUnitOfWork>()
                   .InstancePerLifetimeScope();

            // 3. Register generic Repository
            builder.RegisterGeneric(typeof(Repository<>))
                   .As(typeof(IRepository<>))
                   .InstancePerLifetimeScope();

            // 4. Register Web Forms Pages/Controls in this assembly
            builder.RegisterAssemblyTypes(typeof(Global).Assembly);

            // Build container and store in static Provider
            var container = builder.Build();
            _containerProvider = new ContainerProvider(container);
        }
        /// <summary>
        /// Global unhandled exception handler logging application errors and redirecting to custom error pages.
        /// </summary>
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