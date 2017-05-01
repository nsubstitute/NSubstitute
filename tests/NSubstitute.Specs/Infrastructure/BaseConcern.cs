using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using NUnit.Framework;

namespace NSubstitute.Specs.Infrastructure
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
            ThrowIfAnyTemporaryChangesNotConfigured();
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

        protected TemporaryChangeToBuilder<T> temporarilyChange<T>(Expression<Func<T>> property)
        {
            var memberExpression = (MemberExpression) property.Body;
            var member = memberExpression.Member;
            var originalValue = property.Compile().Invoke();
                        
            var temporaryChange = new TemporaryChange<T>(member, originalValue);
            _temporaryChanges.Add(temporaryChange);
            return new TemporaryChangeToBuilder<T>(temporaryChange);
        }

        void ThrowIfAnyTemporaryChangesNotConfigured()
        {
            foreach (var temporaryChange in _temporaryChanges)
            {
                if (!temporaryChange.IsConfigured)
                {
                    throw new TemporaryChangeNotConfiguredProperlyException(
                        "You tried to temporarily change the value of " + temporaryChange.MemberName +
                        " but did not say what you wanted to change it to.\n" +
                        "The required syntax is:\n\t" +
                        "temporarilyChange(SomeClass.SomeStaticMember).to(someTemporaryValue);"
                        );
                }
            }
        }


    }
}