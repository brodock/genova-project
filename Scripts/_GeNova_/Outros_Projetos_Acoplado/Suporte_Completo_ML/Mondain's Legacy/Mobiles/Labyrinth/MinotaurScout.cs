using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a minotaur scout corpse" )]	
	public class MinotaurScout : Minotaur
	{
		[Constructable]
		public MinotaurScout() : base()
		{
			Name = "a minotaur scout";
			Body = 0x119;			

			SetStr( 352, 374 );
			SetDex( 111, 130 );
			SetInt( 32, 48 );

			SetHits( 354, 377 );

			SetDamage( 11, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 30, 39 );
			SetResistance( ResistanceType.Poison, 31, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Wrestling, 86.8, 102.7 );
			SetSkill( SkillName.Tactics, 86.8, 103.9 );
			SetSkill( SkillName.MagicResist, 61.2, 69.5 );
		}

		public MinotaurScout( Serial serial ) : base( serial )
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