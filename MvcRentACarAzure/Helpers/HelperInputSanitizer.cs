using System.Globalization;

namespace MvcRentACarAzure.Helpers
{
    public static class HelperInputSanitizer
    {
        public static decimal SanitizeDecimalInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input cannot be null or empty", nameof(input));
            }

            // Replace comma with dot
            string sanitizedInput = input.Replace(',', '.');

            if (decimal.TryParse(sanitizedInput, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal result))
            {
                return result;
            }
            else
            {
                throw new FormatException("Invalid decimal format");
            }
        }
    }
}
