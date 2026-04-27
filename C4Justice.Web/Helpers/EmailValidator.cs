using System.Text.RegularExpressions;

namespace C4Justice.Web.Helpers
{
    public static class EmailValidator
    {
        private static readonly Regex _regex = new(
            @"^[a-zA-Z0-9._%+\-]+@[a-zA-Z0-9.\-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static bool IsValid(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            if (email.Length > 254) return false;
            return _regex.IsMatch(email.Trim());
        }
    }
}
