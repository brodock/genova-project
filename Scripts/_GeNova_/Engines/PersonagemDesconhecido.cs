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
using GeNova.Server.Variados;

namespace GeNova.Server.Engines
{
    public abstract class PersonagemDesconhecido
    {
        public static readonly string NomePadrao = "Desconhecido";

        public static void VerificarComRestricoes(Mobile personagem)
        {
            if (personagem.Name.Equals(NomePadrao))
            {
                personagem.Frozen = true;
                personagem.MoveToWorld(PosicionamentoNoMapa.BritainCentro(), Map.Felucca);
                personagem.Hidden = true;                
            }
        }
        public static void SetarParaNomeDesconhecido(Mobile personagem)
        {
            personagem.Name = NomePadrao;
        }

        public static bool Confirmar(Mobile personagem)
        {
            return personagem.Name.Equals(NomePadrao);
        }
    }
}
