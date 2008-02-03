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
using GeNova.Server.Engines.CustomRace;

namespace GeNova.Server.Engines
{
    public abstract class CustomRaceFactory
    {
        public static void VerificarCustomRace(Mobile personagem)
        {
            ModelCustomRace customRace;
            PlayerMobile jogador = (PlayerMobile)personagem;

            // TODO: Decorator? :)
            if (jogador.Race == Race.Elf)
                customRace = new ElfCustomRace(jogador);
            else
                customRace = new HumanCustomRace(jogador);            
        }
    }
}
