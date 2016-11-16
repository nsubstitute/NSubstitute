using System;
using System.Collections.Generic;
using NSubstitute.Core;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
    [TestFixture]
    public class CustomHandlersSpecs
    {
        [Test]
        public void Value_from_custom_handler_is_returned()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    _ => RouteAction.Return("42")));

            //act
            var result = source.GetValue();

            //assert
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Value_from_custom_handler_is_returned_for_setup_after_invocation()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            //invoke it before registering handler
            source.GetValue();

            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    _ => RouteAction.Return("42")));

            //act
            var result = source.GetValue();

            //assert
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Custom_handler_is_called_for_each_time()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            var values = new Queue<string>(new[] { "42", "10" });

            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    _ => RouteAction.Return(values.Dequeue())));

            //act
            var result = source.GetValue();
            result = source.GetValue();

            //assert
            Assert.That(result, Is.EqualTo("10"));
        }

        [Test]
        public void Configured_call_has_more_priority_than_custom_handler()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    _ => RouteAction.Return("xxx")));

            source.GetValue().Returns("42");

            //act
            var result = source.GetValue();

            //assert
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Updated_ref_parameter_doesnt_affect_call_specification()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            //Configure our handler to update "ref" argument value
            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    call =>
                    {
                        if (call.GetMethodInfo().Name != nameof(IValueSource.GetValueWithRef))
                            return RouteAction.Continue();

                        var args = call.GetArguments();
                        args[0] = "refArg";

                        return RouteAction.Return("xxx");
                    }));

            string refValue = "ref";
            source.GetValueWithRef(ref refValue).Returns("42");

            //act
            refValue = "ref";
            var result = source.GetValueWithRef(ref refValue);

            //assert
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Set_out_parameter_doesnt_affect_call_specification()
        {
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            //Configure our handler to update "out" argument value
            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    call =>
                    {
                        if (call.GetMethodInfo().Name != nameof(IValueSource.GetValueWithOut))
                            return RouteAction.Continue();

                        var args = call.GetArguments();
                        args[0] = "outArg";

                        return RouteAction.Return("xxx");
                    }));

            string outArg;
            source.GetValueWithOut(out outArg).Returns("42");

            //act
            string otherOutArg;
            var result = source.GetValueWithOut(out otherOutArg);

            //assert
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void Is_not_called_for_specifying_call()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            bool wasInvoked = false;
            router.RegisterCustomCallHandlerFactory(state =>
                    new ActionHandler(_ =>
                    {
                        wasInvoked = true;
                        return RouteAction.Continue();
                    }));

            //act
            source.MethodWithArgs(Arg.Any<string>(), Arg.Is("42")).Returns("");

            //assert
            Assert.That(wasInvoked, Is.False);
        }

        [Test]
        public void Auto_value_is_returned_if_skipped()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            router.RegisterCustomCallHandlerFactory(state =>
                    new ActionHandler(_ => RouteAction.Continue()));

            //act
            var result = source.GetValue();

            //assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void First_added_handler_has_precedence()
        {
            //arrange
            var source = Substitute.For<IValueSource>();
            var router = SubstitutionContext.Current.GetCallRouterFor(source);

            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    _ => RouteAction.Return("42")));

            router.RegisterCustomCallHandlerFactory(state =>
                new ActionHandler(
                    _ => RouteAction.Return("10")));

            //act
            var result = source.GetValue();

            //assert
            Assert.That(result, Is.EqualTo("42"));
        }


        public interface IValueSource
        {
            string GetValue();
            string GetValueWithRef(ref string arg1);
            string GetValueWithOut(out string arg1);
            string MethodWithArgs(string arg1, string arg2);
        }

        private class ActionHandler : ICallHandler
        {
            private readonly Func<ICall, RouteAction> _handler;

            public ActionHandler(Func<ICall, RouteAction> handler)
            {
                _handler = handler;
            }

            public RouteAction Handle(ICall call) => _handler.Invoke(call);
        }
    }
}