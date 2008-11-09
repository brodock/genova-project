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
using Persistence.ControleTransacao;
using Security.Usuario.Controladores;
using CoreBusiness.Controller;
using Utils;

namespace Web.Business.WebUser
{
    public partial class Register : PaginaBase
    {
        #region Properties
        protected string Name
        {
            get { return this.txtName.Text; }
        }
        protected string Email
        {
            get { return this.txtEmail.Text; }
        }
        protected char Sex
        {
            get { return Conversoes.Char(this.rblSex.SelectedValue); }
        }
        protected DateTime BirthDate
        {
            get { return Conversoes.DataHora(this.txtBirthDate.Text); }
        }
        protected string Login
        {
            get { return this.txtLogin.Text; }
        }
        protected string Password
        {
            get { return this.txtPassword.Text; }
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

                // First, create User
                ColecaoPersistencia colecao = new ColecaoPersistencia();
                ControladorUsuario.Manter(int.MinValue, this.Login, this.Password, Security.Usuario.Enumeradores.TipoUsuario.Usuario, string.Empty, colecao);
                colecao.Persistir();

                // Now, create User Person
                colecao.Limpar();
                int idUsuario = ControladorUsuario.GetUsuarioPorLogin(this.Login).ID;
                ControladorPessoa.Manter(int.MinValue, idUsuario, this.Name, this.Email, this.Sex, this.BirthDate, colecao);
                colecao.Persistir();

                Mensagens.MostrarAlerta(this, "Your account has been created successfully.");
                this.btnRegister.Enabled = false;
            }
            catch (Exception error) { Mensagens.MostrarAlerta(this, error.Message); }
        }
        #endregion

        #region Methods
        private void Reset()
        {
            this.txtName.Text = string.Empty;
            this.txtEmail.Text = string.Empty;
            this.txtLogin.Text = string.Empty;
            this.txtPassword.Text = string.Empty;
        }
        private void PrepareScripts()
        {
            string confirmRegister = "return confirm('All information was completed correctly?');";
            this.btnRegister.Attributes.Add("onclick", confirmRegister);
        }
        private bool ValidateViewItems()
        {
            if (string.IsNullOrEmpty(this.txtName.Text))
                throw new Exception(Erros.ValorInvalido("Web Account", "Full Name"));

            if (string.IsNullOrEmpty(this.txtEmail.Text))
                throw new Exception(Erros.ValorInvalido("Web Account", "Email"));

            if (string.IsNullOrEmpty(this.txtLogin.Text))
                throw new Exception(Erros.ValorInvalido("Web Account", "Login"));

            if (string.IsNullOrEmpty(this.txtPassword.Text))
                throw new Exception(Erros.ValorInvalido("Web Account", "Password"));

            return true;
        }
        #endregion
    }
}