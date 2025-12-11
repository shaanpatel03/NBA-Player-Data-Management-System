using System;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using WPFApplication.Models;

namespace Group2Project
{
    public partial class PlayerCardWindow : Window
    {
        // Tracks whether the front (photo) or back (stats) is showing
        private bool showingFront = true;

        // Stores the PlayerKey used to retrieve stats
        private int _playerKey;

        /// Constructor for the player card window.
        /// Loads player photo, name, and season statistics.
        public PlayerCardWindow(int playerKey, string playerId, string playerName)
        {
            InitializeComponent();

            _playerKey = playerKey;
            PlayerNameText.Text = playerName;

            LoadPlayerImage(playerId);
            LoadStats();
        }


        // Load player photo from website and is front of card 

        /// Attempts to load a player's headshot from Basketball-Reference.
        /// Falls back to a placeholder image if the file does not exist.
        private void LoadPlayerImage(string playerId)
        {
            try
            {
                // Basketball-Reference photo URL format
                string url = $"https://www.basketball-reference.com/req/202106291/images/players/{playerId}.jpg";

                PlayerImage.Source = new BitmapImage(new Uri(url));
            }
            catch
            {
                // Fallback placeholder image (bundled with app)
                PlayerImage.Source = new BitmapImage(new Uri("/Images/placeholder.png", UriKind.Relative));
            }
        }


        // Load player stats, back of card

        /// Loads all season-stat rows associated with this player's PlayerKey
        /// and binds them to the StatsGrid.
        /// Also automatically hides technical columns.
        private void LoadStats()
        {
            using var context = new NBAContext();

            // Retrieve all stats for this player (usually one per season)
            var stats = context.PlayerStats
                               .Where(s => s.PlayerKey == _playerKey)
                               .ToList();

            StatsGrid.ItemsSource = stats;

            // Hide columns that users should not see (IDs, navigation fields)
            StatsGrid.AutoGeneratingColumn += (s, e) =>
            {
                if (e.PropertyName == "StatsId" ||
                    e.PropertyName == "PlayerKey" ||
                    e.PropertyName == "PlayerId" ||
                    e.PropertyName == "PlayerKeyNavigation")
                {
                    e.Column.Visibility = Visibility.Collapsed;
                }
            };
        }


        // Button that flips the card between front and back

        /// Toggles the card between front (photo) and back (stats).
        private void FlipButton_Click(object sender, RoutedEventArgs e)
        {
            showingFront = !showingFront;

            // Switch between front (image) and back (stats)
            FrontSide.Visibility = showingFront ? Visibility.Visible : Visibility.Collapsed;
            BackSide.Visibility = showingFront ? Visibility.Collapsed : Visibility.Visible;

            // Update button label
            FlipButton.Content = showingFront ? "View Stats" : "View Photo";
        }
    }
}
