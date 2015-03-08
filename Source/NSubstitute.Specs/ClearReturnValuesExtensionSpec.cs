using System.Collections.Generic;
using System.Linq;
using NSubstitute.Specs.Infrastructure;
using NUnit.Framework;

namespace NSubstitute.Specs
{
    public class ClearReturnValuesExtensionSpec
    {
        public class When_clearing_return_values_on_a_substitute : StaticConcern
        {
			IFoo _substitute;

            [Test]
            public void Should_clear_calls_on_substitutes_call_router()
            {
	            var things = _substitute.GetThing();
				// TODO: better test for Castle.Proxies.IEnumerable`1Proxy - unsure of best practice for this
				Assert.AreEqual(0, things.ToArray().Length);
            }

	        public override void Because()
            {
				_substitute.ClearReturnValues();
            }

            public override void Context()
            {
				_substitute = Substitute.For<IFoo>();
	            _substitute.GetThing().Returns(new [] { 1 });
            }

	        public interface IFoo
	        {
		        IEnumerable<int> GetThing();
	        }
        }  
    }
}