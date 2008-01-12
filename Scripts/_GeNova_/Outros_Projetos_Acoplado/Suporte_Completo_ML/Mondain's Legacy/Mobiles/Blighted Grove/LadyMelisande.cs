using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Network;

namespace Server.Mobiles
{
	[CorpseName( "a lady melisande corpse" )]
	public class LadyMelisande : BasePeerless
	{
		private DateTime m_Speak;
		private bool m_SpawnedSatyrs;
		private bool m_SpawnedMinions;
	
		[Constructable]
		public LadyMelisande() : base( AIType.AI_Necromage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a lady melisande";
			Body = 0x102;
			BaseSoundID = 451;

			SetStr( 420, 976 );
			SetDex( 306, 327 );
			SetInt( 1588, 1676 );

			SetHits( 30000 );	

			SetDamage( 27, 31 );
			
			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 49, 55 );
			SetResistance( ResistanceType.Fire, 41, 48 );
			SetResistance( ResistanceType.Cold, 57, 63 );
			SetResistance( ResistanceType.Poison, 70, 72 );
			SetResistance( ResistanceType.Energy, 74, 80 );
			
			SetSkill( SkillName.Wrestling, 100.7, 102.0 );
			SetSkill( SkillName.Tactics, 100.1, 101.9 );
			SetSkill( SkillName.MagicResist, 120 );
			SetSkill( SkillName.Magery, 120 );
			SetSkill( SkillName.EvalInt, 120 );
			SetSkill( SkillName.Meditation, 120 );
			SetSkill( SkillName.Necromancy, 120 );
			SetSkill( SkillName.SpiritSpeak, 120 );
			
			SpawnSatyrs();
			PackResources( 8 );
			PackTalismans( 5 );
				
			m_Speak = DateTime.Now;			
			m_SpawnedSatyrs = false;
			m_SpawnedMinions = false;
		}	
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss, 8 );
		}	
		
		public override void OnThink()
		{
			base.OnThink();
			
			if ( !m_SpawnedSatyrs && Hits < 0.5 * HitsMax )
				SpawnSatyrs();
				
			if ( !m_SpawnedMinions && Hits < 200 )
				SpawnMinions();
				
			if ( m_Speak < DateTime.Now && Utility.RandomDouble() < 0.3 )
			{
				m_Speak = DateTime.Now + TimeSpan.FromSeconds( 7 );
				Say( Utility.RandomMinMax( 1075102, 1075115 ) );
			}
		}
		
		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );
			
			if ( Hits != HitsMax && Utility.RandomDouble() < 0.05 )
				HealEffect( defender );
		}
		
		public override void OnDamage( int amount, Mobile from, bool willKill )
		{				
			if ( Utility.RandomDouble() < 0.15 )
				SlowAttack( from );
				
			if ( willKill )
				Say( 1075118 ); // Noooooo!  You shall never defeat me.  Even if I should fall, my tree will sustain me and I will rise again.
				
			base.OnDamage( amount, from, willKill );				
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			c.DropItem( new DiseasedBark() );
			c.DropItem( new EternallyCorruptTree() );
			
			int drop = Utility.Random( 4, 8 );
			
			for ( int i = 0; i < drop; i ++ )
				c.DropItem( new MelisandesFermentedWine() );
			
			if ( Utility.RandomDouble() < 0.6 )				
				c.DropItem( new ParrotItem() );
			
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new MelisandesHairDye() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new MelisandesCorrodedHatchet() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new AlbinoSquirrelImprisonedInCrystal() );
			
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new MyrmidonLegs() );
			
			if ( Utility.RandomDouble() < 0.025 )
				c.DropItem( new CrimsonCincture() );
		}

		public override bool CanAnimateDead{ get{ return true; } }
		public override BaseCreature Animates{ get{ return new LichLord(); } }
		public override bool GivesMinorArtifact{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public LadyMelisande( Serial serial ) : base( serial )
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
			
			m_Speak = DateTime.Now;
			m_SpawnedSatyrs = false;
			m_SpawnedMinions = false;
		}
		
		#region Slow Attack
		private static Hashtable m_Table;
		
		public virtual void SlowAttack( Mobile to )
		{
			if ( m_Table == null )
				m_Table = new Hashtable();
		
			if ( to.Alive && to.Player && m_Table[ to ] == null )
			{
				to.Send( SpeedControl.WalkSpeed );
				/*to.SendLocalizedMessage( 1072069 ); // A cacophonic sound lambastes you, suppressing your ability to move.
				to.PlaySound( 0x584 );*/
				
				m_Table[ to ] = Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerStateCallback( EndSlow_Callback ), to );
			}
		}
		
		private void EndSlow_Callback( object state )
		{
			if ( state is Mobile )
				EndSlow( (Mobile) state );
		}		
		
		public virtual void EndSlow( Mobile from )
		{
			if ( m_Table == null )
				m_Table = new Hashtable();
				
			m_Table[ from ] = null;
				
			from.Send( SpeedControl.Disable );
		}
		
		public static bool UnderSlowAttack( Mobile from )
		{
			if ( m_Table == null )
				m_Table = new Hashtable();
			
			return m_Table[ from ] != null;
		}
		#endregion
		
		public void HealEffect( Mobile from )
		{
			if ( from is PlayerMobile )
				Hits += from.Hits;
			else
				Hits += from.Hits / 3;
			
			PlaySound( 0xF6 );
			PlaySound( 0x1F7 );
			FixedParticles( 0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head );
			
			Say( 1075117 );  // Muahahaha!  Your life essence is MINE!
			Say( 1075120 );  // An unholy aura surrounds Lady Melisande as her wounds begin to close.
		}
		
		public virtual void SpawnSatyrs()
		{
			for ( int i = 0; i < 4; i ++ )
			{
				EnslavedSatyr satyr = new EnslavedSatyr();
				
				satyr.MoveToWorld( GetSpawnPosition( 6 ), Map );	
				
				if ( satyr.Z < 0 )
					satyr.MoveToWorld( Location, Map );			
			}
			
			Say( 1075119 ); // Awake my children!  I summon thee!
			
			m_SpawnedSatyrs = true;
		}
		
		public virtual void SpawnMinions()
		{
			Mobile minion = null;
			
			for ( int i = 0; i < 3; i ++ )
			{
				switch ( Utility.Random( 4 ) )
				{
					default:
					case 0: minion = new InsaneDryad(); break;
					case 1: minion = new Reaper(); break;
					case 2: minion = new StoneHarpy(); break;
				}
				
				minion.MoveToWorld( GetSpawnPosition( 6 ), Map );
				
				if ( minion.Z < 0 )
					minion.MoveToWorld( Location, Map );
			}
			
			Say( 1075119 ); // Awake my children!  I summon thee!
			
			m_SpawnedMinions = true;
		}
	}
}