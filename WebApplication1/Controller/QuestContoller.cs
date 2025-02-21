using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QuestController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public QuestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tüm görevleri listele
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Quest>>> GetQuests()
        {
            return await _context.Quests
                .Include(q => q.PlayerQuests)
                .ThenInclude(pq => pq.Player)
                .ToListAsync();
        }

        // Belirli bir görevi getir
        [HttpGet("{id}")]
        public async Task<ActionResult<Quest>> GetQuest(int id)
        {
            var quest = await _context.Quests
                .Include(q => q.PlayerQuests)
                .ThenInclude(pq => pq.Player)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quest == null)
            {
                return NotFound();
            }

            return quest;
        }

        // Yeni görev ekle (sadece görev ekler, oyuncu ataması yapmaz)
        [HttpPost]
        public async Task<ActionResult<Quest>> CreateQuest(Quest quest)
        {
            _context.Quests.Add(quest);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetQuest), new { id = quest.Id }, quest);
        }

        [HttpPost("{questId}/assign")]
        public async Task<IActionResult> AssignQuestToPlayers(int questId, [FromBody] List<int> playerIds)
        {
            var quest = await _context.Quests.FindAsync(questId);
            if (quest == null)
            {
                return NotFound("Görev bulunamadı.");
            }

            foreach (var playerId in playerIds)
            {
                var player = await _context.Players.FindAsync(playerId);
                if (player == null)
                {
                    return NotFound($"Oyuncu bulunamadı: {playerId}");
                }

                // Görev zaten atanmış mı kontrol et
                var existingAssignment = await _context.PlayerQuests
                    .FirstOrDefaultAsync(pq => pq.QuestId == questId && pq.PlayerId == playerId);

                if (existingAssignment == null)
                {
                    var playerQuest = new PlayerQuest
                    {
                        QuestId = questId,
                        PlayerId = playerId
                    };
                    _context.PlayerQuests.Add(playerQuest);
                }
            }

            await _context.SaveChangesAsync();
            return Ok("Görev başarıyla oyunculara atandı.");
        }

      [HttpGet("player/{playerId}/quests")]
        public async Task<ActionResult<IEnumerable<object>>> GetPlayerAssignedQuests(int playerId)
        {
            var player = await _context.Players
                .Include(p => p.PlayerQuests)
                .ThenInclude(pq => pq.Quest)
                .FirstOrDefaultAsync(p => p.Id == playerId);

            if (player == null)
            {
                return NotFound("Oyuncu bulunamadı.");
            }
            

            var quests = player.PlayerQuests
            .Where(pq => pq.Quest != null) // Null olanları filtrele
            .Select(pq => new
            {
                QuestId = pq.Quest!.Id,
                Title = pq.Quest.Title,
                Description = pq.Quest.Description,
                IsCompleted = pq.IsCompleted
            })
            .ToList();


            return Ok(quests);
        }



        // Görevi güncelle
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateQuest(int id, Quest quest)
        {
            if (id != quest.Id)
            {
                return BadRequest();
            }

            _context.Entry(quest).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Görevi sil
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteQuest(int id)
        {
            var quest = await _context.Quests.FindAsync(id);
            if (quest == null)
            {
                return NotFound();
            }

            _context.Quests.Remove(quest);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Bir oyuncunun görevlerini listele
        

        // Belirli bir oyuncu için görevi tamamla
        [HttpPost("{questId}/complete/{playerId}")]
public async Task<IActionResult> CompleteQuestForPlayer(int questId, int playerId)
{
    var playerQuest = await _context.PlayerQuests
        .FirstOrDefaultAsync(pq => pq.QuestId == questId && pq.PlayerId == playerId);

    // Eğer görev oyuncuya atanmadıysa hata döndür
    if (playerQuest == null)
    {
        return NotFound("Bu oyuncuya atanmış böyle bir görev bulunamadı.");
    }

    playerQuest.IsCompleted = true; // ✅ PlayerQuest tablosunda IsCompleted güncelleniyor.
    _context.Entry(playerQuest).State = EntityState.Modified;
    await _context.SaveChangesAsync();

    return Ok("Oyuncunun görevi tamamlandı!");
}



        
    }
}
