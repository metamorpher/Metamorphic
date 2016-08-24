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
using System.IO.Abstractions;
using System.Linq;
using System.Security.AccessControl;

namespace Test.SourceOnly
{
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "This class is used in other assemblies")]
    internal sealed class MockDirectory : DirectoryBase
    {
        private readonly IEnumerable<string> _files;

        public MockDirectory(IEnumerable<string> files)
        {
            _files = files;
        }

        public override DirectoryInfoBase CreateDirectory(string path)
        {
            return CreateDirectory(path, new DirectorySecurity());
        }

        public override DirectoryInfoBase CreateDirectory(string path, DirectorySecurity directorySecurity)
        {
            return null;
        }

        public override void Delete(string path)
        {
            // Do nothing for now ...
        }

        public override void Delete(string path, bool recursive)
        {
            // Do nothing for now ...
        }

        public override IEnumerable<string> EnumerateDirectories(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFiles(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override bool Exists(string path)
        {
            return _files.Contains(path);
        }

        public override DirectorySecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        public override DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetCreationTime(string path)
        {
            return DateTime.Now.AddHours(-1);
        }

        public override DateTime GetCreationTimeUtc(string path)
        {
            return DateTime.Now.AddHours(-1);
        }

        public override string GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }

        public override string[] GetDirectories(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetDirectories(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public override string GetDirectoryRoot(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFiles(string path)
        {
            return GetFiles(path, "*.*");
        }

        public override string[] GetFiles(string path, string searchPattern)
        {
            return GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly);
        }

        public override string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return _files.ToArray();
        }

        public override string[] GetFileSystemEntries(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] GetFileSystemEntries(string path, string searchPattern)
        {
            throw new NotImplementedException();
        }

        public override DateTime GetLastAccessTime(string path)
        {
            return DateTime.Now.AddHours(-1);
        }

        public override DateTime GetLastAccessTimeUtc(string path)
        {
            return DateTime.Now.AddHours(-1);
        }

        public override DateTime GetLastWriteTime(string path)
        {
            return DateTime.Now.AddHours(-1);
        }

        public override DateTime GetLastWriteTimeUtc(string path)
        {
            return DateTime.Now.AddHours(-1);
        }

        public override string[] GetLogicalDrives()
        {
            throw new NotImplementedException();
        }

        public override DirectoryInfoBase GetParent(string path)
        {
            throw new NotImplementedException();
        }

        public override void Move(string sourceDirName, string destDirName)
        {
            // Do nothing for now ...
        }

        public override void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            // Do nothing for now ...
        }

        public override void SetCreationTime(string path, DateTime creationTime)
        {
            // Do nothing for now ...
        }

        public override void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            // Do nothing for now ...
        }

        public override void SetCurrentDirectory(string path)
        {
            // Do nothing for now ...
        }

        public override void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            // Do nothing for now ...
        }

        public override void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            // Do nothing for now ...
        }

        public override void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            // Do nothing for now ...
        }

        public override void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            // Do nothing for now ...
        }
    }
}
