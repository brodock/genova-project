using System;

namespace Server.Items
{
	public class FlexasEmpilhadas : Item 
	{
		public override int ItemID{ get{ return 3905; } }

		[Constructable]
		public FlexasEmpilhadas() : base( 0xE77 )
		{
			Name = "Flexas Empilhadas";
		}

		public FlexasEmpilhadas( Serial serial ) : base( serial )
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

