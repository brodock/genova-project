using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplicationWebControls.Upload.Exceptions
{
    [global::System.Serializable]
    public class ArquivoInvalidoException : Exception
    {        
        public ArquivoInvalidoException() { }
        public ArquivoInvalidoException(string message) : base(message) { }
        public ArquivoInvalidoException(string message, Exception inner) : base(message, inner) { }
        protected ArquivoInvalidoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
