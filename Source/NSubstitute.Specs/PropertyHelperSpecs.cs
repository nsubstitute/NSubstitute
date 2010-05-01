using System;
using System.Reflection;
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

        [Test]
        public void ShouldIdentifyCallSettingAReadWriteProperty()
        {
            var call = new FakeCall(typeof (string), GetSomePropertySetter(), null, new[] {"arg"});
            Assert.That(sut.IsCallToSetAReadWriteProperty(call), Is.True);
        }
        
        [Test]
        public void ShouldIdentifyThatNormalMethodIsNotAPropertySetter()
        {
            var call = new FakeCall(typeof(void), GetSomeMethod(), null, new object[0]);
            Assert.That(sut.IsCallToSetAReadWriteProperty(call), Is.False);
        }

        [Test]
        public void ShouldIdentifySettingWriteOnlyPropertyIsNotAReadWriteProperty()
        {
            var call = new FakeCall(typeof(string), GetWriteOnlyPropertySetter(), null, new[] { "arg" });
            Assert.That(sut.IsCallToSetAReadWriteProperty(call), Is.False);
        }

        [Test]
        public void ShouldCreateACallToGetterFromSetter()
        {
            var callToSetter = new FakeCall(typeof(string), GetSomePropertySetter(), null, new[] { "arg" });
            var callToGetter = sut.CreateCallToPropertyGetterFromSetterCall(callToSetter);            
            Assert.That(callToGetter.GetMethodInfo(), Is.EqualTo(GetSomePropertyGetter()));            
        }

        [Test]
        public void ShouldThrowWhenTryingToCreateACallToAGetterThatDoesNotExist()
        {
            var callToSetter = new FakeCall(typeof(string), GetWriteOnlyPropertySetter(), null, new[] { "arg" });
            Assert.Throws<InvalidOperationException>(() => sut.CreateCallToPropertyGetterFromSetterCall(callToSetter));
        }

        MethodInfo GetSomePropertyGetter() { return GetMethodOnHelperClass("get_SomeProperty"); }
        MethodInfo GetSomePropertySetter() { return GetMethodOnHelperClass("set_SomeProperty"); }
        MethodInfo GetSomeMethod() { return GetMethodOnHelperClass("SomeMethod"); }
        MethodInfo GetWriteOnlyPropertySetter() { return GetMethodOnHelperClass("set_WriteOnlyProperty"); }
        MethodInfo GetMethodOnHelperClass(string name) { return typeof(ClassToTestPropertyHelper).GetMethod(name); }

        class ClassToTestPropertyHelper
        {
            public string SomeProperty { get; set; }
            public void SomeMethod() { }
            public string WriteOnlyProperty { set {}}
        }
    }
}