namespace Ametrin.Utils; 

public interface IEmpty<out T> {
    public abstract static T Empty { get; }
}
