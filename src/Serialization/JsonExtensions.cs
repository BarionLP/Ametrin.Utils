using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Ametrin.Serialization;

public static class JsonSerializerExtensions
{
    public const string SerializationUnreferencedCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed. Use the overload that takes a JsonTypeInfo or JsonSerializerContext, or make sure all of the required types are preserved.";
    public const string SerializationRequiresDynamicCodeMessage = "JSON serialization and deserialization might require types that cannot be statically analyzed and might need runtime code generation. Use System.Text.Json source generation for native AOT applications.";

    public static JsonSerializerOptions DefaultOptions
    {
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        get => field ??= new(JsonSerializerOptions.Default) { WriteIndented = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip, Converters = { new DirectoryInfoJsonConverter(), new FileInfoJsonConverter() } };
    }

    extension(JsonSerializer)
    {
        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static void SerializeToFile<T>(FileInfo fileInfo, T value, JsonSerializerOptions? options = null, bool overwrite = false)
        {
            if (!overwrite && fileInfo.Exists) throw new IOException($"{fileInfo} already exists");

            using var stream = fileInfo.Create();
            JsonSerializer.Serialize(stream, value, options);
        }

        public static void SerializeToFile<T>(FileInfo fileInfo, T value, JsonTypeInfo<T> jsonTypeInfo, bool overwrite = false)
        {
            if (!overwrite && fileInfo.Exists) throw new IOException($"{fileInfo} already exists");

            using var stream = fileInfo.Create();
            JsonSerializer.Serialize(stream, value, jsonTypeInfo);
        }

        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static async Task SerializeToFileAsync<T>(FileInfo fileInfo, T value, JsonSerializerOptions? options = null, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (!overwrite && fileInfo.Exists) throw new IOException($"{fileInfo} already exists");

            using var stream = fileInfo.Create();
            await JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);
            // we need to await because the stream has to be closed afterwards
        }

        public static async Task SerializeToFileAsync<T>(FileInfo fileInfo, T value, JsonTypeInfo<T> jsonTypeInfo, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (!overwrite && fileInfo.Exists) throw new IOException($"{fileInfo} already exists");

            using var stream = fileInfo.Create();
            await JsonSerializer.SerializeAsync(stream, value, jsonTypeInfo, cancellationToken);
            // we need to await because the stream has to be closed afterwards
        }

        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public static T Deserialize<T>(FileInfo fileInfo, JsonSerializerOptions? options = null)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return JsonSerializer.Deserialize<T>(stream, options) ?? throw new JsonException();
        }

        public static T Deserialize<T>(FileInfo fileInfo, JsonTypeInfo<T> jsonTypeInfo)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return JsonSerializer.Deserialize(stream, jsonTypeInfo) ?? throw new JsonException();
        }

        [RequiresUnreferencedCode(SerializationUnreferencedCodeMessage)]
        [RequiresDynamicCode(SerializationRequiresDynamicCodeMessage)]
        public async static Task<T> DeserializeAsync<T>(FileInfo fileInfo, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return (await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken)) ?? throw new JsonException();
        }

        public async static Task<T> DeserializeAsync<T>(FileInfo fileInfo, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return (await JsonSerializer.DeserializeAsync(stream, jsonTypeInfo, cancellationToken)) ?? throw new JsonException();
        }
    }

    extension(JsonNode)
    {
        public static JsonNode Parse(FileInfo fileInfo, JsonNodeOptions? nodeOptions = null, JsonDocumentOptions documentOptions = default)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return JsonNode.Parse(stream, nodeOptions, documentOptions) ?? throw new JsonException();
        }

        public static async Task<JsonNode> ParseAsync(FileInfo fileInfo, JsonNodeOptions? nodeOptions = null, JsonDocumentOptions documentOptions = default)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return (await JsonNode.ParseAsync(stream, nodeOptions, documentOptions)) ?? throw new JsonException();
        }
    }

    extension(JsonSerializerOptions options)
    {
        public string GetPropertyName(string logicalName)
            => options.PropertyNamingPolicy?.ConvertName(logicalName) ?? logicalName;

        public StringComparison StringComparison() => options.PropertyNameCaseInsensitive ? System.StringComparison.OrdinalIgnoreCase : System.StringComparison.Ordinal;
    }

    extension(ref Utf8JsonReader reader)
    {
        [StackTraceHidden]
        public void HandleUnmappedMember(JsonSerializerOptions options)
        {
            switch (options.UnmappedMemberHandling)
            {
                case JsonUnmappedMemberHandling.Skip:
                    reader.Skip();
                    break;

                case JsonUnmappedMemberHandling.Disallow:
                    throw new JsonException();

                default:
                    throw new UnreachableException();
            }
        }
    }
}