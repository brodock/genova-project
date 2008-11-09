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
using System.Text;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using GeNova.Core.Controladores;
using Server.Items;

namespace GeNova.Server.Engines.LoteGenerico
{
    public class BaseCreatureGeNova
    {
        #region Construtores
        public BaseCreatureGeNova(BaseCreature criatura)
        {
            this._criatura = criatura;
        }
        #endregion

        #region Atributos
        BaseCreature _criatura;
        #endregion

        #region Propriedades
        public string NomeCriatura
        {
            get { return _criatura.Name; }
        }
        public string NomeTipoCriatura
        {
            get { return _criatura.GetType().ToString(); }
        }
        public Container BolsaCriatura
        {
            get { return _criatura.Backpack; }            
        }        
        #endregion

        #region Métodos Auxiliares - Camada entre o objeto referenciado e a Classe customizada
        private void PackItem(Item item)
        {
            this._criatura.PackItem(item);
        }
        #endregion

        #region Métodos
        public void PackODBCItem()
        {
            try
            {
                this.ODBCGiveLoot();
            }
            catch (Exception)
            {
                Console.WriteLine("** Erro ao adicionar lote ODBC na criatura {0}. **", (this.NomeCriatura.Equals(string.Empty) ? this.NomeTipoCriatura : this.NomeCriatura));
            }
        }
        private void ODBCGiveLoot()
        {
            List<Dictionary<string, object>> listaLoots = ControladorODBC.ODBCGiveLoot(this.NomeTipoCriatura, this.NomeCriatura);
            foreach (Dictionary<string, object> linha in listaLoots)
            {
                int id = Convert.ToInt32(linha["IdLoteGenerico"].ToString());
                int quantidade = Convert.ToInt32(linha["Quantidade"].ToString());
                int ouro = 0;

                string nome = linha["Classe"].ToString();
                string item = linha["Item"].ToString();

                if (item.ToUpper().Equals("OURO") || item.ToUpper().Equals("GOLD"))
                    ouro = Convert.ToInt32(quantidade);
                else if (item == "1cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(0, 7, SpellbookType.Regular));
                else if (item == "2cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(8, 15, SpellbookType.Regular));
                else if (item == "3cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(16, 23, SpellbookType.Regular));
                else if (item == "4cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(24, 31, SpellbookType.Regular));
                else if (item == "5cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(32, 39, SpellbookType.Regular));
                else if (item == "6cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(40, 47, SpellbookType.Regular));
                else if (item == "7cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(48, 55, SpellbookType.Regular));
                else if (item == "8cscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(56, 63, SpellbookType.Regular));
                else if (item == "necrolowscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(0, 5, SpellbookType.Necromancer));
                else if (item == "necromedscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(6, 10, SpellbookType.Necromancer));
                else if (item == "necrohighscroll" && quantidade > 0)
                    this.PackItem(Loot.RandomScroll(11, 15, SpellbookType.Necromancer));
                
                AddItemLoot.AddItem(this.BolsaCriatura, item, quantidade, ouro);
            }
        }
        #endregion
    }
}
