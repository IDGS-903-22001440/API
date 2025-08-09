using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;

namespace ProyectoFinal.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasProveedorController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComprasProveedorController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public IActionResult RegistrarCompra([FromBody] CompraProveedor compra)
        {
            try
            {
                ProcesarCompraProveedor(compra, _context);
                return Ok(new { mensaje = "Compra registrada correctamente" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        private void ProcesarCompraProveedor(CompraProveedor compra, DbContext db)
        {
            foreach (var detalle in compra.Detalles)
            {
                var producto = db.Set<Producto>().Find(detalle.ProductoId);
                if (producto == null)
                    throw new Exception($"El producto con ID {detalle.ProductoId} no existe.");

                // Calcular costo total existente y nuevo
                decimal costoTotalAnterior = producto.Precio * producto.Stock;
                decimal costoNuevaCompra = (detalle.PrecioUnitarioCompra + detalle.CostosIndirectos) * detalle.Cantidad;

                // Actualizar stock y precio
                producto.Stock += detalle.Cantidad;
                producto.Precio = (costoTotalAnterior + costoNuevaCompra) / producto.Stock;

                db.Update(producto);
            }

            // Guardar compra con total
            compra.Total = compra.Detalles.Sum(d => (d.PrecioUnitarioCompra + d.CostosIndirectos) * d.Cantidad);
            compra.FechaCompra = DateTime.Now;

            db.Add(compra);
            db.SaveChanges();
        }

        [HttpGet]
        public IActionResult ObtenerCompras(
       [FromQuery] int? proveedorId = null,
       [FromQuery] DateTime? fechaInicio = null,
       [FromQuery] DateTime? fechaFin = null)
        {
            try
            {
                var query = _context.Set<CompraProveedor>()
                    .Include(c => c.Detalles)
                    .Include(c => c.Proveedor)
                    .AsQueryable();

                if (proveedorId.HasValue)
                    query = query.Where(c => c.ProveedorId == proveedorId.Value);

                if (fechaInicio.HasValue)
                    query = query.Where(c => c.FechaCompra >= fechaInicio.Value);

                if (fechaFin.HasValue)
                    query = query.Where(c => c.FechaCompra <= fechaFin.Value);

                var comprasFiltradas = query.ToList();

                return Ok(comprasFiltradas);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerCompraPorId(int id)
        {
            try
            {
                var compra = _context.Set<CompraProveedor>()
                    .Include(c => c.Detalles)
                    .Include(c => c.Proveedor)
                    .FirstOrDefault(c => c.Id == id);

                if (compra == null)
                    return NotFound(new { mensaje = $"No se encontró la compra con id {id}" });

                return Ok(compra);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

    }
}
