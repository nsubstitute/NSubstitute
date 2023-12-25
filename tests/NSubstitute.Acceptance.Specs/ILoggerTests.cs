using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs;

[TestFixture]
public class ILoggerTests
{
    [Test]
    public void Received_LogTrace_call_using_AnyType()
    {
        var logger = Substitute.For<ILogger<ILoggerTests>>();

        logger.LogTrace("Vanished without a trace");

        logger.Received(1)
            .Log(
                LogLevel.Trace,
                Arg.Any<EventId>(),
                Arg.Any<Arg.AnyType>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<Arg.AnyType, Exception, string>>()
            );
    }

    [Test]
    public void Received_LogWarning_call_using_interfaced_types()
    {
        var logger = Substitute.For<ILogger<ILoggerTests>>();

        logger.LogWarning("Warning: Live without warning");

        logger.Received(1)
            .Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Any<IReadOnlyList<KeyValuePair<string, object>>>(),
                Arg.Any<Exception>(),
                Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception, string>>()
            );
    }

    [Test]
    public void Received_LogError_call_using_messageTemplate()
    {
        var logger = Substitute.For<ILogger<ILoggerTests>>();

        logger.LogError("Something bad happened!!!");

        logger.Received(1)
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<IReadOnlyList<KeyValuePair<string, object>>>(list => list.Any(i => i.Key == "{OriginalFormat}" && i.Value.Equals("Something bad happened!!!"))),
                Arg.Any<Exception>(),
                Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception, string>>()
            );

        logger.DidNotReceive()
            .Log(
                LogLevel.Error,
                Arg.Any<EventId>(),
                Arg.Is<IReadOnlyList<KeyValuePair<string, object>>>(list => list.Any(i => i.Key == "{OriginalFormat}" && i.Value.Equals("some other message"))),
                Arg.Any<Exception>(),
                Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception, string>>()
            );
    }

    [Test]
    public void Received_LogInformation_call_using_messageTemplate()
    {
        var now = DateTimeOffset.UtcNow;
        var later = now.AddTicks(1);
        var logger = Substitute.For<ILogger<ILoggerTests>>();

        logger.LogInformation("I have parameters. {param1} {param2} {param3}", 1979, "apocalypse", now);

        logger.Received(1)
            .Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<IReadOnlyList<KeyValuePair<string, object>>>(list =>
                    list.Any(i => i.Key == "{OriginalFormat}" && i.Value.Equals("I have parameters. {param1} {param2} {param3}"))
                    && list.Any(i => i.Key == "param1" && i.Value.Equals(1979))
                    && list.Any(i => i.Key == "param2" && i.Value.Equals("apocalypse"))
                    && list.Any(i => i.Key == "param3" && i.Value.Equals(now))
                ),
                Arg.Any<Exception>(),
                Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception, string>>()
            );

        logger.DidNotReceive()
            .Log(
                LogLevel.Information,
                Arg.Any<EventId>(),
                Arg.Is<IReadOnlyList<KeyValuePair<string, object>>>(list =>
                    list.Any(i => i.Key == "{OriginalFormat}" && i.Value.Equals("I have parameters. {param1} {param2} {param3}"))
                    && list.Any(i => i.Key == "param1" && i.Value.Equals(1979))
                    && list.Any(i => i.Key == "param2" && i.Value.Equals("apocalypse"))
                    && list.Any(i => i.Key == "param3" && i.Value.Equals(later)) // this is the only one that will mismatch
                ),
                Arg.Any<Exception>(),
                Arg.Any<Func<IReadOnlyList<KeyValuePair<string, object>>, Exception, string>>()
            );
    }
}