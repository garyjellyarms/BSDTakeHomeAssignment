using MetaExchangeService.Interfaces;
using MetaExchangeService.Impl;
using OrderBookDataAdapter;

internal class Program
{
  private static void Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSingleton<DataParser>();
    builder.Services.AddSingleton<IExchangeService, ExchangeService>();

    var app = builder.Build();
    app.UseSwagger();
    app.UseSwaggerUI();

    // Configure the HTTP request pipeline.
    //if (app.Environment.IsDevelopment())
    //{
    //}

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
  }
}