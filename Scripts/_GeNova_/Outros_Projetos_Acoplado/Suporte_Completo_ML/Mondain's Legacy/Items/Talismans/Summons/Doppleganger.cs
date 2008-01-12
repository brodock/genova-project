using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedDoppleganger : BaseTalismanSummon
	{
		[Constructable]
		public SummonedDoppleganger() : base()
		{
			Name = "a doppleganger";
			Body = 0x309;
			BaseSoundID = 0x451;
		}

		public SummonedDoppleganger( Serial serial ) : base( serial )
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