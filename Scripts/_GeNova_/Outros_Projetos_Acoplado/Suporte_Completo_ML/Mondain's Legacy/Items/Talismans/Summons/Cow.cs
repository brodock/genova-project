using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedCow : BaseTalismanSummon
	{
		[Constructable]
		public SummonedCow() : base()
		{
			Name = "a cow";
			Body = Utility.RandomList( 0xD8, 0xE7 );
			BaseSoundID = 0x78;
		}

		public SummonedCow( Serial serial ) : base( serial )
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