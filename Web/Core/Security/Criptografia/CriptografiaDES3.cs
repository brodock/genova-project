using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;
using Utils;

namespace Security.Criptografia
{
    public class CriptografiaDES3 : Criptografia
    {
        #region Constantes Privadas
        private readonly string Chave = "Am0ret3r40-2oo08";
        #endregion

        public CriptografiaDES3()
            : base(TipoCriptografia.DES3)
        {
        }

        public override string Criptografar(string texto)
        {
            TripleDESCryptoServiceProvider provider = this.ProviderDES3();
            byte[] bytes = base.Encoder.GetBytes(texto);
            return Conversoes.Base64String(provider.CreateEncryptor().TransformFinalBlock(bytes, 0, bytes.Length));
        }

        public override string Descriptografar(string texto)
        {
            TripleDESCryptoServiceProvider provider = this.ProviderDES3();
            byte[] inputBuffer = Conversoes.DeBase64String(texto);
            return Encoder.GetString(provider.CreateDecryptor().TransformFinalBlock(inputBuffer, 0, inputBuffer.Length));
        }

        public override bool Comparar(string texto, string hash)
        {
            throw new NotImplementedException(Erros.NaoImplementado("CriptografiaDES3", "Comparar"));
        }

        #region Métodos Internos
        private TripleDESCryptoServiceProvider ProviderDES3()
        {
            TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider();
            provider.Key = this.Encoder.GetBytes(this.Chave.ToCharArray());
            provider.Mode = CipherMode.ECB;
            return provider;
        }
        #endregion
    }
}
