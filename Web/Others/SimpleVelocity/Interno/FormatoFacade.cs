using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NVelocity;
using NVelocity.App;
using SimpleVelocity.Dados;
using SimpleVelocity.Exceptions;

namespace SimpleVelocity.Interno
{
    internal class FormatoFacade
    {
        private Template _template;
        private NVRelatorioFacade _relatorio;

        internal FormatoFacade(NVRelatorioFacade relatorio)
        {
            this._relatorio = relatorio;
        }

        internal StringBuilder RecuperarFormatoComDados()
        {
            // Mergiar dados com formato, recuperando relatório
            StringWriter retorno = new StringWriter();
            this._template.Merge(this._relatorio.Dados.Contexto, retorno);
            return retorno.GetStringBuilder();
        }

        internal void SetarFormato()
        {
            // Setar formato
            string pathRelatorio = this._relatorio.CaminhoRelatorio;
            this._template = this._relatorio.Engine.GetTemplate(pathRelatorio);
        }
    }
}
