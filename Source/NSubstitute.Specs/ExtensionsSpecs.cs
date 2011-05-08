using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ExtensionsSpecs
    {
        public class ZipExtension
        {
            [Test]
            public void Zip_two_equal_collections()
            {
                var ints = new[] { 1, 2, 3 };
                var strings = new[] { "a", "b", "c" };

                var zipped = ints.Zip(strings, (i, s) => i + s);
                Assert.That(zipped, Is.EqualTo(new[] { "1a", "2b", "3c" }));
            }
        }
    }
}