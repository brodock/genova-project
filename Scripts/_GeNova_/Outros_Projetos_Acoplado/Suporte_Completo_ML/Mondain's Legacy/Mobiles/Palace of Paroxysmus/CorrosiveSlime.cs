using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a corrosive slime corpse" )]
	public class CorrosiveSlime : Slime
	{
		[Constructable]
		public CorrosiveSlime() : base()
		{
			Name = "a corrosive slime";
		}
		
		public CorrosiveSlime( Serial serial ) : base( serial )
		{
		}	
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			if ( Utility.RandomDouble() < 0.05 )
			{
				switch ( Utility.Random( 3 ) )
				{
					case 0: c.DropItem( new CoagulatedLegs() ); break;
					case 1: c.DropItem( new PartiallyDigestedTorso() ); break;
					case 2: c.DropItem( new GelatanousSkull() ); break;
				}
			}
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
