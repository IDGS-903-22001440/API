using AuthAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")] // Solo admins pueden gestionar proveedores
    public class ProveedoresController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProveedoresController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/proveedores
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var proveedores = await _context.Proveedores
                .Include(p => p.Categoria)
                .ToListAsync();
            return Ok(proveedores);
        }

        // GET: api/proveedores/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var proveedor = await _context.Proveedores
                .Include(p => p.Categoria)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (proveedor == null) return NotFound();

            return Ok(proveedor);
        }

        // POST: api/proveedores
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Proveedor proveedor)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = proveedor.Id }, proveedor);
        }

        // PUT: api/proveedores/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Proveedor proveedor)
        {
            if (id != proveedor.Id) return BadRequest();

            _context.Entry(proveedor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Proveedores.Any(p => p.Id == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/proveedores/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
