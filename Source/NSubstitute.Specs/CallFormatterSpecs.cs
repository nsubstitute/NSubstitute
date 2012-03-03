using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class CallFormatterSpecs : ConcernFor<CallFormatter>
    {
        private ISample _sampleSub;
        private IArgumentsFormatter _argumentsFormatter;
        protected int _ignored;
        private IArgumentFormatInfoFactory _argumentFormatInfoFactory;
        private IArgumentFormatter _argumentFormatter;

        [Test]
        public void Should_format_method_name_and_arguments()
        {
            AssertCallFormat(x => x.SampleMethod(1, "b"), "SampleMethod(args)");
        }

        [Test]
        public void Should_format_generic_method_and_arguments()
        {
            AssertCallFormat(x => x.GenericMethod(1), "GenericMethod<Int32>(args)");
        }

        [Test]
        public void Should_format_method_with_multiple_generic_args()
        {
            AssertCallFormat(x => x.GenericMethodWithMultipleTypes(1, "b"), "GenericMethodWithMultipleTypes<Int32, String>(args)"); 
        }

        [Test]
        public void Should_format_property_set()
        {
            AssertCallFormat(x => x.Property = 2, "Property = args");
        }

        [Test]
        public void Should_format_property_get()
        {
            AssertCallFormat(x => { _ignored = x.Property; }, "Property");
        }

        [Test]
        public void Should_format_indexer_setter()
        {
            AssertCallFormat(x => x["a", "b"] = 4, "this[args] = args"); 
        }

        [Test]
        public void Should_formated_indexer_getter()
        {
            AssertCallFormat(x => { _ignored = x["a", "b"]; }, "this[args]");
        }

        [Test]
        public void Should_format_event_subscription()
        {
            AssertCallFormat(x => x.AnEvent += null, "AnEvent += args");  
        }

        [Test]
        public void Should_format_event_unsubscription()
        {
            AssertCallFormat(x => x.AnEvent -= null, "AnEvent -= args");  
        }

        public override void Context()
        {
            base.Context();

            _argumentFormatter = mock<IArgumentFormatter>();

            _argumentFormatInfoFactory = Substitute.For<IArgumentFormatInfoFactory>();
            _argumentFormatInfoFactory.CreateArgumentFormatInfos(null, null, null).ReturnsForAnyArgs(CreateArgumentFormatInfoMocks);
            
            _argumentsFormatter = mock<IArgumentsFormatter>();
            _argumentsFormatter.stub(x => x.Format(null)).IgnoreArguments().Return("args");
            _sampleSub = Substitute.For<ISample>();
        }

        private IEnumerable<IArgumentFormatInfo> CreateArgumentFormatInfoMocks(CallInfo x)
        {
            for (int i = 0; i < x.Arg<IEnumerable<object>>().Count(); i++)
            {           
                IArgumentFormatInfo argumentFormatInfo = mock<IArgumentFormatInfo>();
                argumentFormatInfo.stub(a => a.Format(_argumentFormatter)).Return("args");

                yield return argumentFormatInfo;
            }
        }

        public override CallFormatter CreateSubjectUnderTest()
        {
            return new CallFormatter(_argumentsFormatter, _argumentFormatInfoFactory);
        }

        private void AssertCallFormat(Action<ISample> callOnSubstitute, string expectedFormat)
        {
            callOnSubstitute(_sampleSub);
            var call =  _sampleSub.ReceivedCalls().First();
            var format = sut.Format(call.GetMethodInfo(), call.GetArguments(), new ArgumentMatchInfo[0]);
            Assert.That(format, Is.EqualTo(expectedFormat));
        }

        public interface ISample
        {
            void SampleMethod(int a, string b);
            void GenericMethod<T>(T t);
            void GenericMethodWithMultipleTypes<T1, T2>(T1 t1, T2 t2);
            int Property { get; set; }
            int this[string a, string b] { get; set; }
            event Action AnEvent;
        }
    }
}