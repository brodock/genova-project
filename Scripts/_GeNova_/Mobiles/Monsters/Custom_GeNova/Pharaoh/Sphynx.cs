using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "the sphinx corpse" )]
	public class Sphinx : BaseCreature
	{
		[Constructable]
		public Sphinx() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "the Sphinx";
			
		        Body = 788;
			BaseSoundID = 1149;

			SetStr( 800 );
			SetDex( 510, 750 );
			SetInt( 310, 400 );

			SetHits( 32000);

			SetDamage( 24,26 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 100 );
			SetResistance( ResistanceType.Fire, 45 );
			SetResistance( ResistanceType.Cold, 45 );
			SetResistance( ResistanceType.Poison, 45 );
			SetResistance( ResistanceType.Energy, 45 );

			SetSkill( SkillName.EvalInt, 170.0 );
			SetSkill( SkillName.Magery, 170.0 );
			SetSkill( SkillName.Meditation, 170.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Tactics, 170.0 );
			SetSkill( SkillName.Wrestling, 170.0 );


			Fame = 0;
			Karma = -20000;

			VirtualArmor = 64;



		 
			PackGem();
			PackGold( 4700, 6950 );
			PackMagicItems( 2, 3 );
			AddLoot( LootPack.UltraRich, 3 );

       } 



		public override bool Unprovokable{ get{ return true; } }
		public override bool AlwaysMurderer{ get{ return true; } }
		public override bool Uncalmable{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 1; } 
               }

		public Sphinx( Serial serial ) : base( serial )
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