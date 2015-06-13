//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metamorphic.Server
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared",
        Justification = "Access modifiers should not be declared on the entry point for a command line application. See FxCop.")]
    static class Program
    {
        /// <summary>
        /// Defines the error code for a normal application exit (i.e without errors).
        /// </summary>
        private const int NormalApplicationExitCode = 0;

        /// <summary>
        /// Defines the error code for an application exit with an unhandled exception.
        /// </summary>
        private const int UnhandledExceptionApplicationExitCode = 1;

        /// <summary>
        /// The default name for the error log.
        /// </summary>
        private const string DefaultErrorFileName = "service.error.{0}.log";

        [STAThread]
        static void Main(string[] args)
        {
        }
    }
}
