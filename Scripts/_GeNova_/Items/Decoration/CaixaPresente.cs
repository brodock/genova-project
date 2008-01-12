using System;

namespace Server.Items
{
	public class CaixaPresente : BaseContainer
	{
		public override int ItemID{ get{ return 9002; } }

		public override Rectangle2D Bounds
		{
			get{ return new Rectangle2D( 33, 36, 109, 112 ); }
		}

		[Constructable]
		public CaixaPresente() : base( 0xE77 )
		{
			Name = "Caixa de Presente";
		}

		public CaixaPresente( Serial serial ) : base( serial )
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

