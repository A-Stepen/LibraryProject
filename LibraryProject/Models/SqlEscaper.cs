namespace LibraryProject.Models
{
    static class SqlEscaper
    {
        public static string Escape(this string value)
        {
            return value.Replace("'", "''");
        }

        public static string PreprocessFilter(this string value)
        {
            return value != null && value.Length > 0 ? string.Format("%{0}%", value.Escape()) : "%";
        }
    }
}
