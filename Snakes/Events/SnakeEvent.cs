using NetCom;

using Snakes_Client.SnakeTypes;

using System;


namespace Snakes_Client
{

    /// <summary>
    /// Represent a handler for snake event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event arguments.</param>
    public delegate void SnakeEventHandler(Object sender, SnakeEventArgs e);

    /// <summary>
    /// Represents an arguments for snake event.
    /// </summary>
    public class SnakeEventArgs : EventArgs
    {

        /// <summary>
        /// Represent an informations about snake.
        /// </summary>
        public SnakeInfo SnakeInfo { get; private set; }

        /// <summary>
        /// Snake.
        /// </summary>
        public Snake Snake { get; private set; }


        /// <summary>
        /// Create new arguments for snake event.
        /// </summary>
        /// <param name="snakeInfo">Represent an informations about snake.</param>
        /// <param name="userSnake">Detrmine if snake has been added by user.</param>
        public SnakeEventArgs(SnakeInfo snakeInfo, Snake snake)
        {
            SnakeInfo = snakeInfo;
            Snake = snake;
        }

    }
}
