namespace NSubstitute.Specs.Infrastructure
{
    public abstract class ConcernFor<TSubjectUnderTest> : BaseConcern
    {
        public TSubjectUnderTest sut { get; private set; }

        public override void AfterContextEstablished()
        {
            sut = CreateSubjectUnderTest();
        }

        public abstract TSubjectUnderTest CreateSubjectUnderTest();
    }
}