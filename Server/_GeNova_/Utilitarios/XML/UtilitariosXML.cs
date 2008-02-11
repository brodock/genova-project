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
        /// XML: Get Root Node.
        /// </summary>
        public XmlNode GetRootNode(string name)
        {
            XmlDocument document = this.IniciarXML();
            return document.SelectSingleNode(name.ToLower());
        }

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
        public string GetAttributeValue(XmlNode node)
        {
            return GetAttributeValue(node, string.Empty);
        }

        /// <summary>
        /// XML: Get property "attributeName" in Attributes from Single Node.
        /// </summary>
        public string GetAttributeValue(XmlNode node, string attributeName)
        {
            // Default property: one param = value.
            string valueProperty;
            if (attributeName.Equals(string.Empty))
                valueProperty = "value";
            else
                valueProperty = attributeName;

            return node.Attributes.GetNamedItem(valueProperty).Value;
        }

        /// <summary>
        /// XML: Find Node by: attribute name and value.
        /// </summary>
        public XmlNode FindNodeByAttribute(XmlNode node, string attributeName, string attributeValue)
        {
            foreach (XmlNode localNode in node)
            {
                if (localNode.NodeType == XmlNodeType.Element)
                {
                    string valor = this.GetAttributeValue(localNode, attributeName);
                    if (valor.Equals(attributeValue))
                        return localNode;
                }
            }
            return null;
        }
        #endregion
    }
}
