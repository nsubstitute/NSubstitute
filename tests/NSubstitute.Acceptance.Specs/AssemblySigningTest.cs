using System.Reflection;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

public class AssemblySigningTest
{
    [Test]
    public void NSubstitute_assembly_should_be_signed()
    {
        var assemblyName = typeof(Substitute).GetTypeInfo().Assembly.GetName();
        var publicKeyToken = string.Join("", assemblyName.GetPublicKeyToken().Select(x => x.ToString("x")));

        Assert.That(publicKeyToken, Is.EqualTo("92dd2e9066daa5ca"));
    }
}