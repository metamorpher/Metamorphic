//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using Metamorphic.Core.Properties;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core.Actions
{
    /// <summary>
    /// Defines methods for building an action that executes a Powershell script.
    /// </summary>
    internal sealed class PowershellActionBuilder : IActionBuilder
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the application.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowershellActionBuilder"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the application.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public PowershellActionBuilder(SystemDiagnostics diagnostics)
        {
            {
                Lokad.Enforce.Argument(() => diagnostics);
            }

            m_Diagnostics = diagnostics;
        }

        private void InvokePowershell(string scriptFile)
        {
            var startInfo = new ProcessStartInfo();
            {
                startInfo.FileName = @"c:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe";

                // Build the command line arguments
                startInfo.Arguments = string.Format("-nologo -noprofile -noninteractive -file \"{0}\" ", scriptFile);

                // do not display an error dialog if the process
                // can't be started
                startInfo.ErrorDialog = false;

                // Do not use the system shell. We want to
                // be able to redirect the output streams
                startInfo.UseShellExecute = false;

                // Redirect the standard output / error streams
                startInfo.RedirectStandardOutput = true;
                startInfo.RedirectStandardError = true;
            };

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
                        // There is no reason to get a lock
                        // before setting the value because
                        // we wait for the process to exit before
                        // checking the value. So there is no
                        // simultaneous access at any point.
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            var output = string.Format(
                                CultureInfo.InvariantCulture,
                                Resources.PowershellActionBuilder_ErrorWhileRunningPowershell_WithError,
                                e.Data);
                            m_Diagnostics.Log(
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
                            m_Diagnostics.Log(
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
                        m_Diagnostics.Log(
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
                m_Diagnostics.Log(
                    LevelToLog.Error,
                    log);
            }
        }

        /// <summary>
        /// Returns a new <see cref="ActionDefinition"/>.
        /// </summary>
        /// <returns>A new action definition.</returns>
        public ActionDefinition ToDefinition()
        {
            var id = new ActionId("powershell");

            var parameters = new List<ActionParameterDefinition>
            {
                new ActionParameterDefinition("scriptFile"),
            };

            Delegate method = new Action<string>(InvokePowershell);

            return new ActionDefinition(
                id,
                parameters.ToArray(),
                method);
        }
    }
}
