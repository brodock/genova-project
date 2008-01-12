using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("Corpo do Dragao de Veneno")]
    public class PoisonDragon : BaseCreature
    {
        [Constructable]
        public PoisonDragon()
            : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4)
        {
            Name = "Dragao de Veneno";
            Body = Utility.RandomList(62);
            BaseSoundID = 362;

            SetStr(500, 800);
            SetDex(200, 250);
            SetInt(120, 180);

            SetHits(2300, 3000);

            SetDamage(25, 56);

            SetDamageType(ResistanceType.Physical, 20);
            SetDamageType(ResistanceType.Poison, 80);

            SetResistance(ResistanceType.Physical, 45, 50);
            SetResistance(ResistanceType.Fire, 50, 60);
            SetResistance(ResistanceType.Cold, 40, 50);
            SetResistance(ResistanceType.Poison, 80, 90);
            SetResistance(ResistanceType.Energy, 55, 65);

            SetSkill(SkillName.MagicResist, 65.1, 80.0);
            SetSkill(SkillName.Tactics, 65.1, 90.0);
            SetSkill(SkillName.Wrestling, 65.1, 80.0);

            Fame = 7700;
            Karma = -7700;
            Hue = 0x232;

            VirtualArmor = 50;

            Tamable = false;

            AddLoot(LootPack.UltraRich);
            PackGem();
            PackGem();
            PackGold(1000, 1540);
            PackMagicItems(1, 5);
            PackItem(new Nightshade(4));
            PackItem(new LesserPoisonPotion());
        }

        public override Poison PoisonImmune { get { return Poison.Lethal; } }

        public override Poison HitPoison { get { return Poison.Lethal; } }
        public override double HitPoisonChance { get { return 0.75; } }

        public override int TreasureMapLevel { get { return 2; } }
        public override FoodType FavoriteFood { get { return FoodType.Meat | FoodType.Fish; } }

        public PoisonDragon(Serial serial)
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
