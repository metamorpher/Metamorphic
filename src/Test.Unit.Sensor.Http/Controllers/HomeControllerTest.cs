//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Web.Mvc;
using Metamorphic.Sensor.Http.Controllers;
using Metamorphic.Sensor.Http.Models;
using NUnit.Framework;

namespace Test.Unit.Sensor.Http.Controllers
{
    [TestFixture]
    public class HomeControllerTest
    {
        [Test]
        [SuppressMessage(
            "Microsoft.Usage",
            "CA1806:DoNotIgnoreMethodResults",
            MessageId = "Metamorphic.Sensor.Http.Controllers.HomeController",
            Justification = "Testing that the constructor throws.")]
        public void CreateWithNullModel()
        {
            Assert.Throws<ArgumentNullException>(() => new HomeController(null));
        }

        [Test]
        public void Index()
        {
            var info = new SiteInformationModel();
            HomeController controller = new HomeController(info);

            ViewResult result = controller.Index() as ViewResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("Home Page", result.ViewBag.Title);
        }
    }
}
