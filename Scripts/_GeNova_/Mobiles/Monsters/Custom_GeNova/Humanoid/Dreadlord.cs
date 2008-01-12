//   RunUO script: DreadLord
//   Copyright (c) 2003 by Humphro
//
//   This script is free software; you can redistribute it and/or modify
//   it under the terms of the GNU General Public License as published by
//   the Free Software Foundation; version 2 of the License applies.
//
//   This program is distributed in the hope that it will be useful,
//   but WITHOUT ANY WARRANTY; without even the implied warranty of
//   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//   GNU General Public License for more details. 
//   http://www.gnu.org/licenses/gpl.html
//
//   In short: Whatever you do to this script, you MUST publish it and
//   let it be used for free. 
//
//   Please do NOT remove or change this header.

using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    public class Dreadlord : BaseCreature
    {
        [Constructable]
        public Dreadlord()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();
            Title = "the Dreaded";

            AddItem(new PlateArms());
            AddItem(new PlateLegs());
            AddItem(new PlateGloves());
            AddItem(new PlateGorget());
            AddItem(new PlateHelm());
            AddItem(new VikingSword());
            AddItem(new HeaterShield());
            AddItem(new Cloak(Hue = 2118));
            AddItem(new HalfApron(Hue = 2118));
            AddItem(new BodySash(Hue = 2118));

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
                AddItem(new FemalePlateChest());
                Hue = 1002;
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
                AddItem(new PlateChest());
                Hue = 1002;
            }

            SetStr(95, 110);
            SetDex(90, 110);
            SetInt(90, 100);

            SetHits(606, 723);
            SetDamage(10, 23);

            SetSkill(SkillName.MagicResist, 90.0, 100.0);
            SetSkill(SkillName.Swords, 95.0, 120.0);
            SetSkill(SkillName.Tactics, 90.0, 100.0);
            SetSkill(SkillName.Parry, 95.0, 110.0);
            SetSkill(SkillName.Magery, 45.0, 65.0);
            SetSkill(SkillName.EvalInt, 45.0, 65.0);

            Fame = 10000;
            Karma = -10000;

            Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A));
            hair.Hue = Utility.RandomNondyedHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem(hair);

            AddLoot(LootPack.UltraRich);
            PackGold(650, 800);
            PackGem();
            PackMagicItems(1, 5, 0.70, 0.55);
        }

        public override bool AlwaysMurderer { get { return true; } }

        public Dreadlord(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}