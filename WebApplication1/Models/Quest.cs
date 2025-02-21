using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Quest
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public bool IsCompleted { get; set; }

        // Many-to-Many ilişkiyi yönetmek için PlayerQuest tablosunu kullanıyoruz
        [JsonIgnore] // JSON içinde PlayerQuests nesnesini gizle
        public List<PlayerQuest> PlayerQuests { get; set; } = new();
    }
}
