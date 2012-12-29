using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Routing.Handlers;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs.Routing.Handlers
{
    public class ReturnOriginalResultCallHandlerSpecs
    {
        public abstract class When_handling_a_call : ConcernFor<ReturnOriginalResultCallHandler>
        {
            protected ICall _call;
            protected RouteAction _result;
            protected readonly object OriginalMethodCallResult = new object();

            public override void Because()
            {
                _result = sut.Handle(_call);
            }

            public override void Context()
            {
                _call = mock<ICall>();
                _call.stub(c => c.CallOriginalMethod()).Return(OriginalMethodCallResult);
                var methodInfo = GetMethodCall();
                _call.stub(x => x.GetMethodInfo()).Return(methodInfo);
            }

            protected abstract MethodInfo GetMethodCall();

            public override ReturnOriginalResultCallHandler CreateSubjectUnderTest()
            {
                return new ReturnOriginalResultCallHandler();
            }
        }

        public abstract class For_base_object_method_call : When_handling_a_call
        {
            [Test]
            public void Return_the_original_method_call_result()
            {
                Assert.That(_result.ReturnValue, Is.EqualTo(OriginalMethodCallResult));
            }
        }

        public class With_to_string_call : For_base_object_method_call
        {
            protected override MethodInfo GetMethodCall()
            {
                return ReflectionHelper.GetMethod(() => ToString());
            }

            public override string ToString()
            {
                return string.Empty;
            }
        }

        public class With_get_hash_code_call : For_base_object_method_call
        {
            protected override MethodInfo GetMethodCall()
            {
                return ReflectionHelper.GetMethod(() => GetHashCode());
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class With_equals_call : For_base_object_method_call
        {
            protected override MethodInfo GetMethodCall()
            {
                return ReflectionHelper.GetMethod(() => Equals(new object()));
            }

            public override bool Equals(object otherObject)
            {
                return false;
            }

            public override int GetHashCode()
            {
                return 0;
            }
        }

        public class For_non_base_object_call : When_handling_a_call
        {
            [Test]
            public void Continue_to_the_next_handler()
            {
                Assert.That(_result.HasReturnValue, Is.False);
            }

            protected override MethodInfo GetMethodCall()
            {
                return ReflectionHelper.GetMethod(() => NonBaseObjectCall());
            }

            public int NonBaseObjectCall()
            {
                return 0;
            }
        }
    }
}