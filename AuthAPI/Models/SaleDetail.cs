using AuthAPI.Models;
using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    public class SaleDetail
    {
        public int Id { get; set; }

        public int ProductId { get; set; }           // FK
        [JsonIgnore]
        public Producto? Producto { get; set; }       // NAVIGATION PROPERTY

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        public int SaleId { get; set; }              // FK hacia Sale
        [JsonIgnore]
        public Sale? Sale { get; set; }               // NAVIGATION
    }
}
