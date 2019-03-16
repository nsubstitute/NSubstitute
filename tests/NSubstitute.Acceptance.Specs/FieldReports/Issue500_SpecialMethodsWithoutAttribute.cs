using System;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs.FieldReports
{
    public class Issue500_SpecialMethodsWithoutAttribute
    {
        private const string EventName = "MyEvent";
        private const string PropertyName = "MyProperty";
        private static readonly Type TypeWithMissingSpecialNameMethodAttributes;

        static Issue500_SpecialMethodsWithoutAttribute()
        {
            TypeWithMissingSpecialNameMethodAttributes = GenerateTypeWithMissingSpecialNameAttributes();
        }

        [Test]
        public void ShouldCorrectlyConfigureProperty()
        {
            var substitute = Substitute.For(new[] {TypeWithMissingSpecialNameMethodAttributes}, new object[0]);
            var fixture = new GeneratedTypeFixture(substitute);

            fixture.MyProperty = "42";

            var result = fixture.MyProperty;
            Assert.That(result, Is.EqualTo("42"));
        }

        [Test]
        public void ShouldCorrectlyConfigureEvent()
        {
            object substitute = Substitute.For(new[] {TypeWithMissingSpecialNameMethodAttributes}, new object[0]);
            var fixture = new GeneratedTypeFixture(substitute);

            bool wasCalled = false;
            fixture.MyEvent += (sender, args) => wasCalled = true;
            fixture.MyEvent += Raise.Event();

            Assert.That(wasCalled, Is.EqualTo(true));
        }

        private static Type GenerateTypeWithMissingSpecialNameAttributes()
        {
            const string assemblyName = "Issue500_SpecialMethodsWithoutAttribute";

            var assembly = AssemblyBuilder
                .DefineDynamicAssembly(
                    new AssemblyName(assemblyName),
                    AssemblyBuilderAccess.Run);
            var module = assembly.DefineDynamicModule(assemblyName);
            var typeBuilder = module.DefineType("TypeWithMissingSpecialAttributes",
                TypeAttributes.Public | TypeAttributes.Abstract);
 
            var evBuilder = typeBuilder.DefineEvent(EventName, EventAttributes.None, typeof(EventHandler));
            var evAdder = typeBuilder.DefineMethod(
                $"add_{EventName}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot /* | MethodAttributes.SpecialName */,
                typeof(void),
                new[] {typeof(EventHandler)});
            var evRemover = typeBuilder.DefineMethod(
                $"remove_{EventName}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot /* | MethodAttributes.SpecialName */,
                typeof(void),
                new[] {typeof(EventHandler)});
            evBuilder.SetAddOnMethod(evAdder);
            evBuilder.SetRemoveOnMethod(evRemover);

            var propBuilder =
                typeBuilder.DefineProperty(PropertyName, PropertyAttributes.None, typeof(string), Type.EmptyTypes);
            var propGetter = typeBuilder.DefineMethod(
                $"get_{PropertyName}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot /* | MethodAttributes.SpecialName */,
                typeof(object),
                Type.EmptyTypes);
            var propSetter = typeBuilder.DefineMethod(
                $"set_{PropertyName}",
                MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.Abstract |
                MethodAttributes.HideBySig | MethodAttributes.NewSlot /* | MethodAttributes.SpecialName */,
                typeof(void),
                new[] {typeof(object)});
            propBuilder.SetGetMethod(propGetter);
            propBuilder.SetSetMethod(propSetter);

            return typeBuilder.CreateTypeInfo().AsType();
        }

        private class GeneratedTypeFixture
        {
            private readonly object _substitute;

            public GeneratedTypeFixture(object substitute)
            {
                _substitute = substitute;
            }

            public object MyProperty
            {
                get => _substitute.GetType().GetProperty(PropertyName).GetValue(_substitute);
                set => _substitute.GetType().GetProperty(PropertyName).SetValue(_substitute, value);
            }

            public event EventHandler MyEvent
            {
                add => _substitute.GetType().GetEvent(EventName).AddEventHandler(_substitute, value);
                remove => _substitute.GetType().GetEvent(EventName).RemoveEventHandler(_substitute, value);
            }
        }
    }
}