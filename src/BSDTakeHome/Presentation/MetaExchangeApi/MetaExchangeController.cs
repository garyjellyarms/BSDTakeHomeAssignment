using Microsoft.AspNetCore.Mvc;
using OrderBookDataAdapter;
using System.Net.Mime;

namespace MetaExchangeApi
{
  [ApiController]
  [Consumes(MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Application.Json)]
  [Route("MetaExchange")]
  public class MetaExchangeController : ControllerBase
  {
    private readonly DataParser _dataParser;
    public MetaExchangeController(DataParser dataParser)
    {
      _dataParser = dataParser;
    }

    [HttpGet]
    public int Get()
    {
      return _dataParser.Exchanges.Count;
    }
  }
}
