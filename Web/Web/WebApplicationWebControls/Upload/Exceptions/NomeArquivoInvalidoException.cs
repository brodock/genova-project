using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplicationWebControls.Upload.Exceptions
{
    [global::System.Serializable]
    public class NomeArquivoInvalidoException : Exception
    {        
        public NomeArquivoInvalidoException() { }
        public NomeArquivoInvalidoException(string message) : base(message) { }
        public NomeArquivoInvalidoException(string message, Exception inner) : base(message, inner) { }
        protected NomeArquivoInvalidoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
