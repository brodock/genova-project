using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class RedAdultDragon : BaseCreature
	{
		[Constructable]
		public RedAdultDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Dragao Vermelho Adulto";
			Body = Utility.RandomList( 12, 59 );
			Hue = Utility.RandomList( 33, 38 );
			BaseSoundID = 362;

			SetStr( 900, 100 );
			SetDex( 150, 180 );
			SetInt( 436, 475 );

			SetHits( 1380, 2018 );
			SetStam( 150, 190 );


			SetDamage( 20, 23 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Fire, 50 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 68, 78 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 56, 65 );
			SetResistance( ResistanceType.Energy, 56, 65 );

			SetSkill( SkillName.EvalInt, 100.1, 105.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 99.1, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 98.5 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = false;
			ControlSlots = 3;
			MinTameSkill = 120.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			PackGold( 700, 800 );

		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 12; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Red; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public RedAdultDragon( Serial serial ) : base( serial )
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