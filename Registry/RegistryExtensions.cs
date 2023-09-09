namespace Ametrin.Utils.Registry; 

public static class RegistryExtensions {
    public static Result<TValue> TryGet<TValue>(this IRegistry<string, TValue> registry, ReadOnlySpan<char> spanKey) {
        foreach(var key in registry.Keys) {
            if(spanKey.SequenceEqual(key)) return registry[key];
        }
        return ResultStatus.Null;
    }
}