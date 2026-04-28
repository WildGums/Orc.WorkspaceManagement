namespace Orc.WorkspaceManagement.Serialization;

using System;
using System.Text.Json;
using System.Text.Json.Serialization;

internal class WorkspaceJsonConverter : JsonConverter<Workspace>
{
    public override Workspace? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var workspace = new Workspace();

        while (reader.Read())
        {
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                continue;
            }

            var propertyName = reader.GetString();

            reader.Read();

            if (propertyName == "metadata")
            {
                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType != JsonTokenType.PropertyName)
                    {
                        continue;
                    }

                    var metadataPropertyName = reader.GetString();
                    reader.Read();

                    switch (metadataPropertyName)
                    {
                        case "title":
                            workspace.Title = reader.GetString() ?? string.Empty;
                            break;

                        case "displayName":
                            workspace.DisplayName = reader.GetString();
                            break;

                        case "workspaceGroup":
                            workspace.WorkspaceGroup = reader.GetString();
                            break;
                    }
                    break;
                }

                continue;
            }

            switch (reader.TokenType)
            {
                case JsonTokenType.Null:
                    workspace.SetWorkspaceValue(propertyName!, null);
                    continue;

                case JsonTokenType.Number:
                    if (reader.TryGetDouble(out var doubleValue))
                    {
                        workspace.SetWorkspaceValue(propertyName!, doubleValue);
                    }
                    continue;

                case JsonTokenType.True:
                case JsonTokenType.False:
                    var boolValue = reader.GetBoolean();
                    workspace.SetWorkspaceValue(propertyName!, boolValue);
                    continue;

                default:
                    var stringValue = reader.GetString();
                    workspace.SetWorkspaceValue(propertyName!, stringValue);
                    continue;
            }
        }

        return workspace;
    }

    public override void Write(Utf8JsonWriter writer, Workspace value, JsonSerializerOptions options)
    {
        var properties = value.GetAllWorkspaceValueNames();

        writer.WriteStartObject();

        writer.WritePropertyName("metadata");

        writer.WriteStartObject();

        writer.WritePropertyName("title");
        writer.WriteStringValue(value.Title);

        writer.WritePropertyName("displayName");
        writer.WriteStringValue(value.DisplayName);

        writer.WritePropertyName("workspaceGroup");
        writer.WriteStringValue(value.WorkspaceGroup);

        writer.WriteEndObject();

        foreach (var property in properties)
        {
            writer.WritePropertyName(property);

            var propertyValue = value.GetWorkspaceValue<object?>(property, null);
            if (propertyValue is null)
            {
                writer.WriteNullValue();
            }
            else if (propertyValue is bool booleanValue)
            {
                writer.WriteBooleanValue(booleanValue);
            }
            else if (propertyValue is int intValue)
            {
                writer.WriteNumberValue(intValue);
            }
            else if (propertyValue is long longValue)
            {
                writer.WriteNumberValue(longValue);
            }
            else if (propertyValue is double doubleValue)
            {
                writer.WriteNumberValue(doubleValue);
            }
            else if (propertyValue is string stringValue)
            {
                writer.WriteStringValue(stringValue);
            }
            else
            {
                writer.WriteStringValue(propertyValue.ToString());
            }
        }

        writer.WriteEndObject();
    }
}
