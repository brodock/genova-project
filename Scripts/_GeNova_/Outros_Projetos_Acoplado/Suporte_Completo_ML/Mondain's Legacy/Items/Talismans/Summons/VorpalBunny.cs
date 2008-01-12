using System;
using Server;
using BunnyHole = Server.Mobiles.VorpalBunny.BunnyHole;

namespace Server.Mobiles
{
	public class SummonedVorpalBunny : BaseTalismanSummon
	{
		[Constructable]
		public SummonedVorpalBunny() : base()
		{
			Name = "a vorpal bunny";
			Body = 205;
			Hue = 0x480;
			BaseSoundID = 0xC9;
			
			DelayBeginTunnel();
		}

		public SummonedVorpalBunny( Serial serial ) : base( serial )
		{
		}
		
		public virtual void DelayBeginTunnel()
		{
			Timer.DelayCall( TimeSpan.FromMinutes( 30.0 ), new TimerCallback( BeginTunnel ) );
		}

		public virtual void BeginTunnel()
		{
			if ( Deleted )
				return;

			new BunnyHole().MoveToWorld( Location, Map );

			Frozen = true;
			Say( "* The bunny begins to dig a tunnel back to its underground lair *" );
			PlaySound( 0x247 );

			Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) );
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