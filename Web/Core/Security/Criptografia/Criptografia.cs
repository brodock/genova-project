using System;
using System.Text;

namespace Security.Criptografia
{
    public abstract class Criptografia
    {        
        #region Atributos
        private TipoCriptografia _tipo;
        #endregion

        #region Propriedades
        protected UTF8Encoding Encoder
        {
            get { return new UTF8Encoding(); }
        }
        #endregion

        public Criptografia(TipoCriptografia tipoCriptografia)
        {
            this._tipo = tipoCriptografia;
        }

        #region Métodos Abstratos
        public abstract string Criptografar(string texto);
        public abstract string Descriptografar(string texto);
        public abstract bool Comparar(string texto, string hash);
        #endregion
    }
}