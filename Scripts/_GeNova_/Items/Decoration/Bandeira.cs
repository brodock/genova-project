using System;

namespace Server.Items
{
	public class Bandeira : Item 
	{
		public override int ItemID{ get{ return 5591; } }

		[Constructable]
		public Bandeira() : base( 0xE77 )
		{
			Name = "Bandeira";
		}

		public Bandeira( Serial serial ) : base( serial )
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

