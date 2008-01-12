using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a fetid essence corpse" )]
	public class FetidEssence : BaseCreature
	{
		[Constructable]
		public FetidEssence() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "a fetid essence";
			Body = 0x110;
			BaseSoundID = 0x56C;

			SetStr( 108, 119 );
			SetDex( 210, 245 );
			SetInt( 453, 537 );

			SetHits( 571, 648 );

			SetDamage( 14, 17 );

			SetDamageType( ResistanceType.Physical, 30 );
			SetDamageType( ResistanceType.Poison, 70 );

			SetResistance( ResistanceType.Physical, 41, 48 );
			SetResistance( ResistanceType.Fire, 41, 50 );
			SetResistance( ResistanceType.Cold, 41, 45 );
			SetResistance( ResistanceType.Poison, 76, 90 );
			SetResistance( ResistanceType.Energy, 75, 79 );

			SetSkill( SkillName.Wrestling, 82.5, 84.7 );
			SetSkill( SkillName.Tactics, 80.5, 84.4 );
			SetSkill( SkillName.MagicResist, 104.6, 113.1 );
			SetSkill( SkillName.Poisoning, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 97.9 );
			SetSkill( SkillName.EvalInt, 80.1, 99.3 );
			SetSkill( SkillName.Meditation, 81.3, 96.3 );
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 3 );
		}
		
		public override void AreaDamageEffect( Mobile m )
		{
			m.FixedParticles( 0x374A, 10, 15, 5038, 1181, 2, EffectLayer.Head );
			m.PlaySound( 0x213 );
		}
		
		public override bool CanAreaDamage{ get{ return true; } }
		public override TimeSpan AreaDamageDelay{ get{ return TimeSpan.FromSeconds( 20 ); } }		
		public override double AreaDamageScalar{ get{ return 0.5; } }		
		public override int AreaFireDamage{ get{ return 0; } }
		public override int AreaColdDamage{ get{ return 100; } }
		public override bool Unprovokable{ get{ return true; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }

		public FetidEssence( Serial serial ) : base( serial )
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
