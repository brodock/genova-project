using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a minotaur captain corpse" )]	
	public class MinotaurCaptain : Minotaur
	{
		[Constructable]
		public MinotaurCaptain() : base()
		{
			Name = "a minotaur captain";
			Body = 0x118;			

			SetStr( 403, 421 );
			SetDex( 92, 109 );
			SetInt( 33, 50 );

			SetHits( 402, 426 );

			SetDamage( 11, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 66, 75 );
			SetResistance( ResistanceType.Fire, 35, 45 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 41, 50 );
			SetResistance( ResistanceType.Energy, 41, 50 );

			SetSkill( SkillName.Wrestling, 90.5, 105.2 );
			SetSkill( SkillName.Tactics, 92.0, 107.1 );
			SetSkill( SkillName.MagicResist, 66.5, 74.9 );
		}
				
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 2 );
		}

		public MinotaurCaptain( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}