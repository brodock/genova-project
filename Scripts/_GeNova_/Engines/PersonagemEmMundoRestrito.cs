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
using Server.Items;

namespace GeNova.Server.Engines
{
    public class PersonagemEmMundoRestrito
    {
        public static readonly bool Ativo = true;
        public static readonly PMList[] GeNovaLists = new PMList[] { PMList.Felucca };
        public static readonly string Mensagem = "Você está tentando acessar um mundo restrito.";

        public static void VerificarComRestricoes(Mobile personagem)
        {
            if (Confirmar(personagem))
            {
                personagem.MoveToWorld(PosicionamentoNoMapa.BritainCentro(), Map.Felucca);
                personagem.SendMessage("Você estava em um mundo restrito e fora movido para o ponto inicial.");
            }
        }

        public static bool Confirmar(Mobile personagem)
        {
            return personagem.Map != Map.Felucca;
        }
    }
}
