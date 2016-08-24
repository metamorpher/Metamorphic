//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Metamorphic.Core
{
    /// <summary>
    /// Defines a set of values related to files and file paths.
    /// </summary>
    [Serializable]
    public sealed class FileConstants
    {
        /// <summary>
        /// The object that stores constant values for the application.
        /// </summary>
        private readonly ApplicationConstants _constants;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileConstants"/> class.
        /// </summary>
        /// <param name="constants">The object that stores constant values for the application.</param>
        public FileConstants(ApplicationConstants constants)
        {
            {
                Lokad.Enforce.Argument(() => constants);
            }

            _constants = constants;
        }

        /// <summary>
        /// Gets the extension for an assembly file.
        /// </summary>
        /// <value>The extension for an assembly file.</value>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "The property is used through a instance reference.")]
        public string AssemblyExtension
        {
            get
            {
                return "dll";
            }
        }

        /// <summary>
        /// Gets the extension for a log file.
        /// </summary>
        /// <value>The extension for a log file.</value>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "The property is used through a instance reference.")]
        public string LogExtension
        {
            get
            {
                return "log";
            }
        }

        /// <summary>
        /// Gets the extension for a feedback file.
        /// </summary>
        [SuppressMessage(
            "Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "The property is used through a instance reference.")]
        public string FeedbackReportExtension
        {
            get
            {
                return "nsdump";
            }
        }

        /// <summary>
        /// Returns the path for the directory in the AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        public string CompanyCommonPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var companyDirectory = Path.Combine(appDataDir, _constants.CompanyName);

            return companyDirectory;
        }

        /// <summary>
        /// Returns the path for the directory in the user specific AppData directory which contains
        /// all the product directories for the current company.
        /// </summary>
        /// <returns>
        /// The full path for the AppData directory for the current company.
        /// </returns>
        public string CompanyUserPath()
        {
            var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var companyDirectory = Path.Combine(appDataDir, _constants.CompanyName);

            return companyDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the global
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        public string ProductSettingsCommonPath()
        {
            var companyDirectory = CompanyCommonPath();
            var productDirectory = Path.Combine(companyDirectory, _constants.ApplicationName);
            var versionDirectory = Path.Combine(productDirectory, _constants.ApplicationCompatibilityVersion.ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the global
        /// settings for the product are written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the global settings
        /// for the product are written to.
        /// </returns>
        public string ProductSettingsUserPath()
        {
            var companyDirectory = CompanyUserPath();
            var productDirectory = Path.Combine(companyDirectory, _constants.ApplicationName);
            var versionDirectory = Path.Combine(productDirectory, _constants.ApplicationCompatibilityVersion.ToString(2));

            return versionDirectory;
        }

        /// <summary>
        /// Returns the path for the directory where the log files are
        /// written to.
        /// </summary>
        /// <returns>
        /// The full path for the directory where the log files are written to.
        /// </returns>
        public string LogPath()
        {
            var versionDirectory = ProductSettingsCommonPath();
            var logDirectory = Path.Combine(versionDirectory, "logs");

            return logDirectory;
        }
    }
}
