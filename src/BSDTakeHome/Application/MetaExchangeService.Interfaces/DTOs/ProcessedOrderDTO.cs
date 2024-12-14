using MetaExchangeService.Interfaces.Enums;

namespace MetaExchangeService.Interfaces.DTOs
{
  public class ProcessedOrderDTO
  {
    public IList<TransactionDTO> Subtransactions { get; set; } = new List<TransactionDTO>();
    public ProcessedOrderTypeEnum ProcessedOrderType { get; set; }
  }
}
