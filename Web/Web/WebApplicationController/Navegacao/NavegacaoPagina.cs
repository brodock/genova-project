using System;
using System.Collections.Generic;
using System.Text;
using WebApplicationController.Enumeradores;
using WebApplicationController.Mapa;
using System.Web;
using Security.Criptografia;

namespace WebApplicationController.Navegacao
{
    public class NavegacaoPagina
    {
        #region Atributos
        private static Criptografia _criptografia = new CriptografiaDES3();
        #endregion

        #region Métodos
        public static void AbrirPagina(int tipo, string parametrosBrowser)
        {
            string parametros = _criptografia.Criptografar(parametrosBrowser);

            string nomePagina = NavegacaoUtils.GetTipoSolicitacaoUrl((TipoSolicitacao)tipo);

            StringBuilder pagina = new StringBuilder();
            pagina.AppendFormat("~/{0}?parametros={1}", nomePagina, HttpContext.Current.Server.UrlEncode(parametros));
            HttpContext.Current.Response.Redirect(pagina.ToString());
        }

        public static string GetUrlPagina(object objTipo, string parametrosBrowser)
        {
            int tipo = Convert.ToInt32(objTipo.ToString());
            string parametros = _criptografia.Criptografar(parametrosBrowser);

            string nomePagina = NavegacaoUtils.GetTipoSolicitacaoUrl((TipoSolicitacao)tipo);

            StringBuilder pagina = new StringBuilder();
            pagina.AppendFormat("{0}?parametros={1}", nomePagina, HttpContext.Current.Server.UrlEncode(parametros));
            return pagina.ToString();
        }

        public static string RecuperarParametro(string parametro)
        {
            string parametrosRequest = HttpContext.Current.Request["parametros"];

            if (String.IsNullOrEmpty(parametrosRequest))
                return null;

            string parametrosDescriptografados = _criptografia.Descriptografar(parametrosRequest);
            return GetValorParametro(parametro, parametrosDescriptografados);
        }

        public static void RedirecionarPaginaPrincipal()
        {
            string paginaPrincipal = string.Concat("~/", PrincipalMapa.Principal);
            HttpContext.Current.Response.Redirect(paginaPrincipal);
        }
        #endregion

        #region Métodos Auxiliares
        private static string GetValorParametro(string parametroSolicitado, string parametrosDescriptografados)
        {
            string[] listaParametros = parametrosDescriptografados.Split("&".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in listaParametros)
            {
                string[] inferior = item.Split('=');
                if (parametroSolicitado.Equals(inferior[0]))
                    return inferior[1];
            }
            return string.Empty;
        }
        #endregion
    }
}
