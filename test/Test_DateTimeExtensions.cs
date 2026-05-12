namespace Ametrin.Utils.Test;

public sealed class Test_DateTimeExtensions
{
    [Test]
    public async Task GetOlder()
    {
        var min = DateTime.MinValue;
        var max = DateTime.MaxValue;
        var now = DateTime.Now;

        await Assert.That(DateTime.GetOlder(min, max)).IsEqualTo(min);
        await Assert.That(DateTime.GetOlder(now, max)).IsEqualTo(now);
        await Assert.That(DateTime.GetOlder(now, now)).IsEqualTo(now);
        await Assert.That(DateTime.GetOlder(now, null)).IsEqualTo(now);
        await Assert.That(DateTime.GetOlder(null, now)).IsEqualTo(now);
        await Assert.That(DateTime.GetOlder(null, null)).IsNull();
        await Assert.That(DateTime.GetOlder(new DateTime?(min), new DateTime?(now))).IsEqualTo(min);
    }

    [Test]
    public async Task GetNewer()
    {
        var min = DateTime.MinValue;
        var max = DateTime.MaxValue;
        var now = DateTime.Now;

        await Assert.That(DateTime.GetNewer(min, max)).IsEqualTo(max);
        await Assert.That(DateTime.GetNewer(now, max)).IsEqualTo(max);
        await Assert.That(DateTime.GetNewer(now, now)).IsEqualTo(now);
        await Assert.That(DateTime.GetNewer(now, null)).IsEqualTo(now);
        await Assert.That(DateTime.GetNewer(null, now)).IsEqualTo(now);
        await Assert.That(DateTime.GetNewer(null, null)).IsNull();
        await Assert.That(DateTime.GetNewer(new DateTime?(min), new DateTime?(now))).IsEqualTo(now);
    }
}
