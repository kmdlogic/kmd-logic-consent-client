using System;
using System.Runtime.Serialization;

namespace Kmd.Logic.Consent.Client
{
    [Serializable]
    public class ConsentConfigurationException : Exception
    {
        public string InnerMessage { get; }

        public ConsentConfigurationException()
        {
        }

        public ConsentConfigurationException(string message)
            : base(message)
        {
        }

        public ConsentConfigurationException(string message, string innerMessage)
            : base(message)
        {
            this.InnerMessage = innerMessage;
        }

        public ConsentConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConsentConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}