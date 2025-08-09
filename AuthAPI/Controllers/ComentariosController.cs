using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Dto;
using ProyectoFinal.Models;
using System.Security.Claims;

namespace ProyectoFinal.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ComentariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ComentariosController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/comentarios
        [HttpPost]
        public async Task<IActionResult> Comentar([FromBody] CrearComentarioDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var producto = await _context.Productos.FindAsync(dto.ProductoId);
            if (producto == null) return NotFound("Producto no encontrado");

            var comentario = new ComentarioProducto
            {
                UserId = user.Id,
                ProductoId = dto.ProductoId,
                Contenido = dto.Contenido
            };

            _context.ComentariosProductos.Add(comentario);
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "Comentario agregado", comentario });
        }

        [HttpGet("producto/{productoId}")]
        [AllowAnonymous]
        public async Task<IActionResult> ObtenerComentarios(int productoId)
        {
            var comentarios = await _context.ComentariosProductos
                .Where(c => c.ProductoId == productoId)
                .Include(c => c.User)
                .OrderByDescending(c => c.Fecha)
                .ToListAsync();

            var userId = User.Identity?.IsAuthenticated == true
                ? User.FindFirstValue(ClaimTypes.NameIdentifier)
                : null;

            var userIds = comentarios.Select(c => c.UserId).Distinct().ToList();

            var ventas = await _context.Sales
                .Where(v => userIds.Contains(v.UserId))
                .Include(v => v.SaleDetails)
                .ToListAsync();

            var comentarioIds = comentarios.Select(c => c.Id).ToList();

            var votos = await _context.ComentarioVotes
                .Where(v => comentarioIds.Contains(v.ComentarioId))
                .ToListAsync();

            var respuesta = comentarios.Select(c =>
            {
                var haComprado = ventas
                    .Where(v => v.UserId == c.UserId)
                    .SelectMany(v => v.SaleDetails)
                    .Any(d => d.ProductId == productoId);

                var votoUsuario = userId == null
                    ? null
                    : votos.FirstOrDefault(v => v.ComentarioId == c.Id && v.UserId == userId);

                return new
                {
                    c.Id,
                    c.Contenido,
                    c.Fecha,
                    Usuario = c.User.FullName,
                    HaCompradoProducto = haComprado,
                    Likes = votos.Count(v => v.ComentarioId == c.Id && v.EsLike),
                    Dislikes = votos.Count(v => v.ComentarioId == c.Id && !v.EsLike),
                    UserVoto = votoUsuario?.EsLike // true, false o null
                };
            });

            return Ok(respuesta);
        }
    }
}
