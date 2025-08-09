using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;
using ProyectoFinal.Dto;
using System.Security.Claims;

namespace ProyectoFinal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ComentarioVotesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ComentarioVotesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/ComentarioVotes
        [HttpPost]
        public async Task<IActionResult> Votar([FromBody] CrearComentarioVoteDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var comentario = await _context.ComentariosProductos.FindAsync(dto.ComentarioId);
            if (comentario == null) return NotFound("Comentario no encontrado");

            // Verificamos si ya votó
            var votoExistente = await _context.ComentarioVotes
                .FirstOrDefaultAsync(v => v.ComentarioId == dto.ComentarioId && v.UserId == userId);

            if (votoExistente != null)
            {
                // Si ya votó y es el mismo voto, lo quitamos (toggle)
                if (votoExistente.EsLike == dto.EsLike)
                {
                    _context.ComentarioVotes.Remove(votoExistente);
                }
                else
                {
                    votoExistente.EsLike = dto.EsLike; // Cambia like ↔ dislike
                    _context.ComentarioVotes.Update(votoExistente);
                }
            }
            else
            {
                var nuevoVoto = new ComentarioVote
                {
                    UserId = userId,
                    ComentarioId = dto.ComentarioId,
                    EsLike = dto.EsLike
                };

                _context.ComentarioVotes.Add(nuevoVoto);
            }

            await _context.SaveChangesAsync();
            return Ok(new { mensaje = "Voto registrado" });
        }

        // GET: api/ComentarioVotes/5
        [HttpGet("{comentarioId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerVotos(int comentarioId)
        {
            var likes = await _context.ComentarioVotes
                .CountAsync(v => v.ComentarioId == comentarioId && v.EsLike);

            var dislikes = await _context.ComentarioVotes
                .CountAsync(v => v.ComentarioId == comentarioId && !v.EsLike);

            return Ok(new { likes, dislikes });
        }
    }
}