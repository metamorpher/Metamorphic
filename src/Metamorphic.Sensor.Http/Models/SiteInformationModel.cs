//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using System.Reflection;
using Nuclei.Build;

namespace Metamorphic.Sensor.Http.Models
{
    /// <summary>
    /// Stores information about the site.
    /// </summary>
    public sealed class SiteInformationModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SiteInformationModel"/> class.
        /// </summary>
        public SiteInformationModel()
        {
            var assembly = Assembly.GetExecutingAssembly();

            var assemblyName = assembly.GetName();
            AssemblyVersion = assemblyName.Version.ToString(4);

            var fileVersionAttribute = (AssemblyFileVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
            FileVersion = fileVersionAttribute != null
                ? fileVersionAttribute.Version
                : assemblyName.Version.ToString(4);

            var productVersionAttribute = (AssemblyInformationalVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyInformationalVersionAttribute));
            ProductVersion = productVersionAttribute != null
                ? productVersionAttribute.InformationalVersion
                : assemblyName.Version.ToString();

            var copyrightAttribute = (AssemblyCopyrightAttribute)assembly.GetCustomAttribute(typeof(AssemblyCopyrightAttribute));
            Copyright = copyrightAttribute != null
                ? copyrightAttribute.Copyright
                : "Copyright - Metamorphic";

            var versionInfoAttribute = (AssemblyBuildInformationAttribute)assembly.GetCustomAttribute(typeof(AssemblyBuildInformationAttribute));
            BuildNumber = versionInfoAttribute != null
                ? versionInfoAttribute.BuildNumber.ToString(CultureInfo.CurrentCulture)
                : "0";
            Commit = versionInfoAttribute != null
                ? versionInfoAttribute.VersionControlInformation
                : string.Empty;

            var buildTimeAttribute = (AssemblyBuildTimeAttribute)assembly.GetCustomAttribute(typeof(AssemblyBuildTimeAttribute));
            BuildTime = buildTimeAttribute != null
                ? buildTimeAttribute.BuildTime.ToString(CultureInfo.CurrentCulture)
                : DateTimeOffset.Now.ToString(CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Gets the version of the site.
        /// </summary>
        public string AssemblyVersion
        {
            get;
        }

        /// <summary>
        /// Gets the number of the build that generated the site.
        /// </summary>
        public string BuildNumber
        {
            get;
        }

        /// <summary>
        /// Gets the date and time the build that generated the site was executed.
        /// </summary>
        public string BuildTime
        {
            get;
        }

        /// <summary>
        /// Gets the commit ID from which the site was build.
        /// </summary>
        public string Commit
        {
            get;
        }

        /// <summary>
        /// Gets the copyright for the site.
        /// </summary>
        public string Copyright
        {
            get;
        }

        /// <summary>
        /// Gets the file version of the site.
        /// </summary>
        public string FileVersion
        {
            get;
        }

        /// <summary>
        /// Gets the product version of the site.
        /// </summary>
        public string ProductVersion
        {
            get;
        }
    }
}
