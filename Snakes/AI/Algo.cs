using System;
using System.Collections.Generic;
using System.Linq;

namespace Snakes_Client.AI
{
    /// <summary>
    /// Represent a static class for common alghoritms.
    /// </summary>
    public static class Algo
    {

        /// <summary>
        /// Breath first search.
        /// </summary>
        /// <param name="start">Start node.</param>
        /// <param name="goals">list of goal nodes.</param>
        /// <returns>Pathes from start to goal nodes.</returns>
        public static List<INode> BFS(INode start, params INode[] goals)
        {
            List<INode> visited = new List<INode>();
            Dictionary<INode, INode> cameFrom = new Dictionary<INode, INode>();
            Queue<INode> frontier = new Queue<INode>();

            visited.Add(start);
            frontier.Enqueue(start);
            while (frontier.Count != 0)
            {
                INode current = frontier.Dequeue();
                if (goals.Contains(current))
                    return CreatePath(cameFrom, start, current);

                foreach (var neighbor in current.GetNeighbors())
                {
                    if (visited.Contains(neighbor))
                        continue;

                    cameFrom[neighbor] = current;
                    visited.Add(neighbor);
                    frontier.Enqueue(neighbor);
                }
            }

            return null;
        }

        /// <summary>
        /// Create path from goal to start.
        /// </summary>
        /// <param name="cameFrom">Database with camefrom data.</param>
        /// <param name="start">Start node.</param>
        /// <param name="goal">Goal node.</param>
        /// <returns>Path from goal to start.</returns>
        private static List<INode> CreatePath(Dictionary<INode, INode> cameFrom, INode start, INode goal)
        {
            List<INode> path = new List<INode>();
            INode current = goal;
            path.Add(current);

            while (current != start)
            {
                current = cameFrom[current];
                path.Add(current);
            }
            path.Reverse();

            return path;
        }

    }
}
