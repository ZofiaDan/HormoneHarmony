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
        public static List<Recipe> LoadRecipes()
        {
            string filePath = "recipes.json";

            if (!File.Exists(filePath))
            {
                return new List<Recipe>();
            }

            string json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var wrapper = JsonSerializer.Deserialize<RecipeWrapper>(json, options);

            return wrapper?.Recipes ?? new List<Recipe>();
        }
    }
    public class RecipeWrapper 
    { 
        public List<Recipe> Recipes { get; set; }
    }
}
