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

namespace Web.Business.Creature
{
    public partial class ViewAll : PaginaBaseSegura
    {
        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.PrepareGridCreatures();
                this.PrepareViewItems();
            }
        }
        
        #endregion

        #region Methods

        protected void PrepareViewItems()
        {
            this.gridCreatures.Visible = this.gridCreatures.Rows.Count > 0;
            this.lblNotFound.Visible = this.gridCreatures.Visible;
        }
        protected void PrepareGridCreatures()
        {
            this.gridCreatures.DataSource = ControladorCreatures.GetActiveCreatures().GetDataSet();
            this.gridCreatures.DataBind();
        }

        #endregion
    }
}