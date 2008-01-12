using System;
using Server;

namespace Server.Items
{
	public class ShimmeringCrystals : Item
	{
		public override int LabelNumber{ get{ return 1075095; } } // Shimmering Crystals
	
		[Constructable]
		public ShimmeringCrystals() : base( Utility.RandomMinMax( 0x2206, 0x222C ) )
		{
			Weight = 1.0;
		}

		public ShimmeringCrystals( Serial serial ) : base( serial )
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

