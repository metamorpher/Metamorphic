//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Reflection;
using Nuclei;

namespace Test.SourceOnly
{
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "This class is used in other assemblies")]
    internal sealed class MockPath : PathBase
    {
        public override string ChangeExtension(string path, string extension)
        {
            return Path.ChangeExtension(path, extension);
        }

        public override string Combine(string path1, string path2)
        {
            return Path.Combine(path1, path2);
        }

        public override string GetDirectoryName(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public override string GetExtension(string path)
        {
            return Path.GetExtension(path);
        }

        public override string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public override string GetFileNameWithoutExtension(string path)
        {
            return Path.GetFileNameWithoutExtension(path);
        }

        public override string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public override char[] GetInvalidFileNameChars()
        {
            return Path.GetInvalidFileNameChars();
        }

        public override char[] GetInvalidPathChars()
        {
            return Path.GetInvalidPathChars();
        }

        public override string GetPathRoot(string path)
        {
            return Path.GetPathRoot(path);
        }

        public override string GetRandomFileName()
        {
            return "abcde.fgh";
        }

        public override string GetTempFileName()
        {
            var path = GetTempPath();
            return Path.Combine(path, GetRandomFileName());
        }

        public override string GetTempPath()
        {
            return Assembly.GetExecutingAssembly().LocalDirectoryPath();
        }

        public override bool HasExtension(string path)
        {
            return Path.HasExtension(path);
        }

        public override bool IsPathRooted(string path)
        {
            return Path.IsPathRooted(path);
        }

        public override string Combine(params string[] paths)
        {
            return Path.Combine(paths);
        }

        public override string Combine(string path1, string path2, string path3)
        {
            return Path.Combine(path1, path2, path3);
        }

        public override string Combine(string path1, string path2, string path3, string path4)
        {
            return Path.Combine(path1, path2, path3, path4);
        }

        public override char AltDirectorySeparatorChar
        {
            get
            {
                return Path.AltDirectorySeparatorChar;
            }
        }

        public override char DirectorySeparatorChar
        {
            get
            {
                return Path.DirectorySeparatorChar;
            }
        }

        [Obsolete]
        public override char[] InvalidPathChars
        {
            get
            {
                return Path.InvalidPathChars;
            }
        }

        public override char PathSeparator
        {
            get
            {
                return Path.PathSeparator;
            }
        }

        public override char VolumeSeparatorChar
        {
            get
            {
                return Path.VolumeSeparatorChar;
            }
        }
    }
}
