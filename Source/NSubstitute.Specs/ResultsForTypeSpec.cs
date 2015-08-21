using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NSubstitute.Core;
using NSubstitute.Specs.Infrastructure;

namespace NSubstitute.Specs
{
    public class ResultsForTypeSpec
    {
        public abstract class Concern : ConcernFor<ResultsForType>
        {
            public override ResultsForType CreateSubjectUnderTest()
            {
                return new ResultsForType();
            }
        }
    }
}
