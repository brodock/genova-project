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
using Server.Commands;
using Server;
using GeNova.Core.Controladores;
using GeNova.Server.Engines;

namespace GeNova.Server.Commands
{
    public class RegistroDePersonagem
    {
        public static readonly int NomeTamanhoMinimo = 2;
        public static readonly int NomeTamanhoMaximo = 16;

        public static void Initialize()
        {
            CommandSystem.Register("RegistrarPersonagem", AccessLevel.Player, new CommandEventHandler(RegistrarPersonagem_OnCommand));
        }

        [Usage("RegistrarPersonagem <text>")]
        [Description("Registrar nome do Personagem <text>.")]
        public static void RegistrarPersonagem_OnCommand(CommandEventArgs e)
        {
            string nomePersonagem = e.ArgString.Trim();
            if (!PersonagemDesconhecido.Confirmar(e.Mobile))
                e.Mobile.SendMessage(60, "Seu personagem já está cadastrado no sistema. Não utilize este comando desnecessariamente.");
            else if (nomePersonagem.Length <= 0)
                e.Mobile.SendMessage(60, "Digite um nome válido para cadastrar o personagem. Não utilize este comando desnecessariamente.");
            else if (nomePersonagem.Length < NomeTamanhoMinimo || nomePersonagem.Length > NomeTamanhoMaximo)
                e.Mobile.SendMessage(60, "Digite um nome com no mínimo 2 e no máximo 16 caracteres.");
            else
                RegistroDePersonagemProcessamento.Processar(e.Mobile, nomePersonagem);
        }
    }

    public abstract class RegistroDePersonagemProcessamento
    {
        public static void Processar(Mobile personagem, string nomePersonagem)
        {
            ControladorODBC.ODBCRegistrarPersonagem(personagem, nomePersonagem);
        }
    }
}
