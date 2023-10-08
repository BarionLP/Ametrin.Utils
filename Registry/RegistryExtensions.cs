namespace Ametrin.Utils.Registry; 

public static class RegistryExtensions {
    public static Result<TValue> TryGet<TValue>(this IRegistry<string, TValue> registry, ReadOnlySpan<char> spanKey) {
        foreach(var key in registry.Keys) {
            if(spanKey.SequenceEqual(key)) return registry[key];
        }
        return ResultStatus.Null;
    }

    public static Result TryRegister<TType>(this MutableTypeRegistry<string> registry) {
        var type = typeof(TType);
        if(type.FullName is not string name) throw new ArgumentException("Cannot register Type without name");
        return registry.TryRegister(name, type);
    }
}