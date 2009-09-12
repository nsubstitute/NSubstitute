using System;
using NUnit.Framework;

namespace NSubstitute.Tests.TestInfrastructure
{
    public abstract class BaseConcern
    {
        public virtual void Context() { }
        public virtual void AfterContextEstablished() { }
        public virtual void Because() { }

        [SetUp]
        public void SetUp()
        {
            Context();
            AfterContextEstablished();
            Because();
        }

        protected T mock<T>() where T : class
        {
            return MockingAdaptor.Create<T>();
        }
    }
}