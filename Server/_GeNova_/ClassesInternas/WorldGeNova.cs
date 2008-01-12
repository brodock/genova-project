using System;
using System.Collections.Generic;
using System.Text;
using Server;

namespace GeNova.Core.ClassesInternas
{
    public abstract class WorldGeNova
    {
        public static void DecayItems()
        {
            try
            {
                List<Item> decaying = new List<Item>();
                foreach (Item item in World.Items.Values)
                {
                    if (item.Decays && item.Parent == null && item.Map != Map.Internal && (item.LastMoved + item.DecayTime) <= DateTime.Now)
                        decaying.Add(item);
                }

                foreach (Item item in decaying)
                {
                    if (item.OnDecay())
                        item.Delete();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("** Erro interno - remocao de itens: {0}", ex.Message);
            }
        }
    }
}
