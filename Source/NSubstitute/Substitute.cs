namespace NSubstitute
{
    public class Substitute
    {
        public static T For<T>()
        {
            return SubstitutionFactory.Current.Create<T>();
        }        
    }
}