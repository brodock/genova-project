using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a baby dragon corpse" )]
	public class GreenDrake : BaseCreature
	{
		[Constructable]
		public GreenDrake () : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Filhote de Dragao Verde";
			Body = Utility.RandomList( 60, 61 );
			Hue = 61;
			BaseSoundID = 362;

			SetStr( 361, 390 );
			SetDex( 133, 152 );
			SetInt( 101, 140 );

			SetHits( 701, 758 );
			SetStam( 35, 55 );

			SetDamage( 13, 15 );

			SetDamageType( ResistanceType.Physical, 80 );
			SetDamageType( ResistanceType.Poison, 20 );

			SetResistance( ResistanceType.Physical, 25, 40 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.MagicResist, 65.1, 80.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Poisoning, 40.1, 60.0 );
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
		public override Poison HitPoison{ get{ return Poison.Deadly; } }		
		public override ScaleType ScaleType{ get{ return ScaleType.Green; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Fish; } }

		public GreenDrake( Serial serial ) : base( serial )
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