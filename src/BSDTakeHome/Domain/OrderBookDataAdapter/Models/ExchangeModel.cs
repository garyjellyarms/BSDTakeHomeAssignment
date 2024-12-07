using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBookDataAdapter.Models
{
  public class ExchangeModel
  {
    public int Id { get; set; }
    public string? Name { get; set; }
    public OrderBookModel OrderBook { get; set; } = new OrderBookModel();
  }
}
