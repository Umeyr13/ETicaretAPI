
namespace ETicaretAPI.Infrastructure.Operations
{
    public static class NameOperation
    {
        public static string CharacterRegulatory(string name)
        =>
            name.Replace(" ", "")
            .Replace("ı", "i")
            .Replace("İ", "I")
            .Replace("ğ", "g")
            .Replace("Ğ", "G")
            .Replace("ü", "u")
            .Replace("Ü", "U")
            .Replace("ş", "s")
            .Replace("Ş", "S")
            .Replace("ö", "o")
            .Replace("Ö", "O")
            .Replace("ç", "c")
            .Replace("Ç", "C")
            .Replace("/", "")
            .Replace("!", "")
            .Replace("'", "")
            .Replace("^", "")
            .Replace("+", "")
            .Replace("%", "")
            .Replace("-", "")
            .Replace("!", "")
            .Replace("&", "")
            .Replace("*", "")
            .Replace("(", "")
            .Replace(")", "")
            .Replace("[", "")
            .Replace("]", "")
            .Replace("{", "")
            .Replace("}", "")
            .Replace("=", "")
            .Replace("<", "")
            .Replace(">", "")
            .Replace("?", "")
            .Replace("|", "")
            .Replace("`", "")
            .Replace("~", "")
            .Replace("@", "")
            .Replace("#", "")
            .Replace("$", "")
            .Replace(";", "")
            .Replace(":", "")
            .Replace(",", "")
            .Replace(".", "")
            .Replace("_", "");         
     
    }
}
