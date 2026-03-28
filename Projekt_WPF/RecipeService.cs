using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace Projekt_WPF
{
    public class RecipeService
    {
        private static readonly string filePath = "recipes.json";
        public static List<Recipe> LoadRecipes()
        {
            if (!File.Exists(filePath))
            {
                return new List<Recipe>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var wrapper = JsonSerializer.Deserialize<RecipeWrapper>(json, options);

                return wrapper?.Recipes ?? new List<Recipe>();
            }
            catch (Exception)
            {
                return new List<Recipe>();
            }
        }

        public static void SaveRecipes(List<Recipe> recipes)
        {
            try
            {
                var wrapper = new RecipeWrapper { Recipes = recipes };
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(wrapper, options);

                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Błąd zapisu: {ex.Message}");
            }
        }
    }

    public class RecipeWrapper
    {
        public List<Recipe> Recipes { get; set; }
    }
}