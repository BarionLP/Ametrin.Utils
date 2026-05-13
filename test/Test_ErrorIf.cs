using Ametrin.Guards;
using Ametrin.Optional;

namespace Ametrin.Utils.Test;

public sealed class Test_ErrorIf
{
    [Test]
    public async Task Test_Empty_Collection()
    {
        string[] array = [""];
        IEnumerable<string> enumerable = ["value"];

        await Assert.That(ErrorIf.Empty(array)).IsSuccess(r => ReferenceEquals(r, array));
        await Assert.That(ErrorIf.Empty(Result.Of(enumerable))).IsSuccess(r => ReferenceEquals(r, enumerable));

        array = [];
        enumerable = [];

        await Assert.That(ErrorIf.Empty(array)).IsErrorOfType<string[], ArgumentException>();
        await Assert.That(ErrorIf.Empty(Result.Of(enumerable))).IsErrorOfType<IEnumerable<string>, ArgumentException>();

        await Assert.That(ErrorIf.Empty<string[]>(new NullReferenceException())).IsErrorOfType<string[], NullReferenceException>();
    }

    [Test]
    public async Task Test_Empty_String()
    {
        await Assert.That(ErrorIf.Empty("value")).IsSuccess();
        await Assert.That(ErrorIf.Empty(" ")).IsSuccess();
        await Assert.That(ErrorIf.Empty("")).IsErrorOfType<string, ArgumentException>();
        await Assert.That(ErrorIf.Empty(new NullReferenceException())).IsErrorOfType<string, NullReferenceException>();
    }

    [Test]
    public async Task Test_WhiteSpace_String()
    {
        await Assert.That(ErrorIf.WhiteSpace("value")).IsSuccess();
        await Assert.That(ErrorIf.WhiteSpace(" ")).IsErrorOfType<string, ArgumentException>();
        await Assert.That(ErrorIf.WhiteSpace("  ")).IsErrorOfType<string, ArgumentException>();
        await Assert.That(ErrorIf.WhiteSpace("")).IsErrorOfType<string, ArgumentException>();
        await Assert.That(ErrorIf.WhiteSpace(new NullReferenceException())).IsErrorOfType<string, NullReferenceException>();
    }

    [Test]
    public async Task Test_InRange()
    {
        await Assert.That(ErrorIf.InRange(0, 1, 5)).IsSuccess(0);
        await Assert.That(ErrorIf.InRange(5, 1, 5)).IsSuccess(5);
        await Assert.That(ErrorIf.InRange(6, 1, 5)).IsSuccess(6);
        await Assert.That(ErrorIf.InRange(1, 1, 5)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.InRange(4, 1, 5)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.InRange(new NullReferenceException(), 1, 5)).IsErrorOfType<int, NullReferenceException>();
    }

    [Test]
    public async Task Test_OutOfRange()
    {
        await Assert.That(ErrorIf.OutOfRange(1, 1, 5)).IsSuccess(1);
        await Assert.That(ErrorIf.OutOfRange(4, 1, 5)).IsSuccess(4);
        await Assert.That(ErrorIf.OutOfRange(0, 1, 5)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.OutOfRange(5, 1, 5)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.OutOfRange(6, 1, 5)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.OutOfRange(new NullReferenceException(), 1, 5)).IsErrorOfType<int, NullReferenceException>();
    }

    [Test]
    public async Task Test_LessThan()
    {
        await Assert.That(ErrorIf.LessThan(1, 1)).IsSuccess(1);
        await Assert.That(ErrorIf.LessThan(int.MaxValue, 1)).IsSuccess(int.MaxValue);
        await Assert.That(ErrorIf.LessThan(0, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.LessThan(int.MinValue, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.LessThan(new NullReferenceException(), 1)).IsErrorOfType<int, NullReferenceException>();
    }

    [Test]
    public async Task Test_LessThanOrEqual()
    {
        await Assert.That(ErrorIf.LessThanOrEqual(2, 1)).IsSuccess(2);
        await Assert.That(ErrorIf.LessThanOrEqual(int.MaxValue, 1)).IsSuccess(int.MaxValue);
        await Assert.That(ErrorIf.LessThanOrEqual(1, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.LessThanOrEqual(0, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.LessThanOrEqual(int.MinValue, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.LessThanOrEqual(new NullReferenceException(), 1)).IsErrorOfType<int, NullReferenceException>();
    }

    [Test]
    public async Task Test_GreaterThan()
    {
        await Assert.That(ErrorIf.GreaterThan(0, 1)).IsSuccess(0);
        await Assert.That(ErrorIf.GreaterThan(1, 1)).IsSuccess(1);
        await Assert.That(ErrorIf.GreaterThan(int.MinValue, 1)).IsSuccess(int.MinValue);
        await Assert.That(ErrorIf.GreaterThan(2, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.GreaterThan(int.MaxValue, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.GreaterThan(new NullReferenceException(), 1)).IsErrorOfType<int, NullReferenceException>();
    }

    [Test]
    public async Task Test_GreaterThanOrEqual()
    {
        await Assert.That(ErrorIf.GreaterThanOrEqual(0, 1)).IsSuccess(0);
        await Assert.That(ErrorIf.GreaterThanOrEqual(int.MinValue, 1)).IsSuccess(int.MinValue);
        await Assert.That(ErrorIf.GreaterThanOrEqual(1, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.GreaterThanOrEqual(2, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.GreaterThanOrEqual(int.MaxValue, 1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.GreaterThanOrEqual(new NullReferenceException(), 1)).IsErrorOfType<int, NullReferenceException>();
    }

    [Test]
    public async Task Test_Positive()
    {
        await Assert.That(ErrorIf.Positive(0)).IsSuccess(0);
        await Assert.That(ErrorIf.Positive(int.MinValue)).IsSuccess(int.MinValue);
        await Assert.That(ErrorIf.Positive(1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Positive(2)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Positive(int.MaxValue)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Positive<int>(new NullReferenceException())).IsErrorOfType<int, NullReferenceException>();
        
        await Assert.That(ErrorIf.Positive(0.0)).IsSuccess(0.0);
        await Assert.That(ErrorIf.Positive(-0.0)).IsSuccess(-0.0);
    }

    [Test]
    public async Task Test_Zero()
    {
        await Assert.That(ErrorIf.Zero(1)).IsSuccess(1);
        await Assert.That(ErrorIf.Zero(-1)).IsSuccess(-1);
        await Assert.That(ErrorIf.Zero(int.MinValue)).IsSuccess(int.MinValue);
        await Assert.That(ErrorIf.Zero(int.MaxValue)).IsSuccess(int.MaxValue);
        await Assert.That(ErrorIf.Zero(0)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Zero<int>(new NullReferenceException())).IsErrorOfType<int, NullReferenceException>();

        await Assert.That(ErrorIf.Zero(0.0)).IsErrorOfType<double, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Zero(-0.0)).IsErrorOfType<double, ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Test_Negative()
    {
        await Assert.That(ErrorIf.Negative(0)).IsSuccess(0);
        await Assert.That(ErrorIf.Negative(1)).IsSuccess(1);
        await Assert.That(ErrorIf.Negative(int.MaxValue)).IsSuccess(int.MaxValue);
        await Assert.That(ErrorIf.Negative(-1)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Negative(int.MinValue)).IsErrorOfType<int, ArgumentOutOfRangeException>();
        await Assert.That(ErrorIf.Negative<int>(new NullReferenceException())).IsErrorOfType<int, NullReferenceException>();
        
        await Assert.That(ErrorIf.Negative(0.0)).IsSuccess(0.0);
        await Assert.That(ErrorIf.Negative(-0.0)).IsErrorOfType<double, ArgumentOutOfRangeException>();
    }
}
