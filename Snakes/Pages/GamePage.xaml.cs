using Snakes_Client.SnakeTypes;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;


namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for GamePage.xaml
    /// </summary>
    public partial class GamePage : Page
    {

        private readonly Dictionary<UInt32, Snake> snakes;
        private readonly IEnumerable<Food> foods;
        private readonly Stopwatch stopwatch;
        private readonly MainWindow window;
        private readonly Map map;
        private readonly GameType gameType;
        private readonly Client client;
        private readonly Int32 seed;

        private Dictionary<Snake, StackPanel> snakeTable;
        private TimeSpan tickTime;
        private DispatcherTimer gameTimer;

        public Game Game { get; private set; }

        public Double GameSpeed { get; set; } = 1;

        public GamePage(MainWindow window, Map map, Dictionary<UInt32, Snake> snakes, IEnumerable<Food> foods, GameType gameType)
        {
            InitializeComponent();
            if (gameType == GameType.Local)
                InitializeTimers();

            this.snakes = snakes;
            this.foods = foods;
            this.window = window;
            this.map = map;
            this.gameType = gameType;

            Game = new Game(window, map);
            stopwatch = new Stopwatch();
            mapLabel.Content += map.Name.Split('.').First();
        }

        public GamePage(MainWindow window, Map map, Dictionary<UInt32, Snake> snakes, IEnumerable<Food> foods, GameType gameType, Client client, Int32 seed)
            : this(window, map, snakes, foods, gameType)
        {
            this.client = client;
            this.seed = seed;

            client.Update += Client_Update;
            client.OnSnakeMove += Client_OnSnakeMove;
        }

        private void InitializeTimers()
        {
            tickTime = TimeSpan.FromMilliseconds(500 / GameSpeed);
            gameTimer = new DispatcherTimer()
            {
                Interval = tickTime,
            };
            gameTimer.Tick += GameTimer_Tick;
        }

        private void InitializeSnakeTable()
        {
            snakeTable = new Dictionary<Snake, StackPanel>();
            foreach (var snake in snakes.Values)
            {
                snake.StatChanged += Snake_StatChanged;
                AddRecord(snake);
                snakesStackPanel.Children.Add(snakeTable[snake]);
            }
        }

        private void AddRecord(Snake snake)
        {
            snakeTable.Add(snake, new StackPanel()
            {
                Orientation = Orientation.Horizontal,
            });
            snakeTable[snake].Children.Add(new Border()
            {
                BorderBrush = new SolidColorBrush(Colors.Black),
                BorderThickness = new Thickness(1, 1, 1, 1),
                MinWidth = colorLabel.ActualWidth,
            });
            (snakeTable[snake].Children[^1] as Border).Child = new Canvas()
            {
                Background = new SolidColorBrush(snake.Color),
                MinWidth = colorLabel.ActualWidth,
            };
            snakeTable[snake].Children.Add(CreateLabel(snake.Fat.ToString(), fatLabel.ActualWidth));
            snakeTable[snake].Children.Add(CreateLabel(snake.Score.ToString(), scoreLabel.ActualWidth));
            snakeTable[snake].Children.Add(new CheckBox()
            {
                IsEnabled = false,
                IsChecked = !snake.Alive,
                MinWidth = aliveLabel.ActualWidth,
                HorizontalAlignment = HorizontalAlignment.Center,
            });
        }

        private Label CreateLabel(String content, Double minWidth)
        {
            return new Label()
            {
                Content = content,
                MinWidth = minWidth,
                FontSize = 15,
                HorizontalAlignment = HorizontalAlignment.Center,
            };
        }

        private Boolean GameOver()
        {
            Boolean alive = false;
            foreach (var snake in snakes.Values)
            {
                if (snake.Alive)
                {
                    alive = true;
                    break;
                }
            }

            return !alive;
        }

        private void Snake_StatChanged(Object sender, StatChangeEventArgs e)
        {
            (snakeTable[(Snake)sender].Children[1] as Label).Content = e.Fat;
            (snakeTable[(Snake)sender].Children[2] as Label).Content = e.Score;
            (snakeTable[(Snake)sender].Children[3] as CheckBox).IsChecked = e.Alive;
        }

        private void GameCanvas_Loaded(Object sender, RoutedEventArgs e)
        {
             Game.Initialize(gameCanvas, snakes.Values, foods,
                gameType == GameType.Local ? (Int32)DateTime.Now.Ticks : seed);
            InitializeSnakeTable();
            if (gameType == GameType.Local)
                gameTimer.Start();
        }

        private void GameTimer_Tick(Object sender, EventArgs e)
        {
            stopwatch.Restart();
            Game.Update();
            stopwatch.Stop();

            if (GameOver())
            {
                gameTimer.Stop();
                MessageBox.Show("Game Over!");
                window.Navigate(new MenuPage(window));
            }

            TimeSpan interval = tickTime - stopwatch.Elapsed;
            gameTimer.Interval = interval.Milliseconds > 0 ? interval : TimeSpan.FromMilliseconds(0);
        }

        private void Client_Update(Object sender, EventArgs e)
        {
            gameCanvas.Dispatcher.Invoke(() =>
            {
                Game.Update();

                if (GameOver())
                {
                    client.SendGameOver();
                    MessageBox.Show("Game Over!");
                    window.Navigate(new MenuPage(window));
                }
            });
        }

        private void Client_OnSnakeMove(Object sender, MovementEventArgs e)
        {
            if (snakes[e.Id] is NetworkSnake)
                (snakes[e.Id] as NetworkSnake).LastDirection = e.Direction;
            else
                (snakes[e.Id] as PlayerSnake).LastDirection = e.Direction;
        }

    }
}
