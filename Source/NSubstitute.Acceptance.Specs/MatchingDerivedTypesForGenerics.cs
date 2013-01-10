using NUnit.Framework;

namespace NSubstitute.Acceptance.Specs
{
	[TestFixture]
	public class MatchingDerivedTypesForGenerics
	{
		IGenMethod _sub;

		[SetUp]
		public void Setup()
		{
			_sub = Substitute.For<IGenMethod>();
		}


		[Test]
		public void Calls_to_generic_types_with_derived_parameters_should_be_matched ()
		{
			_sub.Call(new GMParam1());
			_sub.Call(new GMParam2());

			_sub.Received(2).Call(Arg.Any<IGMParam>());
		}


		public interface IGenMethod
		{
			void Call<T>(T param) where T : IGMParam;
		}
		public interface IGMParam { }
		public class GMParam1 : IGMParam { }
		public class GMParam2 : IGMParam { }
	}

}
