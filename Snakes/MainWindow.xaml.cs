using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public List<Food> Foods { get; private set; }

        public String DefaultMapFile
        {
            get => "Maps/Rocks.map";
        }

        public MainWindow()
        {
            InitializeComponent();
            LoadFoods();

            Navigate(new MenuPage(this));
        }

        private void LoadFoods()
        {
            Foods = new List<Food>();
            String[] files = Directory.GetFiles(Food.FoodFolder);

            foreach (var file in files)
                Foods.Add(Food.FromFile(file));
        }

        public void Navigate(Page page)
        {
            mainFrame.Navigate(page);
        }

    }
}
