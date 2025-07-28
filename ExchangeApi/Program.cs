using ExchangeApi.Data;
using ExchangeApi.Services;
using ExchangeApi.Services.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbConnect>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Services.AddControllers();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient<IExchangeService,ExchangeService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();





var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
//when controller triggered ,it works
app.UseHttpsRedirection();
app.Run();
