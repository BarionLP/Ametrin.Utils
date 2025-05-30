using Ametrin.Guards;
using TUnit.Assertions.AssertConditions.Throws;

namespace Ametrin.Utils.Test;

public sealed class Test_Guard
{
    [Test]
    public async Task Test_ThrowIfNull()
    {
        await Assert.That(() => Guard.ThrowIfNull((string?)null)).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNull(new int?())).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNull(new int?(1))).IsEqualTo(1);
        await Assert.That(() => Guard.ThrowIfNull("")).IsEqualTo("");
    }

    [Test]
    public async Task Test_Is()
    {
        await Assert.That(() => Guard.Is<B>(new A())).Throws<InvalidCastException>();
        await Assert.That(() => Guard.Is<A>(new A())).ThrowsNothing();
        await Assert.That(() => Guard.Is<A>(new B())).ThrowsNothing();
        await Assert.That(() => Guard.Is<A>(new C())).Throws<InvalidCastException>();
        await Assert.That(() => Guard.Is<A>(null)).Throws<ArgumentNullException>();
    }

    [Test]
    public async Task Test_ThrowIfNullOrEmpty_Collection()
    {
        await Assert.That(() => Guard.ThrowIfNullOrEmpty<int[]>(null)).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrEmpty<int[]>([])).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrEmpty<int[]>([1])).ThrowsNothing();
    }

    [Test]
    public async Task Test_ThrowIfNullOrEmpty_String()
    {
        await Assert.That(() => Guard.ThrowIfNullOrEmpty(null)).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrEmpty("")).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrEmpty(" ")).ThrowsNothing();
    }

    [Test]
    public async Task Test_ThrowIfNullOrWhiteSpace()
    {
        await Assert.That(() => Guard.ThrowIfNullOrWhiteSpace(null)).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrWhiteSpace("")).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrWhiteSpace("   ")).Throws<ArgumentNullException>();
        await Assert.That(() => Guard.ThrowIfNullOrWhiteSpace("hello world")).ThrowsNothing();
    }

    [Test]
    public async Task Test_InRange()
    {
        await Assert.That(() => Guard.InRange(0, 0, 2)).ThrowsNothing();
        await Assert.That(() => Guard.InRange(1, 0, 2)).ThrowsNothing();
        await Assert.That(() => Guard.InRange(2, 0, 2)).ThrowsNothing();
        await Assert.That(() => Guard.InRange(3, 0, 2)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => Guard.InRange(-1, 0, 2)).Throws<ArgumentOutOfRangeException>();
    }

    [Test]
    public async Task Test_LessThan()
    {
        await Assert.That(() => Guard.LessThan(0, 1)).ThrowsNothing();
        await Assert.That(() => Guard.LessThan(0, 0)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => Guard.LessThan(1, 0)).Throws<ArgumentOutOfRangeException>();
    }
    
    [Test]
    public async Task Test_LessThanOrEqual()
    {
        await Assert.That(() => Guard.LessThanOrEqual(0, 1)).ThrowsNothing();
        await Assert.That(() => Guard.LessThanOrEqual(0, 0)).ThrowsNothing();
        await Assert.That(() => Guard.LessThanOrEqual(1, 0)).Throws<ArgumentOutOfRangeException>();
    }
}


file class A;
file class B : A;
file class C;