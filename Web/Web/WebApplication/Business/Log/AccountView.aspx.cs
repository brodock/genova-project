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
using CoreBusiness.Controller;
using Utils;

namespace Web.Business.Log
{
    public partial class AccountView : PaginaBaseSegura
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
                this.PrepareGridLog();
                if (this.gridLog.Rows.Count <= 0) throw new Exception(Erros.NaoEncontrado("Logs"));
            }
            catch (Exception erro) { base.ExibirMensagemAlerta(erro.Message); }
        }
        protected void gridLog_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.gridLog.PageIndex = e.NewPageIndex;
            this.PrepareGridLog();
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
        protected void PrepareGridLog()
        {
            this.gridLog.DataSource = ControladorConnectionLog.GetLogs(this.IdAccount).GetDataSet();
            this.gridLog.DataBind();
        }
        #endregion
    }
}