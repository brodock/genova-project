using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public abstract class Erros
    {
        #region Atributos
        public static readonly string MsgSenhaInvalida = "Invalid Password.";
        public static readonly string MsgUsuarioInexistente = "User not exists.";
        #endregion

        #region Métodos
        public static string ValorInvalido(string objetoNome, string atributoNome)
        {
            return String.Format("[{0}] - [{1}] invalid value.", objetoNome, atributoNome);
        }

        public static string ObjetoInvalido(string objetoNome)
        {
            return String.Format("Object [{0}] invalid or not found.");
        }

        public static string ConversaoInvalida(string objetoNome, string metodoNome)
        {
            return String.Format("[{0}] - The method [{1}] returned an invalid value.", objetoNome, metodoNome);
        }

        public static string NaoImplementado(string objetoNome, string metodoNome)
        {
            return String.Format("[{0}] - The method [{1}] was not/is implemented in the object request.", objetoNome, metodoNome);
        }

        public static string NaoEncontrado(string objetoNome)
        {
            return String.Format("Could not find records for [{0}].", objetoNome);
        }
        #endregion
    }
}
