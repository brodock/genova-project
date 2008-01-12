using System;
using Server;

namespace Server.Mobiles
{
	public class SummonedSkeletalKnight : BaseTalismanSummon
	{
		[Constructable]
		public SummonedSkeletalKnight() : base()
		{
			Name = "a skeletal knight";
			Body = 147;
			BaseSoundID = 451;
		}

		public SummonedSkeletalKnight( Serial serial ) : base( serial )
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