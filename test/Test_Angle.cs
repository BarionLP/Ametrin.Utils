namespace Ametrin.Utils.Test;

public sealed class Test_Angle
{
    [Test]
    [Arguments(0, 0)]
    [Arguments(180, Math.PI)]
    [Arguments(360, Math.PI*2)]
    public async Task FromDegrees(double degrees, double radians)
    {
        await Assert.That(Angle.FromDegrees(degrees).Radians).IsEqualTo(radians);
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(90, 0.25)]
    [Arguments(360, 1)]
    [Arguments(540, 1.5)]
    [Arguments(720, 2)]
    [Arguments(3600, 10)]
    public async Task Revolutions(double degrees, double revolutions)
    {
        await Assert.That(Angle.FromDegrees(degrees).Revolutions).IsEqualTo(revolutions);
    }

    [Test]
    [Arguments(0, 0)]
    [Arguments(180, 180)]
    [Arguments(360, 0)]
    [Arguments(359, 359)]
    [Arguments(540, 180)]
    [Arguments(3600, 0)]
    public async Task Normalized(double degrees, double normalizedDegrees)
    {
        await Assert.That(Angle.FromDegrees(degrees).Normalized.Degrees).IsEqualTo(normalizedDegrees);
    }
}
