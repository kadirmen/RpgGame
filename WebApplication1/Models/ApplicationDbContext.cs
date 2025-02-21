using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Player> Players { get; set; }
        public DbSet<Character> Characters { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<Quest> Quests { get; set; }
        public DbSet<PlayerQuest> PlayerQuests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //  Bir Oyuncunun Birden Fazla Karakteri Olabilir
            modelBuilder.Entity<Player>()
                .HasMany(p => p.Characters)// playerin birden fazla karakter ile ilişkili olduğunu 
                .WithOne(c => c.Player) // karakterin sadece bir player ile ilişkili olduğu
                .HasForeignKey(c => c.PlayerId) // foregin key oluşturarak
                .OnDelete(DeleteBehavior.Cascade); //  Oyuncu silinirse karakterleri de silinsin

            //  Bir Karakterin Birden Fazla Eşyası Olabilir
            modelBuilder.Entity<Character>()
                .HasMany(c => c.Items)
                .WithOne(i => i.Character)
                .HasForeignKey(i => i.CharacterId)
                .OnDelete(DeleteBehavior.Cascade); //  Karakter silinirse eşyaları da silinsin

            //  Player-Quest İlişkisi İçin Composite Key (Bir oyuncu bir görevi birden fazla kez alamaz)
            modelBuilder.Entity<PlayerQuest>()
                .HasKey(pq => new { pq.PlayerId, pq.QuestId });

            //  Oyuncu-Görev (Many-to-Many İlişkisi)
            modelBuilder.Entity<PlayerQuest>()
                .HasOne(pq => pq.Player)
                .WithMany(p => p.PlayerQuests)
                .HasForeignKey(pq => pq.PlayerId)
                .OnDelete(DeleteBehavior.Cascade); // Oyuncu silinirse sadece PlayerQuest kayıtları silinir, Quest kalır

            modelBuilder.Entity<PlayerQuest>()
                .HasOne(pq => pq.Quest)
                .WithMany(q => q.PlayerQuests)
                .HasForeignKey(pq => pq.QuestId)
                .OnDelete(DeleteBehavior.Restrict); // Görev silindiğinde PlayerQuest kayıtları silinmez, önce elle silinmeli

            //  PlayerQuest için IsCompleted varsayılan değeri (false olarak başlar)
            modelBuilder.Entity<PlayerQuest>()
                .Property(pq => pq.IsCompleted)
                .HasDefaultValue(false);

            //  Görevlerin Birden Fazla Oyuncuya Atanmasını Sağla
            modelBuilder.Entity<Quest>()
                .HasMany(q => q.PlayerQuests)
                .WithOne(pq => pq.Quest)
                .HasForeignKey(pq => pq.QuestId)
                .OnDelete(DeleteBehavior.Restrict); // Görev silindiğinde PlayerQuest kayıtları silinmez

            //  Oyuncunun Görevlerini Bağlama
            modelBuilder.Entity<Player>()
                .HasMany(p => p.PlayerQuests)
                .WithOne(pq => pq.Player)
                .HasForeignKey(pq => pq.PlayerId)
                .OnDelete(DeleteBehavior.Cascade); // Oyuncu silinirse, görev atamaları da silinir
        }
    }
}
