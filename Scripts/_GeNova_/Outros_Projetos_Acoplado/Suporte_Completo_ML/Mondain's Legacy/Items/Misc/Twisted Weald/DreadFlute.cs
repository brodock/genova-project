using System;

namespace Server.Items
{
	[Flipable(0x315C, 0x315D)]
	public class DreadFlute : BaseInstrument
	{
		public override int LabelNumber{ get{ return 1075089; } } // Dread Flute
	
		[Constructable]
		public DreadFlute() : base( 0x315C, 0x58B, 0x58C ) // TODO check sounds
		{
			Weight = 1.0;
			UsesRemaining = 700;
			ReplenishesCharges = true;
			Hue = 0x4F2;
		}

		public DreadFlute( Serial serial ) : base( serial )
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

