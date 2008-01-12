using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a dragon corpse" )]
	public class GreenDragon : BaseCreature
	{
		[Constructable]
		public GreenDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Dragao Verde";
			Body = 106;
			Hue = Utility.RandomList( 57, 167 );
			BaseSoundID = 362;

			SetStr( 1500, 1600 );
			SetDex( 450, 460 );
			SetInt( 436, 475 );

			SetHits( 825, 980 );
			SetStam( 75, 85 );

			SetDamage( 15, 18 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Poison, 60 );

			SetResistance( ResistanceType.Physical, 48, 55 );
			SetResistance( ResistanceType.Fire, 45, 55 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 55, 65 );
			SetResistance( ResistanceType.Energy, 35, 45 );

			SetSkill( SkillName.EvalInt, 100.1, 105.0 );

			SetSkill( SkillName.Magery, 75.1, 80.0 );
			SetSkill( SkillName.Poisoning, 60.1, 80.0 );
			SetSkill( SkillName.MagicResist, 79.1, 81.0 );
			SetSkill( SkillName.Tactics, 55.6, 70.0 );
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
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }		
		public override ScaleType ScaleType{ get{ return ScaleType.Green; } }

		public GreenDragon( Serial serial ) : base( serial )
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