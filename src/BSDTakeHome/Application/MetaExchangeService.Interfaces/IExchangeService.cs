using MetaExchangeService.Interfaces.DTOs;
using MetaExchangeService.Interfaces.Enums;

namespace MetaExchangeService.Interfaces
{
  public interface IExchangeService
  {
    ProcessedOrderDTO? FillOrder(decimal amount, OrderTypeEnum orderType);
  }
}
