using ExchangeApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeApi.Data
{
    public class DbConnect(DbContextOptions<DbConnect> options) : DbContext(options)
    {
        public DbSet<ExchangeResult> ExchangeResult { get; set; }
        public DbSet<CurrencyQuery> CurrencyQuery { get; set; }
        public DbSet<FavoriteQueries> FavoriteQueries { get; set; }
        public DbSet<UsersInfo> UsersInfo { get; set; }

      
    }
}
