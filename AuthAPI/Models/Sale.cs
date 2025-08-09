using AuthAPI.Models;

namespace ProyectoFinal.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public decimal Total { get; set; }

        public ICollection<SaleDetail> SaleDetails { get; set; }
    }
}
