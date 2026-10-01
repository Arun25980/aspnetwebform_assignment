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
        // Expose container property so UnityHttpModule can read it
        public static IUnityContainer Container { get; private set; }

        internal static void PostStart()
        {
            var container = new UnityContainer();

            // Register dependencies
            RegisterDependencies(container);

            // Store in static container property
            Container = container;

            // Set Application container
            System.Web.HttpContext.Current.Application.SetContainer(container);
        }

        private static void RegisterDependencies(IUnityContainer container)
        {
            // 1. PrimaryIssueEntities with parameterless constructor
            container.RegisterType<PrimaryIssueEntities>(
                new HierarchicalLifetimeManager(),
                new InjectionConstructor()
            );

            // 2. DbContext mapping
            container.RegisterType<DbContext, PrimaryIssueEntities>(
                new HierarchicalLifetimeManager()
            );

            // 3. Generic Repository
            container.RegisterType(typeof(IRepository<>), typeof(Repository<>));

            // 4. Closed Generic Repository Registration
            container.RegisterType<IRepository<Issue>, Repository<Issue>>(
                new HierarchicalLifetimeManager()
            );

            // 5. Unit of Work
            container.RegisterType<IUnitOfWork, UnitOfWork>();
        }
    }
}