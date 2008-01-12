using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("a centaur king corpse")]
    public class CentaurKing : Centaur
    {
        [Constructable]
        public CentaurKing()
            : base()
        {
            Name = "The Centaur-King";
            Hue = 1448;
            Body = 101;
            BaseSoundID = 679;
            SetStr(250);
            SetDex(200);
            SetInt(200);
            SetHits(6500);
            SetDamage(28);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Cold, 0);
            SetDamageType(ResistanceType.Fire, 50);
            SetDamageType(ResistanceType.Energy, 0);
            SetDamageType(ResistanceType.Poison, 0);

            SetResistance(ResistanceType.Physical, 85);
            SetResistance(ResistanceType.Cold, 70);
            SetResistance(ResistanceType.Fire, 70);
            SetResistance(ResistanceType.Energy, 70);
            SetResistance(ResistanceType.Poison, 50);


            SetSkill(SkillName.Meditation, 170.0);
            SetSkill(SkillName.MagicResist, 120.0);
            SetSkill(SkillName.Tactics, 170.0);
            SetSkill(SkillName.Wrestling, 170.0);

            Fame = 12000;
            Karma = -1000;
            VirtualArmor = 50;


            PackGold(8000);
            AddLoot(LootPack.UltraRich, 2);
        }


        public CentaurKing(Serial serial)
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
