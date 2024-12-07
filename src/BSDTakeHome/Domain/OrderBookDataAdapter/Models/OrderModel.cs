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
    public double Amount { get; set; }
    public double Price { get; set; }  
  }
}
