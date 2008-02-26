using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a black order mage corpse" )] 
	public class SerpentsFangAssassin : BaseCreature
	{	
		[Constructable]
		public SerpentsFangAssassin() : base( AIType.AI_Melee, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "black order assassin";
			Title = "of the serpent's fang sect";
			Female = Utility.RandomBool();
			Race = Race.Human;
			Hue = Race.RandomSkinHue();
			HairItemID = Race.RandomHair( Female );
			HairHue = Race.RandomHairHue();
			Race.RandomFacialHair( this );
			
			AddItem( new ThighBoots( 0x51D ) );
			AddItem( new FancyShirt( 0x51D ) );
			AddItem( new StuddedMempo() );
			AddItem( new JinBaori( 0x2A ) );
			
			Item item;
			
			item = new StuddedGloves();
			item.Hue = 0x2A;
			AddItem( item );
			
			item = new LeatherNinjaPants();
			item.Hue = 0x51D;
			AddItem( item );			
			
			item = new LightPlateJingasa();
			item.Hue = 0x51D;
			AddItem( item );		
			
			item = new Sai();
			item.Hue = 0x51D;
			AddItem( item );
		}

		public SerpentsFangAssassin( Serial serial ) : base( serial )
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
				c.DropItem( new SerpentFangSectBadge() );
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
