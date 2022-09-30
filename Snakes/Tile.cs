using Snakes_Client.AI;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;


namespace Snakes_Client
{
    /// <summary>
    /// Represent a tile on the map.
    /// </summary>
    public class Tile : VisualObject, INode
    {

        /// <summary>
        /// Detemine if snakes can enter the tile.
        /// </summary>
        private Boolean collision = false;
        /// <summary>
        /// Food that is on the tile.
        /// Null if no food is on the tile.
        /// </summary>
        private Food food;
        

        /// <summary>
        /// Determine if tile is wall.
        /// </summary>
        public Boolean Wall { get; private set; }

        /// <summary>
        /// Position of tile.
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Tile's owner.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// Food that is on the tile.
        /// Null if no food is on the tile.
        /// </summary>
        public Food Food
        {
            get => food;
            set
            {
                food = value;
                if (food != null)
                    food.Move(new Vector2(Width * Position.X, Height * Position.Y));
            }
        }

        /// <summary>
        /// Determine if snakes can enter the tile.
        /// Snakes can set this to true when they want to occupy the tile.
        /// </summary>
        public Boolean Collision
        {
            get => collision || Wall;
            set { collision = value; }
        }

        /// <summary>
        /// Create new tile/
        /// </summary>
        /// <param name="map">Tile's owner.</param>
        /// <param name="position">Tile's position.</param>
        /// <param name="wall">Determine if tile is the wall.</param>
        public Tile(Map map, Vector2 position, Boolean wall)
        {
            Map = map;
            Position = position;
            Wall = wall;
        }

        /// <summary>
        /// Get all tile's neighbors.
        /// </summary>
        public IEnumerable<INode> GetNeighbors()
        {
            HashSet<Tile> neighbors = new HashSet<Tile>();
            foreach (var relativePosition in Vector2.RelativePositions)
            {
                Vector2 current = relativePosition + Position;
                if (Map.HasTileOn(current) && !Map[current].Collision)
                    neighbors.Add(Map[current]);
            }

            return neighbors;
        }

        /// <summary>
        /// Create visual apperance for tile.
        /// </summary>
        protected override UIElement CreateVisual()
        {
            if (!Wall)
                return null;

            return new Rectangle()
            {
                Fill = Brushes.Gray,
                Margin = new Thickness(Position.X * Width - 1,
                    Position.Y * Height - 1, 0, 0),
                Width = Width + 2,
                Height = Height + 2,
            };
        }

    }
}
