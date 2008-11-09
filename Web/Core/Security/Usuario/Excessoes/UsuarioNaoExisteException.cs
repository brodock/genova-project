using System;
using System.Runtime.Serialization;

namespace Security.Usuario.Excessoes
{
    [global::System.Serializable]
    public class UsuarioNaoExisteException : Exception
    {
        public UsuarioNaoExisteException() { }
        public UsuarioNaoExisteException(string message) : base(message) { }
        public UsuarioNaoExisteException(string message, Exception inner) : base(message, inner) { }
        protected UsuarioNaoExisteException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
