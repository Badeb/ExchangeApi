using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ExchangeApi.Data
{
    public class DBconnectFactory : IDesignTimeDbContextFactory<DbConnect>
    {
        public DbConnect CreateDbContext(string[] args)
        {
            // Build configuration from appsettings.json
            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<DbConnect>();

            // 🟢 Burada SQL Server provider'ı doğru şekilde bağlanmalı
            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);

            return new DbConnect(optionsBuilder.Options);
        }
    }
}
