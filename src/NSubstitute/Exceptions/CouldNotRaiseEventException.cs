using System;
#if NET35 || NET4 || NET45
using System.Runtime.Serialization;
#endif

namespace NSubstitute.Exceptions
{
#if NET35 || NET4 || NET45
    [Serializable]
#endif
    public class CouldNotRaiseEventException : SubstituteException
    {
        protected const string WhatProbablyWentWrong =
            "Make sure you are using Raise.Event() as part of an event subscription on a substitute.\n" +
            "For example:\n" +
            "\tmySub.Clicked += Raise.Event();\n" +
            "\n" +
            "If you substituted for a class rather than an interface, check that the event on your substitute is virtual/abstract.\n" +
            "Events on classes cannot be raised if they are not declared virtual or abstract.\n" +
            "\n" +
            "Note that the source of the problem may be prior to where this exception was thrown (possibly in a previous test!).\n" +
            "For example:\n" +
            "\tvar notASub = new Button();\n" +
            "\tnotASub.Clicked += Raise.Event(); // <-- Problem here. This is not a substitute.\n" +
            "\tvar sub = Substitute.For<IController>();\n" +
            "\tsub.Load(); // <-- Exception thrown here. NSubstitute thinks the earlier Raise.Event() was meant for this call.";

        public CouldNotRaiseEventException() : base(WhatProbablyWentWrong) { }
#if NET35 || NET4 || NET45
        protected CouldNotRaiseEventException(SerializationInfo info, StreamingContext context) : base(info, context) { }
#endif
    }
}
