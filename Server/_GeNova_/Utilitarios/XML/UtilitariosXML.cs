/***************************************************************************
 *   copyright            : (C) GeNova Project
 *   webSite              : http://code.google.com/p/genovaproject/
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.IO;
using System.Xml;

namespace GeNova.Core.Utilitarios.XML
{
    public class UtilitariosXML
    {
        #region Atributes
        private FileStream _documento;
        private string _caminhoXML;
        #endregion

        #region Constructor
        public UtilitariosXML(string caminho)
        {
            this._caminhoXML = caminho;
        }
        #endregion

        #region Initialize Methods
        /// <summary>
        /// Load XML File. Necessery.
        /// </summary>
        private void CarregarArquivoXML()
        {            
            string path = String.Concat(CaminhosXML.Path_GeNova_XML, this._caminhoXML);
            this._documento = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }

        /// <summary>
        /// Init XML. Necessery.
        /// </summary>
        private XmlDocument IniciarXML()
        {
            this.CarregarArquivoXML();

            XmlDocument xmlRetorno = new XmlDocument();
            XmlTextReader xmltr = new XmlTextReader(this._documento);

            try { xmlRetorno.Load(xmltr); }
            finally { xmltr.Close(); }

            return xmlRetorno;
        }
        #endregion

        #region Methods
        /// <summary>
        /// XML: Get Single Node.
        /// </summary>
        public XmlNode GetSingleNode(string name)
        {
            XmlDocument document = this.IniciarXML();
            return document.DocumentElement.SelectSingleNode(name.ToLower());
        }
        /// <summary>
        /// XML: Get Single Node.
        /// </summary>
        public XmlNode GetSingleNode(XmlNode node, string name)
        {
            return node.SelectSingleNode(name.ToLower());
        }

        /// <summary>
        /// XML: Get property "value" in Attributes from Single Node.
        /// </summary>
        /// <param name="singleNode"></param>
        public string GetAttributeValue(XmlNode node)
        {
            // Default property: one param = value.
            string valueProperty = "value";
            return node.Attributes.GetNamedItem(valueProperty).Value;
        }
        #endregion
    }
}
