using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
    [CorpseName("corpo do troll negro")]
    public class ForumTroll : BaseCreature
    {
        [Constructable]
        public ForumTroll()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Troll Negro";
            Body = Utility.RandomList(53, 54);
            BaseSoundID = 461;
            Hue = 1175;

            SetStr(176, 205);
            SetDex(46, 65);
            SetInt(46, 70);

            SetHits(406, 623);

            SetDamage(8, 14);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Fire, 25, 35);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 5, 15);
            SetResistance(ResistanceType.Energy, 5, 15);

            SetSkill(SkillName.MagicResist, 45.1, 60.0);
            SetSkill(SkillName.Tactics, 50.1, 70.0);
            SetSkill(SkillName.Wrestling, 50.1, 70.0);

            Fame = 3500;
            Karma = -3500;

            VirtualArmor = 20;

            PackGold(250, 350);
            PackArmor(1, 5, 0.70);
            PackWeapon(1, 5, 0.70);

        }

        public override int TreasureMapLevel { get { return 1; } }
        public override int Meat { get { return 2; } }

        public ForumTroll(Serial serial)
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