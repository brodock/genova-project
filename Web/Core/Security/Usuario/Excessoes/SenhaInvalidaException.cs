using System;
using System.Runtime.Serialization;

namespace Security.Usuario.Excessoes
{
    [global::System.Serializable]
    public class SenhaInvalidaException : Exception
    {        
        public SenhaInvalidaException() { }
        public SenhaInvalidaException(string message) : base(message) { }
        public SenhaInvalidaException(string message, Exception inner) : base(message, inner) { }
        protected SenhaInvalidaException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
