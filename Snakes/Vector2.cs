using NetCom;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Snakes_Client
{
    /// <summary>
    /// Represent a 2D vector.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct Vector2
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {

        private static readonly Dictionary<Vector2, Direction> vectors = new Dictionary<Vector2, Direction>()
        {
            { new Vector2(-1, 0), Direction.Left },
            { new Vector2(0, -1), Direction.Up },
            { new Vector2(1, 0), Direction.Right },
            { new Vector2(0, 1), Direction.Down },
        };

        /// <summary>
        /// Contain all relative positions.
        /// Used for graph calculation.
        /// </summary>
#pragma warning disable CA1819 // Properties should not return arrays
        public static Vector2[] RelativePositions
#pragma warning restore CA1819 // Properties should not return arrays
        {
            get => new Vector2[]
            {
                new Vector2(-1, 0),
                new Vector2(0, -1),
                new Vector2(1, 0),
                new Vector2(0, 1),
            };
        }


        public Double X { get; set; }

        public Double Y { get; set; }

        public Vector2(Double x, Double y)
        {
            X = x;
            Y = y;
        }

        public Direction ToDirection()
            => vectors[this];

        public static Vector2 Parse(String str)
        {
            var query = str.Split().Select(Double.Parse);

            return new Vector2(query.First(), query.Last());
        }

        public static Boolean operator >(Vector2 a, Int32 b)
            => a.X > b && a.Y > b;

        public static Boolean operator <(Vector2 a, Int32 b)
            => a.X < b && a.Y < b;

        public static Boolean operator >=(Vector2 a, Int32 b)
            => a.X >= b && a.Y >= b;

        public static Boolean operator <=(Vector2 a, Int32 b)
            => a.X <= b && a.Y <= b;

        public static Vector2 operator +(Vector2 a, Vector2 b)
            =>new Vector2(a.X + b.X, a.Y + b.Y);

        public static Vector2 operator -(Vector2 a, Vector2 b)
            => new Vector2(a.X - b.X, a.Y - b.Y);

    }
}
