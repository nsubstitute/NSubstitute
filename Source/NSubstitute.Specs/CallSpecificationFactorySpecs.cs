using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using Rhino.Mocks;

namespace NSubstitute.Specs
{
    public class CallSpecificationFactorySpec
    {
        public class When_creating_a_specification : ConcernFor<CallSpecificationFactory>
        {
            ICall _call;
            ISubstitutionContext _context;
            IArgumentSpecificationFactory _argumentSpecificationFactory;
            IArgumentSpecification[] _expectedArgumentSpecifications;
            ICallSpecification _result;

            public override void Context()
            {
                base.Context();
                var methodInfo = mock<MethodInfo>();
                methodInfo.Stub(x => x.GetParameters()).Return(new ParameterInfo[0]);

                _call = mock<ICall>();
                _call.stub(x => x.GetMethodInfo()).Return(methodInfo);
                _call.stub(x => x.GetArguments()).Return(new object[0]);
                
                _context = mock<ISubstitutionContext>();
                _context.stub(x => x.DequeueAllArgumentSpecifications()).Return(mock<IList<IArgumentSpecification>>());
                
                _argumentSpecificationFactory = mock<IArgumentSpecificationFactory>();
                _expectedArgumentSpecifications = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };
                _argumentSpecificationFactory
                    .Stub(x => x.Create(
                                _context.DequeueAllArgumentSpecifications(), 
                                _call.GetArguments(), 
                                methodInfo.GetParameters()))
                    .Return(_expectedArgumentSpecifications);
            }

            public override CallSpecificationFactory CreateSubjectUnderTest()
            {
                return new CallSpecificationFactory(_context, _argumentSpecificationFactory);
            }

            [Test]
            public void Should_set_method_info_on_result()
            {
                Assert.That(_result.MethodInfo, Is.SameAs(_call.GetMethodInfo()));
            }

            [Test]
            public void Should_add_parameter_specifications_to_result()
            {
                Assert.That(_result.ArgumentSpecifications, Is.EquivalentTo(_expectedArgumentSpecifications));
            }

            public override void Because()
            {
                _result = sut.CreateFrom(_call);
            }
        }
    }
}