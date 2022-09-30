using System.Collections.Generic;

namespace Snakes_Client.AI
{
    /// <summary>
    /// Represent a graph's node.
    /// </summary>
    public interface INode
    {

        /// <summary>
        /// Get all node's neighbors.
        /// </summary>
        /// <returns>All node's neighbors.</returns>
        public IEnumerable<INode> GetNeighbors();

    }
}
