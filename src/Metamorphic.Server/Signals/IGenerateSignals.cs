namespace Metamorphic.Server.Signals
{
    internal interface IGenerateSignals
    {
        /// <summary>
        /// Starts the signal generating process.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the signal generating process.
        /// </summary>
        void Stop();
    }
}