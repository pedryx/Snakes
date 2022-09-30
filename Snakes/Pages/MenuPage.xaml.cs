using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for MenuPage.xaml
    /// </summary>
    public partial class MenuPage : Page
    {

        private readonly MainWindow window;
        private readonly Map defaultMap;

        public MenuPage(MainWindow window)
        {
            InitializeComponent();

            this.window = window;
            menuImage.Source = TextureManager.Instance.GetTexture("SnakeHead", Colors.Green).Source;
            defaultMap = new Map(window.DefaultMapFile);
        }

        private void ExitButton_Click(Object sender, RoutedEventArgs e)
        {
            window.Close();
        }

        private void FoodsButton_Click(Object sender, RoutedEventArgs e)
        {
            window.Navigate(new FoodsPage(window));
        }

        private void LocalButton_Click(Object sender, RoutedEventArgs e)
        {
            window.Navigate(new LocalPage(window, defaultMap, GameType.Local));
        }

        private void HostButton_Click(Object sender, RoutedEventArgs e)
        {
            Process.Start("Snakes_Server.exe", String.Join(' ', window.DefaultMapFile, defaultMap.SpawnCount));
            TcpClient client = new TcpClient();
            client.Connect(IPAddress.Loopback, Client.Port);
            window.Navigate(new LocalPage(window, defaultMap, GameType.Host,
                new Client(client)));
        }

        private void ConnectButton_Click(Object sender, RoutedEventArgs e)
        {
            IpDialog dialog = new IpDialog();
            if (dialog.ShowDialog().Value)
            {
                window.Navigate(new LocalPage(window, null, GameType.Online,
                new Client(dialog.Client)));
            }
        }
    }
}
