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
