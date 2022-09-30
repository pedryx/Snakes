using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using System.Xml.Serialization;

namespace Snakes_Client
{
    /// <summary>
    /// Represent a food, that can be eaten by snake.
    /// </summary>
    public class Food : VisualObject
    {

        /// <summary>
        /// Folder where food files are stored.
        /// </summary>
        [XmlIgnore]
        public static String FoodFolder
        {
            get => "Food";
        }

        /// <summary>
        /// Parent of UI element that repres4ent apperance.
        /// </summary>
        private Panel panel;
        /// <summary>
        /// Element width.
        /// </summary>
        private Double width;
        /// <summary>
        /// Element height.
        /// </summary>
        private Double height;

        /// <summary>
        /// How much score will snake obtain when he will eat the food.
        /// </summary>
        public Int32 Score { get; set; }

        /// <summary>
        /// How much will snake grow when he will eat the food.
        /// </summary>
        public Single Fat { get; set; }

        /// <summary>
        /// Name of texture for food's apperance.
        /// </summary>
        [XmlElement(ElementName = "Texture")]
        public String TextureName { get; set; }

        /// <summary>
        /// Represent a chance of food spawn.
        /// </summary>
        public Single SpawnChance { get; set; }

        /// <summary>
        /// Determine if eating this food can trigger spawn of foods.
        /// </summary>
        public Boolean FoodSpawn { get; set; }

        /// <summary>
        /// Create new food.
        /// </summary>
        /// <param name="score">How much score will snake obtain when he will eat the food.</param>
        /// <param name="fat">How much will snake grow when he will eat the food./param>
        public Food(Int32 score, Single fat)
        {
            Score = score;
            Fat = fat;
            SpawnChance = .33f;
            FoodSpawn = false;
        }

        /// <summary>
        /// Create new food.
        /// </summary>
        public Food() : this(1, 1) { }

        /// <summary>
        /// Initia;oze vosual apperance.
        /// </summary>
        public void InitVisual(Panel panel, Double width, Double height)
        {
            this.panel = panel;
            this.width = width;
            this.height = height;
        }

        /// <summary>
        /// Move food to a new positiob.
        /// </summary>
        /// <param name="position">New position.</param>
        public void Move(Vector2 position)
        {
            (Visual as Image).Margin = new Thickness()
            {
                Left = position.X,
                Top = position.Y,
            };
        }

        /// <summary>
        /// Create prototype of food.
        /// </summary>
        /// <returns>Create prototype of food..</returns>
        public Food CreatePrototype()
        {
            Food food = new Food(Score, Fat)
            {
                TextureName = TextureName,
                FoodSpawn = FoodSpawn,
                SpawnChance = SpawnChance,
            };
            food.CreateVisual(panel, width, height);

            return food;
        }

        /// <summary>
        /// Create object's visual apperance.
        /// </summary>
        /// <returns></returns>
        protected override UIElement CreateVisual()
        {
            return new Image()
            {
                Width = Width,
                Height = Height,
                Source = TextureManager.Instance[TextureName].Source,
            };
        }

        /// <summary>
        /// Create new food from specific file.
        /// </summary>
        /// <param name="file">Specific file from which will new food be created.</param>
        /// <returns>New food create from specific file.</returns>
        public static Food FromFile(String file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Food));
            using FileStream stream = new FileStream(file, FileMode.Open);

            Food food = (Food)serializer.Deserialize(stream);
            return food;
        }

    }
}
