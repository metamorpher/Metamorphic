//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.IO.Abstractions;
using Autofac;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using NuGet;

using IFileSystem = System.IO.Abstractions.IFileSystem;

namespace Metamorphic.Core
{
    /// <summary>
    /// Defines the dependency injection module for the Core assembly.
    /// </summary>
    public sealed class CoreModule : Module
    {
        private static void RegisterFileSystem(ContainerBuilder builder)
        {
            builder.Register(c => new FileSystem())
                .As<IFileSystem>();
        }

        private static void RegisterPackages(ContainerBuilder builder)
        {
            Func<string, IPackageRepository> repositoryBuilder = PackageRepositoryFactory.Default.CreateRepository;
            Func<IPackageRepository, string, IPackageManager> managerBuilder = (repo, outputLocation) => new PackageManager(repo, outputLocation);
            builder.Register(c => new PackageInstaller(
                    c.Resolve<IConfiguration>(),
                    repositoryBuilder,
                    managerBuilder,
                    c.Resolve<SystemDiagnostics>(),
                    c.Resolve<IFileSystem>()))
                .As<IInstallPackages>();
        }

        /// <summary>
        /// Override to add registrations to the container.
        /// </summary>
        /// <param name="builder">The builder through which components can be registered.</param>
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            RegisterFileSystem(builder);
            RegisterPackages(builder);
        }
    }
}
