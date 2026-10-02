using System.Data.Entity;
using Microsoft.Practices.Unity;
using Unity.WebForms;
using IssueTracker.Core.Entities;
using IssueTracker.Core.Interfaces;
using IssueTracker.Data;
using IssueTracker.Data.Repositories;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(IssueTracker.Web.App_Start.UnityWebFormsStart), "PostStart")]

namespace IssueTracker.Web.App_Start
{
    internal static class UnityWebFormsStart
    {
        public static IUnityContainer Container { get; private set; }

        internal static void PostStart()
        {
            var container = new UnityContainer();

            RegisterDependencies(container);

            Container = container;

            System.Web.HttpContext.Current.Application.SetContainer(container);
        }

        private static void RegisterDependencies(IUnityContainer container)
        {
            // ==========================================
            // 1. PRIMARY DATABASE REGISTRATIONS
            // ==========================================
            container.RegisterType<PrimaryIssueEntities>(
                new HierarchicalLifetimeManager(),
                new InjectionConstructor()
            );

            container.RegisterType<DbContext, PrimaryIssueEntities>(
                new HierarchicalLifetimeManager()
            );

            container.RegisterType<IRepository<Issue>, Repository<Issue>>(
                new HierarchicalLifetimeManager()
            );

            // ==========================================
            // 2. ARCHIVE DATABASE REGISTRATIONS
            // ==========================================
            container.RegisterType<ArchiveIssueEntities>(
                new HierarchicalLifetimeManager(),
                new InjectionConstructor()
            );

            container.RegisterType<DbContext, ArchiveIssueEntities>(
                "ArchiveContext",
                new HierarchicalLifetimeManager()
            );

            // Register ArchiveRepository for ArchiveIssue entity type
            container.RegisterType<IRepository<ArchiveIssue>, Repository<ArchiveIssue>>(
                "ArchiveRepository",
                new HierarchicalLifetimeManager(),
                new InjectionConstructor(new ResolvedParameter<DbContext>("ArchiveContext"))
            );

            // ==========================================
            // 3. COMMON SERVICES
            // ==========================================
            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));
            container.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}