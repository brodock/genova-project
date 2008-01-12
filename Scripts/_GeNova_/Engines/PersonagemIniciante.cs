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
using Server;
using Server.Mobiles;

namespace GeNova.Server.Engines
{
    public abstract class PersonagemIniciante
    {
        public static void VerificarComRestricoes(Mobile personagem)
        {
            PlayerMobile iniciante = (PlayerMobile)personagem;
            if (iniciante.Young)
                iniciante.Young = false;
        }

        public static bool Confirmar(Mobile personagem)
        {
            PlayerMobile iniciante = (PlayerMobile)personagem;
            return iniciante.Young;
        }
    }
}
