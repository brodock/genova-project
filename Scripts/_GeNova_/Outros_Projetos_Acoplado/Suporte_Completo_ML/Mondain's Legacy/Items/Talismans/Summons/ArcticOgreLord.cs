using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedArcticOgreLord : BaseTalismanSummon
	{
		[Constructable]
		public SummonedArcticOgreLord() : base()
		{
			Name = "an arctic ogre lord";
			Body = 135;
			BaseSoundID = 427;
		}

		public SummonedArcticOgreLord( Serial serial ) : base( serial )
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