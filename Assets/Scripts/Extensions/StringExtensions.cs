public static class StringExtensions
{
    public static string Translate(this string key)
    {
        return LocalizationManager.Translate(key);
    }
}
