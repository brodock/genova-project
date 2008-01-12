using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("ancient remains")]
    public class AncientMummy : BaseCreature
    {
        [Constructable]
        public AncientMummy()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.4, 0.8)
        {
            Name = "An Ancient Mummy";
            Body = 154;
            Hue = 1151;
            BaseSoundID = 471;

            SetStr(346, 370);
            SetDex(71, 90);
            SetInt(26, 40);

            SetHits(808, 1222);

            SetDamage(23, 28);

            SetDamageType(ResistanceType.Physical, 40);
            SetDamageType(ResistanceType.Cold, 60);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 40, 50);
            SetResistance(ResistanceType.Cold, 60, 70);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 50, 60);

            SetSkill(SkillName.MagicResist, 140.1, 150.0);
            SetSkill(SkillName.Tactics, 125.1, 130.0);
            SetSkill(SkillName.Wrestling, 115.1, 130.0);

            Fame = 14000;
            Karma = -14000;

            VirtualArmor = 80;

        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.UltraRich);
            AddLoot(LootPack.Gems, 5);

        }

        public override Poison PoisonImmune { get { return Poison.Lesser; } }

        public AncientMummy(Serial serial)
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