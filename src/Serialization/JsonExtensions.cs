using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ametrin.Serialization;

public static class JsonExtensions
{
    public static readonly JsonSerializerOptions DefaultOptions = new() { WriteIndented = true, IncludeFields = true };
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
    public static Task WriteToJsonFileAsync<T>(this T data, string path, JsonSerializerOptions? options = null) => Task.Run(() => data.WriteToJsonFile(path, options));

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
    public static Result<T> Deserialize<T>(Stream stream, JsonSerializerOptions? options = null)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(stream, options ?? DefaultOptions) ?? Result.Error<T>(new NullReferenceException());
        }
        catch(Exception e)
        {
            return e;   
        }
    }

    public static Task<Result<T>> ReadFromJsonFileAsync<T>(this FileInfo fileInfo, JsonSerializerOptions? options = null) => Task.Run(() => ReadFromJsonFile<T>(fileInfo, options));
}
