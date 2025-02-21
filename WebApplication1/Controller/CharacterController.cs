using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CharacterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CharacterController(ApplicationDbContext context)
        {
            _context = context;
        }

        // **1. Tüm karakterleri getir (Tam karakter bilgisi)**
        [HttpGet("all")]  // Farklı route tanımladık
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacters()
        {
            return await _context.Characters.ToListAsync();
        }

        // **2. Sadece belirli alanları içeren karakter listesini getir**
        [HttpGet("simple")]  // Farklı route tanımladık
        public async Task<ActionResult<IEnumerable<object>>> GetSimpleCharacters()
        {
            var characters = await _context.Characters
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Class,
                    c.PlayerId // Sadece PlayerId'yi getir, Player nesnesini değil
                })
                .ToListAsync();

            return Ok(characters);
        }

        [HttpGet("all1")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacters1()
        {
            return await _context.Characters
                .Include(c => c.Player) // Player bilgilerini de çek
                .Include(c => c.Items)  // Karakterin sahip olduğu eşyaları da çek
                .ToListAsync();
        }

        // **3. Belirli bir karakteri getir (ID ile)**
        [HttpGet("{id}")]  
        public async Task<ActionResult<Character>> GetCharacter(int id)
        {
            var character = await _context.Characters.FindAsync(id);
            if (character == null)
            {
                return NotFound();
            }
            return character;
        }

        // **4. Yeni karakter ekle**
        [HttpPost]
        public async Task<ActionResult<Character>> CreateCharacter(Character character)
        {
            _context.Characters.Add(character);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCharacter), new { id = character.Id }, character);
        }
    }
}
