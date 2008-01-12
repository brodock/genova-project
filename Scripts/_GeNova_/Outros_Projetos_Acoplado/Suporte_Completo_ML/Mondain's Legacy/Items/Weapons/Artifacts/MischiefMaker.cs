using System;
using Server.Items;

namespace Server.Items
{
	public class MischiefMaker : MagicalShortbow
	{
		public override int LabelNumber{ get{ return 1072910; } } // Mischief Maker

		[Constructable]
		public MischiefMaker() : base()
		{
			Hue = 0x8AB;
			Balanced = true;
			
			Slayer = SlayerName.Exorcism;
			
			Attributes.WeaponSpeed = 35;
			Attributes.WeaponDamage = 45;
		}

		public MischiefMaker( Serial serial ) : base( serial )
		{
		}
		
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			phys = fire = pois = nrgy = 0;
			cold = 100;
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