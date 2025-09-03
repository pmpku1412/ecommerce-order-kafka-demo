using EcommerceOrderKafka.Services;
using EcommerceOrderKafka.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Kafka
builder.Services.Configure<KafkaConfiguration>(
    builder.Configuration.GetSection("Kafka"));

// Add custom services
builder.Services.AddSingleton<IKafkaProducer, KafkaProducer>();
builder.Services.AddSingleton<IStockService, StockService>();
builder.Services.AddHostedService<OrderConsumerService>();

// Add health checks
builder.Services.AddHealthChecks();

// Add logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
