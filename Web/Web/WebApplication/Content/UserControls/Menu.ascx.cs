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

namespace Web.UserControls
{
    public partial class Menu : UserControl
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.PrepareViewItems();
        }

        #endregion

        #region Methods

        protected void PrepareViewItems()
        {
            PaginaBase page = this.Page as PaginaBase;
            this.loggedBlock.Visible = !string.IsNullOrEmpty(page.RecuperarLoginUsuarioLogado());
            this.notLoggedBlock.Visible = !this.loggedBlock.Visible;
        }

        #endregion
    }
}