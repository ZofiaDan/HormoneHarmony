using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using Microsoft.VisualBasic;
using System.Windows.Media;          
using System.Windows.Input;           
using System.Windows.Controls.Primitives; 

namespace Projekt_WPF
{
    public partial class MainWindow : Window
    {
        public ObservableCollection<Recipe> Recipes { get; } = new ObservableCollection<Recipe>();
        private ICollectionView _recipesView;

        public MainWindow()
        {
            InitializeComponent();
            var settings = ConfigManager.LoadConfig();
            WelcomeText.Text = $"Hello, {settings.UserName}!";

            var recipes = RecipeService.LoadRecipes();
            foreach (var recipe in recipes)
            {
                Recipes.Add(recipe);
            }

            _recipesView = CollectionViewSource.GetDefaultView(Recipes);
            _recipesView.Filter = FilterRecipes;
            DataContext = this;
        }

        private void EditName_Click(object sender, RoutedEventArgs e)
        {
            string newName = Microsoft.VisualBasic.Interaction.InputBox("Enter your name:", "Personalize", "Girlie");
            if (!string.IsNullOrWhiteSpace(newName))
            {
                WelcomeText.Text = $"Hello, {newName}!";
                ConfigManager.SaveConfig(newName);
            }
        }

        private bool FilterRecipes(object obj)
        {
            if (!(obj is Recipe recipe)) return false;

            string searchText = "";
            if (SearchTextBox != null && !string.IsNullOrWhiteSpace(SearchTextBox.Text) &&
                SearchTextBox.Text != "Search by ingredient or dish...")
            {
                searchText = SearchTextBox.Text;
            }

            if (!string.IsNullOrEmpty(searchText))
            {
                if (recipe.Title.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) < 0) return false;
            }

            if (BtnAll.IsChecked == true) return true;

            var categoryMapping = new Dictionary<string, List<string>>(StringComparer.OrdinalIgnoreCase)
            {
                { "Greens", new List<string> { "Spinach", "Kale", "Lettuce", "Avocado", "Cucumber", "Broccoli" } },
                { "Proteins", new List<string> { "Salmon", "Chicken", "Egg", "Tofu", "Beef", "Quinoa" } },
                { "Healthy Fats", new List<string> { "Avocado", "Olive Oil", "Walnuts", "Chia Seeds", "Almond" } },
                { "Fruits", new List<string> { "Banana", "Blueberries", "Raspberries", "Lemon", "Apple" } }
            };

            var activeFilterNames = new List<string>();
            if (BtnProteins.IsChecked == true) activeFilterNames.Add("Proteins");
            if (BtnFats.IsChecked == true) activeFilterNames.Add("Healthy Fats");
            if (BtnGreens.IsChecked == true) activeFilterNames.Add("Greens");
            if (BtnFruits.IsChecked == true) activeFilterNames.Add("Fruits");

            if (activeFilterNames.Count == 0) return true;

            var keywordsToSearch = new List<string>();
            foreach (var filterName in activeFilterNames)
            {
                if (categoryMapping.ContainsKey(filterName)) keywordsToSearch.AddRange(categoryMapping[filterName]);
            }

            string searchPool = $"{recipe.IngredientsRaw} {recipe.KeyIngredientsRaw}";
            return keywordsToSearch.Any(word => searchPool.IndexOf(word, StringComparison.OrdinalIgnoreCase) >= 0);
        }

        private void Filter_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton clickedButton = sender as ToggleButton;
            if (clickedButton == BtnAll)
            {
                if (BtnAll.IsChecked == true)
                {
                    BtnProteins.IsChecked = BtnFats.IsChecked = BtnGreens.IsChecked = BtnFruits.IsChecked = false;
                }
            }
            else
            {
                BtnAll.IsChecked = false;
                if (BtnProteins.IsChecked == false && BtnFats.IsChecked == false &&
                    BtnGreens.IsChecked == false && BtnFruits.IsChecked == false)
                {
                    BtnAll.IsChecked = true;
                }
            }
            _recipesView.Refresh();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && tb.Text == "Search by ingredient or dish...")
            {
                tb.Text = string.Empty;
                tb.Foreground = Brushes.Black;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb && string.IsNullOrWhiteSpace(tb.Text))
            {
                tb.Text = "Search by ingredient or dish...";
                tb.Foreground = Brushes.Gray;
            }
        }

        private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _recipesView?.Refresh();
        }

        private void AddRecipe_Click(object sender, RoutedEventArgs e)
        {
            AddRecipeWindow window = new AddRecipeWindow();
            window.Show();
        }

        private void RecipesListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Recipe selectedRecipe)
            {
                RecipeDetailsWindow detailsWindow = new RecipeDetailsWindow(selectedRecipe);
                detailsWindow.Show();
                listBox.SelectedItem = null;
            }
        }

        private void BrowseRecipes_Click(object sender, RoutedEventArgs e)
        {
            Recipes.Clear();

            var updatedRecipes = RecipeService.LoadRecipes();
            foreach (var recipe in updatedRecipes)
            {
                Recipes.Add(recipe);
            }

            if (SearchTextBox != null)
            {
                SearchTextBox.Text = "Search by ingredient or dish...";
                SearchTextBox.Foreground = Brushes.Gray;
            }

            BtnAll.IsChecked = true;
            BtnProteins.IsChecked = false;
            BtnFats.IsChecked = false;
            BtnGreens.IsChecked = false;
            BtnFruits.IsChecked = false;

            _recipesView?.Refresh();

            MessageBox.Show("List updated!", "Refresh", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}