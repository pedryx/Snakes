using System;

namespace Snakes_Client
{
    /// <summary>
    /// Represent a handler for snake's stat change event.
    /// </summary>
    /// <param name="sender">Event sender.</param>
    /// <param name="e">Event's arguments.</param>
    public delegate void StatChangeEventHandler(Object sender, StatChangeEventArgs e);

    /// <summary>
    /// Represent an arguments for snake's stat change event.
    /// </summary>
    public class StatChangeEventArgs : EventArgs
    {

        /// <summary>
        /// Current snake's fat.
        /// </summary>
        public Single Fat { get; private set; }

        /// <summary>
        /// Current snake's score.
        /// </summary>
        public Int32 Score { get; private set; }

        /// <summary>
        /// Determine if snake is alive.
        /// </summary>
        public Boolean Alive { get; private set; }

        /// <summary>
        /// Createnew arguments for snake's stat change event.
        /// </summary>
        /// <param name="fat">Current snake's fat.</param>
        /// <param name="score">Current snake's score.</param>
        /// <param name="alive">Determine if snake is alive.</param>
        public StatChangeEventArgs(Single fat, Int32 score, Boolean alive)
        {
            Fat = fat;
            Score = score;
            Alive = alive;
        }

    }
}
