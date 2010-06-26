using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using NSubstitute.Core;

namespace NSubstitute.Specs
{
    public class ReflectionExtensions : StaticConcern
    {
        [Test]
        public void Should_get_propertyinfo_from_setter_call()
        {
            var property = typeof(ISample).GetProperty("Property");
            var propertySetter = property.GetSetMethod();
            Assert.That(propertySetter.GetPropertyFromSetterCallOrNull(), Is.EqualTo(property));
        }

        [Test]
        public void Should_return_null_if_method_is_not_a_property_setter()
        {
            var notAPropertySetter = typeof(ISample).GetMethod("OtherMethod");
            Assert.That(notAPropertySetter.GetPropertyFromSetterCallOrNull(), Is.Null);
        }

        public interface ISample
        {
            int Property { get; set; }
            void OtherMethod();
        }
    }
}