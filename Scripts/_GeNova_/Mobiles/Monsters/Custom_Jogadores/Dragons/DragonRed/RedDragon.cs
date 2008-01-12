using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class RedDragon : BaseCreature
	{
		[Constructable]
		public RedDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Dragao Vermelho" ;
			Body = 106;
			Hue = Utility.RandomList( 33, 38 );
			BaseSoundID = 362;

			SetStr( 1500, 1600 );
			SetDex( 450, 460 );
			SetInt( 436, 475 );

			SetHits( 1025, 1289 );
			SetStam( 55, 65 );

			SetDamage( 16, 19 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Fire, 60 );

			SetResistance( ResistanceType.Physical, 58, 68 );
			SetResistance( ResistanceType.Fire, 68, 79 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 55, 65 );
			SetResistance( ResistanceType.Energy, 55, 65 );

			SetSkill( SkillName.EvalInt, 100.1, 105.0 );
			SetSkill( SkillName.Magery, 75.1, 80.0 );
			SetSkill( SkillName.MagicResist, 79.1, 81.0 );
			SetSkill( SkillName.Tactics, 65.6, 80.0 );
			SetSkill( SkillName.Wrestling, 80.1, 92.5 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = false;
			ControlSlots = 3;
			MinTameSkill = 120.9;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			PackGold( 1200, 1600 );

		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 19; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 7; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Red; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public RedDragon( Serial serial ) : base( serial )
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