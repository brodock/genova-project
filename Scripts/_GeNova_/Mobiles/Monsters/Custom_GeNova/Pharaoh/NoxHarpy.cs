using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a poison corpse" )]
	public class NoxHarpy : BaseCreature
	{
		[Constructable]
		public NoxHarpy() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Harpia Venenosa";
			Body = 30;
			Hue = 1269;
			BaseSoundID = 0x24D;

			SetStr( 467, 645 );
			SetDex( 77, 95 );
			SetInt( 126, 150 );

			SetHits( 696, 772 );
			SetMana( 46, 70 );

			SetDamage( 25, 32 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 35, 45 );
			SetResistance( ResistanceType.Poison, 90, 100 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.EvalInt, 70.3, 100.0 );
			SetSkill( SkillName.Magery, 70.3, 100.0 );
			SetSkill( SkillName.Poisoning, 120.1, 130.0 );
			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 120.1, 130.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 50;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich, 5 );
		}

		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }


		public NoxHarpy( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( BaseSoundID == 263 )
				BaseSoundID = 0x24D;
		}
	}
}
