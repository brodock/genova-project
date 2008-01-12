using System;

namespace Server.Items
{
	public class ArmaduraPendurada : Item 
	{
		public override int ItemID{ get{ return 5095; } }

		[Constructable]
		public ArmaduraPendurada() : base( 0xE77 )
		{
			Name = "Armadura";
		}

		public ArmaduraPendurada( Serial serial ) : base( serial )
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

