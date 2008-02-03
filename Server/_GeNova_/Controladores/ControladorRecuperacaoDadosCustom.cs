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
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace GeNova.Core.Controladores
{
    public abstract class ControladorRecuperacaoDadosCustom
    {
        public static List<double> RecuperarValoresListaSkillsCustomRace(DataSet ds, int minColuna, int maxColuna)
        {
            List<double> listaRetorno = null;
            if (ds.Tables[0].Rows.Count > 0)
            {
                int contador = minColuna;
                listaRetorno = new List<double>();
                
                while (contador < maxColuna)                
                    listaRetorno.Add(Convert.ToDouble(ds.Tables[0].Rows[0][contador++].ToString()));
            }
            return listaRetorno;
        }
    }
}
