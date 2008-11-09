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
using WebApplicationController;
using CoreBusiness.Enumerator;
using Utils;
using CoreBusiness.Controller;

namespace Web.Business.ShardUser
{
    public partial class Register : PaginaBaseSegura
    {
        #region Properties
        protected string Login
        {
            get { return this.txtLogin.Text; }
        }
        protected string Password
        {
            get { return this.txtPassword.Text; }
        }
        protected EnumActionShardAccount Action
        {
            get { return (EnumActionShardAccount)Conversoes.Inteiro32(this.rblAction.SelectedValue); }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Reset();
                this.PrepareScripts();
            }
        }
        protected void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateViewItems();
                if (this.Action == EnumActionShardAccount.NewAccount)
                    ControladorShardAccount.Manter(int.MinValue, this.RecuperarLoginUsuarioLogado(), this.Login, this.Password, string.Empty, string.Empty, false, false, null);
                else
                    ControladorUsuarioConta.LinkingWebAccountToShardAccount(this.RecuperarLoginUsuarioLogado(), this.Login, this.Password);

                this.ExibirMensagemAlerta("your account has been created/associated with success.");
                this.btnRegister.Enabled = false;
            }
            catch (Exception erro) { base.ExibirMensagemAlerta(erro.Message); }
        }
        #endregion

        #region Methods
        private void Reset()
        {
            this.txtLogin.Text = string.Empty;
            this.txtPassword.Text = string.Empty;
            this.rblAction.SelectedIndex = 0;
        }
        private void PrepareScripts()
        {
            string confirmRegister = "return confirm('All information was completed correctly?');";
            this.btnRegister.Attributes.Add("onclick", confirmRegister);
        }
        private bool ValidateViewItems()
        {
            if (string.IsNullOrEmpty(this.txtLogin.Text))
                throw new Exception(Erros.ValorInvalido("Shard Account", "Login ID"));

            if (string.IsNullOrEmpty(this.txtPassword.Text))
                throw new Exception(Erros.ValorInvalido("Shard Account", "Password"));

            return true;
        }
        #endregion
    }
}