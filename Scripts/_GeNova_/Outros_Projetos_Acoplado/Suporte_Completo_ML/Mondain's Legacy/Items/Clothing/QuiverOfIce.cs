using System;
using Server.Items;

namespace Server.Items
{
	public class QuiverOfIce : ElvenQuiver
	{
		public override int LabelNumber{ get{ return 1073110; } } // quiver of ice
	
		[Constructable]
		public QuiverOfIce() : base()
		{
			Hue = 0x4ED;
			
			DamageModifier.Cold = 50;
			DamageModifier.Physical = 50;
		}

		public QuiverOfIce( Serial serial ) : base( serial )
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