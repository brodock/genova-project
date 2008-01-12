using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedOrcBrute : BaseTalismanSummon
	{
		[Constructable]
		public SummonedOrcBrute() : base()
		{
			Body = 189;
			Name = "an orc brute";
			BaseSoundID = 0x45A;
		}

		public SummonedOrcBrute( Serial serial ) : base( serial )
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