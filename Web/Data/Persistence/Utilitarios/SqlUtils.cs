using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Utilitarios
{
    public abstract class SqlUtils
    {
        /// <summary>
        /// Previnindo caracteres que podem causar sql Injection.
        /// </summary>
        public static void PrevencaoSqlInjection(ref string parametro)
        {
            parametro = parametro.Replace(";", string.Empty);
            parametro = parametro.Replace("#", string.Empty);
            parametro = parametro.Replace("-- ", string.Empty);
            parametro = parametro.Replace("--", string.Empty);
        }

        /// <summary>
        /// Prevenção interna - caracteres que podem causar sql Injection.
        /// </summary>
        internal static string PrevencaoInternaSqlInjection(object valor)
        {
            string parametro = valor.ToString();
            PrevencaoSqlInjection(ref parametro);
            return parametro;
        }
    }
}
