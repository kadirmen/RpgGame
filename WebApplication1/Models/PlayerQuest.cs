namespace WebApplication1.Models
{
    public class PlayerQuest
    {
        public int PlayerId { get; set; }
        public Player? Player { get; set; }

        public int QuestId { get; set; }
        public Quest? Quest { get; set; }

        public bool IsCompleted { get; set; } = false; // Varsayılan olarak tamamlanmamış
    }
}
