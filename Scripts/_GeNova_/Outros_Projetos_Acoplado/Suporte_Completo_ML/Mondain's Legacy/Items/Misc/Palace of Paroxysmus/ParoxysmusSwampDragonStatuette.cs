using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class ParoxysmusSwampDragonStatuette : BaseImprisonedMobile
	{
		public override int LabelNumber{ get{ return 1072084; } } // Paroxysmus' Swamp Dragon		
		public override BaseCreature Summon{ get { return new SwampDragon(); } }
	
		[Constructable]
		public ParoxysmusSwampDragonStatuette() : base( 0x2619 )
		{
			Weight = 1.0;
		}

		public ParoxysmusSwampDragonStatuette( Serial serial ) : base( serial )
		{
		}
		
		public override void Release( Mobile from, BaseCreature summon )
		{			
			SwampDragon dragon = (SwampDragon) summon;
			
			if ( dragon != null )
			{
				dragon.BardingExceptional = true;
				dragon.BardingResource = CraftResource.Iron;
				dragon.HasBarding = true;
				dragon.Hue = 0x851;			
				dragon.BardingHP = dragon.BardingMaxHP;	
			}
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

