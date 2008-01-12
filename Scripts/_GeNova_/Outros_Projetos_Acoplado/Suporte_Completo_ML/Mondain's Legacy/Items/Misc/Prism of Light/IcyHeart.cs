using System;
using Server;

namespace Server.Items
{
	public class IcyHeart : Item
	{	
		[Constructable]
		public IcyHeart() : base( 0x24B )
		{
			Weight = 1;
		}

		public IcyHeart( Serial serial ) : base( serial )
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

