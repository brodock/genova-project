using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Cadaver de Dragao de Fogo")]
    public class FireDragon : BaseCreature
    {
        [Constructable]
        public FireDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Dragao de Fogo";
            Body = 12;
            BaseSoundID = 362;
            Hue = 49;

            SetStr(300, 350);
            SetDex(150, 200);
            SetInt(6500, 6500);

            SetHits(1300, 1350);

            SetDamage(22, 28);

            SetDamageType(ResistanceType.Physical, 25);
            SetDamageType(ResistanceType.Fire, 100);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 100, 100);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 55, 70);
            SetResistance(ResistanceType.Energy, 55, 70);

            SetSkill(SkillName.EvalInt, 90.1, 120.0);
            SetSkill(SkillName.Magery, 90.1, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);

            Fame = 12500;
            Karma = -12500;

            VirtualArmor = 50;

            Tamable = false;
            ControlSlots = 5;
            MinTameSkill = 120.0;

            AddLoot(LootPack.UltraRich, 2);
            PackGold(3000, 5000);
            PackScroll(1, 7);
            PackMagicItems(1, 5);
        }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 7; } }
        public override ScaleType ScaleType { get { return (Body == 12 ? ScaleType.Yellow : ScaleType.Yellow); } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public FireDragon(Serial serial)
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
