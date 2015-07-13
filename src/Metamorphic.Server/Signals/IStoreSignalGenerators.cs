//-----------------------------------------------------------------------
// <copyright company="Metamorphic">
//     Copyright 2013 Metamorphic. Licensed under the Apache License, Version 2.0.
// </copyright>
//-----------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using Metamorphic.Core.Signals;

namespace Metamorphic.Server.Signals
{
    internal interface IStoreSignalGenerators
    {
        /// <summary>
        /// Returns the <see cref="IGenerateSignals"/> instance which generates a signal with the given <see cref="SignalTypeId"/>.
        /// </summary>
        /// <param name="signalType">The ID of the signal type that the generator creates.</param>
        /// <returns>The signal generator which generates a signal with of the given type.</returns>
        IGenerateSignals Generator(SignalTypeId signalType);

        /// <summary>
        /// Adds a new <see cref="IGenerateSignals"/> to the collection.
        /// </summary>
        /// <param name="generator">The signal generator that should be added.</param>
        void Add(IGenerateSignals generator);

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
        bool HasGeneratorFor(SignalTypeId signalType);
    }
}