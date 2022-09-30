using System;

namespace Snakes_Client
{

    /// <summary>
    /// Represent delegate for snake spawn event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void SpawnEventHandler(Object sender, SpawnEventAergs e);

    /// <summary>
    /// Arguments for snake spawn event.
    /// </summary>
    public class SpawnEventAergs : EventArgs
    {

        /// <summary>
        /// Tile where snake spawned.
        /// </summary>
        public Tile Tile { get; private set; }

        /// <summary>
        /// retae arguments for snake event.
        /// </summary>
        /// <param name="tile">Tile where snake spawned.</param>
        public SpawnEventAergs(Tile tile)
        {
            Tile = tile;
        }

    }
}
