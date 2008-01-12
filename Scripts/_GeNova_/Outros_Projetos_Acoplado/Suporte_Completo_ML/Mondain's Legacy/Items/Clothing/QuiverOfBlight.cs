using System;
using Server.Items;

namespace Server.Items
{
	public class QuiverOfBlight : ElvenQuiver
	{
		public override int LabelNumber{ get{ return 1073111; } } // Quiver of Blight
		
		[Constructable]
		public QuiverOfBlight() : base()
		{
			Hue = 0x4F9;
			
			DamageModifier.Cold = 50;
			DamageModifier.Poison = 50;
		}

		public QuiverOfBlight( Serial serial ) : base( serial )
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