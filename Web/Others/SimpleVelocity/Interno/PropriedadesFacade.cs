using System;
using System.Collections.Generic;
using System.Text;
using Commons.Collections;
using System.Configuration;

namespace SimpleVelocity.Interno
{
    internal class PropriedadesFacade
    {
        private ExtendedProperties _propriedades;

        internal PropriedadesFacade()
        {
            this._propriedades = new ExtendedProperties();
        }

        internal ExtendedProperties RecuperarConfiguracoes()
        {
            // Recuperar ExtendedProperties configurado.
            this.SetarPropriedadesDefault();
            return this._propriedades;
        }

        private void SetarPropriedadesDefault()
        {
            // Definir Propriedades
            string pathTemplates = ConfigurationManager.AppSettings["PathSimpleVelocity"];
            this._propriedades.SetProperty("file.resource.loader.path", pathTemplates);
        }
    }
}
