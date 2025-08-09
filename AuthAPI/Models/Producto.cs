using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Producto
    {
        public int Id { get; set; } // Clave primaria
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public string Imagen { get; set; }

        public int Stock { get; set; }

        // Clave foránea
        public int CategoriaId { get; set; }

        // Navegación
        [JsonIgnore]
        public Categoria? Categoria { get; set; }
    }
}