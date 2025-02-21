using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Character
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Class { get; set; } // Savaşçı, Büyücü, Okçu vb.

        public int PlayerId { get; set; }

        [JsonIgnore] // Player bilgilerini JSON'da göstermemek için
        public Player? Player { get; set; }

        [JsonIgnore] // Karakterin eşyalarını JSON'a dahil etme
        public List<Item> Items { get; set; } = new List<Item>();
    }
}
