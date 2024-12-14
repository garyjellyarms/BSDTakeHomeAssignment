using MetaExchangeService.Interfaces.Enums;

namespace MetaExchangeService.Interfaces.DTOs
{
  public class TransactionDTO
  {
    public string? Exchange { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public OrderTypeEnum OrderType { get; set; }
  }
}
