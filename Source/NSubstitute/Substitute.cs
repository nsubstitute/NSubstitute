namespace NSubstitute
{
    public class Substitute
    {
        public static T For<T>()
        {
            var substituteFactory = SubstitutionContext.Current.GetSubstituteFactory();
            return substituteFactory.Create<T>();
        }
    }
}