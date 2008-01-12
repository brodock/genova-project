using System;
using Server;
using Server.Items;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "a black order mage corpse" )] 
	public class DragonsFlameMage : BaseCreature
	{	
		[Constructable]
		public DragonsFlameMage() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "black order mage";
			Title = "of the dragon's flame sect";
			Female = Utility.RandomBool();
			Race = Race.Human;
			Hue = Race.RandomSkinHue();
			HairItemID = Race.RandomHair( Female );
			HairHue = Race.RandomHairHue();
			Race.RandomFacialHair( this );
			
			AddItem( new NinjaTabi() );
			AddItem( new FancyShirt( 0x51D ) );
			AddItem( new Hakama( 0x51D ) );
			AddItem( new Kasa( 0x51D ) );
		}

		public DragonsFlameMage( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosFilthyRich, 4 );
		}
		
		public override void AlterSpellDamageFrom( Mobile from, ref int damage )
		{
			if ( from != null )
				from.Damage( damage / 2, from );
		}
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );	
			
			if ( Utility.RandomDouble() < 0.3 )
				c.DropItem( new DragonFlameSectBadge() );
						
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new ParrotItem() );
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
