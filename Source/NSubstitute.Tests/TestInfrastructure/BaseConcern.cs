using System.Collections.Generic;
using NUnit.Framework;

namespace NSubstitute.Tests.TestInfrastructure
{
    public abstract class BaseConcern
    {
        IList<ITemporaryChange> _temporaryChanges;
        public virtual void Context() { }
        public virtual void AfterContextEstablished() { }
        public virtual void Because() { }
        void ReverseTemporaryChanges() { foreach (var change in _temporaryChanges) { change.RestoreOriginalValue(); } }

        [SetUp]
        public void SetUp()
        {
            _temporaryChanges = new List<ITemporaryChange>();
            Context();
            AfterContextEstablished();
            Because();
        }

        [TearDown]
        public void TearDown()
        {
            ReverseTemporaryChanges();
        }

        protected T mock<T>() where T : class
        {
            return MockingAdaptor.Create<T>();
        }

        protected TemporaryChangeToBuilder<T> temporarilyChange<T>(T value)
        {
            var temporaryChange = new TemporaryChange<T>(value);
            _temporaryChanges.Add(temporaryChange);
            return new TemporaryChangeToBuilder<T>(temporaryChange);
        }
    }
}