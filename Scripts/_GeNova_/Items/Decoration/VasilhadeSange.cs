using System;

namespace Server.Items
{
	public class VasilhadeSangue : Item 
	{
		public override int ItemID{ get{ return 3619; } }

		[Constructable]
		public VasilhadeSangue() : base( 0xE77 )
		{
			Name = "Flexas Empilhadas";
		}

		public VasilhadeSangue( Serial serial ) : base( serial )
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

