using Ametrin.Guards;

namespace Ametrin.Utils.Test;

public sealed class Test_ThrowIf
{
    [Test]
    public async Task Test_Null()
    {
        await Assert.That(() => ThrowIf.Null((string?)null)).Throws<ArgumentNullException>();
        await Assert.That(() => ThrowIf.Null(new int?())).Throws<ArgumentNullException>();
        await Assert.That(() => ThrowIf.Null(new int?(1))).IsEqualTo(1);
        await Assert.That(() => ThrowIf.Null("")).IsEqualTo("");
    }

    [Test]
    public async Task Test_Not()
    {
        await Assert.That(() => ThrowIf.Not<B>(new A())).Throws<ArgumentException>();
        await Assert.That(() => ThrowIf.Not<A>(new A())).ThrowsNothing();
        await Assert.That(() => ThrowIf.Not<A>(new B())).ThrowsNothing();
        await Assert.That(() => ThrowIf.Not<A>(new C())).Throws<ArgumentException>();
        await Assert.That(() => ThrowIf.Not<A>(null)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Test_NullOrEmpty_Collection()
    {
        await Assert.That(() => ThrowIf.NullOrEmpty<int[]>(null)).Throws<ArgumentNullException>();
        await Assert.That(() => ThrowIf.NullOrEmpty<int[]>([])).Throws<ArgumentException>();
        await Assert.That(() => ThrowIf.NullOrEmpty<int[]>([1])).ThrowsNothing();
    }

    [Test]
    public async Task Test_NullOrEmpty_String()
    {
        await Assert.That(() => ThrowIf.NullOrEmpty(null)).Throws<ArgumentNullException>();
        await Assert.That(() => ThrowIf.NullOrEmpty("")).Throws<ArgumentException>();
        await Assert.That(() => ThrowIf.NullOrEmpty(" ")).ThrowsNothing();
    }

    [Test]
    public async Task Test_NullOrWhiteSpace()
    {
        await Assert.That(() => ThrowIf.NullOrWhiteSpace(null)).Throws<ArgumentNullException>();
        await Assert.That(() => ThrowIf.NullOrWhiteSpace("")).Throws<ArgumentException>();
        await Assert.That(() => ThrowIf.NullOrWhiteSpace("   ")).Throws<ArgumentException>();
        await Assert.That(() => ThrowIf.NullOrWhiteSpace("hello world")).ThrowsNothing();
    }

    [Test]
    public async Task Test_OutOfRange()
    {
        await Assert.That(() => ThrowIf.OutOfRange(0, 0, 2)).ThrowsNothing();
        await Assert.That(() => ThrowIf.OutOfRange(1, 0, 2)).ThrowsNothing();
        await Assert.That(() => ThrowIf.OutOfRange(2, 0, 2)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => ThrowIf.OutOfRange(3, 0, 2)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => ThrowIf.OutOfRange(-1, 0, 2)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Test_InRange()
    {
        await Assert.That(() => ThrowIf.InRange(0, 0, 2)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => ThrowIf.InRange(1, 0, 2)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => ThrowIf.InRange(2, 0, 2)).ThrowsNothing();
        await Assert.That(() => ThrowIf.InRange(3, 0, 2)).ThrowsNothing();
        await Assert.That(() => ThrowIf.InRange(-1, 0, 2)).ThrowsNothing();
    }

    [Test]
    public async Task Test_GreaterThanOrEqual()
    {
        await Assert.That(() => ThrowIf.GreaterThanOrEqual(0, 1)).ThrowsNothing();
        await Assert.That(() => ThrowIf.GreaterThanOrEqual(0, 0)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => ThrowIf.GreaterThanOrEqual(1, 0)).Throws<ArgumentOutOfRangeException>();
    }
    
    [Test]
    public async Task Test_GreaterThan()
    {
        await Assert.That(() => ThrowIf.GreaterThan(0, 1)).ThrowsNothing();
        await Assert.That(() => ThrowIf.GreaterThan(0, 0)).ThrowsNothing();
        await Assert.That(() => ThrowIf.GreaterThan(1, 0)).Throws<ArgumentOutOfRangeException>();
    }
}


file class A;
file class B : A;
file class C;