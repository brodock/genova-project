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
