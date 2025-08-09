using AuthAPI.Models;
using System.Text.Json.Serialization;

namespace ProyectoFinal.Models
{
    public class Proveedor
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }

        // Relación con categoría (solo vende productos de esta categoría)
        public int CategoriaId { get; set; }
        [JsonIgnore]
        public Categoria? Categoria { get; set; }
    }
}
