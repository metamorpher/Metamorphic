//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
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
            AssemblyVersion = assemblyName.Version;

            var fileVersionAttribute = (AssemblyFileVersionAttribute)assembly.GetCustomAttribute(typeof(AssemblyFileVersionAttribute));
            FileVersion = fileVersionAttribute != null
                ? new Version(fileVersionAttribute.Version)
                : assemblyName.Version;

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
                ? versionInfoAttribute.BuildNumber
                : 0;
            Commit = versionInfoAttribute != null
                ? versionInfoAttribute.VersionControlInformation
                : string.Empty;

            var buildTimeAttribute = (AssemblyBuildTimeAttribute)assembly.GetCustomAttribute(typeof(AssemblyBuildTimeAttribute));
            BuildTime = buildTimeAttribute != null
                ? buildTimeAttribute.BuildTime
                : DateTimeOffset.Now;
        }

        /// <summary>
        /// Gets the version of the site.
        /// </summary>
        public Version AssemblyVersion
        {
            get;
        }

        /// <summary>
        /// Gets the number of the build that generated the site.
        /// </summary>
        public int BuildNumber
        {
            get;
        }

        /// <summary>
        /// Gets the date and time the build that generated the site was executed.
        /// </summary>
        public DateTimeOffset BuildTime
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
        public Version FileVersion
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
