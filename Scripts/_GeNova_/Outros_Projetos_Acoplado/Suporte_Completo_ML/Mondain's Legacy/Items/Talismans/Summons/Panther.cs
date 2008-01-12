using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedPanther : BaseTalismanSummon
	{
		[Constructable]
		public SummonedPanther() : base()
		{
			Name = "a panther";
			Body = 0xD6;
			Hue = 0x901;
			BaseSoundID = 0x462;
		}

		public SummonedPanther( Serial serial ) : base( serial )
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