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
using System.Reflection;

namespace GeNova.Core.Utilitarios.XML
{
    /// <summary>
    /// Register path and name for xml file.
    /// </summary>
    public abstract class CaminhosXML
    {
        /*Principal Value*/
        internal static readonly string Path_GeNova_XML = String.Concat(Server.Core.BaseDirectory, @"\GeNova_XML");

        /*Internal Values*/
        internal static readonly string FilePath_Configs_Mysql = @"\Configs\mysql.xml";

        /*Public Values*/
        public static readonly string FilePath_Configs_GeNova = @"\Configs\genova.xml";
        public static readonly string FilePath_Misc_Gumps = @"\Misc\gumps.xml";
    }
}
