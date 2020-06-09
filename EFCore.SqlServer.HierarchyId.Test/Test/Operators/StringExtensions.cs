namespace Microsoft.EntityFrameworkCore.Test.Operators
{
    public static class StringExtensions
    {
        public static string QuoteWith(this string s, string value)
            => $"{value}{s}{value}";

        public static string Stringify(this object o)
            => o is null ? "null" : o.ToString();
    }
}
