using System;
using Server.Network;
using Server;
using GeNova.Core.ClassesInternas;

namespace GeNova.Server.Engines
{
    public class ItemsDecay : Timer
    {
        public ItemsDecay()
            : base(TimeSpan.FromMinutes(2.0), TimeSpan.FromMinutes(2.0))
        {
            Priority = TimerPriority.OneMinute;
        }

        public static void Initialize()
        {
            new ItemsDecay().Start();
        }
        public static void IniciarThread()
        {
            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(ItemsDecayProcessamento.Processar);
            new System.Threading.Thread(threadStart).Start();
        }

        protected override void OnTick()
        {
            IniciarThread();
        }
    }

    public abstract class ItemsDecayProcessamento
    {
        public static void Processar()
        {
            Console.WriteLine("** Apagando items no mundo. **");
            WorldGeNova.DecayItems();
        }
    }
}
