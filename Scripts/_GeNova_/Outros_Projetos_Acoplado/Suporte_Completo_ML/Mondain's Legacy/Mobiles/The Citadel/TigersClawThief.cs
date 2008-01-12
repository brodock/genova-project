using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a black order mage corpse" )] 
	public class TigersClawThief : BaseCreature
	{	
		[Constructable]
		public TigersClawThief() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "black order thief";
			Title = "of the tiger's claw sect";
			Female = Utility.RandomBool();
			Race = Race.Human;
			Hue = Race.RandomSkinHue();
			HairItemID = Race.RandomHair( Female );
			HairHue = Race.RandomHairHue();
			Race.RandomFacialHair( this );
			
			AddItem( new Wakizashi() );
			AddItem( new FancyShirt( 0x51D ) );
			AddItem( new StuddedMempo() );
			AddItem( new JinBaori( 0x69 ) );
			
			Item item;
			
			item = new StuddedGloves();
			item.Hue = 0x69;
			AddItem( item );
			
			item = new LeatherNinjaPants();
			item.Hue = 0x51D;
			AddItem( item );			
			
			item = new LightPlateJingasa();
			item.Hue = 0x51D;
			AddItem( item );
				
			// TODO quest items
		}

		public TigersClawThief( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 4 );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			if ( Utility.RandomDouble() < 0.3 )
				c.DropItem( new TigerClawSectBadge() );
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
