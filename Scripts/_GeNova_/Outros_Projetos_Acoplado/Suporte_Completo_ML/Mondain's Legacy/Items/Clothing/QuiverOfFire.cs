using System;
using Server.Items;

namespace Server.Items
{
	public class QuiverOfFire : ElvenQuiver
	{
		public override int LabelNumber{ get{ return 1073109; } } // quiver of fire
		
		[Constructable]
		public QuiverOfFire() : base()
		{
			Hue = 0x4E7;
			
			DamageModifier.Fire = 50;
			DamageModifier.Physical = 50;
		}

		public QuiverOfFire( Serial serial ) : base( serial )
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