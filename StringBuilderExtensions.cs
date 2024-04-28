using System.Text;

namespace Ametrin.Utils; 

public static class StringBuilderExtensions {
    public static StringBuilder AppendCollection<T>(this StringBuilder sb, IEnumerable<T> values, char separator) => sb.AppendCollection(values.Select(v => v?.ToString() ?? "Null"), separator);
    public static StringBuilder AppendCollection<T>(this StringBuilder sb, IEnumerable<T> values, string separator) => sb.AppendCollection(values.Select(v => v?.ToString() ?? "Null"), separator);
    public static StringBuilder AppendCollection(this StringBuilder sb, IEnumerable<string> values, char separator) {
        if(values.Any()) {
            sb.Append(values.First());
            foreach(var element in values.Skip(1)) {
                sb.Append(separator);
                sb.Append(element);
            }
        }

        return sb;
    }

    public static StringBuilder AppendCollection(this StringBuilder sb, IEnumerable<string> values, string separator) {
        if(values.Any()) {
            sb.Append(values.First());
            foreach(var element in values.Skip(1)) {
                sb.Append(separator);
                sb.Append(element);
            }
        }

        return sb;
    }
}
