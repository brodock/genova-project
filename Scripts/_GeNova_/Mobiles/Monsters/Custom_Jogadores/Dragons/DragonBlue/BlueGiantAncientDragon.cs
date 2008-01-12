using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class BlueGiantAncientDragon : BaseCreature
	{
		public override WeaponAbility GetWeaponAbility()
		{
			return Utility.RandomBool() ? WeaponAbility.Dismount : WeaponAbility.ParalyzingBlow;
		}

		[Constructable]
		public BlueGiantAncientDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Grande Dragao Anciao Azul";
			Body = 46;
			BaseSoundID = 362;
			Hue = Utility.RandomList( 137, 1175 );

			SetStr( 1096, 1185 );
			SetDex( 106, 175 );
			SetInt( 686, 775 );

			SetHits( 8058, 12711 );
			SetStam( 90, 105 );

			SetDamage( 28, 33 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Energy, 90 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 75, 80 );
			SetResistance( ResistanceType.Cold, 70, 75 );
			SetResistance( ResistanceType.Poison, 70, 80 );
			SetResistance( ResistanceType.Energy, 80, 90 );

			SetSkill( SkillName.EvalInt, 110.1, 120.0 );
			SetSkill( SkillName.Magery, 150.1, 190.0 );
			SetSkill( SkillName.Meditation, 150.5, 160.0 );
			SetSkill( SkillName.MagicResist, 170.1, 172.0 );
			SetSkill( SkillName.Tactics, 115.6, 120.0 );
			SetSkill( SkillName.Wrestling, 120.6, 150.0 );
			SetSkill( SkillName.Anatomy, 110.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 3 );
			PackGold( 2500, 3000 );
		}

		public override int GetIdleSound()
		{
			return 0x2D3;
		}

		public override int GetHurtSound()
		{
			return 0x2D1;
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Hides{ get{ return 60; } }
		public override int Meat{ get{ return 19; } }
		public override int Scales{ get{ return 32; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
		public override int TreasureMapLevel{ get{ return 6; } }

		public BlueGiantAncientDragon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();
		}
	}
}