using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI;
using Security.Usuario.Controladores;
using WebApplicationController.Navegacao;
using System.Reflection;

namespace WebApplicationController
{
    public class PaginaBase : Page
    {
        #region Métodos - Usuário
        public string RecuperarLoginUsuarioLogado()
        {
            return ControladorUsuario.GetLoginUsuarioLogado();
        }
        #endregion

        #region Métodos - Navegação
        public void IrParaPaginaInicial()
        {
            NavegacaoPagina.RedirecionarPaginaPrincipal();
        }

        public string RecuperarParametro(string parametro)
        {
            return NavegacaoPagina.RecuperarParametro(parametro);
        }
        #endregion

        #region Métodos - Customizados (JavaScript)
        protected void ExibirMensagemAlerta(string mensagem)
        {
            Mensagens.MostrarAlerta(this, mensagem);
        }

        public void ReloadPaginaAtual(bool forcarPostBack)
        {
            StringBuilder javaScript = new StringBuilder();

            if (forcarPostBack)
                javaScript.AppendLine("self.location.href = self.location.href;");
            else
                javaScript.AppendLine("self.location.reload();");

            this.ClientScript.RegisterStartupScript(typeof(object), "thisreload", javaScript.ToString(), true);
        }

        protected void ReloadPaginaPai(bool fecharJanelaAtual)
        {
            this.ReloadPaginaPai(fecharJanelaAtual, false);
        }
        protected void ReloadPaginaPai(bool fecharJanelaAtual, bool forcarPostBack)
        {
            StringBuilder javaScript = new StringBuilder();
            
            if (forcarPostBack)
                javaScript.AppendLine("opener.location.href = opener.location.href;");
            else
                javaScript.AppendLine("opener.location.reload();");

            if (fecharJanelaAtual)
                javaScript.AppendLine("window.setTimeout(\"window.close()\", 500);");

            this.ClientScript.RegisterStartupScript(typeof(object), "parentreload", javaScript.ToString(), true);
        }

        protected void FecharJanelaAtual()
        {
            StringBuilder javaScript = new StringBuilder();
            javaScript.AppendLine("window.close();");

            this.ClientScript.RegisterStartupScript(typeof(object), "parentreload", javaScript.ToString(), true);
        }

        /// <summary>
        /// Abrir uma nova janela utilizando JavaScript. Necessário importar FuncoesUteis.js para seu .aspx.
        /// </summary>
        protected void SetOnClickParaAbrirNovaJanela(System.Web.UI.WebControls.WebControl controle, string pagina, StringBuilder parametros, bool paginaSegura)        
        {
            // Criptogrando parametros
            Security.Criptografia.Criptografia criptografia = new Security.Criptografia.CriptografiaDES3();
            string parametrosCriptografados = criptografia.Criptografar(parametros.ToString());

            // criando string url. UrlEncode necessario como prevenção (erro de conversão).
            string url = string.Format("{0}?parametros={1}", pagina, Server.UrlEncode(parametrosCriptografados));

            string nomePagina = "frmNovaJanela";
            if (paginaSegura)
                nomePagina = "frmAdministracaoJanela";

            // setando onclick para controle, abrir nova página.
            StringBuilder javaScript = new StringBuilder();
            javaScript.AppendFormat("AbrirJanelaCentroTela('{0}', '{1}', 600, 520);", url, nomePagina);
            controle.Attributes.Add("onclick", javaScript.ToString());
        }
        #endregion

        #region Métodos - Assembly
        public string GetApplicationVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString();
        }
        #endregion
    }
}
