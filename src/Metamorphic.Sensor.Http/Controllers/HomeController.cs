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
    /// Defines the controller for the home landing page.
    /// </summary>
    public class HomeController : BaseMvcController
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HomeController"/> class.
        /// </summary>
        /// <param name="model">The object that contains information about the site.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="model"/> is <see langword="null" />.
        /// </exception>
        public HomeController(SiteInformationModel model)
            : base(model)
        {
        }

        /// <summary>
        /// The controller method for the index view.
        /// </summary>
        /// <returns>The view.</returns>
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            return View();
        }
    }
}
