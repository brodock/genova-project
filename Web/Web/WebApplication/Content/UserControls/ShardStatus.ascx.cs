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
using CoreBusiness.Interface;
using CoreBusiness.Controller;

namespace Web.UserControls
{
    public partial class ShardStatus : UserControl
    {
        #region Events
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
                this.PrepareViewItems();
        }
        #endregion

        #region Methods
        private void PrepareViewItems()
        {
            IServerStatus shard = ControladorStatusServidor.GetServerStatus();
            this.lblDetailDescription.Text = shard.GetDescription();
            this.lblDetailIP.Text = shard.GetIP();
            this.lblDetailPort.Text = shard.GetPort();
            this.lblDetailClient.Text = shard.GetClientRequirement();
            this.lblDetailOnlineUsers.Text = shard.GetOnlineUsers();
            this.lblDetailMobiles.Text = shard.GetTotalMobiles();
            this.lblDetailItems.Text = shard.GetTotalItens();
            this.lblDetailUptime.Text = shard.GetServerUptime().ToLongTimeString();
            this.lblDetailLastInfoUpdate.Text = shard.GetLastInfoUpdated().ToString();
        }
        #endregion
    }
}