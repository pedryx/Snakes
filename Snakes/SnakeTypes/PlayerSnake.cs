using NetCom;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;

namespace Snakes_Client.SnakeTypes
{
    /// <summary>
    /// Represent a snake controlled by human player (WASD).
    /// </summary>
    public class PlayerSnake : Snake
    {

        /// <summary>
        /// Represent a keys used for snake control.
        /// </summary>
        private readonly Key[] controlKeys;

        /// <summary>
        /// Snake's last direction.
        /// </summary>
        public Direction LastDirection { get; set; }

        /// <summary>
        /// Represent a network client.
        /// </summary>
        public Client Client { get; set; }

        /// <summary>
        /// Create new snake controlled by the human player.
        /// </summary>
        /// <param name="color">Color of the snake.</param>
        /// <param name="controlKeys">
        /// Keys which will be used to control the snake.
        /// They must be in { Up, Left, Down, Right } order.
        /// </param>
        public PlayerSnake(Color color, UInt32 id, Key[] controlKeys) 
            : base(color, id)
        {
            this.controlKeys = controlKeys;
        }

        private Direction GetDirection(Key key)
        {
            if (key == controlKeys[0])
                return Direction.Up;
            else if (key == controlKeys[1])
                return Direction.Left;
            else if (key == controlKeys[2])
                return Direction.Down;
            else
                return Direction.Right;
        }

        public override Direction MakeMove()
            => LastDirection;

        public void Window_KeyDown(Object sender, KeyEventArgs e)
        {
            if (controlKeys.Contains(e.Key))
            {
                if (Client != null)
                    Client.SendMovement(Id, GetDirection(e.Key));
                else
                    LastDirection = GetDirection(e.Key);
            }
        }

    }
}
