using System;

namespace Server.Items
{
	public class CabecaDeVeado : Item 
	{
		public override int ItemID{ get{ return 5447; } }

		[Constructable]
		public CabecaDeVeado() : base( 0xE77 )
		{
			Name = "Cabeca de Veado";
		}

		public CabecaDeVeado( Serial serial ) : base( serial )
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

