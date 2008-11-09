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
using Utils;
using CoreBusiness.Controller;

namespace Web.Business.ShardUser
{
    public partial class CharactersView : PaginaBaseSegura
    {
        #region Properties
        protected int IdAccount
        {
            get { return Conversoes.Inteiro32(this.ddlAccount.SelectedValue); }
        }
        #endregion

        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.Reset();
                this.PrepareViewItems();
            }
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateViewItems();
                this.PrepareGridCharacters();
                if (this.gridCharacters.Rows.Count <= 0) throw new Exception(Erros.NaoEncontrado("Characters"));
            }
            catch (Exception erro) { base.ExibirMensagemAlerta(erro.Message); }
        }
        protected void gridCharacters_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblStatus = e.Row.FindControl("lblStatus") as Label;
                if (lblStatus.Text.Equals("1"))
                    lblStatus.Text = "True";
                else
                    lblStatus.Text = "Não";
            }
        }
        #endregion

        #region Methods
        private void Reset()
        {
            this.ddlAccount.SelectedIndex = 0;
        }
        protected void PrepareViewItems()
        {
            this.ddlAccount.DataSource = ControladorShardAccount.GetAccounts(base.RecuperarLoginUsuarioLogado()).GetDataSet();
            this.ddlAccount.DataValueField = "IdUsuario";
            this.ddlAccount.DataTextField = "Login";
            this.ddlAccount.DataBind();

            this.ddlAccount.Items.Insert(0, new ListItem("..:: No account to select ::..", "0"));
        }
        private bool ValidateViewItems()
        {
            if (this.IdAccount <= 0)
                throw new Exception("Select Account.");

            return true;
        }
        protected void PrepareGridCharacters()
        {
            this.gridCharacters.DataSource = ControladorCharacterAccount.GetCharactersAccount(this.IdAccount).GetDataSet();
            this.gridCharacters.DataBind();
        }
        #endregion
    }
}