using System;
using System.Runtime.Serialization;

namespace NSubstitute.Specs.Infrastructure
{
    public class TemporaryChangeNotConfiguredProperlyException : Exception
    {
        public TemporaryChangeNotConfiguredProperlyException()
        {
        }

        public TemporaryChangeNotConfiguredProperlyException(string message) : base(message)
        {
        }

        public TemporaryChangeNotConfiguredProperlyException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected TemporaryChangeNotConfiguredProperlyException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}