using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Utils;

namespace Security.Criptografia
{
    public class CriptografiaMD5 : Criptografia
    {
        public CriptografiaMD5()
            : base(TipoCriptografia.MD5)
        {
        }

        public override string Criptografar(string texto)
        {
            MD5CryptoServiceProvider provider = new MD5CryptoServiceProvider();
            byte[] inputBuffer = provider.ComputeHash(base.Encoder.GetBytes(texto));
            return Conversoes.Base64String(inputBuffer);
        }

        public override string Descriptografar(string texto)
        {
            throw new NotImplementedException(Erros.NaoImplementado("CriptografiaMD5","Descriptografar"));
        }

        public override bool Comparar(string texto, string hash)
        {
            string textoCriptografado = this.Criptografar(texto);

            StringComparer comparador = StringComparer.OrdinalIgnoreCase;
            if (0 == comparador.Compare(textoCriptografado, hash))
                return true;
            else
                return false;
        }
    }
}
