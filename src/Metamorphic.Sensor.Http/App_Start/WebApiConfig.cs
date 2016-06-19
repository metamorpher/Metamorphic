//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Web.Http;

namespace Metamorphic.Sensor.Http
{
    /// <summary>
    /// Defines the configuration for the WebAPI part of the site.
    /// </summary>
    public static class WebApiConfig
    {
        /// <summary>
        /// Registers the routes for the WebAPI part of the site.
        /// </summary>
        /// <param name="config">The configuration.</param>
        public static void Register(HttpConfiguration config)
        {
            // Web API routes
            config.MapHttpAttributeRoutes();

            /*
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });*/
        }
    }
}
