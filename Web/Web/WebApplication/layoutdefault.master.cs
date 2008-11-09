using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using WebApplicationController;

namespace Web
{
    public partial class Layout : MasterPage
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.Page.IsPostBack)
                this.PreparePageItems();
        }

        protected void btnLogo_Click(object sender, ImageClickEventArgs e)
        {
            PaginaBase basePage = this.Page as PaginaBase;
            basePage.IrParaPaginaInicial();
        }
        #endregion

        #region Methods
        protected void PreparePageItems()
        {
            PaginaBase basePage = this.Page as PaginaBase;
            string appVersion = basePage.GetApplicationVersion();
            this.lblInfoBuild.Text = string.Format("build {0}", appVersion);
        }
        #endregion
    }
}