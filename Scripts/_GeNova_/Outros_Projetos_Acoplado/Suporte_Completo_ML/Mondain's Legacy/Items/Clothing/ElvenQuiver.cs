using System;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0x2FB7, 0x3171 )]
	public class ElvenQuiver : BaseQuiver
	{
		public override int LabelNumber{ get{ return 1032657; } } // elven quiver
		
		[Constructable]
		public ElvenQuiver() : base()
		{
			Attributes.WeaponDamage = 10;
			
			WeightReduction = 30;
		}

		public ElvenQuiver( Serial serial ) : base( serial )
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