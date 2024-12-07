namespace OrderBookDataAdapter.Models
{
  public class OrderBookModel
  {
    public DateTime AcqTime { get; set; }
    public List<OrderModel> Bids { get; set; } = new List<OrderModel>();
    public List<OrderModel> Asks { get; set; } = new List<OrderModel>();
  }
}
