using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVelocity.Exceptions
{
    public class NVRelatorioException : Exception
    {
        public NVRelatorioException() : base() { }
        public NVRelatorioException(string mensagem) : base(mensagem) { }
        public NVRelatorioException(string mensagem, Exception excessao) : base(mensagem, excessao) { }
    }
}
