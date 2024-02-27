namespace Ametrin.Utils.Registry;
public sealed class EnumRegistry<TEnum> : Registry<string, TEnum> where TEnum : Enum {
    public EnumRegistry(bool toLowerCase = true) 
        : base(Enum.GetValues(typeof(TEnum)).Cast<TEnum>(), val => toLowerCase ? val.ToString().ToLower() : val.ToString()) { }
}