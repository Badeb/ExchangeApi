using System.ComponentModel.DataAnnotations;

namespace ExchangeApi.Models.Entities
{
    public class UsersInfo
    {
        [Key]
        public required string PhoneNumber { get; set; }
        
        public required string Password { get; set; }
        
        public required string Name { get; set; }
       
    }
}
