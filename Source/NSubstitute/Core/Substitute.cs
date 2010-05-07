namespace NSubstitute.Core
{
    public class Substitute
    {
        public static T For<T>() where T : class
        {
            var substituteFactory = SubstitutionContext.Current.GetSubstituteFactory();
            return substituteFactory.Create<T>();
        }
    }
}