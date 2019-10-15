using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Kmd.Logic.Consent.Client
{
    [Serializable]
    public class ConsentValidationException : Exception
    {
        public IDictionary<string, IList<string>> ValidationErrors { get; }

        public ConsentValidationException()
        {
        }

        public ConsentValidationException(IDictionary<string, IList<string>> validationErrors)
            : base(GenerateMessage(validationErrors))
        {
            this.ValidationErrors = validationErrors;
        }

        public ConsentValidationException(string message, IDictionary<string, IList<string>> validationErrors)
            : base(message)
        {
            this.ValidationErrors = validationErrors;
        }

        public ConsentValidationException(string message, IDictionary<string, IList<string>> validationErrors, Exception innerException)
            : base(message, innerException)
        {
            this.ValidationErrors = validationErrors;
        }

        public ConsentValidationException(IDictionary<string, IList<string>> validationErrors, Exception innerException)
            : base(GenerateMessage(validationErrors), innerException)
        {
        }

        public ConsentValidationException(string message)
            : base(message)
        {
        }

        public ConsentValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ConsentValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        private static string GenerateMessage(IDictionary<string, IList<string>> validationErrors)
        {
            var message = "Invalid consent parameters";

            if (validationErrors != null && validationErrors.Count > 0)
            {
                message += "(" + string.Join(";", validationErrors.Select(x => $"{x.Key}: {string.Join(",", x.Value)}")) + ")";
            }

            return message;
        }
    }
}