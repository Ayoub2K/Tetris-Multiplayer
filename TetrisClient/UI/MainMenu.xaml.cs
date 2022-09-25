using System.Windows;

namespace TetrisClient.UI
{
    /// <summary>
    ///     Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        public MainMenu()
        {
            InitializeComponent();
        }

        private void singlePlayer(object sender, RoutedEventArgs e)
        {
            Window window = new MainWindow();
            window.Show();
            Close();
        }

        private void multiplayer(object sender, RoutedEventArgs e)
        {
            Window window = new MultiplayerWindow();
            window.Show();
        }

        private void easterEgg(object sender, RoutedEventArgs e)
        {
            if (ayoubSlender.Visibility == Visibility.Hidden)
                ayoubSlender.Visibility = Visibility.Visible;
            else
                ayoubSlender.Visibility = Visibility.Hidden;
        }
    }
}