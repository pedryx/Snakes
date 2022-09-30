using Snakes_Client.SnakeTypes;

using System;
using System.Collections.Generic;
using System.Windows.Controls;


namespace Snakes_Client
{
    /// <summary>
    /// Represent a snake game.
    /// </summary>
    public class Game
    {

        /// <summary>
        /// Contains all snakes that participate in the game.
        /// </summary>
        private IEnumerable<Snake> snakes;

        /// <summary>
        /// Represents game's map.
        /// </summary>
        public Map Map { get; private set; }

        /// <summary>
        /// Window in which is game running.
        /// </summary>
        public MainWindow Window { get; private set; }

        /// <summary>
        /// Create new game.
        /// </summary>
        /// <param name="mapFile">File that represents map, which will be loaded.</param>
        /// <param name="window">Main window.</param>
        public Game(MainWindow window, Map map)
        {
            Map = map;
            Window = window;
        }

        /// <summary>
        /// Initialize the game.
        /// </summary>
        /// <param name="panel">UI control that will own control for apperance.</param>
        /// <param name="snakes">Snakes which will participate in the game.</param>
        /// <param name="foods">Foods that can spawn on map.</param>
        public void Initialize(Panel panel, IEnumerable<Snake> snakes, IEnumerable<Food> foods, Int32 seed)
        {
            this.snakes = snakes;

            Double width = panel.ActualWidth / Map.Size.X;
            Double height = panel.ActualHeight / Map.Size.Y;

            foreach (var tile in Map)
                tile.CreateVisual(panel, width, height);
            foreach (var food in foods)
                food.InitVisual(panel, width, height);
            foreach (var snake in snakes)
            {
                snake.CreateVisual(panel, width, height);
                snake.Spawn(Map);
            }

            Map.Initialize(foods, seed);
            Map.SpawnFood();
        }

        /// <summary>
        /// Update game state.
        /// </summary>
        public void Update()
        {
            foreach (var snake in snakes)
                snake.Update();
            foreach (var snake in snakes)
            {
                if (!snake.Alive && snake.VisualAlive)
                {
                    snake.DestroyVisual();
                    snake.VisualAlive = false;
                }
            }
        }

    }
}
