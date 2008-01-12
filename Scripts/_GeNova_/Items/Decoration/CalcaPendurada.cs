using System;

namespace Server.Items
{
	public class CalcaPendurada : Item 
	{
		public override int ItemID{ get{ return 5093; } }

		[Constructable]
		public CalcaPendurada() : base( 0xE77 )
		{
			Name = "Calca Pendurada";
		}

		public CalcaPendurada( Serial serial ) : base( serial )
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

