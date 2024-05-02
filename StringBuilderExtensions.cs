using System.Text;

namespace Ametrin.Utils; 

public static class StringBuilderExtensions {
    public static StringBuilder AppendRepeated(this StringBuilder builder, string value, int count) {
        for(int i = 0; i < count; i++) {
            builder.Append(value);
        }
        return builder;
    }
    
    public static StringBuilder AppendRepeated(this StringBuilder builder, char value, int count) {
        for(int i = 0; i < count; i++) {
            builder.Append(value);
        }
        return builder;
    }
}
