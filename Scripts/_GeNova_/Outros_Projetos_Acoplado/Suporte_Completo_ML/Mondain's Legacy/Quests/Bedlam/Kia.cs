using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Quests
{				
	public class MomentoQuest : BaseQuest
	{							
		/* Momento! */
		public override object Title{ get{ return 1074750; } }
		
		/* I was going to march right out there and get it myself, but no ... Master Gnosos won't let me.  But you 
		see, that bridle means so much to me.  A momento of happier, less-dead ... well undead horseback riding.  
		Could you fetch it for me?  I think my horse, formerly known as 'Resolve', may still be wearing it. */
		public override object Description{ get{ return 1074751; } }
		
		/* Hrmph. */
		public override object Refuse{ get{ return 1074752; } }
		
		/* The bridle would be hard to miss on him now ... since he's skeletal.  Please do what you need to do to 
		retreive it for me. */
		public override object Uncomplete{ get{ return 1074753; } }
		
		/* I'd know that jingling sound anywhere!  You have recovered my bridle.  Thank you. */
		public override object Complete{ get{ return 1074754; } }
	
		public MomentoQuest() : base()
		{			
			AddObjective( new ObtainObjective( typeof( ResolvesBridle ), "resolve's bridle", 1, 0x1727 ) );
			
			AddReward( new BaseReward( typeof( LargeTreasureBag ), 1072706 ) );
		}
		
		public override bool CanOffer()
		{
			return MondainsLegacy.Bedlam;
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
	
	public class Kia : MondainQuester
	{
		public override Type[] Quests{ get{ return new Type[] 
		{ 
			typeof( MomentoQuest )
		}; } }
		
		[Constructable]
		public Kia() : base( "Kia", "the student" )
		{			
		}
		
		public Kia( Serial serial ) : base( serial )
		{
		}
		
		public override void InitBody()
		{
			InitStats( 100, 100, 25 );
			
			Female = true;
			Race = Race.Human;
			
			Hue = 0x8418;
			HairItemID = 0x2046;
			HairHue = 0x466;
		}
		
		public override void InitOutfit()
		{
			AddItem( new Backpack() );
			AddItem( new Shoes( 0x743 ) );
			AddItem( new Robe( 0x485 ) );
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