using System;
using Server.Items;

namespace Server.Items
{
	public class ButchersWarCleaver : WarCleaver
	{
		public override int LabelNumber{ get{ return 1073526; } } // butcher's war cleaver

		[Constructable]
		public ButchersWarCleaver() : base()
		{
			// TODO Bovine slayer
		}

		public ButchersWarCleaver( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}