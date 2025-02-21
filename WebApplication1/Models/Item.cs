using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Item
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public int CharacterId { get; set; }

        [JsonIgnore] // JSON çıktısında karakter nesnesini gizle
        public Character? Character { get; set; }
    }
}
