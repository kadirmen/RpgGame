using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Level { get; set; }
        public int Health { get; set; }

        [JsonIgnore] // JSON içinde karakter listesini göstermemek için
        public List<Character> Characters { get; set; } = new List<Character>();

        [JsonIgnore] // Görev listesini JSON'da göstermemek için
        public List<PlayerQuest> PlayerQuests { get; set; } = new();
    }
}
