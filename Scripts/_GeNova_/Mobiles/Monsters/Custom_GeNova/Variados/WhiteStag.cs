using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Corpo do Veado Branco")]
    public class WhiteStag : BaseCreature
    {
        [Constructable]
        public WhiteStag()
            : base(AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Veado Branco";
            Body = 234;
            Hue = 1153;
            BaseSoundID = 362;

            SetStr(1225, 1260);
            SetDex(201, 220);
            SetInt(50, 70);

            SetHits(1505);

            SetDamage(15, 20);

            SetDamageType(ResistanceType.Physical, 75);
            SetDamageType(ResistanceType.Energy, 25);

            SetResistance(ResistanceType.Physical, 35, 40);
            SetResistance(ResistanceType.Fire, 30, 40);
            SetResistance(ResistanceType.Cold, 25, 35);
            SetResistance(ResistanceType.Poison, 50, 60);
            SetResistance(ResistanceType.Energy, 30, 40);

            SetSkill(SkillName.MagicResist, 110.0);
            SetSkill(SkillName.Tactics, 70.1, 90.0);
            SetSkill(SkillName.Wrestling, 80.1, 100.0);

            Fame = 15000;
            Karma = 15000;

            VirtualArmor = 20;

            Tamable = false;
            ControlSlots = 3;
            MinTameSkill = 100.0;

            AddLoot(LootPack.UltraRich);
            PackGem();
            PackGem();
            PackGold(5000, 7000);
            PackMagicItems(1, 5);
            PackMagicItems(1, 5);
            PackMagicItems(1, 5);
        }

        public override int GetIdleSound()
        {
            return 0x2C4;
        }

        public override int GetAttackSound()
        {
            return 0x82;
        }

        public override int GetDeathSound()
        {
            return 0x84;
        }

        public override int GetAngerSound()
        {
            return 0x2C4;
        }

        public override int GetHurtSound()
        {
            return 0x83;
        }

        public override bool AutoDispel { get { return true; } }
        public override HideType HideType { get { return HideType.Barbed; } }
        public override int Hides { get { return 20; } }
        public override int Meat { get { return 19; } }
        public override int TreasureMapLevel { get { return 4; } }

        public WhiteStag(Serial serial)
            : base(serial)
        {
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.25 >= Utility.RandomDouble() && attacker is BaseCreature)
            {
                BaseCreature c = (BaseCreature)attacker;

                if (c.Controlled && c.ControlMaster != null)
                {
                    c.ControlTarget = c.ControlMaster;
                    c.ControlOrder = OrderType.Attack;
                    c.Combatant = c.ControlMaster;
                }
            }
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