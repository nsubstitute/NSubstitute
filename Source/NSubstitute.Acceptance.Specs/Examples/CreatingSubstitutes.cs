using System;
using NSubstitute.Acceptance.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.Examples
{
    public class CreatingSubstitutes
    {
        [Test]
        public void Basic()
        {
            //{CODE:creating_substitutes.basic}
            var substitute = Substitute.For<ISomeInterface>();
        }

        [Test]
        public void Class_with_args()
        {
            //{CODE:creating_substitutes.class_with_args}
            var someClass = Substitute.For<SomeClassWithCtorArgs>(5, "hello world");
        }

        //{CODE:creating_substitutes.disposable}
        [Test]
        public void Should_run_and_cleanup_disposable_command()
        {
            var command = Substitute.For<ICommand, IDisposable>();
            var runner = new CommandRunner(command);

            runner.RunCommand();

            command.Received().Execute();
            ((IDisposable)command).Received().Dispose();
        }

        [Test]
        public void With_type_array()
        {
            //{CODE:creating_substitutes.with_type_array}
            var substitute = Substitute.For(
                                new[] { typeof(ICommand), typeof(ISomeInterface), typeof(SomeClassWithCtorArgs) },
                                new object[] { 5, "hello world" }
                              );
            Assert.IsInstanceOf<ICommand>(substitute);
            Assert.IsInstanceOf<ISomeInterface>(substitute);
            Assert.IsInstanceOf<SomeClassWithCtorArgs>(substitute);
        }

        public interface ISomeInterface { }
        public abstract class SomeClassWithCtorArgs
        {
            protected SomeClassWithCtorArgs(int anInt, string aString) { }
        }

        public interface ICommand
        {
            void Execute();
        }

        public class CommandRunner
        {
            private readonly ICommand _command;

            public CommandRunner(ICommand command)
            {
                _command = command;
            }

            public void RunCommand()
            {
                _command.Execute();
                if (_command is IDisposable) ((IDisposable)_command).Dispose();
            }
        }
    }
}