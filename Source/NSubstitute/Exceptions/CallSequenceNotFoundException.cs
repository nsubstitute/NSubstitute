﻿using System.Runtime.Serialization;

namespace NSubstitute.Exceptions
{
    public class CallSequenceNotFoundException : SubstituteException
    {
        public CallSequenceNotFoundException(string message) : base(message) { }
    }
}