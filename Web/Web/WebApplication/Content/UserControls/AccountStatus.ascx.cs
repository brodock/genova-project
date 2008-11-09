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
using CoreBusiness.Controller;
using WebApplicationController;

namespace Web.UserControls
{
    public partial class AccountStatus : UserControl
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.PrepareViewItems();
                this.PrepareGridAccounts();
            }
        }
        protected void gridAccounts_RowDataBound(object sender, GridViewRowEventArgs e)
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
        protected void PrepareViewItems()
        {
            PaginaBase page = this.Page as PaginaBase;
            this.activeControl.Visible = !string.IsNullOrEmpty(page.RecuperarLoginUsuarioLogado());
        }
        protected void PrepareGridAccounts()
        {
            if (this.activeControl.Visible)
            {
                PaginaBase page = this.Page as PaginaBase;
                this.gridAccounts.DataSource = ControladorShardAccount.GetAccounts(page.RecuperarLoginUsuarioLogado()).GetDataSet();
                this.gridAccounts.DataBind();
            }
        }
        #endregion
    }
}