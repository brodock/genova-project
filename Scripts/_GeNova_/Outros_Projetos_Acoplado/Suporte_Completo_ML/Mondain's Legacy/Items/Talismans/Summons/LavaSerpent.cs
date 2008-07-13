using System;
using Server;
using Server.Regions;

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
		
		public override void OnThink()
		{
			if ( m_NextWave < DateTime.Now )
				AreaHeatDamage();
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
		
		private DateTime m_NextWave;
		private bool m_Guards;
		
		public void AreaHeatDamage()
		{
			Mobile mob = ControlMaster;
			
			if ( mob != null )
			{			
				if ( mob.InRange( Location, 2 ) )
				{			
					if ( mob.AccessLevel != AccessLevel.Player )
					{					
						AOS.Damage( mob, Utility.Random( 2, 3 ), 0, 100, 0, 0, 0 );
						mob.SendLocalizedMessage( 1008112 ); // The intense heat is damaging you!
					}
				}
				
				GuardedRegion r = Region as GuardedRegion;
				
				if ( r != null && !m_Guards && mob.Alive )
				{
					foreach ( Mobile m in GetMobilesInRange( 2 ) )
					{				
						if ( m is BaseVendor && mob.InRange( m.Location, 10 ) )
						{
							m.Say( "*guards*" );
							r.MakeGuard( mob );
							m_Guards = true;
							
							Timer.DelayCall( TimeSpan.FromMinutes( 1 ), delegate() { m_Guards = false; } );
							break;
						} 
					}
				}
			}
						
			m_NextWave = DateTime.Now + TimeSpan.FromSeconds( 3 );
		}	
	}
}