namespace ProyectoFinal.Dto
{
    public class SaleDto
    {
        public List<SaleItemDto> DetalleVenta { get; set; }
    }

    public class SaleItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}