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
using System.Security.AccessControl;
using System.Text;

namespace Test.SourceOnly
{
    [SuppressMessage(
        "Microsoft.Performance",
        "CA1812:AvoidUninstantiatedInternalClasses",
        Justification = "This class is used in other assemblies")]
    internal sealed class MockFile : FileBase
    {
        private readonly Dictionary<string, string> _content
            = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _copiedOrMovedFiles
            = new Dictionary<string, string>();

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockFile(string path, string content)
        {
            _content.Add(path, content);
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockFile(string path, string content, Dictionary<string, string> copiedFiles)
        {
            _content.Add(path, content);
            _copiedOrMovedFiles = copiedFiles;
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockFile(Dictionary<string, string> files)
        {
            _content = files;
        }

        [SuppressMessage(
            "Microsoft.Performance",
            "CA1811:AvoidUncalledPrivateCode",
            Justification = "This method may be used in other projects.")]
        public MockFile(Dictionary<string, string> files, Dictionary<string, string> copiedFiles)
        {
            _content = files;
            _copiedOrMovedFiles = copiedFiles;
        }

        public override void AppendAllText(string path, string contents)
        {
            // Do nothing for now
        }

        public override void AppendAllText(string path, string contents, Encoding encoding)
        {
            // Do nothing for now
        }

        public override StreamWriter AppendText(string path)
        {
            throw new NotImplementedException();
        }

        public override void Copy(string sourceFileName, string destFileName)
        {
            _copiedOrMovedFiles.Add(sourceFileName, destFileName);
        }

        public override void Copy(string sourceFileName, string destFileName, bool overwrite)
        {
            _copiedOrMovedFiles.Add(sourceFileName, destFileName);
        }

        public override Stream Create(string path)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize, FileOptions options)
        {
            throw new NotImplementedException();
        }

        public override Stream Create(string path, int bufferSize, FileOptions options, FileSecurity fileSecurity)
        {
            throw new NotImplementedException();
        }

        public override StreamWriter CreateText(string path)
        {
            throw new NotImplementedException();
        }

        public override void Decrypt(string path)
        {
            // Do nothing for now
        }

        public override void Delete(string path)
        {
            // Do nothing for now
        }

        public override void Encrypt(string path)
        {
            // Do nothing for now
        }

        public override bool Exists(string path)
        {
            return _content.ContainsKey(path);
        }

        public override FileSecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        public override FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public override FileAttributes GetAttributes(string path)
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

        public override void Move(string sourceFileName, string destFileName)
        {
            _copiedOrMovedFiles.Add(sourceFileName, destFileName);
        }

        public override Stream Open(string path, FileMode mode)
        {
            return Open(path, mode, FileAccess.ReadWrite);
        }

        public override Stream Open(string path, FileMode mode, FileAccess access)
        {
            return Open(path, mode, access, FileShare.ReadWrite);
        }

        [SuppressMessage(
            "Microsoft.Reliability",
            "CA2000:Dispose objects before losing scope",
            Justification = "Disposing of the output stream should be done by the caller.")]
        public override Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            var output = new MemoryStream();
            var stream = new MemoryStream();
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(_content[path]);
                writer.Flush();

                stream.Position = 0;
                stream.CopyTo(output);
                output.Position = 0;
            }

            return output;
        }

        public override Stream OpenRead(string path)
        {
            return Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        public override StreamReader OpenText(string path)
        {
            throw new NotImplementedException();
        }

        public override Stream OpenWrite(string path)
        {
            return Open(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
        }

        public override byte[] ReadAllBytes(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ReadAllLines(string path)
        {
            throw new NotImplementedException();
        }

        public override string[] ReadAllLines(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override string ReadAllText(string path)
        {
            throw new NotImplementedException();
        }

        public override string ReadAllText(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            // Do nothing for now ...
        }

        public override void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            // Do nothing for now ...
        }

        public override void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            // Do nothing for now ...
        }

        public override void SetAttributes(string path, FileAttributes fileAttributes)
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

        public override void WriteAllBytes(string path, byte[] bytes)
        {
            // Do nothing for now ...
        }

        public override void WriteAllLines(string path, string[] contents)
        {
            // Do nothing for now ...
        }

        public override void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            // Do nothing for now ...
        }

        public override void WriteAllText(string path, string contents)
        {
            // Do nothing for now ...
        }

        public override void WriteAllText(string path, string contents, Encoding encoding)
        {
            // Do nothing for now ...
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents)
        {
            // Do nothing for now ...
        }

        public override void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            // Do nothing for now ...
        }

        public override IEnumerable<string> ReadLines(string path)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public override void WriteAllLines(string path, IEnumerable<string> contents)
        {
            // Do nothing for now ...
        }

        public override void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            // Do nothing for now ...
        }
    }
}
