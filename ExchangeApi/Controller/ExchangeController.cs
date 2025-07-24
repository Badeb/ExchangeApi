using ExchangeApi.Data;
using ExchangeApi.Models.Entities;
using ExchangeApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace ExchangeApi.Controllers

{
    //localhost/xxxx/api/....

    [ApiController]
    [Route("api/")]
    public class ExchangeController : ControllerBase
    {
        private readonly DbConnect dbconnect;
        private readonly ExchangeService service;


        public ExchangeController(DbConnect dbconnect, ExchangeService service)
        {
            this.dbconnect = dbconnect;
            this.service = service;
        }


        [HttpGet("exchange-rate")]
        public async Task<IActionResult> GetExchangeRate(string BaseCurrency, string TargetCurrency)
        {
            BaseCurrency = BaseCurrency.ToUpper();
            TargetCurrency = TargetCurrency.ToUpper();
            try
            {
                var result = await service.GetExchangeResultAsync(BaseCurrency, TargetCurrency);

                if (result == null) // if cant get from API
                {
                    return NotFound();
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


                    var exchangeResult = new ExchangeResult 
                    {

                        Rate = result.Rate,
                        TimeStamp = DateTime.UtcNow,
                        CurrencyQueryId = query.Id

                    };
                    dbconnect.ExchangeResult.Add(exchangeResult);
                    await dbconnect.SaveChangesAsync();
                    return Ok(exchangeResult);
                }
                else //if the query exist already
                {

                    var exchangeResult = new ExchangeResult  
                    {

                        Rate = result.Rate,
                        TimeStamp = DateTime.UtcNow,
                        CurrencyQueryId = query.Id

                    };
                    dbconnect.ExchangeResult.Add(exchangeResult);
                    await dbconnect.SaveChangesAsync();
                    return Ok(exchangeResult);
                }
                ;

            }

            catch (Exception)
            {
                return NotFound("Exchange rate not found");

            }


        }
        [HttpPost("queries")]
        public async Task<IActionResult> FavUsed(string BaseCurrency, string TargetCurrency, string Name)
        {
            BaseCurrency = BaseCurrency.ToUpper();
            TargetCurrency = TargetCurrency.ToUpper();
            Name = Name.ToUpper();
            try
            {
                var result = await service.GetExchangeResultAsync(BaseCurrency, TargetCurrency);

                if (result == null) 
                {
                    return NotFound();
                }

                var existingFav = await dbconnect.FavoriteQueries.FirstOrDefaultAsync(q => q.Base == BaseCurrency && q.Target == TargetCurrency && q.Name == Name);

                if (existingFav != null)  //if there is an existing fav it update the rate
                {
                    existingFav.Rate = result.Rate;
                    dbconnect.FavoriteQueries.Update(existingFav);
                    await dbconnect.SaveChangesAsync();


                }
                if (existingFav == null) 
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

                    return Ok(newFav);
                }
                return null;
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        [HttpGet("queries")]
        public async Task<IActionResult> QueriesGet()
        {
            try
            {
                var querylist = await dbconnect.FavoriteQueries.OrderBy(q => q.Id).ToListAsync();  //list the queries order by id 
                return Ok(querylist);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }

        }


        [HttpGet("results")]
        public async Task<IActionResult> ResultsGet(int Id)
        {
            try
            {
                var result_id = await dbconnect.ExchangeResult.Where(r => r.CurrencyQueryId == Id)  //if this Id exist in currencyquery , it order by time 
               .OrderByDescending(r => r.TimeStamp)
               .ToListAsync();
                if (!result_id.Any())
                    return NotFound("No results found for this currency pair.");

                return Ok(result_id);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("remove_favorite")]
        public async Task<IActionResult> RemoveFav(int Id)  //if you want to delete your favorite base-target , you need to enter the id only
        {
            try
            {
                var fav_id = await dbconnect.FavoriteQueries.FirstOrDefaultAsync(f => f.Id == Id);
                if (fav_id == null)
                {
                    return NotFound();
                }
                else
                {
                    dbconnect.FavoriteQueries.Remove(fav_id);
                    await dbconnect.SaveChangesAsync();

                    return Ok("Favorite currency pair was deleted");
                }

            }
            catch (Exception ex)
            {

                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}