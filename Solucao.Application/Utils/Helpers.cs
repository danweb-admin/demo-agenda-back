using System;
using System.Text.RegularExpressions;

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


        public static bool TryFormatarTelefone(string telefone, out string telefoneFormatado)
        {
            telefoneFormatado = null;

            if (string.IsNullOrWhiteSpace(telefone))
                return false;

            // Remove tudo que não for número
            var numeros = Regex.Replace(telefone, @"\D", "");

            // Remove código do país se vier (55)
            if (numeros.StartsWith("55"))
                numeros = numeros.Substring(2);

            // Deve ter entre 10 e 11 dígitos (DDD + número)
            if (numeros.Length < 10 || numeros.Length > 11)
                return false;

            var ddd = numeros.Substring(0, 2);
            var numero = numeros.Substring(2);

            // Se tiver 8 dígitos (fixo), opcional: rejeitar ou aceitar
            if (numero.Length == 8)
            {
                // você pode bloquear fixo se quiser:
                // return false;
            }

            // Se tiver 9 dígitos (celular), validar início com 9
            if (numero.Length == 9 && !numero.StartsWith("9"))
                return false;

            // Monta padrão final (55 + DDD + número)
            telefoneFormatado = $"55{ddd}{numero}";

            return true;
        }

    }
}

