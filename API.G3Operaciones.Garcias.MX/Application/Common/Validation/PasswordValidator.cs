using System.Text.RegularExpressions;

namespace API.G3Operaciones.Garcias.MX.Application.Common.Validation
{
    public static class PasswordValidator
    {
        private static readonly Regex _regex = new Regex(
            @"^(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,32}$",
            RegexOptions.Compiled
        );

        public static bool IsValid(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            return _regex.IsMatch(password);
        }
    }
}
