namespace Ametrin.Utils.Optional;

[Flags] //for fails first bit must be 1
public enum ResultFlag {
    Succeeded           = 0b0000000000000000000000000000000,
    Failed              = 0b1000000000000000000000000000000,
    InvalidArgument     = 0b1000000000000000000000000000001, // a given argument contained an invalid value
    IOError             = 0b1000000000000000000000000000010, 
    WebError            = 0b1000000000000000000000000000100,
    Null                = 0b1000000000000000000000000001000, // a value was null or a collection empty
    ConnectionFailed    = 0b1000000000000000000000000010000,
    AlreadyExists       = 0b1000000000000000000000000100000,
    Canceled            = 0b1000000000000000000000001000000,
    OutOfRange          = 0b1000000000000000000000010000000,
    AccessDenied        = 0b1000000000000000000001000000000,
    InvalidType         = 0b1000000000000000000010000000000,
    DatabaseError       = 0b1000000000000000000100000000000,
    UnknownFormat       = 0b1000000000000000001000000000000,
    Impossible          = 0b1100000000000000000000000000000,
    PathNotFound        = IOError | Null,
    PathAlreadyExists   = IOError | AlreadyExists,
    NoInternet          = WebError | ConnectionFailed,
    InvalidFile         = IOError | InvalidArgument,
    UnknownFileFormat   = InvalidFile | UnknownFormat,
    DatabaseNotConnected= ConnectionFailed | DatabaseError,
}

public static class ResultFlagExtensions
{
    public static bool IsFail(this ResultFlag flag) => flag.HasFlag(ResultFlag.Failed);
    public static bool IsSuccess(this ResultFlag flag) => flag is ResultFlag.Succeeded;
}