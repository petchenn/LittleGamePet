using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using Microsoft.Xna.Framework;

public class CellConverter : JsonConverter<Cell>
{
    public override Cell Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var color = root.GetProperty("Color").Deserialize<Color>(options);

            if (color == Color.Gray) return JsonSerializer.Deserialize<FloorCell>(root.GetRawText(), options);
            if (color == Color.Brown) return JsonSerializer.Deserialize<WallCell>(root.GetRawText(), options);
            if (color == Color.Red) return JsonSerializer.Deserialize<EnemyCell>(root.GetRawText(), options);
            if (color == Color.Green) return JsonSerializer.Deserialize<HealCell>(root.GetRawText(), options);

            // Fallback (например, если цвет белый)
            return JsonSerializer.Deserialize<FloorCell>(root.GetRawText(), options);
        }
    }

    public override void Write(Utf8JsonWriter writer, Cell value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}