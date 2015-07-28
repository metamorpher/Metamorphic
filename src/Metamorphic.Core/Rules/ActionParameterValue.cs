//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Signals;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines a reference to a parameter provided by a signal and the conditions
    /// placed on that parameter.
    /// </summary>
    public sealed class ActionParameterValue
    {
        /// <summary>
        /// The name of the parameter. Note the parameter name is stored in lower case so as to provide 
        /// case-insensitive comparisons between the signal and rule parameter names.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The collection containing the ordered list of signal parameter names that should be used for 
        /// the action parameter value. Note that all parameter names are
        /// stored in lower case so as to provide case-insensitive comparisons between the signal and
        /// rule parameter names.
        /// </summary>
        private readonly List<string> m_SignalParameters
            = new List<string>();

        /// <summary>
        /// The value of the parameter. May be null if the value should be taken from the signal.
        /// </summary>
        private readonly object m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterValue"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to which the current reference applies.</param>
        /// <param name="parameterValue">
        ///     The value of the parameter. Should be <see langword="null" /> if the value should be taken from the signal.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="parameterName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterValue"/> is <see langword="null" />.
        /// </exception>
        public ActionParameterValue(string parameterName, object parameterValue)
        {
            {
                Lokad.Enforce.Argument(() => parameterName);
                Lokad.Enforce.Argument(() => parameterName, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => parameterValue);
            }

            m_Name = parameterName.ToLower();
            m_Value = parameterValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterValue"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to which the current reference applies.</param>
        /// <param name="parameterFormat">The format string for the parameter value.</param>
        /// <param name="signalParameters">The name of the signal parameter that should be used as the value.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="parameterName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterFormat"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="parameterFormat"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalParameters"/> is <see langword="null" />.
        /// </exception>
        public ActionParameterValue(string parameterName, string parameterFormat, List<string> signalParameters)
        {
            {
                Lokad.Enforce.Argument(() => parameterName);
                Lokad.Enforce.Argument(() => parameterName, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => parameterFormat);
                Lokad.Enforce.Argument(() => parameterFormat, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => signalParameters);
            }

            m_Name = parameterName.ToLower();
            m_Value = parameterFormat;
            foreach (var parameter in signalParameters)
            {
                m_SignalParameters.Add(parameter.ToLower());
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given signal has a parameter that matches the current
        /// parameter reference.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns></returns>
        /// <returns>
        ///     <see langword="true" /> if the given signal has a parameter that matches the current reference; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsValidFor(Signal signal)
        {
            if (signal == null)
            {
                return false;
            }

            if ((m_Value != null) && (m_SignalParameters.Count == 0))
            {
                return true;
            }

            foreach (var parameterName in m_SignalParameters)
            {
                if (!signal.ContainsParameter(parameterName))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Returns the value for a the signal parameter with the same name as the current reference.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>
        ///     The value for the signal parameter.
        /// </returns>
        public object ValueForParameter(Signal signal)
        {
            if (!IsValidFor(signal))
            {
                throw new InvalidSignalForRuleException();
            }

            if ((m_Value != null) && (m_SignalParameters.Count == 0))
            {
                return m_Value;
            }

            if (m_SignalParameters.Count == 1)
            {
                return signal.ParameterValue(m_SignalParameters[0]);
            }

            var text = m_Value as string;
            foreach (var parameter in m_SignalParameters)
            {
                text = text.Replace(
                    "{{signal." + parameter + "}}",
                    (string)signal.ParameterValue(parameter));
            }

            return text;
        }
    }
}