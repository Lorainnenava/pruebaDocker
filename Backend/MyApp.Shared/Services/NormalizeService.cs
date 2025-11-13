using System.Globalization;
using System.Text;

namespace MyApp.Shared.Services
{
    public class NormalizeService
    {
        public static string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            // Quitar tildes y acentos
            string sinAcentos = text.Normalize(NormalizationForm.FormD);
            var chars = sinAcentos
                .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                .ToArray();

            // Unir caracteres, quitar espacios extremos y pasar a minúscula
            return new string(chars).Normalize(NormalizationForm.FormC)
                                    .Trim()
                                    .ToLowerInvariant();
        }

    }
}
