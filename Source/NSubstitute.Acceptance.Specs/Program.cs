using System;
using System.Reflection;
using NUnit.Common;
using NUnitLite;

namespace NSubstitute.Acceptance.Specs
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
