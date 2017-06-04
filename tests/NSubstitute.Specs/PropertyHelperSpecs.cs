using System;
using System.Collections.Generic;
using System.Reflection;
using NSubstitute.Core;
using NSubstitute.Core.Arguments;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class PropertyHelperSpecs : ConcernFor<PropertyHelper>
    {
        public override PropertyHelper CreateSubjectUnderTest()
        {
            return new PropertyHelper();
        }

        private Call CreateCall(MethodInfo methodInfo, object[] args)
        {
            return new Call(methodInfo, args, null, new List<IArgumentSpecification>());
        }

        [Test]
        public void ShouldIdentifyCallSettingAReadWriteProperty()
        {
            var call = CreateCall(GetSomePropertySetter(), new[] { "arg" });
            Assert.That(sut.IsCallToSetAReadWriteProperty(call), Is.True);
        }

        [Test]
        public void ShouldIdentifyThatNormalMethodIsNotAPropertySetter()
        {
            var call = CreateCall(GetSomeMethod(), new object[0]);
            Assert.That(sut.IsCallToSetAReadWriteProperty(call), Is.False);
        }

        [Test]
        public void ShouldIdentifySettingWriteOnlyPropertyIsNotAReadWriteProperty()
        {
            var call = CreateCall(GetWriteOnlyPropertySetter(), new[] { "arg" });
            Assert.That(sut.IsCallToSetAReadWriteProperty(call), Is.False);
        }

        [Test]
        public void ShouldCreateACallToGetterFromSetter()
        {
            var callToSetter = CreateCall(GetSomePropertySetter(), new[] { "arg" });
            var callToGetter = sut.CreateCallToPropertyGetterFromSetterCall(callToSetter);
            Assert.That(callToGetter.GetMethodInfo(), Is.EqualTo(GetSomePropertyGetter()));
        }

        [Test]
        public void ShouldCreateACallToIndexerGetterFromIndexSetter()
        {
            const int index = 123;
            const string value = "arg";
            var callToSetter = CreateCall(GetIndexerPropertySetter(), new object[] { index, value });
            var callToGetter = sut.CreateCallToPropertyGetterFromSetterCall(callToSetter);
            Assert.That(callToGetter.GetMethodInfo(), Is.EqualTo(GetIndexerPropertyGetter()));
            Assert.That(callToGetter.GetArguments(), Is.EqualTo(new[] { index }));
        }

        [Test]
        public void ShouldThrowWhenTryingToCreateACallToAGetterThatDoesNotExist()
        {
            var callToSetter = CreateCall(GetWriteOnlyPropertySetter(), new[] { "arg" });
            Assert.Throws<InvalidOperationException>(() => sut.CreateCallToPropertyGetterFromSetterCall(callToSetter));
        }

        MethodInfo GetSomePropertyGetter() { return GetMethodOnHelperClass("get_SomeProperty"); }
        MethodInfo GetSomePropertySetter() { return GetMethodOnHelperClass("set_SomeProperty"); }
        MethodInfo GetSomeMethod() { return GetMethodOnHelperClass("SomeMethod"); }
        MethodInfo GetWriteOnlyPropertySetter() { return GetMethodOnHelperClass("set_WriteOnlyProperty"); }
        MethodInfo GetMethodOnHelperClass(string name) { return typeof(ClassToTestPropertyHelper).GetMethod(name); }
        MethodInfo GetIndexerPropertyGetter() { return GetMethodOnHelperClass("get_Item"); }
        MethodInfo GetIndexerPropertySetter() { return GetMethodOnHelperClass("set_Item"); }

        public class ClassToTestPropertyHelper
        {
            public string SomeProperty { get; set; }
            public void SomeMethod() { }
            public string WriteOnlyProperty { set { } }
            public string this[int i] { get { return ""; } set { } }
        }
    }
}