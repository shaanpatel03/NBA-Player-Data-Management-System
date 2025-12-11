using WPFApplication.Models;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Group2Project
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded; // Runs after window is shown
        }

        // INITIAL DATA LOAD
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadPlayers();
        }

        private void LoadPlayers()
        {
            using var context = new NBAContext();
            PlayersGrid.ItemsSource = context.Players.ToList();
        }


        // FILTERING
        private void ApplyFilters_Click(object sender, RoutedEventArgs e)
        {
            using var context = new NBAContext();
            var query = context.Players.AsQueryable();

            // VALIDATION FOR AGE FILTER
            if (!string.IsNullOrWhiteSpace(FilterAge.Text) &&
                !int.TryParse(FilterAge.Text, out int age))
            {
                MessageBox.Show("Age filter must be a valid number.", "Invalid Input");
                return;
            }

            // VALIDATION FOR SEASON FILTER
            if (!string.IsNullOrWhiteSpace(FilterSeason.Text) &&
                !int.TryParse(FilterSeason.Text, out int season))
            {
                MessageBox.Show("Season filter must be a valid number.", "Invalid Input");
                return;
            }

            // APPLY FILTERS
            if (!string.IsNullOrWhiteSpace(FilterPlayerId.Text))
                query = query.Where(p => p.PlayerId.Contains(FilterPlayerId.Text));

            if (!string.IsNullOrWhiteSpace(FilterName.Text))
                query = query.Where(p => p.Player1.Contains(FilterName.Text));

            if (int.TryParse(FilterAge.Text, out age))
                query = query.Where(p => p.Age == age);

            if (!string.IsNullOrWhiteSpace(FilterTeam.Text))
                query = query.Where(p => p.Team.Contains(FilterTeam.Text));

            if (int.TryParse(FilterSeason.Text, out season))
                query = query.Where(p => p.Season == season);

            var results = query.ToList();

            // SHOW MESSAGE IF NO RESULTS
            if (results.Count == 0)
            {
                MessageBox.Show("No players match the entered search filters.", "No Results");
            }

            PlayersGrid.ItemsSource = results;
        }


        private void ClearFilters_Click(object sender, RoutedEventArgs e)
        {
            FilterPlayerId.Text = "";
            FilterName.Text = "";
            FilterAge.Text = "";
            FilterTeam.Text = "";
            FilterSeason.Text = "";
            LoadPlayers();
        }


        // ADD PLAYER — WITH VALIDATION
        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            // Required fields
            if (string.IsNullOrWhiteSpace(PlayerIdBox.Text) ||
                string.IsNullOrWhiteSpace(PlayerNameBox.Text) ||
                string.IsNullOrWhiteSpace(TeamBox.Text))
            {
                MessageBox.Show("Player ID, Name, and Team are required fields.", "Invalid Input");
                return;
            }

            // Numeric fields
            if (!int.TryParse(AgeBox.Text, out int ageValue))
            {
                MessageBox.Show("Age must be a valid number.", "Invalid Input");
                return;
            }

            if (!int.TryParse(SeasonBox.Text, out int seasonValue))
            {
                MessageBox.Show("Season must be a valid number.", "Invalid Input");
                return;
            }

            try
            {
                using var context = new NBAContext();

                var p = new Player
                {
                    PlayerId = PlayerIdBox.Text,
                    Player1 = PlayerNameBox.Text,
                    Age = ageValue,
                    Team = TeamBox.Text,
                    Season = seasonValue
                };

                context.Players.Add(p);
                context.SaveChanges();

                LoadPlayers();
                MessageBox.Show("Player added successfully.", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding player: {ex.Message}", "Database Error");
            }
        }


        // UPDATE PLAYER — WITH VALIDATION
        private void UpdatePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (PlayersGrid.SelectedItem is not Player selected)
            {
                MessageBox.Show("Please select a player to update.", "No Selection");
                return;
            }

            // Required fields
            if (string.IsNullOrWhiteSpace(PlayerIdBox.Text) ||
                string.IsNullOrWhiteSpace(PlayerNameBox.Text) ||
                string.IsNullOrWhiteSpace(TeamBox.Text))
            {
                MessageBox.Show("Player ID, Name, and Team cannot be empty.", "Invalid Input");
                return;
            }

            // Numeric validation
            if (!int.TryParse(AgeBox.Text, out int ageValue))
            {
                MessageBox.Show("Age must be a valid number.", "Invalid Input");
                return;
            }

            if (!int.TryParse(SeasonBox.Text, out int seasonValue))
            {
                MessageBox.Show("Season must be a valid number.", "Invalid Input");
                return;
            }

            try
            {
                using var context = new NBAContext();
                var p = context.Players.First(x => x.PlayerKey == selected.PlayerKey);

                p.PlayerId = PlayerIdBox.Text;
                p.Player1 = PlayerNameBox.Text;
                p.Age = ageValue;
                p.Team = TeamBox.Text;
                p.Season = seasonValue;

                context.SaveChanges();
                LoadPlayers();

                MessageBox.Show("Player updated successfully.", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error updating player: {ex.Message}", "Database Error");
            }
        }


        // DELETE PLAYER — (NO EXTRA VALIDATION NEEDED)
        private void DeletePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (PlayersGrid.SelectedItem is not Player selected)
            {
                MessageBox.Show("Please select a player to delete.", "No Selection");
                return;
            }

            try
            {
                using var context = new NBAContext();
                var p = context.Players.First(x => x.PlayerKey == selected.PlayerKey);

                context.Players.Remove(p);
                context.SaveChanges();

                LoadPlayers();

                MessageBox.Show("Player deleted successfully.", "Deleted");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting player: {ex.Message}", "Database Error");
            }
        }


        // UI BEHAVIOR
        private void PlayersGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PlayersGrid.SelectedItem is Player p)
            {
                PlayerIdBox.Text = p.PlayerId;
                PlayerNameBox.Text = p.Player1;
                AgeBox.Text = p.Age?.ToString();
                TeamBox.Text = p.Team;
                SeasonBox.Text = p.Season?.ToString();
            }
        }

        private void PlayersGrid_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (PlayersGrid.SelectedItem is Player p)
            {
                var card = new PlayerCardWindow(p.PlayerKey, p.PlayerId, p.Player1);
                card.ShowDialog();
            }
        }
    }
}
