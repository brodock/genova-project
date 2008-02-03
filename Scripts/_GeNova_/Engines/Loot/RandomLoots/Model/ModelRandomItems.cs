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
using Server.Mobiles;
using GeNova.Server.Variados.Enums;

namespace GeNova.Server.Engines.LoteGenerico.RandomLoots
{
    public abstract class ModelRandomItems
    {
        private BaseCreature _creature;
        private EnumChance _chance;

        private static Random random = new Random();
        private List<Item> _listItems = new List<Item>();

        public ModelRandomItems(BaseCreature creature, EnumChance chance)
        {
            this._creature = creature;
            this._chance = chance;
        }

        public void AddItemsInLoot()
        {
            if (this.VerifySuccess())
                AddItemLoot.AddItem(this._creature.Backpack, this._listItems[this.GetNumberSelectedItem()]);
        }

        protected void AddSortableItem(Item item)
        {
            this._listItems.Add(item);
        }

        protected virtual bool VerifySuccess()
        {
            int success = random.Next((int)this._chance);
            return success.Equals(0);
        }

        private int GetNumberSelectedItem()
        {
            int number = random.Next(this._listItems.Count);
            return number;
        }
    }
}