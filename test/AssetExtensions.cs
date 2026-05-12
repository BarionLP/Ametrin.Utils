namespace Ametrin.Utils.Test;

internal static class AssetExtensions
{
    extension(Assert)
    {
        public static RaisedEvent<T> Raises<T>(Action<EventHandler<T>> attach, Action<EventHandler<T>> detach, Action action)
        {
            RaisedEvent<T>? result = null;
            attach(Handler);
            try
            {
                action();
            }
            finally
            {
                detach(Handler);
            }

            if (result is null)
            {
                throw new TUnit.Assertions.Exceptions.AssertionException("Expected event of type " + typeof(T).Name + " to be raised but it was not");
            }

            return result;
            void Handler(object? sender, T args)
            {
                if (result is not null) throw new TUnit.Assertions.Exceptions.AssertionException("Duplicate event thrown.");
                result = new RaisedEvent<T>(sender, args);
            }
        }
        
        public static void NotRaises<T>(Action<EventHandler<T>> attach, Action<EventHandler<T>> detach, Action action)
        {
            attach(Handler);
            try
            {
                action();
            }
            finally
            {
                detach(Handler);
            }

            static void Handler(object? sender, T args)
            {
                throw new TUnit.Assertions.Exceptions.AssertionException($"Expected no event to be thrown but. {args}");
            }
        }
    }
}

public sealed class RaisedEvent<T>(object? sender, T arguments)
{
    public object? Sender { get; } = sender;
    public T Arguments { get; } = arguments;
}
