namespace ExchangeApi.DTO

{
    public class ExchangeRateApiResponse
    {
        public bool Success { get; set; }
        public required string Terms { get; set; }

        public required string Privacy { get; set; }
        public required Query Query { get; set; }
        public required Info Info { get; set; }
        public decimal Result { get; set; }
    }
    public class Query
    {
        public required string From { get; set; }
        public required string To { get; set; }
        public decimal Amount { get; set; }

    }
    public class Info
    {
        public long Timestamp { get; set; }
        public decimal Quote { get; set; }

    }
}

