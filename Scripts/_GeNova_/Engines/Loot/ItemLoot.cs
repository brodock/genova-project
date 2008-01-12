using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Items;

namespace GeNova.Server.Engines.LoteGenerico
{
    public abstract class AddItemLoot
    {
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
