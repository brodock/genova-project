using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Corpo de Elemental Minax")]
    public class MinaxElemental : BaseCreature
    {
        [Constructable]
        public MinaxElemental()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Elemental Minax";
            Hue = 1209;
            Body = 131;
            BaseSoundID = 263;

            SetStr(340, 370);
            SetDex(59, 68);
            SetInt(191, 212);

            SetHits(212, 235);

            SetDamage(24, 33);

            SetDamageType(ResistanceType.Fire, 55);
            SetDamageType(ResistanceType.Energy, 55);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 65, 75);
            SetResistance(ResistanceType.Poison, 35, 45);
            SetResistance(ResistanceType.Energy, 45, 55);

            SetSkill(SkillName.EvalInt, 70.1, 80.0);
            SetSkill(SkillName.Magery, 75.1, 87.0);
            SetSkill(SkillName.MagicResist, 74.1, 82.0);
            SetSkill(SkillName.Tactics, 63.1, 83.0);
            SetSkill(SkillName.Wrestling, 62.1, 83.0);

            Fame = 75000;
            Karma = -75000;

            VirtualArmor = 38;

            //PackItem(new MinaxArrow(10));
            //PackItem(new MinaxBow());
        }

        public MinaxElemental(Serial serial)
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