using System;
using System.Collections.Generic;
using System.Text;

namespace Snakes_Client
{

    /// <summary>
    /// Represents handler for game init event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void GameInitEventHandler(Object sender, GameInitEventArgs e);

    /// <summary>
    /// Represent an arguments for game init event.
    /// </summary>
    public class GameInitEventArgs
    {

        /// <summary>
        /// Random generator's seed.
        /// </summary>
        public Int32 Seed { get; private set; }

        /// <summary>
        /// Create new argument for game init event.
        /// </summary>
        /// <param name="seed">Random generator's seed.</param>
        public GameInitEventArgs(Int32 seed)
        {
            Seed = seed;
        }

    }
}
