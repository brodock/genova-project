using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class WhiteAncientDragon : BaseCreature
	{
		[Constructable]
		public WhiteAncientDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Dragao Anciao Branco";
			Body = 46;
			BaseSoundID = 362;
			Hue = 1153;

			SetStr( 1096, 1185 );
			SetDex( 106, 175 );
			SetInt( 686, 775 );

			SetHits( 3658, 5711 );
			SetStam( 200, 238 );

			SetDamage( 22, 24 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Cold, 80 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 60, 75 );
			SetResistance( ResistanceType.Cold, 55, 68 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 65, 80 );

			SetSkill( SkillName.EvalInt, 100.1, 110.0 );
			SetSkill( SkillName.Magery, 100.1, 120.0 );
			SetSkill( SkillName.Meditation, 100.5, 120.0 );
			SetSkill( SkillName.MagicResist, 100.1, 122.0 );
			SetSkill( SkillName.Tactics, 80.0, 91.0 );
			SetSkill( SkillName.Wrestling, 100.6, 110.0 );
			SetSkill( SkillName.Anatomy, 50.0 );

			Fame = 22500;
			Karma = -22500;

			VirtualArmor = 70;

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = false;
			ControlSlots = 3;
			MinTameSkill = 120.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 3 );
			PackGold( 2200, 2600 );

		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 15; } }
		public override ScaleType ScaleType{ get{ return ScaleType.White; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public WhiteAncientDragon( Serial serial ) : base( serial )
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