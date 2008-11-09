using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Security.Usuario.Controladores;
using WebApplicationController;
using Utils;
using System.Text;

namespace Web.UserControls
{
    public partial class WebUser : UserControl
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (ControladorUsuario.IsUsuarioLogado())
                    this.Logged(true);
                else
                    this.Logged(false);

                this.PrepareScripts();
            }
        }
        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string login = this.txtLogin.Text;
            string password = this.txtPassword.Text;

            try
            {
                ControladorUsuario.Autenticar(login, password);
                this.Logged(true);
                (Page as PaginaBase).ReloadPaginaAtual(true);
            }
            catch (Exception error) { Mensagens.MostrarAlerta(this.Page, error.Message); }
        }
        protected void btnLogout_Click(object sender, EventArgs e)
        {
            ControladorUsuario.Desconectar();
            this.Logged(false);
            (Page as PaginaBase).ReloadPaginaAtual(true);
        }
        #endregion

        #region Methods
        private void Reset()
        {
            this.imgUserAvatar.ImageUrl = ConfiguracoesWeb.ExibicaoPadrao;
            this.txtLogin.Text = string.Empty;
            this.txtPassword.Text = string.Empty;
            this.lblUserLogin.Text = string.Empty;
            this.lblUserType.Text = string.Empty;
        }
        private void Logged(bool logged)
        {
            notLoggedConfirm.Visible = !logged;
            notLoggedLogin.Visible = !logged;
            notLoggedPassword.Visible = !logged;
            notLoggedInfo.Visible = !logged;

            loggedConfirm.Visible = logged;
            loggedLogin.Visible = logged;
            loggedPassword.Visible = logged;

            if (!logged)
                this.Reset();
            else
            {
                this.imgUserAvatar.ImageUrl = ControladorUsuario.GetAvatarUsuarioLogado();
                this.lblUserLogin.Text = ControladorUsuario.GetLoginUsuarioLogado();
                this.lblUserType.Text = ControladorUsuario.GetTipoUsuarioLogado();
            }
        }
        private void PrepareScripts()
        {
            string confirmLogout = "return confirm('Do you really want to disconnect?');";
            this.btnLogout.Attributes.Add("onclick", confirmLogout);

            StringBuilder clickRegister = new StringBuilder();
            clickRegister.AppendFormat(@"
            if ((event.which && event.which == 13) || (event.keyCode && event.keyCode == 13))
            {{
	            document.forms[0].elements['{0}'].click();
	            return false;
            }} 
            else 
	            return true;
            ", this.btnLogin.UniqueID);
            this.txtLogin.Attributes.Add("onkeydown", clickRegister.ToString());
            this.txtPassword.Attributes.Add("onkeydown", clickRegister.ToString());
        }
        #endregion
    }
}