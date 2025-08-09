using AuthAPI.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProyectoFinal.Models;
namespace AuthAPI.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Producto> Productos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<ComentarioProducto> ComentariosProductos { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleDetail> SaleDetails { get; set; }

        public DbSet<ComentarioVote> ComentarioVotes { get; set; }

        public DbSet<CompraProveedor> ComprasProveedor { get; set; }
        public DbSet<DetalleCompraProveedor> DetallesCompraProveedor { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configura la relación ComentarioVote → ComentarioProducto sin cascade delete
            modelBuilder.Entity<ComentarioVote>()
                .HasOne(v => v.Comentario)
                .WithMany()
                .HasForeignKey(v => v.ComentarioId)
                .OnDelete(DeleteBehavior.Restrict); // ← evita cascade

            // Puedes hacer lo mismo con la relación a User si es necesario
            modelBuilder.Entity<ComentarioVote>()
                .HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleCompraProveedor>()
               .HasOne(d => d.Producto)
               .WithMany()
               .HasForeignKey(d => d.ProductoId)
               .OnDelete(DeleteBehavior.Restrict); // Aquí evitas cascade delete para evitar ciclos

            modelBuilder.Entity<DetalleCompraProveedor>()
                .HasOne(d => d.CompraProveedor)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.CompraProveedorId)
                .OnDelete(DeleteBehavior.Cascade); // Esta relación puede ser cascade
        }
    }
}

