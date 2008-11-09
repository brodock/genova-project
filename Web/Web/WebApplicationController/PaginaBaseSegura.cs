using System;
using System.Collections.Generic;
using System.Text;
using Security.Usuario.Controladores;

namespace WebApplicationController
{
    public class PaginaBaseSegura : PaginaBase
    {
        #region Métodos - Usuário
        public bool IsUserLogged()
        {
            return ControladorUsuario.IsUsuarioLogado();
        }
        public bool IsAdministrador()
        {
            return ControladorUsuario.IsAdministrador();
        }
        #endregion

        protected override void OnLoad(EventArgs e)
        {
            if (!this.IsUserLogged())
            {
                try
                {
                    base.IrParaPaginaInicial();
                }
                catch (Exception)
                {
                    throw new System.Web.HttpException(403, string.Empty);
                }

            }

            base.OnLoad(e);
        }
    }
}
