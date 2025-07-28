using ExchangeApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ExchangeApi.Services.Interfaces
{
    public interface IExchangeService
    {
        Task<ExchangeResult?> SaveExchanges(string BaseCurrency, string TargetCurrency);
        Task<FavoriteQueries?> AddFavoriteQueries(string BaseCurrency, string TargetCurrency, string Name);
        Task<List<FavoriteQueries>> ListOfFavQueries(string name);
        Task<List<ExchangeResult>> ResultsOrderByTime(int Id);
        Task <FavoriteQueries?> RemoveFromFavList(int Id);

    }
}
