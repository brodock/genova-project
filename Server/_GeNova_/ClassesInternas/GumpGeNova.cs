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
using System.Text;
using GeNova.Core.Utilitarios.XML;
using System.Xml;

namespace GeNova.Core.ClassesInternas
{
    public abstract class GumpGeNova
    {
        public static int GetIdKRGump(Type type)
        {
            string gumpName = type.Name;

            UtilitariosXML xmlUtil = new UtilitariosXML(CaminhosXML.FilePath_Misc_Gumps);

            XmlNode rootNode = xmlUtil.GetRootNode("gumps");
            XmlNode objectGumpNode = xmlUtil.FindNodeByAttribute(rootNode, "name", gumpName);

            int idKRGump = Convert.ToInt32(xmlUtil.GetAttributeValue(objectGumpNode));
            return idKRGump;
        }
    }
}
