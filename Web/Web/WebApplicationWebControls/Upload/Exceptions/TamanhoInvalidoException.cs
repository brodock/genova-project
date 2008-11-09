using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplicationWebControls.Upload.Exceptions
{
    [global::System.Serializable]
    public class TamanhoInvalidoException : Exception
    {
        public TamanhoInvalidoException() { }
        public TamanhoInvalidoException(string message) : base(message) { }
        public TamanhoInvalidoException(string message, Exception inner) : base(message, inner) { }
        protected TamanhoInvalidoException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
