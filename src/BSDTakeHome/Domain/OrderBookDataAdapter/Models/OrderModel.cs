namespace OrderBookDataAdapter.Models
{
  public class OrderModel
  {
    public Order Order { get; set; } = new Order();
  }
  public class Order
  {
    public int? Id { get; set; }
    public DateTime Time { get; set; }
    public string? Type { get; set; }
    public string? Kind { get; set; }
    public decimal Amount { get; set; }
    public decimal Price { get; set; }  
    public string? ExchangeName { get; set; } 
  }
}
