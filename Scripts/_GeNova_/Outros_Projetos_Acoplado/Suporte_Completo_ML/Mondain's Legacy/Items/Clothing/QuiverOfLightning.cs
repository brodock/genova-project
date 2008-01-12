using System;
using Server.Items;

namespace Server.Items
{
	public class QuiverOfLightning : ElvenQuiver
	{		
		public override int LabelNumber{ get{ return 1073112; } } // Quiver of Lightning
		
		[Constructable]
		public QuiverOfLightning() : base()
		{
			Hue = 0x4F3;
			
			DamageModifier.Energy = 50;
			DamageModifier.Physical = 50;
		}

		public QuiverOfLightning( Serial serial ) : base( serial )
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