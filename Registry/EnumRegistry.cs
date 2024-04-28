namespace Ametrin.Utils.Registry;
public sealed class EnumRegistry<TEnum>(bool toLowerCase = true) 
    : Registry<string, TEnum>(Enum.GetValues(typeof(TEnum)).Cast<TEnum>(), val => toLowerCase ? val.ToString().ToLower() : val.ToString()) where TEnum : Enum {
}