using ExchangeApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExchangeApi.Data
{
    public class DbConnect : DbContext
    {

        public DbConnect(DbContextOptions<DbConnect> options) : base(options)
        {

        }
        public DbSet<ExchangeResult> ExchangeResult { get; set; }
        public DbSet<CurrencyQuery> CurrencyQuery { get; set; }
        public DbSet<FavoriteQueries> FavoriteQueries
        {
            get; set;

        }
    }
}
