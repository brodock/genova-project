using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("corpo de drow arqueiro")]
    public class DrowArcher : BaseCreature
    {
        [Constructable]
        public DrowArcher()
            : base(AIType.AI_Archer, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            SpeechHue = Utility.RandomDyedHue();
            Title = "Drow Arqueiro";
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

            SetStr(146, 180);
            SetDex(101, 130);
            SetInt(116, 140);

            SetHits(222, 508);
            SetDamage(22, 41);

            SetDamageType(ResistanceType.Physical, 50);

            SetResistance(ResistanceType.Physical, 40, 75);
            SetResistance(ResistanceType.Fire, 10, 70);
            SetResistance(ResistanceType.Cold, 10, 70);
            SetResistance(ResistanceType.Poison, 10, 70);
            SetResistance(ResistanceType.Energy, 10, 70);

            SetSkill(SkillName.Anatomy, 60.2, 190.0);
            SetSkill(SkillName.Archery, 80.1, 130.0);
            SetSkill(SkillName.MagicResist, 65.1, 130.0);
            SetSkill(SkillName.Tactics, 50.1, 175.0);
            SetSkill(SkillName.Wrestling, 50.1, 75.0);

            Fame = 6500;
            Karma = -6500;
            VirtualArmor = 45;

            Item bodySash = new BodySash();
            bodySash.Hue = 422;
            AddItem(bodySash);

            Item hair = new Item(Utility.RandomList(0x203B, 0x2049, 0x2048, 0x204A));
            hair.Hue = 1153;
            hair.Layer = Layer.Hair;
            hair.Movable = false;
            AddItem(hair);

            //AddItem(new Piwafwi());
            //AddItem(new DrowRingmailGloves());
            //AddItem(new DrowChainChest());
            //AddItem(new DrowChainLegs());
            AddItem(new Bow());
            AddItem(new Boots());

            PackArmor(2, 5, 0.8);
            PackWeapon(3, 5, 0.8);
            PackSlayer();
            PackItem(new Arrow(Utility.Random(50, 120)));
            PackGold(750, 1750);
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

        public override int Hides { get { return 8; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public override bool AlwaysMurderer { get { return true; } }
        public override bool CanRummageCorpses { get { return true; } }

        public DrowArcher(Serial serial)
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

            if (Body == 42)
            {
                Body = 0x8E;
                Hue = 0;
            }
        }
    }
}