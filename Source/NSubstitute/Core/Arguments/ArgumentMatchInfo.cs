namespace NSubstitute.Core.Arguments
{
    public class ArgumentMatchInfo
    {
        public ArgumentMatchInfo(int index, object argument, IArgumentSpecification specification)
        {
            Index = index;
            Argument = argument;
            Specification = specification;
        }

        public int Index { get; private set; }
        public object Argument { get; private set; }
        public IArgumentSpecification Specification { get; private set; }

        public bool IsMatch { get { return Specification.IsSatisfiedBy(Argument); } }
    }
}