//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using Metamorphic.Core.Signals;
using System;
using System.Diagnostics.CodeAnalysis;

namespace Metamorphic.Core.Rules
{
    /// <summary>
    /// Defines a reference to a parameter provided by a signal and the conditions
    /// placed on that parameter.
    /// </summary>
    public sealed class SignalParameterReference
    {
        /// <summary>
        /// The predicate that will be used if a parameter does not have any conditions on it.
        /// </summary>
        private static readonly Predicate<object> s_PassThrough = o => true;

        /// <summary>
        /// The condition that the signal parameter has to match in order for the signal to match
        /// the current reference.
        /// </summary>
        private readonly Predicate<object> m_Condition;

        /// <summary>
        /// The name of the parameter.
        /// </summary>
        private readonly string m_Name;

        /// <summary>
        /// The value of the parameter. May be null if the value should be taken from the signal.
        /// </summary>
        private readonly object m_Value;

        /// <summary>
        /// Initializes a new instance of the <see cref="SignalParameterReference"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to which the current reference applies.</param>
        /// <param name="parameterValue">
        ///     The value of the parameter. Should be <see langword="null" /> if the value should be taken from the signal.
        /// </param>
        /// <param name="condition">
        ///     The predicate used to verify if a given parameter value is allowed for the current
        ///     parameter. If there are no conditions on the parameter then <see langword="null" />
        ///     is allowed.
        /// </param>
        public SignalParameterReference(string parameterName, object parameterValue = null, Predicate<object> condition = null)
        {
            {
                Lokad.Enforce.Argument(() => parameterName);
                Lokad.Enforce.Argument(() => parameterName, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Name = parameterName;
            m_Condition = condition ?? s_PassThrough;
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

            if (!signal.ContainsParameter(m_Name))
            {
                return false;
            }

            return m_Condition(signal.ParameterValue(m_Name));
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
            return m_Value ?? signal.ParameterValue(m_Name);
        }
    }
}