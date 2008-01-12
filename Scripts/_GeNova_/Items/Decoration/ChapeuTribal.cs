using System;

namespace Server.Items
{
	public class ChapeuTribal : Item 
	{
		public override int ItemID{ get{ return 5449; } }

		[Constructable]
		public ChapeuTribal() : base( 0xE77 )
		{
			Name = "Chapeu Tribal";
		}

		public ChapeuTribal( Serial serial ) : base( serial )
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

