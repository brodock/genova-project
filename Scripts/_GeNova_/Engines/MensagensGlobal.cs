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

namespace GeNova.Server.Engines
{
    public class MensagensGlobal : Timer
    {
        public MensagensGlobal()
            : base(TimeSpan.FromMinutes(30.0), TimeSpan.FromMinutes(30.0))
        {
            this.Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new MensagensGlobal().Start();
        }
        public static void IniciarThread()
        {
            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(MensagensGlobalProcessamento.Processar);
            new System.Threading.Thread(threadStart).Start();
        }

        protected override void OnTick()
        {
            IniciarThread();
        }
    }

    public abstract class MensagensGlobalProcessamento
    {
        public static void Processar()
        {
            ControladorODBC.ODBCProcessarMensagensGlobal();
        }
    }
}
