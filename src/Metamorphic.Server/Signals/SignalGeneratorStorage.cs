//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Signals;
using Metamorphic.Server.Properties;

namespace Metamorphic.Server.Signals
{
    internal sealed class SignalGeneratorStorage : IStoreSignalGenerators
    {
        /// <summary>
        /// The collection of known actions.
        /// </summary>
        private readonly Dictionary<SignalTypeId, IGenerateSignals> m_Generators = new Dictionary<SignalTypeId, IGenerateSignals>();

        /// <summary>
        /// Returns the <see cref="IGenerateSignals"/> instance which generates a signal with the given <see cref="SignalTypeId"/>.
        /// </summary>
        /// <param name="signalType">The ID of the signal type that the generator creates.</param>
        /// <returns>The signal generator which generates a signal with of the given type.</returns>
        public IGenerateSignals Generator(SignalTypeId signalType)
        {
            if (!HasGeneratorFor(signalType))
            {
                return null;
            }

            return m_Generators[signalType];
        }

        /// <summary>
        /// Adds a new <see cref="IGenerateSignals"/> to the collection.
        /// </summary>
        /// <param name="generator">The signal generator that should be added.</param>
        public void Add(IGenerateSignals generator)
        {
            {
                Lokad.Enforce.Argument(() => generator);
                Lokad.Enforce.With<DuplicateGeneratorDefinitionException>(
                    !HasGeneratorFor(generator.Id),
                    Resources.Exceptions_Messages_DuplicateSignalGenerator);
            }

            m_Generators.Add(generator.Id, generator);
        }

        /// <summary>
        /// Returns a value indicating whether the storage has an <see cref="IGenerateSignals"/>
        /// which generates signals of the the given <see cref="SignalTypeId"/>.
        /// </summary>
        /// <param name="signalType">The ID of the signal type that the generator creates.</param>
        /// <returns>
        ///   <see langword="true" /> if the storage has a generator that generates signals with of the given type; otherwise, <see langword="false" />.
        /// </returns>
        [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1628:DocumentationTextMustBeginWithACapitalLetter",
            Justification = "Documentation can start with a language keyword")]
        public bool HasGeneratorFor(SignalTypeId signalType)
        {
            if (signalType == null)
            {
                return false;
            }

            return m_Generators.ContainsKey(signalType);
        }
    }
}
