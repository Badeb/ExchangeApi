using ExchangeApi.Data;
using ExchangeApi.Services;
using ExchangeApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DbConnect>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));




builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
{
    ValidateIssuer = true,
    ValidateAudience = true,
    ValidateLifetime = true,
    ValidateIssuerSigningKey = true,
    ValidIssuer = builder.Configuration["Jwt:Issuer"],
    ValidAudience = builder.Configuration["Jwt:Audience"],
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))


});
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(
        "log.txt",
        rollingInterval: RollingInterval.Month,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} {Message} {NewLine} {Exception}"
    )
    .CreateLogger();


builder.Host.UseSerilog();
builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer",
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new String []{ }
        }
    });
});

    //var jwtSecuritySchema = new OpenApiSecurityScheme
    //{
    //    BearerFormat = "JWT",
    //    Name = "Authorization",
    //    In = ParameterLocation.Header,
    //    Type = SecuritySchemeType.Http,
    //    Scheme = JwtBearerDefaults.AuthenticationScheme,
    //    Description = "Enter JWT token in the format: Bearer {your token}",
    //    Reference = new OpenApiReference
    //    {
    //        Id = JwtBearerDefaults.AuthenticationScheme,
    //        Type = ReferenceType.SecurityScheme
    //    }
    //};



builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();

builder.Services.AddHttpClient<IExchangeService,ExchangeService>();
builder.Services.AddScoped<IExchangeService, ExchangeService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();


//builder.Services.AddLogging();


var app = builder.Build();


app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
//when controller triggered ,it works
app.UseHttpsRedirection();
app.Run();


