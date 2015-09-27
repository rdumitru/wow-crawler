using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using WoW.Crawler.Model.DTO;
using WoW.Crawler.Service;
using WoW.Crawler.Service.Messaging;

namespace WoW.Crawler.Web
{
    public static class AutofacConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            RegisterServices(builder);

            // Register local services.
            RegisterServices(builder);

            // Register services from other projects.
            WoW.Crawler.Service.IoC.RegisterServices(builder);

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }

        private static void RegisterServices(ContainerBuilder builder)
        { }
    }
}