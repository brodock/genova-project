using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a black order grand mage corpse" )] 
	public class DragonsFlameGrandMage : DragonsFlameMage
	{	
		[Constructable]
		public DragonsFlameGrandMage() : base()
		{
			Name = "black order grand mage";
			Title = "of the dragon's flame sect";
		}

		public DragonsFlameGrandMage( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 6 );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			if ( Utility.RandomDouble() < 0.3 )
				c.DropItem( new DragonFlameKey() );
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
