using AuthAPI.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoFinal.Models
{
    public class ComentarioProducto
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public AppUser User { get; set; }

        public int ProductoId { get; set; }
        public Producto Producto { get; set; }

        public string Contenido { get; set; }

        public DateTime Fecha { get; set; } = DateTime.Now;

        // Nuevo: sistema de likes/dislikes
        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;

        // Nuevo: indicador si el usuario compró el producto
        [NotMapped] // no se guarda directamente, se calcula al momento de mostrar
        public bool HaCompradoProducto { get; set; }
    }
}
