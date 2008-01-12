using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Cadaver de Wyrm")]
    public class GoldWyrm : BaseCreature
    {
        [Constructable]
        public GoldWyrm()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Body = 49;
            Hue = 2213;
            Name = "Wyrm de Ouro";
            BaseSoundID = 362;

            SetStr(721, 760);
            SetDex(101, 130);
            SetInt(386, 425);

            SetHits(2433, 2456);

            SetDamage(27, 30);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 55, 70);
            SetResistance(ResistanceType.Energy, 55, 70);

            SetSkill(SkillName.EvalInt, 90.1, 120.0);
            SetSkill(SkillName.Magery, 90.1, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);

            Fame = 18000;
            Karma = 18000;

            VirtualArmor = 64;

            Tamable = false;
            ControlSlots = 5;
            MinTameSkill = 120.0;

            AddLoot(LootPack.UltraRich, 2);
            PackGem();
            PackGem();
            PackGold(3000, 5100);
            PackMagicItems(1, 5, 0.70, 0.55);
            PackMagicItems(1, 5, 0.50, 0.45);
            PackMagicItems(1, 5);
        }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 9; } }
        public override ScaleType ScaleType { get { return ScaleType.Yellow; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public GoldWyrm(Serial serial)
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
