using System;

namespace Server.Items
{
	[Furniture]
	[Flipable( 0x2DE9, 0x2DEA )]
	public class OrnateElvenChest : BaseContainer
	{
		public override int DefaultGumpID{ get{ return 0x4A; } }
		public override int DefaultDropSound{ get{ return 0x42; } }
		public override int LabelNumber{ get{ return 1031753; } } // ornate elven chest

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 20, 105, 150, 180 ); }
		}

		[Constructable]
		public OrnateElvenChest() : base( 0x2DE9 )
		{
			Weight = 1.0;
		}

		public OrnateElvenChest( Serial serial ) : base( serial )
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

