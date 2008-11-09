using System;
using System.Collections.Generic;
using System.Text;
using Utils.Ambiente;
using System.Globalization;

namespace Utils
{
    public abstract class Conversoes
    {
        #region Int16
        public static Int16 Inteiro16(object valor)
        {
            try
            {
                return Convert.ToInt16(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return Int16.MinValue;
            }
        }
        #endregion

        #region Int32
        public static Int32 Inteiro32(object valor)
        {
            try
            {
                return Convert.ToInt32(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return Int32.MinValue;
            }
        }
        #endregion

        #region Int64
        public static Int64 Inteiro64(object valor)
        {
            try
            {
                return Convert.ToInt64(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return Int64.MinValue;
            }
        }
        #endregion

        #region Double
        public static Double Double(object valor)
        {
            try
            {
                return Convert.ToDouble(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return double.MinValue;
            }
        }
        #endregion

        #region Decimal
        public static Decimal Decimal(object valor)
        {
            try
            {
                return Convert.ToDecimal(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return decimal.MinValue;
            }
        }
        #endregion

        #region Boolean
        public static bool Booleano(object valor)
        {
            try
            {
                return Convert.ToBoolean(valor, Sistema.Cultura);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region DateTime
        public static DateTime DataHora(object valor)
        {
            try
            {
                return Convert.ToDateTime(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }
        #endregion

        #region Char
        public static char Char(object valor)
        {
            try
            {
                return Convert.ToChar(valor.ToString(), Sistema.Cultura);
            }
            catch (Exception)
            {
                return char.MinValue;
            }
        }
        #endregion

        #region Base64String
        public static string Base64String(byte[] array)
        {
            try
            {
                return Convert.ToBase64String(array);
            }
            catch (Exception)
            {
                throw new Exception(Erros.ConversaoInvalida("Conversoes", "Base64String"));
            }
        }

        public static byte[] DeBase64String(string valor)
        {
            try
            {
                return Convert.FromBase64String(valor);
            }
            catch (Exception)
            {
                throw new Exception(Erros.ConversaoInvalida("Conversoes", "DeBase64String"));
            }
        }
        #endregion
    }
}