namespace LibraryProject.Models
{
    static class SqlEscaper
    {
        public static string Escape(this string value)
        {
            return value.Replace("'", "''");
        }
    }
}
