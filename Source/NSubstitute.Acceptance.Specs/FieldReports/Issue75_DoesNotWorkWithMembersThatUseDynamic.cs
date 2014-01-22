#if NET4
using System;

using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue75_DoesNotWorkWithMembersThatUseDynamic
    {
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
    }
}
#endif