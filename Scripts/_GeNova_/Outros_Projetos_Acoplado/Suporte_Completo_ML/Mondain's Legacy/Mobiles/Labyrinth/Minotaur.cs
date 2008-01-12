using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a minotaur corpse" )]	
	public class Minotaur : BaseCreature
	{
		[Constructable]
		public Minotaur() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a minotaur";
			Body = 0x107;			

			SetStr( 301, 322 );
			SetDex( 94, 108 );
			SetInt( 35, 49 );

			SetHits( 302, 332 );

			SetDamage( 11, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 25, 33 );
			SetResistance( ResistanceType.Cold, 32, 40 );
			SetResistance( ResistanceType.Poison, 31, 39 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Wrestling, 83.2, 99.7 );
			SetSkill( SkillName.Tactics, 80.2, 95.2 );
			SetSkill( SkillName.MagicResist, 55.2, 65.0 );
		}
				
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich );
		}
		
		public override WeaponAbility GetWeaponAbility()
		{
			return WeaponAbility.ParalyzingBlow;
		}
		
		public override void OnThink()
		{
			if ( Combatant != null )
			{
				if ( !InRange( Combatant.Location, 5 ) )
					CurrentSpeed = 0.05;
				else
					CurrentSpeed = ActiveSpeed;
			}			
		}
		
		public override int TreasureMapLevel{ get{ return 3; } }
		
		public override int GetDeathSound()	{ return 0x596; }
		public override int GetAttackSound() { return 0x597; }
		public override int GetIdleSound() { return 0x598; }
		public override int GetAngerSound() { return 0x599; }
		public override int GetHurtSound() { return 0x59A; }

		public Minotaur( Serial serial ) : base( serial )
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