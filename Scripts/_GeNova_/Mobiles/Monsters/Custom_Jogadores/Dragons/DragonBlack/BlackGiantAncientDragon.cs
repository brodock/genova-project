using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class BlackGiantAncientDragon : BaseCreature
		{

		public override bool IsScaredOfScaryThings{ get{ return false; } }
		public override bool IsScaryToPets{ get{ return true; } }

		[Constructable]
		public BlackGiantAncientDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Grande Dragao Anciao Negro";
			Body = 46;
			BaseSoundID = 362;
			Hue = Utility.RandomList( 1175, 90000 );

			SetStr( 496, 585 );
			SetDex( 106, 175 );
			SetInt( 686, 775 );

			SetHits( 4058, 6711 );
			SetStam( 90, 105 );

			SetDamage( 28, 33 );

			SetDamageType( ResistanceType.Physical, 90 );
			SetDamageType( ResistanceType.Poison, 10 );

			SetResistance( ResistanceType.Physical, 65, 75 );
			SetResistance( ResistanceType.Fire, 65, 70 );
			SetResistance( ResistanceType.Cold, 60, 65 );
			SetResistance( ResistanceType.Poison, 80, 90 );
			SetResistance( ResistanceType.Energy, 60, 70 );

			SetSkill( SkillName.EvalInt, 110.1, 120.0 );
			SetSkill( SkillName.Magery, 150.1, 190.0 );
			SetSkill( SkillName.Meditation, 150.5, 160.0 );
			SetSkill( SkillName.MagicResist, 170.1, 172.0 );
			SetSkill( SkillName.Tactics, 115.6, 120.0 );
			SetSkill( SkillName.Wrestling, 120.6, 150.0 );
			SetSkill( SkillName.Anatomy, 110.0 );

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
		public override int Scales{ get{ return 32; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Black; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }				

		public BlackGiantAncientDragon( Serial serial ) : base( serial )
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