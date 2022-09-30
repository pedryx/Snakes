using Snakes_Client.SnakeTypes;

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Snakes_Client
{
    /// <summary>
    /// Interaction logic for AddPlayerWindow.xaml
    /// </summary>
    public partial class AddPlayerDialog : Window
    {

        private Color color;

        public PlayerSnake PlayerSnake { get; private set; }

        public AddPlayerDialog(Color color)
        {
            InitializeComponent();

            this.color = color;
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void DoneButton_Click(Object sender, RoutedEventArgs e)
        {
            Key[] keys =
            {
                (Key)Enum.Parse(typeof(Key), upTextBox.Text),
                (Key)Enum.Parse(typeof(Key), leftTextBox.Text),
                (Key)Enum.Parse(typeof(Key), downTextBox.Text),
                (Key)Enum.Parse(typeof(Key), rightTextBox.Text),
            };
            PlayerSnake = new PlayerSnake(color, 0, keys);

            DialogResult = true;
            Close();
        }

        private void KeyTextBox_KeyDown(Object sender, KeyEventArgs e)
        {
            (sender as TextBox).Text = e.Key.ToString().Split('.').Last();
        }
    }
}
