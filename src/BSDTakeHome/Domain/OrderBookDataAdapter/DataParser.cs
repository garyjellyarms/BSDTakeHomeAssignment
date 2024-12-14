using OrderBookDataAdapter.Models;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OrderBookDataAdapter
{
  public class DataParser
  {
    public List<ExchangeModel> Exchanges { get; set; } = new List<ExchangeModel>();
    private string? _path {  get; set; }
    public DataParser()
    {
#if DEBUG
      _path = @"bin\Debug\net8.0\Data";
#else
      _path = @"Data";
#endif
      ParseData(_path);
    }
    
    public DataParser(string path)
    {
      _path = path;
      ParseData(_path);
    }

    private void ParseData(string path)
    {
      foreach (string file in Directory.EnumerateFiles(path, "*.json"))
      {
        using (StreamReader reader = new StreamReader(file))
        {
          var exchangeOrderBook = JsonSerializer.Deserialize<OrderBookModel>(reader.ReadToEnd());
          var exchangeName = "exchange_" + file.Split("\\").Last().Split('.')[0].Last();
          exchangeOrderBook!.Asks.ForEach(x =>
          {
            x.Order.ExchangeName = exchangeName;
          });
          exchangeOrderBook!.Bids.ForEach(x =>
          {
            x.Order.ExchangeName = exchangeName;
          });
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
