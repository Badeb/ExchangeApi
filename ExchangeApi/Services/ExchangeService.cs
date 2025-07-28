using ExchangeApi.Controllers;
using ExchangeApi.Data;
using ExchangeApi.DTO;
using ExchangeApi.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using ExchangeApi.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExchangeApi.Services
{
    public class ExchangeService : IExchangeService
    {
        private readonly HttpClient httpClient;
        private const string AccessKey = "1d5676c36d2508bed228dbfa2d1e5527";
        private readonly DbConnect dbconnect;
        private readonly ILogger<ExchangeService> logger;


        public ExchangeService(HttpClient httpClient, DbConnect dbconnect, ILogger<ExchangeService> logger)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://api.exchangerate.host/");
            this.dbconnect = dbconnect;
            this.logger = logger;

        }


        public async Task<ExchangeResult?> GetExchangeResultAsync(string fromCurrency, string toCurrency)
        {

            string url = $"convert?access_key={AccessKey}&from={fromCurrency}&to={toCurrency}&amount=1";

            try
            {
                var result = await httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(url);


                if (result != null)
                {
                    return new ExchangeResult
                    {
                        Rate = result.Result,
                        TimeStamp = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exchange API calling error: {ex.Message}");
            }

            return null;
        }
        public async Task<ExchangeResult?> SaveExchanges(string BaseCurrency, string TargetCurrency)  //GetExchangeRate
        {
            try
            {
                var result = await GetExchangeResultAsync(BaseCurrency, TargetCurrency);
                if (result == null)
                {
                    logger.LogWarning("Can not found the currencies");
                    return null;

                }

                var query = await dbconnect.CurrencyQuery.FirstOrDefaultAsync(q => q.Base == BaseCurrency && q.Target == TargetCurrency); //check is there  a same base and target

                if (query == null) //if there is not

                {
                    query = new CurrencyQuery
                    {
                        Base = BaseCurrency,
                        Target = TargetCurrency
                    };
                    dbconnect.CurrencyQuery.Add(query);
                    await dbconnect.SaveChangesAsync();
                    logger.LogInformation("New currency query created for {BaseCurrency} to {TargetCurrency}", BaseCurrency, TargetCurrency);

                }
                var exchangeResult = new ExchangeResult
                {

                    Rate = result.Rate,
                    TimeStamp = DateTime.UtcNow,
                    CurrencyQueryId = query.Id

                };
                dbconnect.ExchangeResult.Add(exchangeResult);
                await dbconnect.SaveChangesAsync();
                logger.LogInformation("Exchange rate saved for {BaseCurrency} to {TargetCurrency} at {TimeStamp}", BaseCurrency, TargetCurrency, exchangeResult.TimeStamp);
                return exchangeResult;
            }

            catch (Exception)
            {
                logger.LogError("Error retrieving exchange rate for {BaseCurrency} to {TargetCurrency}", BaseCurrency, TargetCurrency);
                return null;


            }
        }
        public async Task<FavoriteQueries?> AddFavoriteQueries(String BaseCurrency, String TargetCurrency, String Name)
        {

            try
            {
                var result = await GetExchangeResultAsync(BaseCurrency, TargetCurrency);

                if (result == null)
                {
                    logger.LogWarning("Exchange rate not found for {BaseCurrency} to {TargetCurrency}", BaseCurrency, TargetCurrency);
                    return null;

                }
                var existingFav = await dbconnect.FavoriteQueries.FirstOrDefaultAsync(q => q.Base == BaseCurrency && q.Target == TargetCurrency && q.Name == Name);

                if (existingFav != null)  //if there is an existing fav it update the rate
                {
                    existingFav.Rate = result.Rate;
                    dbconnect.FavoriteQueries.Update(existingFav);
                    await dbconnect.SaveChangesAsync();
                    logger.LogInformation("Favorite query updated for {BaseCurrency} to {TargetCurrency} with name {Name}", BaseCurrency, TargetCurrency, Name);
                    return existingFav;
                }
                else
                {
                    var newFav = new FavoriteQueries
                    {
                        Base = BaseCurrency,
                        Target = TargetCurrency,
                        Name = Name,
                        Rate = result.Rate
                    };

                    dbconnect.FavoriteQueries.Add(newFav);
                    await dbconnect.SaveChangesAsync();
                    logger.LogInformation("New favorite query created for {BaseCurrency} to {TargetCurrency} with name {Name}", BaseCurrency, TargetCurrency, Name);
                  
                    return newFav;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error saving favorite query for {BaseCurrency} to {TargetCurrency} with name {Name}: {Message}", BaseCurrency, TargetCurrency, Name ,ex.Message);
                return null;
            }
        }

        public async Task<List<FavoriteQueries>> ListOfFavQueries(string name)  //that list the all the favorite quaries 
        {
            
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    logger.LogWarning("Name parameter is empty");
                    throw new KeyNotFoundException($"Name can not be empty");
                }
                name = name.ToUpper();
                var querylist = await dbconnect.FavoriteQueries.Where(q => q.Name == name).OrderBy(q => q.Id).ToListAsync(); //list the queries order by id 
               
                logger.LogInformation("Retrieved {Count} favorite queries from the database for name {Name}", querylist.Count, name);
                return querylist;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving favorite queries: {Message}", ex.Message);
                throw new Exception("Error retrieving favorite queries");
            }

        }
        public async Task<List<ExchangeResult>> ResultsOrderByTime(int Id)  //list the id of currency query and all rates of this
        {
          // List<ExchangeResult> results = new List<ExchangeResult>();
            try
            {
                //if this Id exist in currencyquery , it order by time 
                var result_id = await dbconnect.ExchangeResult.Where(r => r.CurrencyQueryId == Id)
                    .OrderByDescending(r => r.TimeStamp)
                    .ToListAsync();

                if (!result_id.Any())
                {
                    logger.LogWarning("No results found for CurrencyQueryId: {Id}", Id);
                    throw new KeyNotFoundException($"No results found for CurrencyQueryId: {result_id}");
                }

                logger.LogInformation("Retrieved exchange results for CurrencyQueryId: {Id}", Id);
                return result_id;
            }
            catch (Exception ex)
            {
                logger.LogError("Error retrieving exchange results for CurrencyQueryId: {Id}: {Message}", Id,ex.Message);
                throw new Exception("Error retrieving exchange results for CurrencyQueryId");
            }
        }
        public async Task<FavoriteQueries?> RemoveFromFavList(int Id) {
            try
            {
                var fav_id = await dbconnect.FavoriteQueries.FirstOrDefaultAsync(f => f.Id == Id);
                if (fav_id == null)
                {
                    logger.LogWarning("Favorite currency pair with Id {Id} not found", Id);
                    throw new KeyNotFoundException($"No results found for ID: {Id}");
                }
                else
                {
                    dbconnect.FavoriteQueries.Remove(fav_id);
                    await dbconnect.SaveChangesAsync();
                    logger.LogInformation("Favorite currency pair with Id {Id} was deleted", Id);
                    return null;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Error deleting favorite currency pair with Id {Id}: {Message}", Id,ex.Message);
                throw new Exception("Error retrieving exchange results for FavoriteId");

            }
        }

    }
      

 }
