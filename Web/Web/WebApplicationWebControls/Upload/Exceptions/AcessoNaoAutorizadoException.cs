using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplicationWebControls.Upload.Exceptions
{
    [global::System.Serializable]
    public class AcessoNaoAutorizadoException : Exception
    {        
        public AcessoNaoAutorizadoException() { }
        public AcessoNaoAutorizadoException(string message) : base(message) { }
        public AcessoNaoAutorizadoException(string message, Exception inner) : base(message, inner) { }
        protected AcessoNaoAutorizadoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
