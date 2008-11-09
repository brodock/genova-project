using System;
using System.Collections.Generic;
using System.Text;

namespace Security.Usuario.Excessoes
{
    [global::System.Serializable]
    public class AlterarSenhaDadosIncompletosException : Exception
    {
        public AlterarSenhaDadosIncompletosException() { }
        public AlterarSenhaDadosIncompletosException(string message) : base(message) { }
        public AlterarSenhaDadosIncompletosException(string message, Exception inner) : base(message, inner) { }
        protected AlterarSenhaDadosIncompletosException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
