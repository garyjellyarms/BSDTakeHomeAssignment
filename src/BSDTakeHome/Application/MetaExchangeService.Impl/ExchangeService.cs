using MetaExchangeService.Interfaces;
using MetaExchangeService.Interfaces.DTOs;
using MetaExchangeService.Interfaces.Enums;
using OrderBookDataAdapter;
using OrderBookDataAdapter.Models;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace MetaExchangeService.Impl
{
  public class ExchangeService : IExchangeService
  {
    public DataParser _dataParser { get; set; }
    public ExchangeService(DataParser dataParser)
    {
      _dataParser = dataParser;
    }

    public ProcessedOrderDTO? FillOrder(decimal amount, OrderTypeEnum orderType)
    {
      if (amount <= 0)
      {
        return new ProcessedOrderDTO
        {
          ProcessedOrderType = ProcessedOrderTypeEnum.UnfillableOrder,
        };
      }
      if (orderType == OrderTypeEnum.Sell)
      {
        var result = ProcessSellOrder(amount);
        return result;
      }
      else
      {
        var result = ProcessBuyOrder(amount);
        return result;
      }
    }
    #region Private methods
    private ProcessedOrderDTO ProcessSellOrder(decimal amount)
    {
      ProcessedOrderDTO optimalProcessedOrder = new ProcessedOrderDTO();
      List<ExchangeConstraint> exchangeConstraints = new List<ExchangeConstraint>(_dataParser.Exchanges.Count);
      List<OrderModel> bids = new List<OrderModel>();
      _dataParser.Exchanges.ForEach(exchange =>
      {
        exchangeConstraints.Add(new ExchangeConstraint
        {
          BTCAmountLimit = exchange.OrderBook.BTCAmountLimit,
          EURAmountLimit = exchange.OrderBook.EURAmountLimit,
          Name = exchange.Name
        });
        bids.AddRange(exchange.OrderBook.Bids);
      });

      bids = bids.OrderByDescending(order => order.Order.Price).ToList();

      foreach (var bid in bids)
      {
        var exchangeConstraint = exchangeConstraints.FirstOrDefault(x => x.Name == bid.Order.ExchangeName);
        if (bid.Order.Type != "Buy")
        {
          continue;
        }

        if (amount < bid.Order.Amount)
        {
          if (exchangeConstraint!.CurrentEURAmount + amount * bid.Order.Price > exchangeConstraint.EURAmountLimit)
          {
            continue;
          }
          optimalProcessedOrder.Subtransactions.Add(new TransactionDTO
          {
            Price = bid.Order.Price,
            Amount = amount,
            Exchange = bid.Order.ExchangeName,
            OrderType = OrderTypeEnum.Sell
          });
          amount = 0;
          break;
        }
        else
        {
          if (exchangeConstraint!.CurrentEURAmount + bid.Order.Amount * bid.Order.Price > exchangeConstraint.EURAmountLimit)
          {
            continue;
          }
          optimalProcessedOrder.Subtransactions.Add(new TransactionDTO
          {
            Price = bid.Order.Price,
            Amount = bid.Order.Amount,
            Exchange = bid.Order.ExchangeName,
            OrderType = OrderTypeEnum.Sell
          });
          exchangeConstraint!.CurrentEURAmount += bid.Order.Amount * bid.Order.Price;
          amount -= bid.Order.Amount;
        }
      }

      optimalProcessedOrder.ProcessedOrderType = ProcessedOrderTypeEnum.Success;

      if (amount != 0)
      {
        optimalProcessedOrder.Subtransactions.Clear();
        optimalProcessedOrder.ProcessedOrderType = ProcessedOrderTypeEnum.UnfillableOrder;
      }

      return optimalProcessedOrder;
    }

    private ProcessedOrderDTO ProcessBuyOrder(decimal amount)
    {
      ProcessedOrderDTO optimalProcessedOrder = new ProcessedOrderDTO();
      List<ExchangeConstraint> exchangeConstraints = new List<ExchangeConstraint>(_dataParser.Exchanges.Count);
      List<OrderModel> asks = new List<OrderModel>();
      _dataParser.Exchanges.ForEach(exchange =>
      {
        exchangeConstraints.Add(new ExchangeConstraint
        {
          BTCAmountLimit = exchange.OrderBook.BTCAmountLimit,
          EURAmountLimit = exchange.OrderBook.EURAmountLimit,
          Name = exchange.Name
        });
        asks.AddRange(exchange.OrderBook.Asks);
      });
      asks = asks.OrderBy(order => order.Order.Price).ToList();

      foreach (var ask in asks)
      {
        if (ask.Order.Type != "Sell")
        {
          continue;
        }
        var exchangeConstraint = exchangeConstraints.FirstOrDefault(x => x.Name == ask.Order.ExchangeName);

        if (amount < ask.Order.Amount) 
        {
          if (exchangeConstraint!.CurrentBTCAmount + amount > exchangeConstraint.BTCAmountLimit)
          {
            continue;
          }
          optimalProcessedOrder.Subtransactions.Add(new TransactionDTO
          {
            Price = ask.Order.Price,
            Amount = amount,
            Exchange = ask.Order.ExchangeName,
            OrderType = OrderTypeEnum.Buy
          });
          amount = 0;
          break;
        }
        else
        {
          if (exchangeConstraint!.CurrentBTCAmount + ask.Order.Amount > exchangeConstraint.BTCAmountLimit)
          {
            continue;
          }
          optimalProcessedOrder.Subtransactions.Add(new TransactionDTO
          {
            Price = ask.Order.Price,
            Amount = ask.Order.Amount,
            Exchange = ask.Order.ExchangeName,
            OrderType = OrderTypeEnum.Buy
          });
          exchangeConstraint!.CurrentBTCAmount += ask.Order.Amount;
          amount -= ask.Order.Amount;
        }

      }
      optimalProcessedOrder.ProcessedOrderType = ProcessedOrderTypeEnum.Success;


      if (amount != 0)
      {
        optimalProcessedOrder.Subtransactions.Clear();
        optimalProcessedOrder.ProcessedOrderType = ProcessedOrderTypeEnum.UnfillableOrder;
      }

      return optimalProcessedOrder;
    }

    
    #endregion
  }
}
