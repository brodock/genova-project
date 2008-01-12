using System;

namespace Server.Items
{
	public class QuadroDecorativo : Item 
	{
		public override int ItemID{ get{ return 9038; } }

		[Constructable]
		public QuadroDecorativo() : base( 0xE77 )
		{
			Name = "Quadro Decorativo";
		}

		public QuadroDecorativo( Serial serial ) : base( serial )
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

