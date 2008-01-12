using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    public class Murderer : BaseCreature
    {
        [Constructable]
        public Murderer()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "the murderer";
            Hue = Utility.RandomSkinHue();

            if (this.Female = Utility.RandomBool())
            {
                this.Body = 0x191;
                this.Name = NameList.RandomName("female");
                AddItem(new BoneChest());
            }
            else
            {
                this.Body = 0x190;
                this.Name = NameList.RandomName("male");
                AddItem(new BoneChest());
            }

            SetStr(200, 275);
            SetDex(200, 200);
            SetInt(161, 175);

            SetDamage(12, 15);
            SetHits(606, 723);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 30);
            SetResistance(ResistanceType.Cold, 25, 30);
            SetResistance(ResistanceType.Poison, 10, 20);
            SetResistance(ResistanceType.Energy, 10, 20);

            SetSkill(SkillName.Anatomy, 125.0);
            SetSkill(SkillName.Fencing, 90.0, 107.5);
            SetSkill(SkillName.Macing, 105.0, 110.5);
            SetSkill(SkillName.Poisoning, 60.0, 82.5);
            SetSkill(SkillName.MagicResist, 100.0, 125.0);
            SetSkill(SkillName.Swords, 125.0);
            SetSkill(SkillName.Tactics, 125.0);
            SetSkill(SkillName.Lumberjacking, 125.0);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 40;

            AddItem(new BoneArms());
            AddItem(new BoneGloves());
            AddItem(new StuddedGorget());
            AddItem(new BoneHelm());
            AddItem(new BoneLegs());
            AddItem(new FullApron(Utility.RandomMetalHue()));
            AddItem(new HeaterShield());
            AddItem(new WarFork());

            Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A));
            hair.Hue = Utility.RandomNondyedHue();
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem(hair);

            AddLoot(LootPack.UltraRich);
            PackGold(2000, 2225);
            PackMagicItems(2, 5);
        }

        public override bool AlwaysMurderer { get { return true; } }

        public Murderer(Serial serial)
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