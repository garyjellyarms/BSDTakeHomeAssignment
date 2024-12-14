using MetaExchangeService.Interfaces;
using MetaExchangeService.Interfaces.DTOs;
using MetaExchangeService.Interfaces.Enums;
using Microsoft.AspNetCore.Mvc;
using OrderBookDataAdapter;
using System.ComponentModel.DataAnnotations;
using System.Net.Mime;

namespace MetaExchangeApi
{
  [ApiController]
  [Consumes(MediaTypeNames.Application.Json)]
  [Produces(MediaTypeNames.Application.Json)]
  [Route("MetaExchange")]
  public class MetaExchangeController : ControllerBase
  {
    private readonly IExchangeService _exchangeService;
    public MetaExchangeController(IExchangeService exchangeService)
    {
      _exchangeService = exchangeService;
    }

    [HttpGet("FillOrder")]
    public ActionResult<ProcessedOrderDTO> FillOrder([Required] decimal amount, [Required] OrderTypeEnum orderTypeEnum)
    {
      var response = _exchangeService.FillOrder(amount, orderTypeEnum);
      if (response == null)
      {
        return NotFound();
      }
      return Ok(response);
    }
  }
}
