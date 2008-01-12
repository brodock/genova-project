using System;

namespace Server.Items
{
	public class CartasDeTaro : Item 
	{
		public override int ItemID{ get{ return 4776; } }

		[Constructable]
		public CartasDeTaro() : base( 0xE77 )
		{
			Name = "Cartas de Taro";
		}

		public CartasDeTaro( Serial serial ) : base( serial )
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

