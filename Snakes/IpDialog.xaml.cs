using System;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for IpWindow.xaml
    /// </summary>
    public partial class IpDialog : Window
    {

        public TcpClient Client { get; private set; }

        public IpDialog()
        {
            InitializeComponent();

            Client = new TcpClient();
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void ConnectButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!IPAddress.TryParse(ipTextBox.Text, out IPAddress address))
            {
                MessageBox.Show("Invalid IP address!");
                return;
            }
            if (!Int32.TryParse(portTextBox.Text, out Int32 port))
            {
                MessageBox.Show("Invalid port!");
                return;
            }

            try
            {
                Client.Connect(address, port);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            DialogResult = true;
            Close();
        }
    }
}
