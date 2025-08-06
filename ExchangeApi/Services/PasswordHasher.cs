using System.Security.Cryptography;
using System.Text;

namespace ExchangeApi.Services
{
    public static class PasswordHasher
    {
        // Şifreyi hash'ler
        public static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        // Girilen şifre hash'lenmiş olanla uyuşuyor mu kontrol eder
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            var hashOfInput = HashPassword(password);
            return hashOfInput == hashedPassword;
        }
    }
}
