using System;

namespace Snakes_Client
{

    /// <summary>
    /// Represent a handler for map change event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void MapChangeEventHandler(Object sender, MapChangeEventArgs e);

    /// <summary>
    /// Represent an arguments for map change event.
    /// </summary>
    public class MapChangeEventArgs : EventArgs
    {

        /// <summary>
        /// New map.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// Create new arguments for map change event.
        /// </summary>
        /// <param name="map">New map.</param>
        public MapChangeEventArgs(Map map)
        {
            Map = map;
        }

    }
}
