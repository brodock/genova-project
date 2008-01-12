using System;
using Server;
using Server.Items;
using Server.Misc;
using Server.Spells;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "a dread horns corpse" )]	
	public class DreadHorn : BasePeerless
	{		
		public virtual int StrikingRange{ get{ return 5; } }
	
		[Constructable]
		public DreadHorn() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a Dread Horn";
			Body = 257;
			BaseSoundID = 0xA8;

			SetStr( 812, 999 );
			SetDex( 507, 669 );
			SetInt( 1206, 1389 );

			SetHits( 50000 );
			SetStam( 507, 669 );
			SetMana( 1206, 1389 );

			SetDamage( 27, 31 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Poison, 60 );

			SetResistance( ResistanceType.Physical, 40, 53 );
			SetResistance( ResistanceType.Fire, 50, 63 );
			SetResistance( ResistanceType.Cold, 50, 62 );
			SetResistance( ResistanceType.Poison, 67, 73 );
			SetResistance( ResistanceType.Energy, 60, 73 );

			SetSkill( SkillName.Wrestling, 90.0 );
			SetSkill( SkillName.Tactics, 90.0 );
			SetSkill( SkillName.MagicResist, 110.0 );
			SetSkill( SkillName.Poisoning, 120.0 );
			SetSkill( SkillName.Magery, 110.0 );
			SetSkill( SkillName.EvalInt, 110.0 );
			SetSkill( SkillName.Meditation, 110.0 );
			
			// TODO 1-3 spellweaving scroll
			
			PackResources( 8 );
			PackTalismans( 5 );	
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss, 8 );
		}	
		
		public override void OnThink()
		{
			base.OnThink();
			
			if ( Combatant != null )
			{
				if ( InRange( Combatant.Location, StrikingRange ) && !InRange( Combatant.Location, 2 ) && InLOS( Combatant.Location ) && Utility.RandomDouble() < 0.2 )
				{
					Location = BasePeerless.GetSpawnPosition( Combatant.Location, Combatant.Map, 1 );
					FixedParticles( 0x376A, 9, 32, 0x13AF, EffectLayer.Waist );
					PlaySound( 0x1FE );
				}
				
				// TODO weak area spell?
			}
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			c.DropItem( new DreadHornMane() );	
			
			if ( Utility.RandomDouble() < 0.6 )
				c.DropItem( new TaintedMushroom() );
			
			if ( Utility.RandomDouble() < 0.6 )				
				c.DropItem( new ParrotItem() );
				
			if ( Utility.RandomDouble() < 0.5 )
				c.DropItem( new MangledHeadOfDreadhorn() );
				
			if ( Utility.RandomDouble() < 0.5 )
				c.DropItem( new HornOfTheDreadhorn() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new PristineDreadHorn() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new DreadFlute() );
				
			if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new DreadsRevenge() );
				
			if ( Utility.RandomDouble() < 0.025 )
				c.DropItem( new CrimsonCincture() );
				
			if ( Utility.RandomDouble() < 0.05 )
			{
				switch ( Utility.Random( 4 ) )
				{
					case 0: c.DropItem( new LeafweaveLegs() ); break;
					case 1: c.DropItem( new DeathChest() ); break;
					case 2: c.DropItem( new AssassinLegs() ); break;
					case 3: c.DropItem( new Feathernock() ); break;
				}
			}		
		}
		
		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Regular; } } 
		
		public override int Meat{ get{ return 5; } }
		public override MeatType MeatType{ get{ return MeatType.Ribs; } }
		
		public override bool GivesMinorArtifact{ get{ return true; } }
        public override bool Unprovokable{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }		
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public override int TreasureMapLevel{ get{ return 5; } }

		public DreadHorn( Serial serial ) : base( serial )
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
