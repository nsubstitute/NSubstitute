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

	[TestFixture]
	public class Calls_to_members_on_generic_interface
	{
		[Test]
		public void Calls_to_members_on_generic_interfaces_match_based_on_type_compatibility()
		{
			var sub = Substitute.For<IGenInterface<IGMParam>>();
			sub.Call(new GMParam1());
			sub.Call(new GMParam2());
			sub.Call(new GMParam3());

			sub.Received(2).Call(Arg.Any<IGMParam>());
			sub.Received(1).Call(Arg.Any<GMParam3>());
		}

		public interface IGenInterface<T>
		{
			void Call(T param);
			void Call(GMParam3 param);
		}

		public interface IGMParam { }
		public class GMParam1 : IGMParam { }
		public class GMParam2 : IGMParam { }
		public class GMParam3 { }

	}


}
