using NetCom;

using System;
using System.Windows.Media;


namespace Snakes_Client.SnakeTypes
{
    public class NetworkSnake : Snake
    {

        public Direction LastDirection { get; set; }

        public NetworkSnake(Color color, UInt32 id) 
            : base(color, id) { }

        public override Direction MakeMove()
            => LastDirection;

    }
}
