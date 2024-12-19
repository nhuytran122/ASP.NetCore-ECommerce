namespace SV21T1020105.Shop
{
    public class DateTimeHelper
    {
        public static string FormatDateTime(DateTime? dateTime)
        {
            if (dateTime == null) return string.Empty;
            return string.Format(new System.Globalization.CultureInfo("en-GB"), "{0:dd/MM/yyyy HH:mm}", dateTime);
        }
    }
}
