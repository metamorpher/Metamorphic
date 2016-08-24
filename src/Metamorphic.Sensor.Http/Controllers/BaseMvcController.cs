//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Web.Mvc;
using Metamorphic.Sensor.Http.Models;

namespace Metamorphic.Sensor.Http.Controllers
{
    /// <summary>
    /// The base controller for all ASP MVC web controllers.
    /// </summary>
    public abstract class BaseMvcController : Controller
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMvcController"/> class.
        /// </summary>
        /// <param name="model">The object that contains information about the site.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        protected BaseMvcController(SiteInformationModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            SiteInformation = model;
        }

        /// <summary>
        /// The action that renders the footer.
        /// </summary>
        /// <returns>The footer view.</returns>
        [ChildActionOnly]
        public PartialViewResult Footer()
        {
            return PartialView(SiteInformation);
        }

        /// <summary>
        /// Gets the object that describes the site.
        /// </summary>
        protected SiteInformationModel SiteInformation
        {
            get;
        }
    }
}
