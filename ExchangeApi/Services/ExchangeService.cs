using ExchangeApi.DTO;
using ExchangeApi.Models.Entities;
using System.Net.Http.Json;

namespace ExchangeApi.Services
{
    public class ExchangeService
    {
        private readonly HttpClient httpClient;
        private const string AccessKey = "1d5676c36d2508bed228dbfa2d1e5527";

        public ExchangeService(HttpClient httpClient)
        {
            this.httpClient = httpClient;
            this.httpClient.BaseAddress = new Uri("https://api.exchangerate.host/");
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
    }
}
