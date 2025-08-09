using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    public class CompraProveedor
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }

        [JsonIgnore]
        public Proveedor? Proveedor { get; set; }

        public List<DetalleCompraProveedor> Detalles { get; set; } = new List<DetalleCompraProveedor>();
    }
}
