using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using NUnit.Common;
using NUnitLite;

namespace NSubstitute.Acceptance.Spec.Runner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            new AutoRun(typeof(Program).GetTypeInfo().Assembly)
               .Execute(args, new ExtendedTextWrapper(Console.Out), Console.In);
        }
    }
}
