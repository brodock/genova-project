using System;
using System.Collections.Generic;
using System.Text;
using WebApplicationController.Enumeradores;
using WebApplicationController.Mapa;

namespace WebApplicationController.Navegacao
{
    public abstract class NavegacaoUtils
    {
        public static string GetTipoSolicitacaoUrl(TipoSolicitacao tipo)
        {
            string retorno = string.Empty;
            switch (tipo)
            {
                case TipoSolicitacao.Default:
                    retorno = PrincipalMapa.Principal;
                    break;
            }
            return retorno;
        }
    }
}
