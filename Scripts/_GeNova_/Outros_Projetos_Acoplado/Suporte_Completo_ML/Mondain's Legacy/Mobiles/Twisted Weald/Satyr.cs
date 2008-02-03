using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a satyr's corpse" )] 
	public class Satyr : BaseCreature
	{		
		// peace
		public virtual bool CanPeace{ get{ return true; } }
		public virtual int PeaceDuration{ get{ return 20; } }
		public virtual int PeaceMinDelay{ get{ return 10; } }
		public virtual int PeaceMaxDelay{ get{ return 10; } }
		
		// discord
		public virtual bool CanDiscord{ get{ return true; } }
		public virtual int DiscordDuration{ get{ return 20; } }
		public virtual int DiscordMinDelay{ get{ return 22; } }
		public virtual int DiscordMaxDelay{ get{ return 5; } }
		public virtual double DiscordModifier{ get{ return 0.28; } }
		
		// provocation
		public virtual bool CanProvoke{ get{ return true; } }
		public virtual int ProvokeMinDelay{ get{ return 5; } }
		public virtual int ProvokeMaxDelay{ get{ return 5; } }
				
		public virtual int PerceptionRange{ get{ return 12; } }
	
		[Constructable]
		public Satyr() : base( AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4 )
		{
			Name = "a satyr";
			Body =  0x10F;

			SetStr( 178, 94 );
			SetDex( 254, 268 );
			SetInt( 151, 169 );

			SetHits( 356, 400 );
			SetStam( 254, 268 );
			SetMana( 151, 169 );

			SetDamage( 20, 25 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 55, 60 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 30, 39 );
			SetResistance( ResistanceType.Poison, 31, 39 );
			SetResistance( ResistanceType.Energy, 31, 39 );

			SetSkill( SkillName.Wrestling, 81.2, 95.1 );
			SetSkill( SkillName.Tactics, 81.9, 98.0 );
			SetSkill( SkillName.MagicResist, 55.3, 63.9 );
		}

		public Satyr( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosRich, 3 );
		}
		
		public override void OnThink()
		{
			if ( CanPeace && m_NextPeaceTime <= DateTime.Now )
			{
				Mobile target = Combatant;
				
				if ( target != null && target.InRange( this, PerceptionRange ) && CanBeHarmful( target ) )
					Peace( target );
			}
			
			if ( CanDiscord && m_NextDiscordTime <= DateTime.Now )
			{
				Mobile target = Combatant;
				
				if ( target != null && target.InRange( this, PerceptionRange ) && CanBeHarmful( target ) )
					Discord( target );
			}
			
			if ( CanProvoke && m_NextProvokeTime <= DateTime.Now )
			{
				Mobile target = Combatant;
				
				if ( target != null && target.InRange( this, PerceptionRange ) && CanBeHarmful( target ) )
					Provoke( target );
			}
		}
		
		public override int GetDeathSound()	{ return 0x585; }
		public override int GetAttackSound() { return 0x586; }
		public override int GetIdleSound() { return 0x587; }
		public override int GetAngerSound() { return 0x588; }
		public override int GetHurtSound() { return 0x589; }
		
		private DateTime m_NextPeaceTime;
		private DateTime m_NextDiscordTime;
		private DateTime m_NextProvokeTime;
		
		public void Peace( Mobile target )
		{
			if ( target is PlayerMobile )
			{
				PlayerMobile player = (PlayerMobile) target;
				
				if ( player.PeacedUntil <= DateTime.Now )
				{
					player.PeacedUntil = DateTime.Now + TimeSpan.FromSeconds( PeaceDuration );
					player.SendLocalizedMessage( 500616 ); // You hear lovely music, and forget to continue battling!					
				}
			}
			else if ( target is BaseCreature )
			{
				BaseCreature creature = (BaseCreature) target;
				
				if ( !creature.BardPacified )
					creature.Pacify( this, DateTime.Now + TimeSpan.FromSeconds( PeaceDuration ) );
			}
			
			PlaySound( 0x58B );
						
			m_NextPeaceTime = DateTime.Now + TimeSpan.FromSeconds( PeaceMinDelay + Utility.RandomDouble() * PeaceMaxDelay );
		}
		
		public void Provoke( Mobile target )
		{		
			foreach ( Mobile m in GetMobilesInRange( PerceptionRange ) )
			{					
				if ( m is BaseCreature )
				{	
					BaseCreature c = (BaseCreature) m;
					
					if ( !c.CanBeHarmful( target, false ) || target == c || c.BardTarget == target )
						continue;
					
					if ( Utility.RandomDouble() < 0.9 )
					{
						c.BardMaster = this;
						c.BardTarget = target;
						c.Combatant = target;
						c.BardEndTime = DateTime.Now + TimeSpan.FromSeconds( 30.0 );
						
						target.SendLocalizedMessage( 1072062 ); // You hear angry music, and start to fight.
						PlaySound( 0x58A );
					}	
					else
					{
						target.SendLocalizedMessage( 1072063 ); // You hear angry music that fails to incite you to fight.
						PlaySound( 0x58C );
					}
						
					break;
				}		
			}		
		
			m_NextProvokeTime = DateTime.Now + TimeSpan.FromSeconds( ProvokeMinDelay + Utility.RandomDouble() * ProvokeMaxDelay );			
		}
		
		public void Discord( Mobile target )
		{
			if ( Utility.RandomDouble() < 0.9 )
			{							
				target.AddSkillMod( new TimedSkillMod( SkillName.Cooking, true, Combatant.Skills.Cooking.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );
				target.AddSkillMod( new TimedSkillMod( SkillName.Fishing, true, Combatant.Skills.Fishing.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );
				target.AddSkillMod( new TimedSkillMod( SkillName.Tactics, true, Combatant.Skills.Tactics.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );
				target.AddSkillMod( new TimedSkillMod( SkillName.Swords, true, Combatant.Skills.Swords.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );
				target.AddSkillMod( new TimedSkillMod( SkillName.Mining, true, Combatant.Skills.Mining.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );
				target.AddSkillMod( new TimedSkillMod( SkillName.Focus, true, Combatant.Skills.Focus.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );
				target.AddSkillMod( new TimedSkillMod( SkillName.Chivalry, true, Combatant.Skills.Chivalry.Base * DiscordModifier * -1, TimeSpan.FromSeconds( DiscordDuration ) ) );			
								
				Timer.DelayCall( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 1 ), (int) DiscordDuration, new TimerStateCallback( Animate ), target );
				
				target.SendLocalizedMessage( 1072061 ); // You hear jarring music, suppressing your strength.
				target.PlaySound( 0x58B );
			}	
			else
			{
				target.SendLocalizedMessage( 1072064 ); // You hear jarring music, but it fails to disrupt you.
				target.PlaySound( 0x58C );
			}							
			
			m_NextDiscordTime = DateTime.Now + TimeSpan.FromSeconds( DiscordMinDelay + Utility.RandomDouble() * DiscordMaxDelay );	
		}
		
		private void Animate( object state )
		{
			if ( state is Mobile )
			{
				Mobile mob = (Mobile) state;
				
				mob.FixedEffect( 0x376A, 1, 32 );
			}
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
