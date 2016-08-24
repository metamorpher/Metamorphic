//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
// Copyright (c) Metamorphic. All rights reserved.
// Licensed under the Apache License, Version 2.0 license. See LICENCE.md file in the project root for full license information.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using Metamorphic.Core.Rules;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server
{
    /// <summary>
    /// Defines a reference to a parameter provided by a signal and the conditions
    /// placed on that parameter.
    /// </summary>
    internal sealed class ActionParameterValue
    {
        /// <summary>
        /// The regex that is used to extract the parameter names from a string so that we can
        /// upper case them.
        /// </summary>
        private static readonly Regex StringParameterTransform = new Regex(
            RuleConstants.TriggerParameterRegex,
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// If the parameter value is a string and it contains a section like {{signal.PARAMETER_NAME}} then we
        /// replace the 'PARAMETER_NAME' section with the upper case variant so that we can do a parameter match
        /// later on.
        /// </summary>
        /// <param name="value">The parameter value.</param>
        /// <returns>The processed parameter value.</returns>
        private static object TransformParameterValueIfRequired(object value)
        {
            if (!(value is string))
            {
                return value;
            }

            var stringValue = value as string;
            return StringParameterTransform.Replace(
                stringValue,
                m => string.Format(
                    CultureInfo.InvariantCulture,
                    "{{{{signal.{0}}}}}",
                    m.Groups[1].ToString().ToUpper(CultureInfo.InvariantCulture)));
        }

        /// <summary>
        /// The collection containing the ordered list of signal parameter names that should be used for
        /// the action parameter value. Note that all parameter names are
        /// stored in lower case so as to provide case-insensitive comparisons between the signal and
        /// rule parameter names.
        /// </summary>
        private readonly List<string> _signalParameters
            = new List<string>();

        /// <summary>
        /// The value of the parameter. May be null if the value should be taken from the signal.
        /// </summary>
        private readonly object _value;

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterValue"/> class.
        /// </summary>
        /// <param name="parameterValue">
        ///     The value of the parameter. Should be <see langword="null" /> if the value should be taken from the signal.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterValue"/> is <see langword="null" />.
        /// </exception>
        public ActionParameterValue(object parameterValue)
        {
            {
                Lokad.Enforce.Argument(() => parameterValue);
            }

            _value = TransformParameterValueIfRequired(parameterValue);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ActionParameterValue"/> class.
        /// </summary>
        /// <param name="parameterFormat">The format string for the parameter value.</param>
        /// <param name="signalParameters">The name of the signal parameter that should be used as the value.</param>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="parameterFormat"/> is <see langword="null" />.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///     Thrown if <paramref name="parameterFormat"/> is an empty string.
        /// </exception>
        /// <exception cref="ArgumentNullException">
        ///     Thrown if <paramref name="signalParameters"/> is <see langword="null" />.
        /// </exception>
        public ActionParameterValue(string parameterFormat, IEnumerable<string> signalParameters)
        {
            {
                Lokad.Enforce.Argument(() => parameterFormat);
                Lokad.Enforce.Argument(() => parameterFormat, Lokad.Rules.StringIs.NotEmpty);

                Lokad.Enforce.Argument(() => signalParameters);
            }

            _value = TransformParameterValueIfRequired(parameterFormat);
            foreach (var parameter in signalParameters)
            {
                _signalParameters.Add(parameter.ToUpper(CultureInfo.InvariantCulture));
            }
        }

        /// <summary>
        /// Returns a value indicating whether the given signal has a parameter that matches the current
        /// parameter reference.
        /// </summary>
        /// <param name="signal">The signal.</param>
        /// <returns>
        ///     <see langword="true" /> if the given signal has a parameter that matches the current reference; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage(
            "Microsoft.StyleCop.CSharp.DocumentationRules",
            "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool IsValidFor(Signal signal)
        {
            if (signal == null)
            {
                return false;
            }

            if ((_value != null) && (_signalParameters.Count == 0))
            {
                return true;
            }

            foreach (var parameterName in _signalParameters)
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
        [SuppressMessage(
            "Microsoft.Design",
            "CA1062:Validate arguments of public methods",
            MessageId = "0",
            Justification = "The signal is validated through the IsValid method.")]
        public object ValueForParameter(Signal signal)
        {
            if (!IsValidFor(signal))
            {
                throw new InvalidSignalForRuleException();
            }

            var text = _value as string;
            if ((_value != null) && (_signalParameters.Count == 0))
            {
                if ((text != null) && signal.ContainsParameter(text))
                {
                    return signal.ParameterValue(text);
                }

                return _value;
            }

            foreach (var parameter in _signalParameters)
            {
                text = text.Replace(
                    "{{signal." + parameter + "}}",
                    signal.ParameterValue(parameter).ToString());
            }

            return text;
        }
    }
}
