using System.Text.Json.Serialization;

namespace MetaExchangeService.Interfaces.Enums
{
  [JsonConverter(typeof(JsonStringEnumConverter))]
  public enum OrderTypeEnum
  {
    Buy,
    Sell
  }
}
