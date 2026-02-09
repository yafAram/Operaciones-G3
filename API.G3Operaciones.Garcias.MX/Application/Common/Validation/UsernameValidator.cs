using System.Text.RegularExpressions;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Validation
{
    public static class UsernameValidator
    {
        private static readonly Regex _regex =
            new("^[A-Za-z]+$", RegexOptions.Compiled);

        public static bool IsValid(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return false;

            return _regex.IsMatch(username);
        }
    }
}
