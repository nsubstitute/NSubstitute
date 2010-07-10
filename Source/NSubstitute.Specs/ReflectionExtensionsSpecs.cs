using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;
using NSubstitute.Core;

namespace NSubstitute.Specs
{
    public abstract class ReflectionExtensionsSpecs
    {
        public class When_getting_property_setter_from_call : StaticConcern
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
        }

        public class When_getting_property_getter_from_call : StaticConcern
        {
            [Test]
            public void Should_get_propertyinfo_from_getter_call()
            {
                var property = typeof(ISample).GetProperty("Property");
                var propertyGetter = property.GetGetMethod();
                Assert.That(propertyGetter.GetPropertyFromGetterCallOrNull(), Is.EqualTo(property));
            }

            [Test]
            public void Should_return_null_if_method_if_not_a_property_getter()
            {
                var notAGetter = typeof(ISample).GetMethod("OtherMethod");
                Assert.That(notAGetter.GetPropertyFromGetterCallOrNull(), Is.Null);
            }
        }

        public interface ISample
        {
            int Property { get; set; }
            void OtherMethod();
        }
    }
}