using System;
using System.Collections.Generic;
using System.Text;

namespace WebApplicationWebControls.Upload.Exceptions
{
    [global::System.Serializable]
    public class ExtensaoInvalidaException : Exception
    {
        public ExtensaoInvalidaException() { }
        public ExtensaoInvalidaException(string message) : base(message) { }
        public ExtensaoInvalidaException(string message, Exception inner) : base(message, inner) { }
        protected ExtensaoInvalidaException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
