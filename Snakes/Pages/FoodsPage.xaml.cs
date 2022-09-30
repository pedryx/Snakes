using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for FoodsPage.xaml
    /// </summary>
    public partial class FoodsPage : Page
    {

        private const String scorePrefix = "Score: ";
        private const String fatPrefix = "Fat: ";
        private const String chancePrefix = "Spawn Chance: ";
        private const String spawnPrefix = "Spawn Food: ";
        private const String chancePostfix = "%";
        private static readonly Dictionary<Boolean, String> chanceValues = new Dictionary<Boolean, String>()
        {
            { true, "Yes" },
            { false, "No" },
        };

        private readonly MainWindow window;
        private readonly TextureManager manager;

        private Int32 index;

        public FoodsPage(MainWindow window)
        {
            this.window = window;
            manager = TextureManager.Instance;

            InitializeComponent();
        }

        private void ShowFood()
        {
            Food current = window.Foods[index];

            nameLabel.Content = current.TextureName.ToUpper();
            foodImage.Source = manager.GetTexture(current.TextureName).Source;
            scoreLabel.Content = scorePrefix + current.Score;
            fatLabel.Content = fatPrefix + current.Fat;
            chanceLabel.Content = chancePrefix + current.SpawnChance + chancePostfix;
            spawnLabel.Content = spawnPrefix + chanceValues[current.FoodSpawn];
        }

        private void PrevButton_Click(Object sender, RoutedEventArgs e)
        {
            index++;
            if (index >= window.Foods.Count)
                index = 0;
            ShowFood();
        }

        private void NextButton_Click(Object sender, RoutedEventArgs e)
        {
            index--;
            if (index < 0)
                index = window.Foods.Count - 1;
            ShowFood();
        }

        private void BackButton_Click(Object sender, RoutedEventArgs e)
        {
            window.Navigate(new MenuPage(window));
        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            ShowFood();
        }
    }
}
