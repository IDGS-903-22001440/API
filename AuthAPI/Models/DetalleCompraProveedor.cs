using AuthAPI.Models;
using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    public class DetalleCompraProveedor
    {
        public int Id { get; set; }
        public int CompraProveedorId { get; set; }
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitarioCompra { get; set; }
        public decimal CostosIndirectos { get; set; } // Para costeo absorbente

        [JsonIgnore]
        public CompraProveedor? CompraProveedor { get; set; }
        [JsonIgnore]
        public Producto? Producto { get; set; }
    }
}
