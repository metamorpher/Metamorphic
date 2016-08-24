//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using Metamorphic.Actions.Powershell.Properties;
using Metamorphic.Core.Actions;
using Nuclei;
using Nuclei.Configuration;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Actions.Powershell
{
    /// <summary>
    /// Defines methods for building an action that executes a Powershell script.
    /// </summary>
    [ActionProvider]
    public sealed class PowershellActions
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics _diagnostics;

        /// <summary>
        /// The full path to the directory that contains all the powershell scripts
        /// </summary>
        private readonly string _scriptPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowershellActions"/> class.
        /// </summary>
        /// <param name="configuration">The object that provides the configuration for the application.</param>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="configuration"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PowershellActions(IConfiguration configuration, SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => configuration);
                Lokad.Enforce.Argument(() => diagnostics);
            }

            _diagnostics = diagnostics;

            _scriptPath = configuration.HasValueFor(PowershellConfigurationKeys.ScriptDirectory)
                ? configuration.Value<string>(PowershellConfigurationKeys.ScriptDirectory)
                : PowershellConstants.DefaultScriptDirectory;
            if (!Path.IsPathRooted(_scriptPath))
            {
                var exeDirectoryPath = Assembly.GetExecutingAssembly().LocalDirectoryPath();
                _scriptPath = Path.GetFullPath(Path.Combine(exeDirectoryPath, _scriptPath));
            }
        }

        /// <summary>
        /// Invokes a powershell script with the specified arguments.
        /// </summary>
        /// <param name="scriptFile">The full path to the powershell script file.</param>
        /// <param name="arguments">The arguments that should be provided to the powershell file.</param>
        [SuppressMessage(
            "Microsoft.Design",
            "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Catching and logging and ignoring because we don't know exactly what the script will throw at us.")]
        [Action("powershell")]
        public void InvokePowershell(string scriptFile, string arguments)
        {
            var scriptFullPath = scriptFile;
            if (!Path.IsPathRooted(scriptFullPath))
            {
                scriptFullPath = Path.GetFullPath(Path.Combine(_scriptPath, scriptFullPath));
            }

            var scriptLocalPath = scriptFullPath;
            if (new Uri(scriptFullPath).IsUnc)
            {
                // Copy the file to a local temp directory
                var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }

                _diagnostics.Log(
                    LevelToLog.Info,
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resources.PowershellActionBuilder_CopyRemoteScript_ToLocalDirectory_WithSourcePath,
                        scriptFullPath));

                var originDirectory = Path.GetDirectoryName(scriptFullPath);
                foreach (var sourcePath in Directory.GetFiles(originDirectory))
                {
                    var destinationPath = Path.Combine(tempDirectory, Path.GetFileName(sourcePath));
                    _diagnostics.Log(
                        LevelToLog.Info,
                        string.Format(
                            CultureInfo.InvariantCulture,
                            Resources.PowershellActionBuilder_CopyRemoteScript_ToLocalDirectory_WithSourcePathAndDestinationPath,
                            scriptFullPath,
                            destinationPath));

                    File.Copy(sourcePath, destinationPath);
                }

                scriptLocalPath = Path.Combine(tempDirectory, Path.GetFileName(scriptFullPath));
            }

            var startInfo = new ProcessStartInfo();
            {
                startInfo.FileName = @"c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

                // Build the command line arguments
                startInfo.Arguments = string.Format(
                    CultureInfo.InvariantCulture,
                    "-nologo -noprofile -noninteractive -windowstyle hidden -file \"{0}\" {1}",
                    scriptLocalPath,
                    arguments);

                // do not display an error dialog if the process
                // can't be started
                startInfo.ErrorDialog = false;

                // Do not use the system shell. We want to
                // be able to redirect the output streams
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;

                // Redirect the standard output / error streams
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
            }

            _diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.PowershellActionBuilder_ExecutingScript_WithScriptAndArguments,
                    startInfo.FileName,
                    startInfo.Arguments));
            try
            {
                // Start the console app?
                using (var exec = new Process())
                {
                    // Define the application we want to start
                    exec.StartInfo = startInfo;

                    // Redirect the output and error stream. Note that
                    // we need to be careful how to read from these streams
                    // in order to prevent deadlocks from happening
                    // see e.g. here: http://msdn.microsoft.com/en-us/library/system.diagnostics.process.errordatareceived.aspx
                    exec.ErrorDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            var output = string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.PowershellActionBuilder_ErrorWhileRunningPowershell_WithError,
                                e.Data);
                            _diagnostics.Log(
                                LevelToLog.Warn,
                                output);
                        }
                    };
                    exec.OutputDataReceived += (s, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            var output = string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.PowershellActionBuilder_OutputWhileRunning_WithOutput,
                                e.Data);
                            _diagnostics.Log(
                                LevelToLog.Debug,
                                output);
                        }
                    };

                    // Start the process
                    exec.Start();
                    exec.BeginErrorReadLine();
                    exec.BeginOutputReadLine();

                    // Wait for the process to exit
                    exec.WaitForExit();

                    // Notify the user that the process has exited.
                    {
                        _diagnostics.Log(
                            LevelToLog.Info,
                            Resources.PowershellActionBuilder_Output_ProcessCompleted);
                    }
                }
            }
            catch (Exception e)
            {
                var log = string.Format(
                    CultureInfo.InvariantCulture,
                    Resources.PowershellActionbuilder_Error_FailedToRunExe_WithError,
                    e);
                _diagnostics.Log(
                    LevelToLog.Error,
                    log);
            }
        }
    }
}
