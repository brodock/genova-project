using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedGreatHart : BaseTalismanSummon
	{
		[Constructable]
		public SummonedGreatHart() : base()
		{
			Name = "a great hart";
			Body = 0xEA;
			BaseSoundID = 0x82;
		}

		public SummonedGreatHart( Serial serial ) : base( serial )
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