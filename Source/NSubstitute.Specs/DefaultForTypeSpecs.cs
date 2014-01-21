using System;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class DefaultForTypeSpecs : ConcernFor<DefaultForType>
    {
        [Test]
        public void Should_return_null_for_reference_type()
        {
            Assert.That(sut.GetDefaultFor(typeof(object)), Is.Null);
        }

        [Test]
        public void Should_return_default_for_value_type()
        {
            Assert.That(sut.GetDefaultFor(typeof(int)), Is.EqualTo(default(int)));
        }

        [Test]
        public void Should_return_null_for_void()
        {
            Assert.That(sut.GetDefaultFor(typeof(void)), Is.Null);
        }

#if NET45
        [Test]
        public void Should_not_return_null_for_iobservable()
        {
            Assert.That(sut.GetDefaultFor(typeof(IObservable<string>)), Is.Not.Null);
        }

        [Test]
        public void Should_return_default_value_for_iobservable()
        {
            IObservable<string> output = (IObservable<string>)sut.GetDefaultFor(typeof(IObservable<string>));

            string onNextDefault = "notNull";
            bool wasCompleted = false;
            Exception error = null;

            output.Subscribe(new AnonymousObserver<string>(
                x => onNextDefault = x,
                ex => error = ex,
                () => wasCompleted = true));

            Assert.That(onNextDefault, Is.Null);
            Assert.That(wasCompleted, Is.True);
            Assert.That(error, Is.Null);
        }

        [Test]
        public void Should_return_default_value_for_boxed_value_iobservable()
        {
            IObservable<int> output = (IObservable<int>)sut.GetDefaultFor(typeof(IObservable<int>));

            int onNextDefault = default(int) + 1;
            bool wasCompleted = false;
            Exception error = null;

            output.Subscribe(new AnonymousObserver<int>(
                x => onNextDefault = x,
                ex => error = ex,
                () => wasCompleted = true));

            Assert.That(onNextDefault, Is.EqualTo(default(int)));
            Assert.That(wasCompleted, Is.True);
            Assert.That(error, Is.Null);
        }
#endif

        public override DefaultForType CreateSubjectUnderTest()
        {
            return new DefaultForType();
        }
    }
}