namespace EcommerceProject.Models
{
    // This is the OrderItem model that represents an item in an order
    // It has properties for the item's ID, order ID, product ID, quantity, unit price, and total price
    public class OrderItem
    {
        public int Id { get; set; }

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal TotalPrice => Quantity * UnitPrice;
    }
} 