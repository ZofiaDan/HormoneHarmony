using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;

namespace Projekt_WPF
{
    public partial class AddRecipeWindow : Window
    {
        private string finalImageRelativePath = "images/default.jpg";

        public AddRecipeWindow()
        {
            InitializeComponent();
        }

        private void ImageDropZone_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    string sourceFilePath = files[0];
                    string extension = System.IO.Path.GetExtension(sourceFilePath).ToLower();

                    if (extension == ".jpg" || extension == ".jpeg" || extension == ".png")
                    {
                        try
                        {
                            string imagesFolder = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "images");
                            if (!Directory.Exists(imagesFolder))
                            {
                                Directory.CreateDirectory(imagesFolder);
                            }

                            string fileName = Guid.NewGuid().ToString() + extension;
                            string destFilePath = System.IO.Path.Combine(imagesFolder, fileName);

                            File.Copy(sourceFilePath, destFilePath, true);

                            finalImageRelativePath = System.IO.Path.Combine("images", fileName);

                            ImgPreview.Source = new BitmapImage(new Uri(destFilePath));
                            DropText.Visibility = Visibility.Collapsed;
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Błąd podczas kopiowania obrazu: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Proszę upuszczać tylko pliki graficzne (.jpg, .png)", "Niepoprawny format", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(TitleInput.Text) ||
                    string.IsNullOrWhiteSpace(KcalInput.Text) ||
                    string.IsNullOrWhiteSpace(TimeInput.Text) ||
                    string.IsNullOrWhiteSpace(ShortDescInput.Text) ||
                    string.IsNullOrWhiteSpace(DescriptionInput.Text) ||
                    string.IsNullOrWhiteSpace(IngredientsInput.Text) ||
                    string.IsNullOrWhiteSpace(StepsInput.Text))
                {
                    MessageBox.Show("Wszystkie pola muszą zostać wypełnione przed zapisem!", "Brak danych", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (!int.TryParse(KcalInput.Text, out int kcal) || kcal < 0)
                {
                    MessageBox.Show("Pole 'Kalorie' musi zawierać poprawną liczbę dodatnią!", "Błąd danych", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!int.TryParse(TimeInput.Text, out int time) || time < 0)
                {
                    MessageBox.Show("Pole 'Czas' musi zawierać poprawną liczbę dodatnią!", "Błąd danych", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (finalImageRelativePath == "images/default.jpg")
                {
                    var result = MessageBox.Show("Nie dodano zdjęcia. Czy chcesz zapisać przepis z domyślną grafiką?", "Brak zdjęcia", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.No) return;
                }

                var allRecipes = RecipeService.LoadRecipes();

                string absoluteImagePath = System.IO.Path.GetFullPath(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, finalImageRelativePath));

                Recipe newRecipe = new Recipe
                {
                    Id = allRecipes.Any() ? allRecipes.Max(r => r.Id) + 1 : 1,
                    Title = TitleInput.Text,
                    Kcal = kcal,
                    TimeMin = time,
                    ShortDescription = ShortDescInput.Text,
                    Description = DescriptionInput.Text,
                    IngredientsRaw = IngredientsInput.Text,
                    StepsRaw = StepsInput.Text,
                    KeyIngredientsRaw = KeyIngredientsInput.Text,
                    ImagePath = absoluteImagePath
                };

                allRecipes.Add(newRecipe);
                RecipeService.SaveRecipes(allRecipes);

                MessageBox.Show("Przepis został pomyślnie dodany!", "Sukces", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Wystąpił nieoczekiwany błąd: {ex.Message}", "Błąd", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}