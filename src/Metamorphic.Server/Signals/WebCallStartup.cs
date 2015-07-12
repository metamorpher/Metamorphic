//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Owin;

namespace Metamorphic.Server.Signals
{
    internal sealed class WebCallStartup
    {
        private readonly IContainer m_Container;

        public WebCallStartup(IContainer container)
        {
            {
                Lokad.Enforce.Argument(() => container);
            }

            m_Container = container;
        }

        public void Configuration(IAppBuilder appBuilder)
        {
            RunWebApiConfiguration(appBuilder);
            appBuilder.UseWelcomePage();
        }

        private void RunWebApiConfiguration(IAppBuilder appBuilder)
        {
            var httpConfiguration = new HttpConfiguration();
            httpConfiguration.Routes.MapHttpRoute(
                name: "WebApi",
                routeTemplate: "{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });
            httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(m_Container);

            // Register the Autofac middleware FIRST, then the Autofac Web API middleware,
            // and finally the standard Web API middleware.
            appBuilder.UseAutofacMiddleware(m_Container);
            appBuilder.UseAutofacWebApi(httpConfiguration);
            appBuilder.UseWebApi(httpConfiguration);
        }
    }
}
