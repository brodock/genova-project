using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a baby dragon corpse" )]
	public class BlackDrake : BaseCreature
	
	{

		public override bool IsScaredOfScaryThings{ get{ return false; } }
		public override bool IsScaryToPets{ get{ return true; } }


	
		[Constructable]
		public BlackDrake () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Filhote de Dragao Negro";
			Body = Utility.RandomList( 60, 61 );
			Hue = 961;
			BaseSoundID = 362;

			SetStr( 361, 390 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );

			SetHits( 701, 758 );
			SetStam( 35, 55 );

			SetDamage( 13, 15 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 80.0 );

			Fame = 5500;
			Karma = -5500;

			VirtualArmor = 46;

			Tamable = false;
			ControlSlots = 2;
			MinTameSkill = 84.3;


		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
			PackGold( 400, 500 );
		}

		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override int TreasureMapLevel{ get{ return 2; } }
		public override int Meat{ get{ return 10; } }
		public override int Hides{ get{ return 20; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 2; } }		
		public override ScaleType ScaleType{ get{ return ScaleType.Black; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public BlackDrake( Serial serial ) : base( serial )
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