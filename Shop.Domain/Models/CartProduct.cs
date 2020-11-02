namespace Shop.Domain.Models
{
    public class CartProduct
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }  
        public int StockId { get; set; }
        public ushort Qty { get; set; }
        public ushort Value { get; set; }
    }
}
