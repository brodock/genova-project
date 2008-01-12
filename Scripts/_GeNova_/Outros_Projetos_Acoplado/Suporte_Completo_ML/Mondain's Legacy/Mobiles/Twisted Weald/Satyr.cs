using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a satyr's corpse" )] 
	public class Satyr : BaseCreature
	{
		public virtual double Modifier{ get{ return -0.28; } }
		public virtual TimeSpan ModifierDuration{ get{ return TimeSpan.FromMinutes( 1 ); } }
	
		[Constructable]
		public Satyr() : base( AIType.AI_Animal, FightMode.Closest, 10, 1, 0.2, 0.4 )
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
			if ( Combatant != null )
			{
				if ( Utility.RandomDouble() < 0.05 && !m_Table.Contains( Combatant ) )
					Suppress();
			}
		}
		
		public override int GetDeathSound()	{ return 0x585; }
		public override int GetAttackSound() { return 0x586; }
		public override int GetIdleSound() { return 0x587; }
		public override int GetAngerSound() { return 0x588; }
		public override int GetHurtSound() { return 0x589; }
		
		private static Hashtable m_Table = new Hashtable();
		
		public virtual void Suppress()
		{
			if ( Utility.RandomDouble() < 0.75 )
			{				
				Combatant.SendLocalizedMessage( 1072061 ); // You hear jarring music, suppressing your strength.
				Combatant.PlaySound( 0x58B );
			
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Cooking, true, Combatant.Skills.Cooking.Base * Modifier, ModifierDuration ) );
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Fishing, true, Combatant.Skills.Fishing.Base * Modifier, ModifierDuration ) );
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Tactics, true, Combatant.Skills.Tactics.Base * Modifier, ModifierDuration ) );
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Swords, true, Combatant.Skills.Swords.Base * Modifier, ModifierDuration ) );
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Mining, true, Combatant.Skills.Mining.Base * Modifier, ModifierDuration ) );
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Focus, true, Combatant.Skills.Focus.Base * Modifier, ModifierDuration ) );
				Combatant.AddSkillMod( new TimedSkillMod( SkillName.Chivalry, true, Combatant.Skills.Chivalry.Base * Modifier, ModifierDuration ) );				
				
				m_Table[ Combatant ] = true;
				
				Timer.DelayCall( TimeSpan.FromSeconds( 1 ), TimeSpan.FromSeconds( 1 ), (int) ModifierDuration.TotalSeconds, new TimerStateCallback( Animate ), Combatant );
				Timer.DelayCall( TimeSpan.FromMinutes( 1 ), new TimerStateCallback( Timeout ), Combatant );
			}	
			else
			{
				Combatant.SendLocalizedMessage( 1072063 ); // You hear angry music that fails to incite you to fight.
				Combatant.PlaySound( 0x58C );
			}							
		}
		
		public virtual void Animate( object state )
		{
			if ( state is Mobile )
			{
				Mobile mob = (Mobile) state;
				
				mob.FixedEffect( 0x376A, 1, 32 );
			}
		}
		
		public virtual void Timeout( object state )
		{
			if ( state is Mobile )
			{
				Mobile mob = (Mobile) state;
				
				m_Table.Remove( mob );
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
