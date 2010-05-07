using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routes
{
    public abstract class ConcernForRoute<TRoute> : ConcernFor<IRoute> where TRoute : IRoute
    {
        TestRouteParts _routeParts;
        readonly object _expectedResult = new object();
        object _result;
        ICall _call;

        [Test]
        public void Should_return_expected_result()
        {
            Assert.That(_result, Is.SameAs(_expectedResult));
        }

        public override void Because()
        {
            _result = sut.Handle(_call);
        }

        public override void Context()
        {
            _call = mock<ICall>();
            _routeParts = new TestRouteParts();
        }

        public override IRoute CreateSubjectUnderTest()
        {
            return (IRoute)Activator.CreateInstance(typeof(TRoute), _routeParts);
        }

        protected void AssertPartHandledCall<TPart>() where TPart : ICallHandler
        {
            var part = _routeParts.GetPart<TPart>();
            part.received(x => x.Handle(_call));
        }

        protected void ExpectReturnValueToComeFromPart<TPart>() where TPart : ICallHandler
        {
            var part = _routeParts.GetPart<TPart>();
            part.stub(x => x.Handle(_call)).Return(_expectedResult);
        }

        class TestRouteParts : IRouteParts
        {
            readonly IDictionary<Type, ICallHandler> _parts = new Dictionary<Type, ICallHandler>();
            public ICallHandler GetPart<TPart>() where TPart : ICallHandler
            {
                var partKey = typeof(TPart);
                return _parts.ContainsKey(partKey) ? _parts[partKey] : MockPartAndReturn(partKey);
            }
            private ICallHandler MockPartAndReturn(Type partKey)
            {
                var part = MockingAdaptor.Create<ICallHandler>();
                _parts[partKey] = part;
                return part;
            }
        }
    }
}