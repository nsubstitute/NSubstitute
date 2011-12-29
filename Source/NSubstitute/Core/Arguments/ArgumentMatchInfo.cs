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

        public bool Equals(ArgumentMatchInfo other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return other.Index == Index && Equals(other.Argument, Argument) && Equals(other.Specification, Specification);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (ArgumentMatchInfo)) return false;
            return Equals((ArgumentMatchInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int result = Index;
                result = (result*397) ^ (Argument != null ? Argument.GetHashCode() : 0);
                result = (result*397) ^ (Specification != null ? Specification.GetHashCode() : 0);
                return result;
            }
        }
    }
}