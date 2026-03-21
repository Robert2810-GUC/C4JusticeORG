using System.Security.Cryptography;
using System.Text;

namespace C4Justice.Web.Helpers
{
    public static class AuthHelper
    {
        private const string Salt = "c4j_salt_2024";

        public static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password + Salt));
            return Convert.ToHexString(bytes).ToLower();
        }

        public static bool VerifyPassword(string password, string hash)
            => HashPassword(password) == hash;

        public static string GenerateSlug(string title)
        {
            var slug = title.ToLowerInvariant();
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"[^a-z0-9\s-]", "");
            slug = System.Text.RegularExpressions.Regex.Replace(slug, @"\s+", "-");
            slug = slug.Trim('-');
            if (slug.Length > 200) slug = slug[..200];
            return slug;
        }
    }
}
