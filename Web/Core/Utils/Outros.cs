using System;
using System.Collections.Generic;
using System.Text;

namespace Utils
{
    public abstract class Outros
    {
        #region Atributos Internos
        private static readonly string ValorDefault = "-";
        private static readonly string Virgula = ",";
        #endregion

        public static string InserirVirgula(int contador, int tamanho)
        {
            return contador.Equals(tamanho) ? string.Empty : Virgula;
        }

        public static bool Comparar(Type comparar, Type comparado)
        {
            return comparar.Equals(comparado);
        }

        public static string StringDefault(string valor)
        {
            return string.IsNullOrEmpty(valor) ? ValorDefault : valor;
        }
    }
}
