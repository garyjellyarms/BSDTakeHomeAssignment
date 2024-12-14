using MetaExchangeService.Impl;
using OrderBookDataAdapter;
using MetaExchangeService.Interfaces.Enums;
using MetaExchangeService.Interfaces.DTOs;
using System.Formats.Asn1;
using OrderBookDataAdapter.Models;

namespace MetaExchangeService.Tests
{
  [TestClass]
  public class MetaExchangeServiceTests
  {
    private ExchangeService? exchangeService;

    [TestInitialize]
    public void InitTest()
    {
      exchangeService = new ExchangeService(new DataParser(@"Data"));
    }

    #region Positive Cases

    [TestMethod]
    public void FillOrder_Buys_Success_Test()
    {
      int btcAmountToFill = 2;
      var result = exchangeService!.FillOrder(btcAmountToFill, OrderTypeEnum.Buy);

      Assert.IsNotNull(result);
      Assert.AreEqual(result.Subtransactions.Sum(x => x.Amount), btcAmountToFill);
      Assert.AreEqual(ProcessedOrderTypeEnum.Success, result.ProcessedOrderType);

      CheckTransactionOrderStatuses(OrderTypeEnum.Buy, result.Subtransactions);
    }

    [TestMethod]
    public void FillOrder_Sell_Success_Test()
    {
      int btcAmountToFill = 2;

      var result = exchangeService!.FillOrder(btcAmountToFill, OrderTypeEnum.Sell);

      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.Success, result.ProcessedOrderType);
      Assert.AreEqual(result.Subtransactions.Sum(x => x.Amount), btcAmountToFill);
      CheckTransactionOrderStatuses(OrderTypeEnum.Sell, result.Subtransactions);
    }

    [TestMethod]
    public void FillOrder_Buy_OnlyOneExchange_Test()
    {
      int btcAmountToFill = 2;
      exchangeService!._dataParser.Exchanges[0].OrderBook.Asks.ForEach(x =>
      {
        x.Order.Price = 5000;
      });

      var result = exchangeService!.FillOrder(btcAmountToFill, OrderTypeEnum.Buy);

      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.Success, result.ProcessedOrderType);
      Assert.AreEqual(result.Subtransactions.Sum(x => x.Amount), btcAmountToFill);
      Assert.AreEqual(CheckNumberOfDifferentExchanges(result.Subtransactions), 1);
    }

    [TestMethod]
    public void FillOrder_Sell_OnlyOneExchange_Test()
    {
      exchangeService!._dataParser.Exchanges[0].OrderBook.Bids.ForEach(x =>
      {
        x.Order.Price = 5000;
      });


      var result = exchangeService!.FillOrder(2, OrderTypeEnum.Sell);
      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.Success, result.ProcessedOrderType);
      Assert.AreEqual(CheckNumberOfDifferentExchanges(result.Subtransactions), 1);
    }

    #endregion

    #region Negative Cases

    [TestMethod]
    public void FillOrder_Buy_UnfillableOrder_ExchangeLimit_ToLow_Test()
    {
      exchangeService!._dataParser.Exchanges[0].OrderBook.BTCAmountLimit = 1;
      exchangeService!._dataParser.Exchanges[1].OrderBook.BTCAmountLimit = 1;

      var result = exchangeService!.FillOrder(3, OrderTypeEnum.Buy);

      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.UnfillableOrder, result.ProcessedOrderType);
      Assert.AreEqual(0, result.Subtransactions.Count);
    }


    [TestMethod]
    public void FillOrder_Sell_UnfillableOrder_ExchangeLimit_ToLow_Test()
    {
      exchangeService!._dataParser.Exchanges[0].OrderBook.EURAmountLimit = 2000;
      exchangeService!._dataParser.Exchanges[1].OrderBook.EURAmountLimit = 2000;

      var result = exchangeService!.FillOrder(2, OrderTypeEnum.Sell);

      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.UnfillableOrder, result.ProcessedOrderType);
      Assert.AreEqual(0, result.Subtransactions.Count);
    }

    [TestMethod]
    public void FillOrder_Buy_UnfillableOrder_NotEnough_Asks_Test()
    {
      exchangeService!._dataParser.Exchanges[0].OrderBook.Asks = GenerateOrders(2, exchangeService!._dataParser.Exchanges[0].Name!, "Sell");
      exchangeService!._dataParser.Exchanges[1].OrderBook.Asks = GenerateOrders(1, exchangeService!._dataParser.Exchanges[0].Name!, "Sell");

      var result = exchangeService!.FillOrder(200, OrderTypeEnum.Sell);

      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.UnfillableOrder, result.ProcessedOrderType);
      Assert.AreEqual(0, result.Subtransactions.Count);
    }

    [TestMethod]
    public void FillOrder_Sell_UnfillableOrder_NotEnough_Asks_Test()
    {
      exchangeService!._dataParser.Exchanges[0].OrderBook.Bids = GenerateOrders(2, exchangeService!._dataParser.Exchanges[0].Name!, "Buy");
      exchangeService!._dataParser.Exchanges[1].OrderBook.Bids = GenerateOrders(1, exchangeService!._dataParser.Exchanges[0].Name!, "Buy");

      var result = exchangeService!.FillOrder(200, OrderTypeEnum.Buy);

      Assert.IsNotNull(result);
      Assert.AreEqual(ProcessedOrderTypeEnum.UnfillableOrder, result.ProcessedOrderType);
      Assert.AreEqual(0, result.Subtransactions.Count);
    }


    #endregion

    #region Private Methods 

    private List<OrderModel> GenerateOrders(int count, string exchangeName, string orderType)
    {
      Random rnd = new Random();
      List<OrderModel> orders = new List<OrderModel>(count);
      for (int i = 0; i < count; i++)
      {
        orders.Add(new OrderModel
        {
          Order = new Order
          {
            ExchangeName = exchangeName,
            Amount = (decimal)(rnd.NextDouble() * rnd.Next(1, 4)),
            Price = (decimal)(rnd.NextDouble() * 10000),
            Type = orderType
          }
        });
      }

      return orders;
    }

    private void CheckTransactionOrderStatuses(OrderTypeEnum expectedOrderType, IList<TransactionDTO> transactions)
    {
      foreach (var transaction in transactions)
      {
        Assert.AreEqual(expectedOrderType, transaction.OrderType);
      }
    }
    
    private int CheckNumberOfDifferentExchanges(IList<TransactionDTO> transactions)
    {
      List<string> uniqueExchanges = new List<string>();
      foreach(var transaction in transactions)
      {
        if (uniqueExchanges.Contains(transaction.Exchange!))
        {
          continue;
        }
        uniqueExchanges.Add(transaction.Exchange!);
      }
      return uniqueExchanges.Count;
    }


    #endregion
  }
}