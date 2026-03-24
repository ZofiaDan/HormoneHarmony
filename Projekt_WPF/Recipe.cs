using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Projekt_WPF
{
    public class Recipe
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("kcal")]
        public int Kcal { get; set; }

        [JsonPropertyName("time_min")]
        public int TimeMin { get; set; }

        [JsonPropertyName("short_description")]
        public string ShortDescription { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("image_path")]
        public string ImagePath { get; set; }

        // Pola "Raw" przechowują dane w formacie tekstowym (pod SQLite/JSON)
        [JsonPropertyName("ingredients")]
        public string IngredientsRaw { get; set; }

        [JsonPropertyName("key_ingredients")]
        public string KeyIngredientsRaw { get; set; }

        [JsonPropertyName("steps")]
        public string StepsRaw { get; set; }

        // Właściwości pomocnicze, które zamieniają tekst na listy dla WPF
        [JsonIgnore] // Deserializator zignoruje te pola, bo bazujemy na "Raw"
        public List<string> Ingredients => IngredientsRaw?.Split(';').ToList() ?? new List<string>();

        [JsonIgnore]
        public List<string> KeyIngredients => KeyIngredientsRaw?.Split(';').ToList() ?? new List<string>();

        [JsonIgnore]
        public List<string> Steps => StepsRaw?.Split('|').ToList() ?? new List<string>();

        // Konstruktor bezparametrowy wymagany do deserializacji JSON
        public Recipe() { }

        // Opcjonalny konstruktor dla ułatwienia tworzenia obiektów w kodzie
        public Recipe(int id, string title, int kcal, int timeMin, string shortDescription, string description, string imagePath, string ingredientsRaw, string keyIngredientsRaw, string stepsRaw)
        {
            Id = id;
            Title = title;
            Kcal = kcal;
            TimeMin = timeMin;
            ShortDescription = shortDescription;
            Description = description;
            ImagePath = imagePath;
            IngredientsRaw = ingredientsRaw;
            KeyIngredientsRaw = keyIngredientsRaw;
            StepsRaw = stepsRaw;
        }
    }
}