using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AuthAPI.Models
{
    public class Categoria
    {
        public int Id { get; set; } // Clave primaria
        public string Nombre { get; set; }

        // Relación uno a muchos
        [JsonIgnore]
        public ICollection<Producto>? Productos { get; set; }
    }
}
