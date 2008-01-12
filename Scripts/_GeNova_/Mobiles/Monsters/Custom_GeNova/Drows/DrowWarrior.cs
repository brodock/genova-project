using System;
using System.Collections;
using Server.Items;
using Server.ContextMenus;
using Server.Misc;
using Server.Network;

namespace Server.Mobiles
{
    [CorpseName("corpo de drow guerreiro")]
    public class DrowWarrior : BaseCreature
    {
        [Constructable]
        public DrowWarrior()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "Drow Guerreiro";
            Hue = 902;

            if (this.Female = Utility.RandomBool())
            {
                Body = 0x191;
                Name = NameList.RandomName("female");
            }
            else
            {
                Body = 0x190;
                Name = NameList.RandomName("male");
            }

            SetStr(400, 800);
            SetDex(150, 175);
            SetInt(275, 325);

            SetHits(1400, 1600);
            SetDamage(33, 58);

            SetSkill(SkillName.MagicResist, 95.5, 127.5);
            SetSkill(SkillName.Swords, 95.5, 177.5);
            SetSkill(SkillName.Tactics, 95.5, 177.5);
            SetSkill(SkillName.Wrestling, 95.5, 177.5);
            SetSkill(SkillName.Meditation, 100.0, 177.5);
            SetSkill(SkillName.Magery, 100.0, 177.5);
            SetSkill(SkillName.Chivalry, 100.0, 177.5);

            Nightmare montaria = new Nightmare();
            montaria.Hue = 33;
            montaria.Rider = this;
            montaria.MinTameSkill = 115.1;

            Fame = 2000;
            Karma = -2000;
            VirtualArmor = 55;

            Item bodySash = new BodySash();
            bodySash.Hue = 422;
            AddItem(bodySash);

            //AddItem(new Piwafwi());
            AddItem(new Boots());
            //AddItem(new DrowRingmailGloves());
            //AddItem(new DrowChainChest());
            //AddItem(new DrowChainLegs());

            switch (Utility.Random(3))
            {
                case 0: AddItem(new Longsword()); break;
                case 1: AddItem(new Katana()); break;
                case 2: AddItem(new VikingSword()); break;
            }

            Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A));
            hair.Hue = 1153;
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem(hair);

            PackGold(500, 2500);
            PackArmor(2, 5, 0.8);
            PackWeapon(3, 5, 0.8);
            PackSlayer();
        }

        public void AddArcane(Item item)
        {
            if (item is IArcaneEquip)
            {
                IArcaneEquip eq = (IArcaneEquip)item;
                eq.CurArcaneCharges = eq.MaxArcaneCharges = 20;
            }
            item.Hue = ArcaneGem.DefaultArcaneHue;
            item.LootType = LootType.Newbied;
            AddItem(item);
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }

        public DrowWarrior(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }
}