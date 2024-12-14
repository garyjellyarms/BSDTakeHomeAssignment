using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBookDataAdapter.Models
{
  public class ExchangeConstraint
  {
    public string? Name { get; set; }
    public decimal BTCAmountLimit { get; set; } = 6;
    public decimal EURAmountLimit { get; set; } = 15000;
    public decimal CurrentBTCAmount = 0;
    public decimal CurrentEURAmount = 0;
  }
}
