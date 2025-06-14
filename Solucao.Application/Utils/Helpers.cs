using System;
namespace Solucao.Application.Utils
{
    public static class Helpers
    {
        public static DateTime DateTimeNow()
        {
            DateTime dateTime = DateTime.UtcNow;
            TimeZoneInfo brasiliaTime = TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time");
            return TimeZoneInfo.ConvertTimeFromUtc(dateTime, brasiliaTime);
        }

        public static string FormatTime(decimal houras)
        {
            int totalMinutos = (int)(houras * 60);
            int horasInteiras = totalMinutos / 60;
            int minutos = totalMinutos % 60;

            string tempoFormatado = string.Format("{0:D2}:{1:D2}", horasInteiras, minutos);

            return tempoFormatado;
        }

        public static double RentalTime(string startTime, string endTime)
        {
            startTime = startTime.Replace(":", "");
            endTime = endTime.Replace(":", "");
            var now = DateTime.Now;

            var _startTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(startTime.Substring(0, 2)), int.Parse(startTime.Substring(2)), 0);
            var _endTime = new DateTime(now.Year, now.Month, now.Day, int.Parse(endTime.Substring(0, 2)), int.Parse(endTime.Substring(2)), 0);

            TimeSpan difference = _endTime - _startTime;

            return difference.TotalHours;
        }
    }
}

