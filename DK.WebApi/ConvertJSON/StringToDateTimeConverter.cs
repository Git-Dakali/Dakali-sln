using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DK.WebApi.ConvertAutoMapper
{
    public class StringToDateTimeConverter : JsonConverter<DateTime?>
    {
        public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            if (string.IsNullOrWhiteSpace(value))
                return null;

            var formatos = new[] { "dd-MM-yyyy", "dd-MM-yyyy HH:mm" };

            if (DateTime.TryParseExact(value, formatos, CultureInfo.InvariantCulture, DateTimeStyles.None, out var fecha))
                return fecha;

            throw new JsonException("Formato de fecha inválido.");
        }

        public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString("dd-MM-yyyy HH:mm"));
        }
    }
}
