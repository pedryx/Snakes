using NetCom;
using System.Windows;
using System.Windows.Controls;

namespace Snakes_Client.SnakeTypes
{
    /// <summary>
    /// Represent a part of snake's body.
    /// </summary>
    public class BodyPart : VisualObject
    {

        /// <summary>
        /// Represent gloval texture manager.
        /// </summary>
        private readonly TextureManager manager;

        /// <summary>
        /// Tile where body part is curently located.
        /// </summary>
        public Tile Tile { get; private set; }

        /// <summary>
        /// Owner of the body part.
        /// </summary>
        public Snake Snake { get; private set; }

        /// <summary>
        /// Direction of the body part.
        /// </summary>
        public Direction Direction { get; private set; }

        public Image Apperance
        {
            get => Visual as Image;
        }

        /// <summary>
        /// Create new body part.
        /// </summary>
        /// <param name="tile">Tile where body part will be located.</param>
        /// <param name="direction">Direction of the body part.</param>
        /// <param name="snake">Owner of the body part.</param>
        public BodyPart(Tile tile, Direction direction, Snake snake)
        {
            Snake = snake;
            manager = TextureManager.Instance;
            Direction = direction;
            Tile = tile;
            tile.Collision = true;
        }

        /// <summary>
        /// Move body part to a new tile.
        /// </summary>
        /// <param name="tile">Tile where body part will be moved.</param>
        /// <param name="direction">Direction of the body part.</param>
        public void Move(Tile tile, Direction direction)
        {
            Direction = direction;
            Tile.Collision = false;
            Tile = tile;
            Tile.Collision = true;
            Apperance.Margin = new Thickness(Tile.Position.X * Width - 3,
                  Tile.Position.Y * Height - 3, 0, 0);
            Apperance.Source = manager.GetTexture("SnakeHead", Snake.Color).GetRotation(Direction);
        }

        /// <summary>
        /// Tranform body part to body.
        /// </summary>
        /// <param name="prev">Direction of previous body part.</param>
        /// <param name="next">Direction of next body part.</param>
        public void TransformToBody(Direction prev, Direction next)
        {
            if (prev == next)
                Apperance.Source = manager.GetTexture("SnakeBody", Snake.Color).GetRotation(Direction);
            if ((next == Direction.Down && prev == Direction.Left) ||
                (next == Direction.Right && prev == Direction.Up))
            {
                Apperance.Source = manager.GetTexture("SnakeTurn", Snake.Color).GetRotation(0);
            }
            else if ((next == Direction.Up && prev == Direction.Left) ||
                (next == Direction.Right && prev == Direction.Down))
            {
                Apperance.Source = manager.GetTexture("SnakeTurn", Snake.Color).GetRotation(270);
            }
            else if ((next == Direction.Up && prev == Direction.Right) ||
                (next == Direction.Left && prev == Direction.Down))
            {
                Apperance.Source = manager.GetTexture("SnakeTurn", Snake.Color).GetRotation(180);
            }
            else if ((next == Direction.Left && prev == Direction.Up) ||
                (next == Direction.Down && prev == Direction.Right))
            {
                Apperance.Source = manager.GetTexture("SnakeTurn", Snake.Color).GetRotation(90);
            }
        }

        /// <summary>
        /// Transform body part to tail.
        /// </summary>
        /// <param name="direction">Direction of body part after the tail.</param>
        public void TransformToTail(Direction direction)
        {
            Direction = direction;
            Apperance.Source = manager.GetTexture("SnakeTail", Snake.Color).GetRotation(direction);
        }

        /// <summary>
        /// Create visual apperance for snake's body part.
        /// </summary>
        /// <returns></returns>
        protected override UIElement CreateVisual()
        {
            AddDestroyAction(() =>
            {
                Tile.Collision = false;
            });
            return new Image()
            {
                Margin = new Thickness(Tile.Position.X * Width - 3, Tile.Position.Y * Height - 3, 0, 0),
                Width = Width + 6,
                Height = Height + 6,
                Source = manager.GetTexture("SnakeHead", Snake.Color).GetRotation(Direction),
            };
        }
    }
}
