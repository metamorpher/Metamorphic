//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace Metamorphic.Server.Signals
{
    internal sealed class WebCallStartup
    {
        public static IContainer Container
        {
            get;
            set;
        }

        private static void RunWebApiConfiguration(IAppBuilder appBuilder)
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.Routes.MapHttpRoute(
                name: "WebApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(Container);

            // Register the Autofac middleware FIRST, then the Autofac Web API middleware,
            // and finally the standard Web API middleware.
            appBuilder.UseAutofacMiddleware(Container);
            appBuilder.UseAutofacWebApi(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This method is called by the ASP MVC framework.")]
        public void Configuration(IAppBuilder appBuilder)
        {
            RunWebApiConfiguration(appBuilder);
            appBuilder.UseWelcomePage();
        }
    }
}