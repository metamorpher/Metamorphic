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
    public sealed class ActionParameterValue
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
        /// The name of the signal parameter that should be used for the action parameter value.
        /// </summary>
        private readonly string m_SignalParameter;

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

            m_Name = parameterName;
            m_Value = parameterValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterValue"/> class.
        /// </summary>
        /// <param name="parameterName">The name of the parameter to which the current reference applies.</param>
        /// <param name="signalParameter">The name of the signal parameter that should be used as the value.</param>
        /// <param name="condition">
        ///     The predicate used to verify if a given parameter value is allowed for the current
        ///     parameter. If there are no conditions on the parameter then <see langword="null" />
        ///     is allowed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterName"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="parameterName"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalParameter"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="signalParameter"/> is an empty string.
        /// </exception>
        public ActionParameterValue(string parameterName, string signalParameter, Predicate<object> condition = null)
        {
            {
                Lokad.Enforce.Argument(() => parameterName);
                Lokad.Enforce.Argument(() => parameterName, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => signalParameter);
                Lokad.Enforce.Argument(() => signalParameter, Lokad.Rules.StringIs.NotEmpty);
            }

            m_Name = parameterName;
            m_SignalParameter = signalParameter;
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

            if (m_Value != null)
            {
                return true;
            }

            if (!signal.ContainsParameter(m_SignalParameter))
            {
                return false;
            }

            return (m_Condition != null) ? m_Condition(signal.ParameterValue(m_SignalParameter)) : true;
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

            return m_Value ?? signal.ParameterValue(m_SignalParameter);
        }
    }
}