using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedWailingBanshee : BaseTalismanSummon
	{
		[Constructable]
		public SummonedWailingBanshee() : base()
		{
			Name = "a wailing banshee";
			Body = 310;
			BaseSoundID = 0x482;
		}

		public SummonedWailingBanshee( Serial serial ) : base( serial )
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