using MetaExchangeService.Impl;
using OrderBookDataAdapter;
using MetaExchangeService.Interfaces.Enums;


internal class Program
{
  private static void Main(string[] args)
  {
    DataParser parser = new DataParser(@"Data");
    ExchangeService exchangeService = new ExchangeService(parser);
    var response = exchangeService.FillOrder(9, OrderTypeEnum.Buy);
    var response2 = exchangeService.FillOrder(9, OrderTypeEnum.Sell);
    Console.WriteLine();
  }
}