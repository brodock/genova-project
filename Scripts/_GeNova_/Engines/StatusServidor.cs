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
