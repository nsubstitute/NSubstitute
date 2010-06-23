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
            IArgumentSpecification[] _argSpecsFromFactory;
            ICallSpecification _result;
            const bool WithAnyArguments = true;

            public override void Context()
            {
                base.Context();
                var methodInfo = mock<MethodInfo>();
                var arguments = new[] { new object() };
                methodInfo.Stub(x => x.GetParameters()).Return(new ParameterInfo[0]);

                _call = CreateStubCall(methodInfo, arguments);
                
                _context = mock<ISubstitutionContext>();
                _context.stub(x => x.DequeueAllArgumentSpecifications()).Return(mock<IList<IArgumentSpecification>>());
                
                _argumentSpecificationFactory = mock<IArgumentSpecificationFactory>();
                _argSpecsFromFactory = new[] { mock<IArgumentSpecification>(), mock<IArgumentSpecification>() };
                _argumentSpecificationFactory
                    .Stub(x => x.Create(
                                _context.DequeueAllArgumentSpecifications(), 
                                _call.GetArguments(), 
                                methodInfo.GetParameters(), 
                                WithAnyArguments))
                    .Return(_argSpecsFromFactory);
            }

            public override CallSpecificationFactory CreateSubjectUnderTest()
            {
                return new CallSpecificationFactory(_context, _argumentSpecificationFactory);
            }

            public override void Because()
            {
                _result = sut.CreateFrom(_call, WithAnyArguments);
            }

            [Test]
            public void Resulting_spec_should_be_satisfied_if_call_method_and_arg_specs_are_satisfied()
            {
                var specifiedMethod = _call.GetMethodInfo();
                var args = CreateArgsThatMatchArgSpecsFromFactory();

                var callThatShouldMatch = CreateStubCall(specifiedMethod, args);

                Assert.That(_result.IsSatisfiedBy(callThatShouldMatch));
            }

            [Test]
            public void Resulting_spec_should_not_be_satisfied_if_call_method_does_not_match()
            {
                var specifiedMethod = mock<MethodInfo>();
                var args = CreateArgsThatMatchArgSpecsFromFactory();

                var callToDifferentMethod = CreateStubCall(specifiedMethod, args);

                Assert.That(_result.IsSatisfiedBy(callToDifferentMethod), Is.False);
            }

            [Test]
            public void Resulting_spec_should_not_be_satisfied_if_call_method_matches_but_args_differ()
            {
                var specifiedMethod = _call.GetMethodInfo();
                var args = CreateArgsThatDoNotMatchArgSpecsFromFactory(); 

                var callWithOneArgNotMatching = CreateStubCall(specifiedMethod, args);

                Assert.That(_result.IsSatisfiedBy(callWithOneArgNotMatching), Is.False);
            }

            private ICall CreateStubCall(MethodInfo methodInfo, object[] arguments)
            {
                var call = mock<ICall>();
                call.stub(x => x.GetMethodInfo()).Return(methodInfo);
                call.stub(x => x.GetArguments()).Return(arguments);
                return call;
            }

            private object[] CreateArgsThatMatchArgSpecsFromFactory()
            {
                var args = new[] {new object(), new object()};
                _argSpecsFromFactory[0].stub(x => x.IsSatisfiedBy(args[0])).Return(true);
                _argSpecsFromFactory[1].stub(x => x.IsSatisfiedBy(args[1])).Return(true);
                return args;
            }

            private object[] CreateArgsThatDoNotMatchArgSpecsFromFactory()
            {
                var args = new[] {new object(), new object()};
                _argSpecsFromFactory[0].stub(x => x.IsSatisfiedBy(args[0])).Return(true);
                _argSpecsFromFactory[1].stub(x => x.IsSatisfiedBy(args[1])).Return(false);
                return args;
            }
        }
    }
}