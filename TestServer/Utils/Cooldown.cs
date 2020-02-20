using System;
using System.Collections.Generic;
using System.Text;
using TestServer.Manager;

namespace TestServer.Utils
{

    public enum DataType
    {
        Day,
        Month,
        Year,
        Hour,
        Minute,
        Second
    }

    public enum CooldownLanguage
    {
        PORTUGUESE,
        ENGLISH,
    }

    class Cooldown
    {

        private static InstanceManager<Cooldown> lazy = InstanceLazy.getOrCreate<Cooldown>("Cooldown");
        private static Dictionary<string, string> CLTexts;

        public static void LoadData()
        {
            CLTexts = new Dictionary<string, string>
            {
                //PORTUGUESE ( BRAZIL )
                {"PORTUGUESE_Year", "Ano" },
                {"PORTUGUESE_Years", "Anos" },
                {"PORTUGUESE_Month", "Mês" },
                {"PORTUGUESE_Months", "Meses" },
                {"PORTUGUESE_Day", "Dia" },
                {"PORTUGUESE_Days", "Dias" },

                {"PORTUGUESE_Hour", "Hora" },
                {"PORTUGUESE_Hours", "Horas" },
                {"PORTUGUESE_Minute", "Minuto" },
                {"PORTUGUESE_Minutes", "Minutos" },
                {"PORTUGUESE_Second", "Segundo" },
                {"PORTUGUESE_Seconds", "Segundos" },

                {"PORTUGUESE_And", "E" },
                {"PORTUGUESE_At", "Às" },

                //ENGLISH ( US )
                {"ENGLISH_Year", "Year" },
                {"ENGLISH_Years", "Years" },
                {"ENGLISH_Month", "Month" },
                {"ENGLISH_Months", "Months" },
                {"ENGLISH_Day", "Day" },
                {"ENGLISH_Days", "Days" },

                {"ENGLISH_Hour", "Hour" },
                {"ENGLISH_Hours", "Hours" },
                {"ENGLISH_Minute", "Minute" },
                {"ENGLISH_Minutes", "Minutes" },
                {"ENGLISH_Second", "Second" },
                {"ENGLISH_Seconds", "Seconds" },

                {"ENGLISH_And", "And" },
                {"ENGLISH_At", "At" },
            };
        }

        public static Cooldown getInstance() => lazy.getOrComputer(() => new Cooldown());

        public CooldownLanguage language { get; set; }

        private string Format(long value) => (value > 9L) ? $"{value}" : $"0{value}";

        public bool isTimeExpired(long time) => !(time - this.getCurrentTime() > 0L);

        public long getCurrentTime()
        {
            try
            {
                long _seconds = 0;
                DateTime _date = DateTime.Now;
                DateTime _gmt = TimeZoneInfo.ConvertTime(_date, TimeZoneInfo.Local);
                string _format = _gmt.ToString("HH:mm:ss:dd:MM:yyyy");
                string[] _data = _format.Split(":");

                _seconds += 3600L * long.Parse(_data[0]);
                _seconds += 60L * long.Parse(_data[1]);
                _seconds += long.Parse(_data[2]);
                _seconds += 86400L * long.Parse(_data[3]);
                _seconds += 2592000L * long.Parse(_data[4]);
                _seconds += 31104000L * long.Parse(_data[5]);

                return _seconds;
            }
            catch (Exception e) { return 0; }
        }

        public string getDate()
        {
            DateTime _date = DateTime.Now;
            DateTime _gmt = TimeZoneInfo.ConvertTime(_date, TimeZoneInfo.Local);
            return _gmt.ToString("dd/MM/yyyy Ç HH:mm:ss").Replace("Ç", Translate("At", true));
        }

        public string getDate(long time)
        {
            if (time == 0L)
                return $"00/00/00 {Translate("At", true)} 00:00:00";
            long years = time / 31104000L;
            time -= years * 31104000L;
            long months = time / 2592000L;
            time -= months * 2592000L;
            long days = time / 86400L;
            time -= days * 86400L;
            long hours = time / 3600L;
            time -= hours * 3600L;
            long minutes = time / 60L;
            time -= minutes * 60L;
            long seconds = time;
            return $"{Format(days)}/{Format(months)}/{Format(years)} {Translate("At", true)} {Format(hours)}:{Format(minutes)}:{Format(seconds)}";
        }

        public long getDataType(long time, DataType dt) {
            long _time = time;
            switch (dt)
            {
                case DataType.Year:
                    _time = 31104000L * time;
                    break;
                case DataType.Month:
                    _time = 2592000L * time;
                    break;
                case DataType.Day:
                    _time = 86400L * time;
                    break;
                case DataType.Hour:
                    _time = 3600L * time;
                    break;
                case DataType.Minute:
                    _time = 60L * time;
                    break;
                case DataType.Second:
                    _time = time;
                    break;
            }
            return _time;
        }

        public string getDateTimeString(long time)
        {
            if (time == 0L)
                return $"0 {Translate("Second", true)}";

            long years = time / 31104000L;
            time -= years * 31104000L;
            long months = time / 2592000L;
            time -= months * 2592000L;
            long days = time / 86400L;
            time -= days * 86400L;
            long hours = time / 3600L;
            time -= hours * 3600L;
            long minutes = time / 60L;
            long seconds;
            time = (seconds = time - minutes * 60L);

            StringBuilder sb = new StringBuilder();
            if (years > 0L)
                sb.Append(", ").Append(years).Append(" ").Append(Translate((years == 1L) ? "Year" : "Years", true));
            if (months > 0L)
                sb.Append(", ").Append(months).Append(" ").Append(Translate((years == 1L) ? "Month" : "Months", true));
            if (days > 0L)
                sb.Append(", ").Append(days).Append(" ").Append(Translate((years == 1L) ? "Day" : "Days", true));
            if (hours > 0L)
                sb.Append(", ").Append(hours).Append(" ").Append(Translate((years == 1L) ? "Hour" : "Hours", true));
            if (minutes > 0L)
                sb.Append($",  {Translate("And", true)} ").Append(minutes).Append(" ").Append(Translate((years == 1L) ? "Minute" : "Minutes", true));
            if (seconds > 0L)
                sb.Append($" {Translate("And", true)} ").Append(seconds).Append(" ").Append(Translate((years == 1L) ? "Second" : "Seconds", true));

            return ServerUtils.ReplaceFirst(ServerUtils.ReplaceFirst(sb.ToString(), ", ", ""), $" {Translate("And", true)} ", "");
        }

        private string Translate(string _type) => Translate(_type, false);
        private string Translate(string _type, bool lower)
        {
            string _text = CLTexts[$"{language.ToString()}_{_type}"];
            return (lower) ? _text.ToLower() : _text;
        }

    }
}
