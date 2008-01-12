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
using Server.Gumps;
using Server.Items;
using GeNova.Core.Controladores;

namespace GeNova.Server.Engines
{
    public class StatusServidor : Timer
    {
        public StatusServidor()
            : base(TimeSpan.FromMinutes(1.0), TimeSpan.FromMinutes(1.0))
        {
            this.Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new StatusServidor().Start();
        }
        public static void IniciarThread()
        {
            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(StatusServidorProcessamento.Processar);
            new System.Threading.Thread(threadStart).Start();
        }

        protected override void OnTick()
        {
            IniciarThread();
        }
    }

    public abstract class StatusServidorProcessamento
    {
        public static void Processar()
        {
            string tempoOnline = AdminGump.FormatTimeSpan(DateTime.Now - Clock.ServerStart);
            ControladorODBC.ODBCProcessarStatusServidor(tempoOnline);
        }
    }
}
