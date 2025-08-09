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
    public class SalesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public SalesController(AppDbContext context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // POST: api/sales
        [HttpPost]
        public async Task<IActionResult> CreateSale([FromBody] SaleDto saleDto)
        {
            if (saleDto == null || saleDto.DetalleVenta == null || !saleDto.DetalleVenta.Any())
                return BadRequest("La venta no contiene detalles.");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var sale = new Sale
                {
                    UserId = user.Id,
                    Date = DateTime.Now,
                    SaleDetails = new List<SaleDetail>()
                };

                decimal total = 0;

                foreach (var item in saleDto.DetalleVenta)
                {
                    var product = await _context.Productos.FindAsync(item.ProductId);
                    if (product == null)
                        return BadRequest($"Producto con ID {item.ProductId} no existe.");

                    if (product.Stock < item.Quantity)
                        return BadRequest($"No hay suficiente stock para el producto {product.Nombre}.");

                    product.Stock -= item.Quantity; // Actualizamos el stock
                    _context.Productos.Update(product);

                    var detail = new SaleDetail
                    {
                        ProductId = product.Id,
                        Quantity = item.Quantity,
                        UnitPrice = product.Precio
                    };

                    total += detail.UnitPrice * detail.Quantity;
                    sale.SaleDetails.Add(detail);
                }

                sale.Total = total;

                _context.Sales.Add(sale);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(new { message = "Venta completada exitosamente.", saleId = sale.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return BadRequest(new { mensaje = ex.Message });
            }
        }



        // GET: api/sales
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllSales()
        {
            var sales = await _context.Sales
                .Include(s => s.User)
                .Include(s => s.SaleDetails)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return Ok(sales);
        }

        // GET: api/sales/user
        [HttpGet("user")]
        public async Task<IActionResult> GetMySales()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return Unauthorized();

            var sales = await _context.Sales
                .Where(s => s.UserId == user.Id)
                .Include(s => s.SaleDetails)
                    .ThenInclude(d => d.Producto)
                .ToListAsync();

            return Ok(sales);
        }
    }
}
