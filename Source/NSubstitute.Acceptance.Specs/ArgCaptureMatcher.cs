using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    public class ArgCaptureMatcher
    {
        private IFoo _sub;
        private readonly object _someObject = new object();
        string capturedA = null;
        object capturedObject = null;

        public interface IFoo
        {
            void Bar(string a);
            int Zap(object c);
        }

        
        [SetUp]
        public void SetUp()
        {
            _sub = Substitute.For<IFoo>();
        }

        [Test]
        public void Should_capture_strings()
        {
            _sub.Bar(Arg.CaptureTo(() => capturedA));

            _sub.Bar("abc");

            Assert.That(capturedA, Is.EqualTo("abc"));
        }


        [Test]
        public void Should_capture_objects()
        {
            _sub.Zap(Arg.CaptureTo(() => capturedObject));

            _sub.Zap(_someObject);

            Assert.That(capturedObject, Is.SameAs(_someObject));
        }


    }
}