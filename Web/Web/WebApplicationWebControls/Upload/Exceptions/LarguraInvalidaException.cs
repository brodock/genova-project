using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplicationWebControls.Upload.Exceptions
{
    [global::System.Serializable]
    public class LarguraInvalidaException : Exception
    {
        public LarguraInvalidaException() { }
        public LarguraInvalidaException(string message) : base(message) { }
        public LarguraInvalidaException(string message, Exception inner) : base(message, inner) { }
        protected LarguraInvalidaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
