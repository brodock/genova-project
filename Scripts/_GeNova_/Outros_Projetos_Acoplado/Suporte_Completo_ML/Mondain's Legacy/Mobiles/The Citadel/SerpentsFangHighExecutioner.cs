using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a black order high executioner corpse" )] 
	public class SerpentsFangHighExecutioner : SerpentsFangAssassin
	{	
		[Constructable]
		public SerpentsFangHighExecutioner() : base()
		{
			Name = "black order high executioner";
			Title = "of the serpent's fang sect";
		}

		public SerpentsFangHighExecutioner( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 6 );
		}
		
		public override void AlterMeleeDamageFrom( Mobile from, ref int damage )
		{
			if ( from != null )
				from.Damage( damage / 2, from );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			if ( Utility.RandomDouble() < 0.2 )
				c.DropItem( new SerpentFangKey() );
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
