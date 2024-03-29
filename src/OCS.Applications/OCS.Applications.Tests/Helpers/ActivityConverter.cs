using System.Text.Json;
using System.Text.Json.Serialization;
using EnumFastToStringGenerated;
using OCS.Applications.Domain.Entitites;

namespace OCS.Applications.Tests.Helpers;

/// <summary>
/// Кастомный конвертер для <see cref="Activity"/>
/// </summary>
/// <remarks>System.Text.Json не умеет парсить из строки nullable тип :clown_face:</remarks>
public class ActivityConverter : JsonConverter<Activity>
{
    public override Activity Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException();
        }
        var activityString = reader.GetString();
        if (ActivityEnumExtensions.TryParseFast(activityString, out var activity))
        {
            return activity;
        }
        throw new JsonException();
    }

    public override void Write(Utf8JsonWriter writer, Activity value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToStringFast());
    }
}