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
using Server.Mobiles;
using Server.Items;
using GeNova.Server.Engines.LoteGenerico;
using GeNova.Server.Variados.Enums;

namespace GeNova.Server.Engines.LoteGenerico.RandomLoots
{
    public class RandomCraftItem : ModelRandomItems
    {
        public RandomCraftItem(BaseCreature creature)
            : base(creature, EnumChance.Normal)
        {
            this.AddSortableItem(new Pickaxe());
            this.AddSortableItem(new SmithHammer());
            this.AddSortableItem(new Saw());
            this.AddSortableItem(new TinkerTools());
        }
    }
}
