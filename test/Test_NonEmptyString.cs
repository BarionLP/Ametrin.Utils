namespace Ametrin.Utils.Test;

public sealed class Test_NonEmptyString
{
    [Test]
    [Arguments("")]
    [Arguments(" ")]
    [Arguments("   ")]
    public async Task Construction_Throws(string value)
    {
        await Assert.That(() => new NonEmptyString(value)).Throws<ArgumentException>();
    }
    
    [Test]
    [Arguments(".")]
    [Arguments("Hello World")]
    [Arguments("The Cake is a lie")]
    public async Task Construction_Success(string value)
    {
        await Assert.That(() => new NonEmptyString(value)).ThrowsNothing();
    }
}
