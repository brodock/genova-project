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
using GeNova.Core.Controladores;
using Server.Accounting;

namespace GeNova.Server.Engines
{
    public class CriacaoDeContas : Timer
    {
        public CriacaoDeContas()
            : base(TimeSpan.FromMinutes(2.0), TimeSpan.FromMinutes(2.0))
        {
            this.Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new CriacaoDeContas().Start();
        }
        public static void IniciarThread()
        {
            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(CriacaoDeContasProcessamento.Processar);
            new System.Threading.Thread(threadStart).Start();
        }

        protected override void OnTick()
        {
            IniciarThread();
        }
    }

    public abstract class CriacaoDeContasProcessamento
    {
        public static void Processar()
        {
            List<Dictionary<string, object>> contas = ControladorODBC.ODBCRecuperarListaUsuariosNaoAtivos();
            if (contas.Count > 0)
                Console.WriteLine("* Iniciando processo de Criacao de Contas.");
            foreach (Dictionary<string, object> conta in contas)
            {
                int idUsuario = Convert.ToInt32(conta["IdUsuario"].ToString());
                string login = conta["Login"].ToString();
                string senha = conta["Senha"].ToString();

                if (Accounts.GetAccount(login) == null)
                {
                    Account novaConta = new Account(login, senha);
                    Console.WriteLine("** Conta criada: {0} **", login);
                    ControladorODBC.ODBCAtivarContaUsuario(idUsuario, login);
                }
                else
                    Console.WriteLine("** Erro: Conta {0} ja existe! **", login);
            }
        }
    }
}
