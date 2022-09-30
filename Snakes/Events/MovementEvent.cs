using NetCom;

using System;


namespace Snakes_Client
{

    /// <summary>
    /// Represent a handler for movement event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void MovementEventHandler(Object sender, MovementEventArgs e);

    /// <summary>
    /// Represent an arguments for movement event.
    /// </summary>
    public class MovementEventArgs : EventArgs
    {

        /// <summary>
        /// Id of snake, who moved.
        /// </summary>
        public UInt32 Id { get; private set; }

        /// <summary>
        /// Direction of movement.
        /// </summary>
        public Direction Direction { get; private set; }

        /// <summary>
        /// Create new movement event arguments.
        /// </summary>
        /// <param name="id">Id of snake, who moved.</param>
        /// <param name="direction">Direction of movement.</param>
        public MovementEventArgs(UInt32 id, Direction direction)
        {
            Id = id;
            Direction = direction;
        }

    }
}
