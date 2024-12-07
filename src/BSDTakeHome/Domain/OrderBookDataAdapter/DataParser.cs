using OrderBookDataAdapter.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OrderBookDataAdapter
{
  public class DataParser
  {
    public List<ExchangeModel> Exchanges { get; set; } = new List<ExchangeModel>();
    public DataParser()
    {
#if DEBUG
      string path = @"bin\Debug\net8.0\Data";
#else
      string path = @"bin\Release\net8.0\Data"; 
#endif
      foreach (string file in Directory.EnumerateFiles(path, "*.json"))
      {
        using (StreamReader reader = new StreamReader(file))
        {
          var exchangeOrderBook = JsonSerializer.Deserialize<OrderBookModel>(reader.ReadToEnd());
          if (exchangeOrderBook == null)
          {
            continue;
          }
          Exchanges.Add(new ExchangeModel
          {
            Name = "exchange_" + file.Split("\\").Last().Split('.')[0].Last(),
            OrderBook = exchangeOrderBook
          });
        }
      }
    }
  }
}
