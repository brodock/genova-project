using System;

namespace Server.Items
{
	public class CabecaDeUrso : Item 
	{
		public override int ItemID{ get{ return 5445; } }

		[Constructable]
		public CabecaDeUrso() : base( 0xE77 )
		{
			Name = "Pele de Urso";
		}

		public CabecaDeUrso( Serial serial ) : base( serial )
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

