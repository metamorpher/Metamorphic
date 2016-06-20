//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Web.Http.Routing;

namespace Metamorphic.Sensor.Http
{
    /// <summary>
    /// Provides an attribute route that's restricted to a specific version of the api.
    /// </summary>
    internal sealed class VersionedApiRoute : RouteFactoryAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedApiRoute"/> class.
        /// </summary>
        /// <param name="template">The route template</param>
        /// <param name="allowedVersion">The version for the current route.</param>
        public VersionedApiRoute(string template, int allowedVersion)
            : base(template)
        {
            AllowedVersion = allowedVersion;
        }

        /// <summary>
        /// Gets the version for the current route.
        /// </summary>
        public int AllowedVersion
        {
            get;
        }

        /// <summary>
        /// Gets the route constraints, if any; otherwise <see langword="null"/>.
        /// </summary>
        /// <returns>
        /// The route constraints, if any; otherwise <see langword="null"/>.
        /// </returns>
        public override IDictionary<string, object> Constraints
        {
            get
            {
                var constraints = new HttpRouteValueDictionary();
                constraints.Add("version", new ApiRouteVersionConstraint(AllowedVersion));
                return constraints;
            }
        }
    }
}
