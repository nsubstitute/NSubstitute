using System;

using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue75_DynamicTests
    {
#if (NET4 || NET45)
        public interface IInterface
        {
            dynamic ReturnsDynamic(string a);
            dynamic ReturnsAndGetsDynamic(dynamic a);
            int GetsDynamic(dynamic a);
            dynamic DynamicProperty { get; set; }
        }

        [Test]
        [Pending, Explicit]
        public void MethodGetsDynamicAndSpecifiedWithDynamic()
        {
            var sut = Substitute.For<IInterface>();
            // Fails
            // Because dynamic typing doesn't support extension methods
            // We can't do much here, I see only two options:
            // 1. Good documentation. Tell people to use exact type instead of dynamic or use static Returns method.
            // 2. Try to catch calls with dynamic typing and throw descriptive exception. Can be made via StackTrace. A bit hacky and risky.
            // TBD
            sut.GetsDynamic(Arg.Any<dynamic>()).Returns(1);

            dynamic expando = new System.Dynamic.ExpandoObject();
            var result = sut.GetsDynamic(expando);

            Assert.That(result, Is.EqualTo(1));
        } 

        [Test]
        public void MethodGetsDynamicButSpecifiedWithExplicitType()
        {
            var sut = Substitute.For<IInterface>();
            sut.GetsDynamic(Arg.Any<object>()).Returns(1);

            dynamic expando = new System.Dynamic.ExpandoObject();
            var result = sut.GetsDynamic(expando);

            Assert.That(result, Is.EqualTo(1));
        } 
        
        [Test]
        public void DynamicProperty()
        {
            var sut = Substitute.For<IInterface>();
            sut.DynamicProperty.Returns(1);

            var result = sut.DynamicProperty;

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void MethodGetsDynamicAsAnArgumentAndReturnsDynamic()
        {
            var sut = Substitute.For<IInterface>();
            sut.ReturnsAndGetsDynamic(Arg.Any<dynamic>()).Returns(1);

            dynamic expando = new System.Dynamic.ExpandoObject();
            var result = sut.ReturnsAndGetsDynamic(expando);

            Assert.That(result, Is.EqualTo(1));
        }

        [Test]
        public void MethodReturnsDynamic()
        {
            var sut = Substitute.For<IInterface>();
            sut.ReturnsDynamic(Arg.Any<string>()).Returns(1);

            var result = sut.ReturnsDynamic("");

            Assert.That(result, Is.EqualTo(1));
        }
#endif
    }

    //todo 75 remove redundant tests below?
    public class Issue75_DoesNotWorkWithMembersThatUseDynamic
    {
#if (NET4 || NET45)
        public interface ILog { void Error(Exception e); }
        public interface IClient { dynamic Post(string a, string b); }

        public class ClassUnderTest
        {
            private readonly IClient _client;
            private readonly ILog _log;

            public ClassUnderTest(IClient client, ILog log)
            {
                _client = client;
                _log = log;
            }

            public void Handle(string a)
            {
                dynamic response = _client.Post("asdf", "fdsa");
                var error = response.error;
                _log.Error(new Exception(error));
            }
        }

        [Test]
        public void SampleTest()
        {
            var client = Substitute.For<IClient>();
            var log = Substitute.For<ILog>();
            var target = new ClassUnderTest(client, log);
            const string errorResponse = "some issue";
            dynamic postResponse = new { error = errorResponse };
            client.Post("", "").ReturnsForAnyArgs(postResponse);

            // Act
            target.Handle("test");

            // Assert
            log.Received().Error(Arg.Is<Exception>(x => x.Message == errorResponse));
        }

        // src: http://stackoverflow.com/questions/13182007/nsubstitute-mock-generic-method/
        public interface ISettingsUtil
        {
            T GetConfig<T>(string setting, dynamic settings);
        }

        [Test]
        [Pending, Explicit]
        public void AnotherTest()
        {
            // Adapted from: http://stackoverflow.com/questions/13182007/nsubstitute-mock-generic-method/
            var settingsUtil = Substitute.For<ISettingsUtil>();
            // This works:
            //SubstituteExtensions.Returns(settingsUtil.GetConfig<long>("maxImageSize", Arg.Any<dynamic>()), 100L);

            // This fails, with RuntimeBinderException: 'long' does not contain a definition for 'Returns':
            settingsUtil.GetConfig<long>("maxImageSize", Arg.Any<dynamic>()).Returns(100L);

            dynamic settings = new System.Dynamic.ExpandoObject();
            var result = settingsUtil.GetConfig<long>("maxImageSize", settings);
            Assert.That(result, Is.EqualTo(100L));
        }
#endif
    }
}