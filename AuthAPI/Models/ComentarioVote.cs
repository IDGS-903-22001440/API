using AuthAPI.Models;
using ProyectoFinal.Models;

public class ComentarioVote
{
    public int Id { get; set; }

    public string UserId { get; set; }
    public AppUser User { get; set; }

    public int ComentarioId { get; set; }
    public ComentarioProducto Comentario { get; set; }

    // true = like, false = dislike
    public bool EsLike { get; set; }
}