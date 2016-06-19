//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Metamorphic.Sensor.Http
{
    /// <summary>
    /// Defines the route configuration.
    /// </summary>
    public static class RouteConfig
    {
        /// <summary>
        /// Registers the routes for the site.
        /// </summary>
        /// <param name="routes">The route collection.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="routes"/> is smaller than 1.
        /// </exception>
        public static void RegisterRoutes(RouteCollection routes)
        {
            if (routes == null)
            {
                throw new ArgumentNullException("routes");
            }

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });
        }
    }
}
