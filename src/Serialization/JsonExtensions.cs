using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization.Metadata;
using System.Threading;
using System.Threading.Tasks;

namespace Ametrin.Serialization;

#if NET10_0_OR_GREATER
[Obsolete]
#endif

public static class JsonExtensions
{
    public static readonly JsonSerializerOptions DefaultOptions = new(JsonSerializerOptions.Default) { WriteIndented = true, IncludeFields = true };

    static JsonExtensions()
    {
        DefaultOptions.Converters.Add(new DirectoryInfoJsonConverter());
        DefaultOptions.Converters.Add(new FileInfoJsonConverter());
    }

    public static string ConvertToJson<T>(this T data, JsonSerializerOptions? options = null) => JsonSerializer.Serialize(data, options ?? DefaultOptions);
    public static JsonElement ConvertToJsonElement<T>(this T data, JsonSerializerOptions? options = null) => JsonSerializer.SerializeToElement(data, data!.GetType(), options ?? DefaultOptions);

    public static Task<string> ConvertToJsonAsync<T>(this T data, JsonSerializerOptions? options = null) => Task.Run(() => data.ConvertToJson(options));
    public static Task<JsonElement> ConvertToJsonElementAsync<T>(this T data, JsonSerializerOptions? options = null) => Task.Run(() => data.ConvertToJsonElement(options));


    public static void WriteToJsonFile<T>(this T data, string path, JsonSerializerOptions? options = null)
    {
        File.WriteAllText(path, data.ConvertToJson(options));
    }
    public static void WriteToStreamAsJson<T>(this T data, Stream stream, JsonSerializerOptions? options = null)
    {
        JsonSerializer.Serialize(stream, data, options ?? DefaultOptions);
    }
    public static Task WriteToJsonFileAsync<T>(this T data, string path, JsonSerializerOptions? options = null) => data.WriteToJsonFileAsync(path, options);

    public static Result<T> ReadFromJsonFile<T>(string path, JsonSerializerOptions? options = null)
    {
        using var stream = File.OpenRead(path);
        return Deserialize<T>(stream, options ?? DefaultOptions);
    }
    public static Task<Result<T>> ReadFromJsonFileAsync<T>(string path, JsonSerializerOptions? options = null) => Task.Run(() => ReadFromJsonFile<T>(path, options));

    public static void WriteToJsonFile<T>(this T data, FileInfo fileInfo, JsonSerializerOptions? options = null)
    {
        using var stream = fileInfo.Create();
        JsonSerializer.Serialize(stream, data, options ?? DefaultOptions);
    }
    public static void WriteToJsonFile<T>(this T data, FileInfo fileInfo, JsonTypeInfo<T> typeInfo)
    {
        using var stream = fileInfo.Create();
        JsonSerializer.Serialize(stream, data, typeInfo);
    }
    public static Task WriteToJsonFileAsync<T>(this T data, FileInfo fileInfo, JsonSerializerOptions? options = null) => Task.Run(() => data.WriteToJsonFile(fileInfo, options));

    public static Result<T> ReadFromJsonFile<T>(this FileInfo fileInfo, JsonSerializerOptions? options = null)
    {
        if (!fileInfo.Exists)
        {
            return new FileNotFoundException(null, fileInfo.FullName);
        }
        using var stream = fileInfo.OpenRead();
        return Deserialize<T>(stream, options ?? DefaultOptions);
    }

    public static Result<T> ReadFromJsonFile<T>(this FileInfo fileInfo, JsonTypeInfo<T> typeInfo, JsonSerializerOptions? options = null)
    {
        if (!fileInfo.Exists)
        {
            return new FileNotFoundException(null, fileInfo.FullName);
        }
        using var stream = fileInfo.OpenRead();
        return Deserialize<T>(stream, options ?? DefaultOptions);
    }
    public static Result<T> Deserialize<T>(Stream stream, JsonSerializerOptions? options = null)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(stream, options ?? DefaultOptions) ?? Result.Error<T>(new NullReferenceException());
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Result<T> Deserialize<T>(Stream stream, JsonTypeInfo<T> typeInfo)
    {
        try
        {
            return JsonSerializer.Deserialize(stream, typeInfo) ?? Result.Error<T>(new NullReferenceException());
        }
        catch (Exception e)
        {
            return e;
        }
    }

    public static Task<Result<T>> ReadFromJsonFileAsync<T>(this FileInfo fileInfo, JsonSerializerOptions? options = null) => Task.Run(() => ReadFromJsonFile<T>(fileInfo, options));
}

#if NET10_0_OR_GREATER

public static class JsonSerializerExtensions
{
    extension(JsonSerializer)
    {
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

        public static Task SerializeToFileAsync<T>(FileInfo fileInfo, T value, JsonSerializerOptions? options = null, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (!overwrite && fileInfo.Exists) throw new IOException($"{fileInfo} already exists");

            using var stream = fileInfo.Create();
            return JsonSerializer.SerializeAsync(stream, value, options, cancellationToken);
        }

        public static Task SerializeToFileAsync<T>(FileInfo fileInfo, T value, JsonTypeInfo<T> jsonTypeInfo, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (!overwrite && fileInfo.Exists) throw new IOException($"{fileInfo} already exists");

            using var stream = fileInfo.Create();
            return JsonSerializer.SerializeAsync(stream, value, jsonTypeInfo, cancellationToken);
        }


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

        public async static Task<T> DeserializeAsync<T>(FileInfo fileInfo, JsonSerializerOptions? options = null, CancellationToken cancellationToken = default)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return (await JsonSerializer.DeserializeAsync<T>(stream, options, cancellationToken)) ?? throw new Exception();
        }

        public async static Task<T> DeserializeAsync<T>(FileInfo fileInfo, JsonTypeInfo<T> jsonTypeInfo, CancellationToken cancellationToken = default)
        {
            FileNotFoundException.ExistsOrThrow(fileInfo);

            using var stream = fileInfo.OpenRead();
            return (await JsonSerializer.DeserializeAsync<T>(stream, jsonTypeInfo, cancellationToken)) ?? throw new Exception();
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
}

#endif