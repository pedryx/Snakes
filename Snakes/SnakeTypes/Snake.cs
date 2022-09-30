using NetCom;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;

namespace Snakes_Client.SnakeTypes
{
    /// <summary>
    /// Represent a base class for snake.
    /// </summary>
    public abstract class Snake : IEnumerable<BodyPart>
    {

        #region static
        /// <summary>
        /// Contains relative positions for all directions.
        /// </summary>
        private static readonly Dictionary<Direction, Vector2> directions = new Dictionary<Direction, Vector2>()
        {
            { Direction.Up, new Vector2(0, -1) },
            { Direction.Left, new Vector2(-1, 0) },
            { Direction.Down, new Vector2(0, 1) },
            { Direction.Right, new Vector2(1, 0) },
        };

        /// <summary>
        /// Contains textures for pre-loading.
        /// </summary>
        private static readonly String[] textures = { "SnakeBody", "SnakeHead", "SnakeTail", "SnakeTurn" };
        #endregion

        
        /// <summary>
        /// Contains all body part that are part of the snake.
        /// (Head is last, tail is first.)
        /// </summary>
        private readonly Queue<BodyPart> bodyParts;
        /// <summary>
        /// Contains actions, which will occur when visual apperance is destroyed.
        /// </summary>
        private readonly List<Action> destroyActions;

        /// <summary>
        /// Represent a current direction of snake.
        /// </summary>
        private Direction current;
        /// <summary>
        /// Represent a previos direction os snake.
        /// </summary>
        private Direction prev;
        /// <summary>
        /// Parent of UI elements which represents visual apperance.
        /// </summary>
        public Panel panel;
        /// <summary>
        /// Elements width.
        /// </summary>
        private Double width;
        /// <summary>
        /// Elements height.
        /// </summary>
        private Double height;

        /// <summary>
        /// Color of the snake.
        /// </summary>
        public Color Color { get; private set; }

        /// <summary>
        /// Represent snake's score.
        /// </summary>
        public Int32 Score { get; private set; }

        /// <summary>
        /// Determine if its game over for this snake.
        /// </summary>
        public Boolean Alive { get; private set; }

        /// <summary>
        /// Detemrine if snak'es visual is alive.
        /// </summary>
        public Boolean VisualAlive { get; set; } = true;

        /// <summary>
        /// Represent the amount of body parts thats didnt grown yet.
        /// </summary>
        public Single Fat { get; private set; } = 3;

        /// <summary>
        /// Game map.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// Identificator.
        /// (Used only for networking.)
        /// </summary>
        public UInt32 Id { get; set; }

        /// <summary>
        /// Get location of snake head.
        /// </summary>
        public Tile Location
        {
            get => bodyParts.Last().Tile;
        }

        /// <summary>
        /// Occur when snake has been spawned.
        /// </summary>
        public event SpawnEventHandler SnakeSpawned;

        /// <summary>
        /// Occur when snake's stat has been changed.
        /// </summary>
        public event StatChangeEventHandler StatChanged;

        /// <summary>
        /// Create new snake.
        /// </summary>
        /// <param name="color">Color of the snake.</param>
        protected Snake(Color color, UInt32 id)
        {
            bodyParts = new Queue<BodyPart>();
            destroyActions = new List<Action>();

            Id = id;
            current = Direction.Right;
            Alive = true;

            Double[] hsv = Texture.RgbToHsv(color);
            Color = Texture.HsvToRgb(hsv[0], hsv[1], 1);
            TextureManager.Instance.LoadTextures(color, textures);
        }

        /// <summary>
        /// Determine where snake will move next.
        /// </summary>
        /// <returns>Direction of snake's next move.</returns>
        public abstract Direction MakeMove();

        /// <summary>
        /// Create visual apperance.
        /// </summary>
        /// <param name="panel">Owner of UI elements which are representing visual apperance.</param>
        /// <param name="width">Elements width.</param>
        /// <param name="height">Elements height.</param>
        public void CreateVisual(Panel panel, Double width, Double height)
        {
            this.panel = panel;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Destroy visual apperance.
        /// </summary>
        public void DestroyVisual()
        {
            foreach (var bodyPart in bodyParts)
                bodyPart.DestroyVisual();
            foreach (var action in destroyActions)
                action.Invoke();
            destroyActions.Clear();
        }

        /// <summary>
        /// Add new action that will occur when visual apperance is destroyed.
        /// </summary>
        /// <param name="action">Action that will occur when visual apperance is destroyed.</param>
        public void AddDestroyAction(Action action)
        {
            destroyActions.Add(action);
        }

        /// <summary>
        /// Spawn snake to the map.
        /// </summary>
        public void Spawn(Map map)
        {
            Map = map;
            Tile spawn = Map.GetSpawn();

            BodyPart head = new BodyPart(spawn, Direction.Right, this);
            head.CreateVisual(panel, width, height);
            bodyParts.Enqueue(head);
            Fat--;

            SnakeSpawned?.Invoke(this, new SpawnEventAergs(spawn));
            StatChanged?.Invoke(this, new StatChangeEventArgs(Fat, Score, Alive));
        }

        /// <summary>
        /// Update snake's inner state.
        /// </summary>
        public void Update()
        {
            if (!Alive)
                return;


            prev = current;
            current = MakeMove();
            Vector2 nextPosition = directions[current] + bodyParts.Last().Tile.Position;
            if (!Map.HasTileOn(nextPosition))
            {
                GameOver();
                return;
            }
            Tile next = Map[nextPosition];

            if (next.Collision)
            {
                GameOver();
                return;
            }
            else if (next.Food != null)
                Eat(next);

            bodyParts.Last().TransformToBody(prev, current);
            if (Fat < 1)
            {
                BodyPart tail = bodyParts.Dequeue();
                tail.Move(next, current);
                bodyParts.Enqueue(tail);
            }
            else
            {
                BodyPart tail = new BodyPart(next, current, this);
                tail.CreateVisual(panel, width, height);
                bodyParts.Enqueue(tail);
                Fat--;
                StatChanged?.Invoke(this, new StatChangeEventArgs(Fat, Score, Alive));
            }
            bodyParts.Peek().TransformToTail(bodyParts.Skip(1).First().Direction);
        }

        /// <summary>
        /// Ends game for this snake.
        /// </summary>
        private void GameOver()
        {
            Alive = false;
            StatChanged?.Invoke(this, new StatChangeEventArgs(Fat, Score, Alive));
        }


        /// <summary>
        /// Makes snake eat the food.
        /// </summary>
        /// <param name="tile">Tile where food is located.</param>
        private void Eat(Tile tile)
        {
            Score += tile.Food.Score;
            Fat += tile.Food.Fat;
            if (tile.Food.FoodSpawn)
                Map.SpawnFood();
            tile.Food.DestroyVisual();
            tile.Food = null;
            StatChanged?.Invoke(this, new StatChangeEventArgs(Fat, Score, Alive));
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerator<BodyPart> GetEnumerator()
            => bodyParts.GetEnumerator();

        
    }
}
