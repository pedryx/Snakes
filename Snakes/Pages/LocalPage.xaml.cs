using Microsoft.Win32;

using Snakes_Client.SnakeTypes;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;


namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for LocalPage.xaml
    /// </summary>
    public partial class LocalPage : Page
    {

        private readonly Dictionary<UInt32, Snake> snakes;
        private readonly MainWindow window;
        private readonly GameType gameType;
        private readonly Client client;
        private readonly List<Color> colors = new List<Color>()
        {
            Color.FromArgb(255, 255, 0, 0),
            Colors.Green,
            Colors.MidnightBlue,
            Colors.Magenta,
            Colors.Yellow,
            Colors.Cyan,
        };

        private Map map;
        private UInt32 lastSnakeId;

        public LocalPage(MainWindow window, Map map, GameType gameType)
        {
            InitializeComponent();

            this.window = window;
            this.gameType = gameType;
            this.map = map;

            snakes = new Dictionary<UInt32, Snake>();
        }

        public LocalPage(MainWindow window, Map map, GameType gameType, Client client)
            : this(window, map, gameType)
        {
            this.client = client;
            if (gameType == GameType.Host)
                client.StartReceiving();

            client.SnakeAdded += Client_SnakeAdded;
            client.SnakeRemoved += Client_SnakeRemoved;
            client.MapChanged += Client_MapChanged;
            client.GameStarted += Client_GameStarted;
        }

        private void AddSnake(Snake snake, Boolean remove)
        {
            snakes.Add(snake.Id, snake);
            if (snake is PlayerSnake)
            {
                window.KeyDown += (snake as PlayerSnake).Window_KeyDown;
                snake.AddDestroyAction(() => window.KeyDown -= (snake as PlayerSnake).Window_KeyDown);
            }
            #region Init UI Elelemets
            StackPanel snakeStackPanel = new StackPanel()
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5, 5, 5, 5),
            };
            snakeStackPanel.Children.Add(new Border()
            {
                BorderThickness = new Thickness(1, 1, 1, 1),
                BorderBrush = new SolidColorBrush(Colors.Black),
                MinWidth = 40,
                MinHeight = 40,
            });
            (snakeStackPanel.Children[^1] as Border).Child = new Canvas()
            {
                Background = new SolidColorBrush(snake.Color),
                MinWidth = 40,
                MinHeight = 40,
            };
            snakeStackPanel.Children.Add(new Label()
            {
                FontSize = 20,
                Content = snake is AISnake ? "Ai" : "Player",
            });
            snakeStackPanel.Children.Add(new Button()
            {
                FontSize = 20,
                Content = "Remove Snake",
                IsEnabled = remove || gameType != GameType.Online,
            });
            snake.AddDestroyAction(() =>
            {
                snakesStackPanel.Children.Remove(snakeStackPanel);
                snakes.Remove(snake.Id);
            });
            (snakeStackPanel.Children[^1] as Button).Click += (sender, e) =>
            {
                if (gameType == GameType.Local)
                    snake.DestroyVisual();
                else
                    client.SendRemoveSnake(snake.Id);
            };
            snakesStackPanel.Children.Add(snakeStackPanel);
            #endregion
        }

        private void CreatePreview()
        {
            Double width = mapCanvas.ActualWidth / map.Size.X;
            Double height = mapCanvas.ActualHeight / map.Size.Y;

            foreach (var tile in map)
                tile.CreateVisual(mapCanvas, width, height);
        }

        private void UnloadMap()
        {
            foreach (var tile in map)
                tile.DestroyVisual();
        }

        private Color GetColor()
            =>colors[snakes.Count % colors.Count];

        private void Client_GameStarted(Object sender, GameInitEventArgs e)
        {
            mapCanvas.Dispatcher.Invoke(() => UnloadMap());
            window.Dispatcher.Invoke(() 
                => window.Navigate(new GamePage(window, map, snakes, window.Foods, gameType, client, e.Seed)));
        }

        private void Client_MapChanged(Object sender, MapChangeEventArgs e)
        {
            mapCanvas.Dispatcher.Invoke(() =>
            {
                UnloadMap();
                map = e.Map;
                CreatePreview(); 
            });
        }

        private void Client_SnakeAdded(Object sender, SnakeEventArgs e)
        {
            snakesStackPanel.Dispatcher.Invoke(() =>
            {
                Snake snake = e.Snake;
                Boolean remove = true;
                if (snake == null)
                {
                    if (e.SnakeInfo.AI)
                        snake = new AISnake(Texture.ConvertColor(e.SnakeInfo.Color), e.SnakeInfo.Id);
                    else
                        snake = new NetworkSnake(Texture.ConvertColor(e.SnakeInfo.Color), e.SnakeInfo.Id);
                    remove = false;
                }

                AddSnake(snake, remove);
            });
        }

        private void Client_SnakeRemoved(Object sender, RemoveSnakeEventArgs e)
        {
            snakesStackPanel.Dispatcher.Invoke(() => snakes[e.Id].DestroyVisual());
        }

        private void Page_Loaded(Object sender, RoutedEventArgs e)
        {
            if (gameType == GameType.Online)
            {
                IEnumerable<Snake> snakes = client.ReceiveInit(out map);
                foreach (var snake in snakes)
                    AddSnake(snake, false);
                client.StartReceiving();
            }
            else
            {
                PlayerSnake snake = new PlayerSnake(GetColor(), lastSnakeId++,
                    new Key[] { Key.W, Key.A, Key.S, Key.D })
                {
                    Client = client
                };
                AddSnake(snake, true);
            }
            CreatePreview();
        }

        private void ChangeMapButton_Click(Object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            if (dialog.ShowDialog().Value)
            {
                if (gameType == GameType.Local)
                {
                    UnloadMap();
                    map = new Map(dialog.FileName);
                    CreatePreview();
                }
                else
                    client.SendMapChange(new Map(dialog.FileName));
            }
        }

        private void AddPlayerButton_Click(Object sender, RoutedEventArgs e)
        {
            if (snakes.Count >= map.SpawnCount)
            {
                MessageBox.Show($"Maximum number of snakes for this map is {map.SpawnCount}!");
                return;
            }

            AddPlayerDialog dialog = new AddPlayerDialog(GetColor());
            if (dialog.ShowDialog().Value)
            {
                dialog.PlayerSnake.Id = lastSnakeId++;
                dialog.PlayerSnake.Client = client;
                if (gameType == GameType.Local)
                    AddSnake(dialog.PlayerSnake, true);
                else
                    client.SendAddSnakeRequest(dialog.PlayerSnake);

            }
        }

        private void AddAIButton_Click(Object sender, RoutedEventArgs e)
        {
            if (snakes.Count >= map.SpawnCount)
            {
                MessageBox.Show($"Maximum number of snakes for this map is {map.SpawnCount}!");
                return;
            }

            if (gameType == GameType.Local)
                AddSnake(new AISnake(GetColor(), lastSnakeId++), true);
            else
                client.SendAddSnakeRequest(new AISnake(GetColor(), lastSnakeId++));
        }

        private void StartGameButton_Click(Object sender, RoutedEventArgs e)
        {
            if (gameType == GameType.Local)
            {
                UnloadMap();
                window.Navigate(new GamePage(window, map, snakes, window.Foods, gameType));
            }
            else
                client.SendStartGame();
        }

        private void BackToMenuButton_Click(Object sender, RoutedEventArgs e)
        {
            window.Navigate(new MenuPage(window));
        }
    }
}
