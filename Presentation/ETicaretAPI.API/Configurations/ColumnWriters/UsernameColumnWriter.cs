using NpgsqlTypes;
using Serilog.Events;
using Serilog.Sinks.PostgreSQL.ColumnWriters;

namespace ETicaretAPI.API.Configurations.ColumnWriters
{
    public class UsernameColumnWriter : ColumnWriterBase
    {
        public UsernameColumnWriter(): base(NpgsqlDbType.Varchar)
        {
        }

        public override object GetValue(LogEvent logEvent, IFormatProvider formatProvider = null)
        {
            var(username, value) = logEvent.Properties.FirstOrDefault(p => p.Key =="user_name");//key değeri user_name olan property varsa yakala ve onu username key ine karşılık value olarak kaydet
            return value?.ToString() ?? null;
        }
    }
}
