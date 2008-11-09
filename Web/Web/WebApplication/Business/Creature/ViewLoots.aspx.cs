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

namespace Web.Business.Creature
{
    public partial class ViewLoots : PaginaBaseSegura
    {
        #region Properties

        protected string NameCreature
        {
            get { return this.txtCreature.Text; }
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                this.PrepareViewItems();
            }
        }
        protected void btnFindCreature_Click(object sender, EventArgs e)
        {
            try
            {
                this.ValidateViewItems();
                this.PrepareGridLoots();
            }
            catch (Exception erro) { base.ExibirMensagemAlerta(erro.Message); }
        }

        #endregion

        #region Methods

        protected void PrepareViewItems()
        {
            this.txtCreature.Text = string.Empty;
        }
        protected bool ValidateViewItems()
        {
            if (string.IsNullOrEmpty(this.txtCreature.Text))
                throw new Exception(Erros.ValorInvalido("Creature", "Name of Creature"));

            return true;
        }
        protected void PrepareGridLoots()
        {
            this.gridLoot.DataSource = ControladorCreatures.GetCreatureLoots(this.NameCreature, true).GetDataSet();
            this.gridLoot.DataBind();
            if (this.gridLoot.Rows.Count <= 0) throw new Exception(Erros.NaoEncontrado("Generic Loot's for Creature"));
        }

        #endregion
    }
}