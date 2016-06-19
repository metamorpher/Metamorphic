//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Metamorphic.Sensor.Http
{
    /// <summary>
    /// A Constraint implementation that matches an HTTP header against an expected version value.
    /// </summary>
    /// <remarks>
    /// Original sample code taken from the route constraint sample here: http://aspnet.codeplex.com/
    /// </remarks>
    internal sealed class ApiRouteVersionConstraint : IHttpRouteConstraint
    {
        /// <summary>
        /// The default version of the API that will be served if no version is specified in the request.
        /// </summary>
        private const int DefaultVersion = 1;

        /// <summary>
        /// Defines the text used to identify the API version of the request.
        /// </summary>
        public const string VersionHeaderName = "api-version";

        private static int? GetVersionHeader(HttpRequestMessage request)
        {
            string versionAsString;
            IEnumerable<string> headerValues;
            if (request.Headers.TryGetValues(VersionHeaderName, out headerValues) && headerValues.Count() == 1)
            {
                versionAsString = headerValues.First();
            }
            else
            {
                return null;
            }

            int version;
            if (versionAsString != null && int.TryParse(versionAsString, out version))
            {
                return version;
            }

            return null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiRouteVersionConstraint"/> class.
        /// </summary>
        /// <param name="allowedVersion">The allowed version of the route.</param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Thrown if <paramref name="allowedVersion"/> is smaller than 1.
        /// </exception>
        public ApiRouteVersionConstraint(int allowedVersion)
        {
            if (allowedVersion < DefaultVersion)
            {
                throw new ArgumentOutOfRangeException("allowedVersion");
            }

            AllowedVersion = allowedVersion;
        }

        /// <summary>
        /// Gets the allowed version for the route.
        /// </summary>
        public int AllowedVersion
        {
            get;
        }

        /// <summary>
        /// Determines whether this instance equals a specified route.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <param name="route">The route to compare.</param>
        /// <param name="parameterName">The name of the parameter.</param>
        /// <param name="values">A list of parameter values.</param>
        /// <param name="routeDirection">The route direction.</param>
        /// <returns>
        ///     <see langword="true"/> if this instance equals a specified route; otherwise, <see langword="false"/>.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (routeDirection == HttpRouteDirection.UriResolution)
            {
                int version = GetVersionHeader(request) ?? DefaultVersion;
                return version == AllowedVersion;
            }

            return true;
        }
    }
}
