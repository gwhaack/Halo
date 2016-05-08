﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Autofac;
using Autofac.Features.ResolveAnything;
using Autofac.Integration.Mvc;
using GHaack.Halo.Api;
using GHaack.Halo.Data;
using GHaack.Halo.Domain;
using GHaack.Halo.Domain.Services;
using MongoDB.Driver;

namespace GHaack.Halo.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly Uri HaloApiUri = new Uri(
            ConfigurationManager.AppSettings["HaloApiUri"]);
        private static readonly string SubscriptionKey =
            ConfigurationManager.AppSettings["SubscriptionKey"];
        private static readonly string MongoDbConnection =
            ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            var builder = new ContainerBuilder();

            // Register MVC controllers
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            // OPTIONAL: Register model binders that require DI.
            //builder.RegisterModelBinders(Assembly.GetExecutingAssembly());
            //builder.RegisterModelBinderProvider();

            // OPTIONAL: Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            // OPTIONAL: Enable property injection in view pages.
            //builder.RegisterSource(new ViewRegistrationSource());

            // OPTIONAL: Enable property injection into action filters.
            builder.RegisterFilterProvider();

            // Custom type registration
            builder.RegisterType<HaloDataManager>()
                .As<IHaloDataManager>();
            builder.RegisterType<MongoClient>()
                .As<IMongoClient>()
                .UsingConstructor(typeof(string))
                .WithParameter(new TypedParameter(typeof(string), MongoDbConnection));
            builder.RegisterType<MongoHaloRepository>()
                .As<IHaloRepository>();
            builder.RegisterType<HaloApi>()
                .As<IHaloApi>()
                .WithParameter(new TypedParameter(typeof(Uri), HaloApiUri))
                .WithParameter(new TypedParameter(typeof(string), SubscriptionKey));
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            // Set the dependency resolver to be Autofac.
            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }
    }
}