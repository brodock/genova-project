using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("an mages corpse")]
    public class RedMage : BaseCreature
    {
        [Constructable]
        public RedMage()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = NameList.RandomName("evil mage");
            Title = "the murderous mage";
            Body = 400;

            SetStr(81, 105);
            SetDex(91, 115);
            SetInt(130, 175);

            SetHits(622, 744);

            SetDamage(4, 10);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 15, 20);
            SetResistance(ResistanceType.Fire, 5, 10);
            SetResistance(ResistanceType.Poison, 5, 10);
            SetResistance(ResistanceType.Energy, 5, 10);

            SetSkill(SkillName.EvalInt, 83.1, 100.0);
            SetSkill(SkillName.Magery, 93.1, 100.0);
            SetSkill(SkillName.MagicResist, 112.0, 120.5);
            SetSkill(SkillName.Tactics, 65.0, 87.5);
            SetSkill(SkillName.Wrestling, 20.2, 60.0);

            Fame = 7000;
            Karma = -7000;

            VirtualArmor = 31;

            PackReg(15);
            PackGold(500, 1000);
            PackScroll(2, 7);
            AddLoot(LootPack.UltraRich);

            PackNecroScroll(3); // Curse Weapon
            PackNecroScroll(1); // Blood Oath
            PackNecroScroll(10); // Strangle

            AddItem(new Robe(Utility.RandomNeutralHue())); // TODO: Proper hue
            AddItem(new Sandals(Utility.RandomNeutralHue()));
        }

        public override bool AlwaysMurderer { get { return true; } }
        public override int Meat { get { return 1; } }

        public RedMage(Serial serial)
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