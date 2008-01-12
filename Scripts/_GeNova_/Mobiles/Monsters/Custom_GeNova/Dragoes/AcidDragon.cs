using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Cadaver de Dragao")]
    public class AcidDragon : BaseCreature
    {
        [Constructable]
        public AcidDragon()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Dragao de Acido";
            Body = Utility.RandomList(12, 59);
            BaseSoundID = 362;
            Hue = 463;

            SetStr(796, 825);
            SetDex(86, 105);
            SetInt(436, 475);

            SetHits(2478, 2495);

            SetDamage(16, 22);

            SetDamageType(ResistanceType.Physical, 50);

            SetResistance(ResistanceType.Physical, 50, 60);
            SetResistance(ResistanceType.Fire, 50, 70);
            SetResistance(ResistanceType.Cold, 50, 60);
            SetResistance(ResistanceType.Poison, 50, 70);
            SetResistance(ResistanceType.Energy, 55, 60);

            SetSkill(SkillName.EvalInt, 90.1, 120.0);
            SetSkill(SkillName.Magery, 90.1, 120.0);
            SetSkill(SkillName.MagicResist, 100.0, 120.0);
            SetSkill(SkillName.Tactics, 100.0, 120.0);
            SetSkill(SkillName.Wrestling, 100.0, 120.0);

            Fame = 15000;
            Karma = -15000;

            VirtualArmor = 40;

            Tamable = false;
            ControlSlots = 5;
            MinTameSkill = 120.0;

            for (int i = 0; i < 8; ++i)
                PackGem();

            AddLoot(LootPack.UltraRich, 2);
            PackGold(4600, 4800);
            PackMagicItems(1, 5);
            PackMagicItems(1, 5);
            PackSlayer();
        }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override bool AutoDispel { get { return true; } }
        public override Poison PoisonImmune { get { return Poison.Deadly; } }
        public override Poison HitPoison { get { return Poison.Deadly; } }
        public override int TreasureMapLevel { get { return 4; } }
        public override int Meat { get { return 19; } }
        public override int Hides { get { return 20; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Scales { get { return 7; } }
        public override ScaleType ScaleType { get { return (Body == 12 ? ScaleType.Green : ScaleType.Green); } }
        public override FoodType FavoriteFood { get { return FoodType.Meat; } }

        public AcidDragon(Serial serial)
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
