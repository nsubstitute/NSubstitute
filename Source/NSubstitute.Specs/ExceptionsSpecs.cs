using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using NSubstitute.Exceptions;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ExceptionsSpecs
    {
        private IEnumerable<Type> _exceptionTypes;

        [SetUp]
        public void SetUp()
        {
            _exceptionTypes = typeof(SubstituteException).Assembly.GetTypes().Where(x => x.Namespace != null && x.Namespace.StartsWith("NSubstitute.Exceptions"));
        }

        [Test]
        public void Check_that_out_setup_has_given_us_some_exception_types()
        {
            Assert.That(_exceptionTypes.Any());
        }

        [Test]
        public void All_nsub_exceptions_should_inherit_from_SubstituteException()
        {
            foreach (var exceptionType in _exceptionTypes)
            {
                if (!typeof(SubstituteException).IsAssignableFrom(exceptionType))
                {
                    Assert.Fail("{0} is not a SubstituteException", exceptionType);
                }
            }
        }

        [Test]
        public void All_nsub_exceptions_should_implement_serialisation_ctor_so_its_stacktrace_can_be_preserved()
        {
            /* This is meant to be good practice (http://msdn.microsoft.com/en-us/library/ms229064.aspx), but
             * at one stage was also (or still is?) used to preserve stack traces from RaiseEventHandler.
             */
            foreach (var exceptionType in _exceptionTypes)
            {
                Assert.That(
                    exceptionType
                        .GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
                        .Any(IsSerialisationConstructor)
                , "{0} does not have a protected serialisation constructor. i.e. ctor(SerializationInfo, StreamingContext).", exceptionType);
            }
        }

        private bool IsSerialisationConstructor(ConstructorInfo ctor)
        {
            var ctorParameterTypes = ctor.GetParameters().Select(x => x.ParameterType).ToArray();
            if (ctorParameterTypes.Length != 2) return false;

            if (ctorParameterTypes[0] != typeof(SerializationInfo)) return false;
            if (ctorParameterTypes[1] != typeof(StreamingContext)) return false;
            return true;
        }
    }
}