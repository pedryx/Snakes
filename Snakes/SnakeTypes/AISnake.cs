using NetCom;
using Snakes_Client.AI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;


namespace Snakes_Client.SnakeTypes
{
    public class AISnake : Snake
    {

        private Direction lastDirection;

        public AISnake(Color color, UInt32 id) 
            : base(color, id) { }

        public override Direction MakeMove()
        {
            IEnumerable<INode> path = Algo.BFS(Location, Map.SpawnedFoods.Keys.ToArray());

            if (path == null)
                return lastDirection;

            Tile next = (Tile)path.Skip(1).First();
            lastDirection = (next.Position - Location.Position).ToDirection();
            return lastDirection;
        }
    }
}
