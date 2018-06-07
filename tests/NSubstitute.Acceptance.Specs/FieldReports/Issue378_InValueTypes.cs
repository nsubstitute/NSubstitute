using System;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    /// <summary>
    /// Issue from https://github.com/nsubstitute/NSubstitute/issues/378.
    /// </summary>
    public class Issue378_InValueTypes
    {
        public readonly struct Struct { }

        public interface IStructByRefConsumer { void Consume(in Struct message); }

        public interface IStructByValueConsumer { void Consume(Struct message); }

        [Test]
        public void IStructByRefConsumer_Test()
        {
            _ = Substitute.For<IStructByRefConsumer>();
        }

        [Test]
        public void IStructByValueConsumer_Test()
        {
            _ = Substitute.For<IStructByValueConsumer>();
        }
    }
}