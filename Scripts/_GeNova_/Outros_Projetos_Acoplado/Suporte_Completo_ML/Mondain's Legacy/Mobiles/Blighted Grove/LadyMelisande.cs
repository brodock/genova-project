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
			
			PackResources( 8 );
			PackTalismans( 5 );
				
			m_Speak = DateTime.Now;		
			m_NextHeal = DateTime.Now;
			m_NextSlowAttack = DateTime.Now;
			m_SpawnedMinions = false;
		}	
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss, 8 );
		}	
		
		public override void OnThink()
		{
			base.OnThink();
			
			if ( Combatant != null && Hits < 1000 && Combatant.InRange( Location, 2 ) && m_NextHeal < DateTime.Now )
				HealEffect( Combatant );
				
			if ( Combatant != null && m_Speak < DateTime.Now && Utility.RandomDouble() < 0.6 )
				Talk();
				
			if ( Combatant != null && m_Speak < DateTime.Now && Utility.RandomDouble() < 0.3 )
				SlowAttack( Combatant );
		}
		
		public override void OnDamage( int amount, Mobile from, bool willKill )
		{								
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
		}
		
		#region Talk
		public void Talk()
		{
			Say( Utility.RandomMinMax( 1075102, 1075115 ) );		
			
			m_Speak = DateTime.Now + TimeSpan.FromSeconds( 5 + 20 - Hits / HitsMax * 20 );	
		}
		#endregion
		
		#region Slow Attack
		private static Hashtable m_Table;
		private DateTime m_NextSlowAttack;
		
		public virtual void SlowAttack( Mobile to )
		{
			if ( m_Table == null )
				m_Table = new Hashtable();
		
			if ( to.Alive && to.Player && m_Table[ to ] == null )
			{				
				m_Table[ to ] = Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerStateCallback( EndSlow_Callback ), to );
			}
			
			m_NextSlowAttack = DateTime.Now + TimeSpan.FromSeconds( 35 + Utility.RandomDouble() * 20 );
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
		}
		
		public static bool UnderSlowAttack( Mobile from )
		{
			if ( m_Table == null )
				m_Table = new Hashtable();
			
			return m_Table[ from ] != null;
		}
		#endregion
		
		#region Heal
		private DateTime m_NextHeal;
		
		public void HealEffect( Mobile from )
		{
			if ( from.Player )
				Hits += from.Hits;
			else
				Hits += from.Hits / 3;
			
			FixedParticles( 0x376A, 9, 32, 5005, EffectLayer.Waist );
			PlaySound( 0x1F2 );
			
			Say( 1075117 );  // Muahahaha!  Your life essence is MINE!
			
			if ( Altar != null )
				Altar.SendMessage( 1075120 );  // An unholy aura surrounds Lady Melisande as her wounds begin to close.
			
			m_NextHeal = DateTime.Now + TimeSpan.FromSeconds( 10 + Utility.RandomDouble() * 5 );
		}
		#endregion
		
		#region Helpers
		public override bool CanSpawnHelpers{ get{ return true; } }
		public override int MaxHelpersWaves{ get{ return 1; } }
		public override double SpawnHelpersChance{ get{ return 0.1; } }
		
		private bool m_SpawnedMinions;
		
		public override bool CanSpawnWave()
		{
			if ( !m_SpawnedMinions && Hits < 200 )
				return ( m_SpawnedMinions = true );
				
			return base.CanSpawnWave();
		}
		
		public override void SpawnHelpers()
		{			
			if ( Hits < 1000 )
			{
				SpawnHelper( new InsaneDryad(), 6498, 945, 17 );
				SpawnHelper( new Reaper(), 6491, 948, 18 );
				SpawnHelper( new StoneHarpy(), 6500, 939, 10 ); 				
			}
			else
			{
				SpawnHelper( new EnslavedSatyr(), 6493, 947, 16 );
				SpawnHelper( new EnslavedSatyr(), 6498, 944, 16 );
				SpawnHelper( new EnslavedSatyr(), 6501, 940, 8 ); 
			}
						
			Say( 1075119 ); // Awake my children!  I summon thee!
		}
		#endregion
	}
}