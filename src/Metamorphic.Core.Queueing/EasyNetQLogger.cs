//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2015 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Globalization;
using EasyNetQ;
using Nuclei.Diagnostics;
using Nuclei.Diagnostics.Logging;

namespace Metamorphic.Core.Queueing
{
    /// <summary>
    /// Implements the <see cref="IEasyNetQLogger"/> interface to allow logging EasyNetQ log messages.
    /// </summary>
    internal sealed class EasyNetQLogger : IEasyNetQLogger
    {
        /// <summary>
        /// The object that provides the diagnostics methods for the system.
        /// </summary>
        private readonly SystemDiagnostics m_Diagnostics;

        /// <summary>
        /// Initializes a new instance of the <see cref="EasyNetQLogger"/> class.
        /// </summary>
        /// <param name="diagnostics">The object that provides the diagnostics methods for the system.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="diagnostics"/> is <see langword="null" />.
        /// </exception>
        public EasyNetQLogger(SystemDiagnostics diagnostics)
        {
            if (diagnostics == null)
            {
                throw new ArgumentNullException("diagnostics");
            }

            m_Diagnostics = diagnostics;
        }

        public void DebugWrite(string format, params object[] args)
        {
            m_Diagnostics.Log(
                LevelToLog.Debug,
                string.Format(
                    CultureInfo.InvariantCulture,
                    format,
                    args));
        }

        public void ErrorWrite(Exception exception)
        {
            m_Diagnostics.Log(
                LevelToLog.Error,
                exception.ToString());
        }

        public void ErrorWrite(string format, params object[] args)
        {
            m_Diagnostics.Log(
                LevelToLog.Error,
                string.Format(
                    CultureInfo.InvariantCulture,
                    format,
                    args));
        }

        public void InfoWrite(string format, params object[] args)
        {
            m_Diagnostics.Log(
                LevelToLog.Info,
                string.Format(
                    CultureInfo.InvariantCulture,
                    format,
                    args));
        }
    }
}
