using System;
using Server;
using Server.Items;

namespace Server.Mobiles 
{ 
	[CorpseName( "a troglodyte corpse" )] 
	public class Troglodyte : BaseCreature 
	{ 		
		[Constructable] 
		public Troglodyte() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 ) 
		{ 			
			Name = "a troglodyte";
			Body = 0x10B;
			
			SetStr( 161, 207 );
			SetDex( 98, 118 );
			SetInt( 55, 69 );
			
			SetHits( 304, 338 );
			SetStam( 98, 118 );
			SetMana( 55, 69 );

			SetDamage( 6, 12 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 33 );
			SetResistance( ResistanceType.Fire, 21, 29 );
			SetResistance( ResistanceType.Cold, 35, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Wrestling, 70.7, 93.2 );	
			SetSkill( SkillName.Tactics, 82.0, 94.3 );
			SetSkill( SkillName.MagicResist, 50.3, 64.6 );
			SetSkill( SkillName.Anatomy, 74.8, 94.3 );
			SetSkill( SkillName.Healing, 70.9, 93.2 );

			Fame = 1000;
			Karma = 1000;
			
			PackItem( new Bandage( Utility.RandomMinMax( 6, 17 ) ) );
			PackItem( new TreasureMap( 2, Map.Trammel ) );
		}

		public Troglodyte( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosRich, 3 );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			if ( Utility.RandomDouble() < 0.25 )
				c.DropItem( new PrimitiveFetish() );
		}
		
		public override bool CanHeal{ get{ return true; } }		
		public override double MinHealDelay{ get{ return 4.0; } }
		public override int Meat{ get{ return 1; } }

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