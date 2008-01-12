using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedAntLion : BaseTalismanSummon
	{
		[Constructable]
		public SummonedAntLion() : base()
		{
			Name = "an ant lion";
			Body = 787;
			BaseSoundID = 1006;
		}

		public SummonedAntLion( Serial serial ) : base( serial )
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