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
using Server.Items;

namespace GeNova.Server.Engines.LoteGenerico
{
    public abstract class AddItemLoot
    {
        public static void AddItem(Container container, Item item)
        {
            if (item != null)
                container.DropItem(item);
        }
        public static void AddItem(Container container, Item item, int quantidade)
        {
            if (item != null)
            {
                for (int i = 0; i < quantidade; i++)
                    container.DropItem(item);
            }
        }
        public static void AddItem(Container container, string valorTipoItem, int quantidade, int ouro)
        {
            Type tipoItem = AddItemLootItemType.GetType(valorTipoItem);
            if (tipoItem != null)
            {
                object objetoItem = Activator.CreateInstance(tipoItem);
                if (objetoItem is Gold)
                    container.DropItem(new Gold(Utility.RandomMinMax(quantidade, ouro)));
                else if (objetoItem is Item)
                {
                    Item item = (Item)objetoItem;
                    container.DropItem(item);
                }
            }
        }
    }

    public abstract class AddItemLootItemType
    {
        public static Type GetType(string valorTipo)
        {
            return ScriptCompiler.FindTypeByName(valorTipo);
        }
    }
}
