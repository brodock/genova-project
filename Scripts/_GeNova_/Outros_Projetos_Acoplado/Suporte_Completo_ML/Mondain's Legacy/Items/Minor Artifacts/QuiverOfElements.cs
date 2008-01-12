using System;
using Server.Items;

namespace Server.Items
{
	public class QuiverOfElements : BaseQuiver
	{
		public override int LabelNumber{ get{ return 1075040; } } // Quiver of the Elements

		[Constructable]
		public QuiverOfElements() : base()
		{
			Hue = 0x104;
			
			Attributes.WeaponDamage = 10;
			
			DamageModifier.Fire = 100;
			
			WeightReduction = 50;
		}

		public QuiverOfElements( Serial serial ) : base( serial )
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