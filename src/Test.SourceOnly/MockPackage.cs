//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.Versioning;
using NuGet;

namespace Test.SourceOnly
{
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "This class is used in other assemblies")]
    internal sealed class MockPackage : IPackage
    {
        private readonly IEnumerable<IPackageAssemblyReference> _assemblyReferences
            = new IPackageAssemblyReference[0];

        private readonly IEnumerable<PackageDependencySet> _dependencySets
            = new PackageDependencySet[0];

        private readonly IEnumerable<FrameworkName> _supportedFrameworks
            = new FrameworkName[0];

        private readonly string _id;

        private readonly SemanticVersion _version;

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockPackage(PackageName packageName)
        {
            _id = packageName.Id;
            _version = packageName.Version;
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockPackage(string id, SemanticVersion version)
        {
            _id = id;
            _version = version;
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockPackage(PackageName packageName, IEnumerable<PackageDependencySet> dependencies)
            : this(packageName)
        {
            _dependencySets = dependencies;
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockPackage(string id, SemanticVersion version, IEnumerable<PackageDependencySet> dependencies)
            : this(id, version)
        {
            _dependencySets = dependencies;
        }

        public IEnumerable<IPackageAssemblyReference> AssemblyReferences
        {
            get
            {
                return _assemblyReferences;
            }
        }

        public IEnumerable<string> Authors
        {
            get
            {
                return new string[0];
            }
        }

        public string Copyright
        {
            get
            {
                return string.Empty;
            }
        }

        public IEnumerable<PackageDependencySet> DependencySets
        {
            get
            {
                return _dependencySets;
            }
        }

        public string Description
        {
            get
            {
                return string.Empty;
            }
        }

        public bool DevelopmentDependency
        {
            get
            {
                return false;
            }
        }

        public int DownloadCount
        {
            get
            {
                return 0;
            }
        }

        public IEnumerable<FrameworkAssemblyReference> FrameworkAssemblies
        {
            get
            {
                return new FrameworkAssemblyReference[0];
            }
        }

        public Uri IconUrl
        {
            get
            {
                return new Uri("http://nuget.org");
            }
        }

        public string Id
        {
            get
            {
                return _id;
            }
        }

        public bool IsAbsoluteLatestVersion
        {
            get
            {
                return false;
            }
        }

        public bool IsLatestVersion
        {
            get
            {
                return true;
            }
        }

        public string Language
        {
            get
            {
                return "en";
            }
        }

        public Uri LicenseUrl
        {
            get
            {
                return null;
            }
        }

        public bool Listed
        {
            get
            {
                return true;
            }
        }

        public Version MinClientVersion
        {
            get
            {
                return new Version("1.0.0.0");
            }
        }

        public IEnumerable<string> Owners
        {
            get
            {
                return new string[0];
            }
        }

        public ICollection<PackageReferenceSet> PackageAssemblyReferences
        {
            get
            {
                return new PackageReferenceSet[0];
            }
        }

        public Uri ProjectUrl
        {
            get
            {
                return null;
            }
        }

        public DateTimeOffset? Published
        {
            get
            {
                return null;
            }
        }

        public string ReleaseNotes
        {
            get
            {
                return string.Empty;
            }
        }

        public Uri ReportAbuseUrl
        {
            get
            {
                return null;
            }
        }

        public bool RequireLicenseAcceptance
        {
            get
            {
                return false;
            }
        }

        public string Summary
        {
            get
            {
                return string.Empty;
            }
        }

        public string Tags
        {
            get
            {
                return string.Empty;
            }
        }

        public string Title
        {
            get
            {
                return _id;
            }
        }

        public SemanticVersion Version
        {
            get
            {
                return _version;
            }
        }

        public void ExtractContents(IFileSystem fileSystem, string extractPath)
        {
            // Do nothing ..
        }

        public IEnumerable<IPackageFile> GetFiles()
        {
            return new IPackageFile[0];
        }

        public Stream GetStream()
        {
            return new MemoryStream();
        }

        public IEnumerable<FrameworkName> GetSupportedFrameworks()
        {
            return _supportedFrameworks;
        }
    }
}
