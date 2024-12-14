namespace OrderBookDataAdapter.Models
{
  public class OrderBookModel
  {
    public decimal BTCAmountLimit { get; set; } = 6;
    public decimal EURAmountLimit { get; set; } = 15000;
    public DateTime AcqTime { get; set; }
    public List<OrderModel> Bids { get; set; } = new List<OrderModel>();
    public List<OrderModel> Asks { get; set; } = new List<OrderModel>();
  }
}
