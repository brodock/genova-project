using System;
using Server;


namespace Server.Mobiles
{
	public class SummonedLavaSerpent : BaseTalismanSummon
	{
		[Constructable]
		public SummonedLavaSerpent() : base()
		{
			Name = "a lava serpent";
			Body = 90;
			BaseSoundID = 219;
		}

		public SummonedLavaSerpent( Serial serial ) : base( serial )
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