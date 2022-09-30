using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace Snakes_Client
{
    /// <summary>
    /// Represent a game map.
    /// </summary>
    public class Map : IEnumerable<Tile>
    {

        /// <summary>
        /// Contains tiles.
        /// Keys are positions of tiles.
        /// </summary>
        private readonly Dictionary<Vector2, Tile> tiles;
        /// <summary>
        /// Contains all tiles, where snakes can spawn.
        /// </summary>
        private readonly Queue<Tile> spawns;

        /// <summary>
        /// Contains all foods that can spawn on map.
        /// </summary>
        private IEnumerable<Food> foods;
        /// <summary>
        /// Random number generator.
        /// </summary>
        private Random random;

        /// <summary>
        /// Name of the map.
        /// </summary>
        public String Name { get; private set; }

        /// <summary>
        /// Map width and height.
        /// </summary>
        public Vector2 Size { get; private set; }

        /// <summary>
        /// File from which was map created.
        /// </summary>
        public String File { get; private set; }

        /// <summary>
        /// Contains all food, that is currently spawned.
        /// </summary>
        public Dictionary<Tile, Food> SpawnedFoods { get; private set; }

        /// <summary>
        /// Get spawn count.
        /// </summary>
        public Int32 SpawnCount
        {
            get => spawns.Count;
        }

        /// <summary>
        /// Create new map.
        /// </summary>
        /// <param name="file">Name of the map file.</param>
        public Map(String file)
        {
            tiles = new Dictionary<Vector2, Tile>();
            spawns = new Queue<Tile>();
            SpawnedFoods = new Dictionary<Tile, Food>();
            File = file;

            Load();
        }

        /// <summary>
        /// Initialize map.
        /// When map is not initialized, no food can be spawned on it.
        /// </summary>
        /// <param name="foods">Foods which can spawn on map.</param>
        public void Initialize(IEnumerable<Food> foods, Int32 seed)
        {
            this.foods = foods;
            random = new Random(seed);
        }

        /// <summary>
        /// Spawn new foods on map.
        /// </summary>
        public void SpawnFood()
        {
            var availbleTiles = tiles.Values.Where(t => !t.Collision && t.Food == null).ToList();
            foreach (var food in foods)
            {
                if (random.NextDouble() <= food.SpawnChance)
                {
                    Tile tile = availbleTiles[random.Next(availbleTiles.Count)];
                    tile.Food = food.CreatePrototype();
                    SpawnedFoods.Add(tile, tile.Food);
                    tile.Food.AddDestroyAction(() =>
                    {
                        SpawnedFoods.Remove(tile);
                    });
                    availbleTiles.Remove(tile);
                }
            }
        }

        /// <summary>
        /// Get map's tiles.
        /// </summary>
        /// <returns>Map's tiles.</returns>
        public IEnumerable<Tile> GetTiles()
            => tiles.Values;

        /// <summary>
        /// Get next tile where snake can spawn and removes it from spawn list.
        /// </summary>
        /// <returns>Next tile where snake can spawn.</returns>
        public Tile GetSpawn()
            => spawns.Dequeue();

        /// <summary>
        /// Determine if map has tile on specific position.
        /// </summary>
        /// <param name="position">Specific position.</param>
        /// <returns>True, if map has tile on specific position, otherwise false.</returns>
        public Boolean HasTileOn(Vector2 position)
            => tiles.ContainsKey(position);

        /// <summary>
        /// Load map from file.
        /// </summary>
        /// <param name="file">Map file.</param>
        private void Load()
        {
            Name = File.Split('.').First();

            using StreamReader reader = new StreamReader(File);
            Size = Vector2.Parse(reader.ReadLine());

            for (Int32 y = 0; y < Size.Y; y++)
            {
                Int32[] row = reader.ReadLine().Split().Select(Int32.Parse).ToArray();
                for (Int32 x = 0; x < row.Length; x++)
                {
                    Tile tile = new Tile(this, new Vector2(x, y), row[x] == 1);
                    tiles.Add(tile.Position, tile);

                    if (row[x] == 2)
                        spawns.Enqueue(tile);
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public IEnumerator<Tile> GetEnumerator()
            => tiles.Values.GetEnumerator();

        /// <summary>
        /// Get tile at specific position.
        /// </summary>
        /// <param name="position">Position of the tile.</param>
        /// <returns>Tile at specific position.</returns>
        public Tile this[Vector2 position]
            => tiles[position];

    }
}
