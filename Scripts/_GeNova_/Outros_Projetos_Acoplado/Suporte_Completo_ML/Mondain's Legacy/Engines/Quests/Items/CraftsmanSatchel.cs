using System;
using Server;
using Server.Engines.Quests;
using Reward = Server.Engines.Quests.BaseReward;

namespace Server.Items
{
	public class BaseCraftsmanSatchel : Backpack
	{
		public BaseCraftsmanSatchel() : base()
		{
			Hue = Reward.SatchelHue();
			
			int count = 1;
			
			if ( 0.015 > Utility.RandomDouble() )
				count = 2;
			
			bool equipment = false;
			bool jewlery = false;
			bool talisman = false;
			
			while ( Items.Count < count )
			{				
				if ( 0.25 > Utility.RandomDouble() && !talisman )
				{
					AddItem( new RandomTalisman() );
					talisman = true;					
				}
				else if ( 0.4 > Utility.RandomDouble() && !equipment )
				{
					AddItem( RandomItem() );		
					equipment = true;		
				}
				else if ( 0.88 > Utility.RandomDouble() && !jewlery )
				{
					AddItem( Reward.Jewlery() );
					jewlery = true;
				}
			}
		}
		
		public BaseCraftsmanSatchel( Serial serial ) : base( serial )
		{
		}
		
		public virtual Item RandomItem()
		{
			return null;
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


	public class FletcherCraftsmanSatchel : BaseCraftsmanSatchel
	{
		[Constructable]
		public FletcherCraftsmanSatchel() : base()
		{			
			if ( Items.Count < 2 && 0.5 > Utility.RandomDouble() )
				AddItem( Reward.FletcherRecipe() );
				
			if ( 0.01 > Utility.RandomDouble() )
				AddItem( Reward.FletcherRunic() );
		}
		
		public FletcherCraftsmanSatchel( Serial serial ) : base( serial )
		{
		}
		
		public override Item RandomItem()
		{
			return Reward.RangedWeapon();
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
	
	public class TailorsCraftsmanSatchel : BaseCraftsmanSatchel
	{
		[Constructable]
		public TailorsCraftsmanSatchel() : base()
		{			
			if ( Items.Count < 2 && 0.5 > Utility.RandomDouble() )
				AddItem( Reward.TailorRecipe() );
		}
		
		public TailorsCraftsmanSatchel( Serial serial ) : base( serial )
		{
		}
		
		public override Item RandomItem()
		{
			return Reward.Armor();
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
	
	public class SmithsCraftsmanSatchel : BaseCraftsmanSatchel
	{
		[Constructable]
		public SmithsCraftsmanSatchel() : base()
		{			
			if ( Items.Count < 2 && 0.5 > Utility.RandomDouble() )
				AddItem( Reward.SmithRecipe() );
		}
		
		public SmithsCraftsmanSatchel( Serial serial ) : base( serial )
		{
		}
		
		public override Item RandomItem()
		{
			return Reward.Weapon();
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
	
	public class TinkersCraftsmanSatchel : BaseCraftsmanSatchel
	{
		[Constructable]
		public TinkersCraftsmanSatchel() : base()
		{			
			if ( Items.Count < 2 && 0.5 > Utility.RandomDouble() )
				AddItem( Reward.TinkerRecipe() );
		}
		
		public TinkersCraftsmanSatchel( Serial serial ) : base( serial )
		{
		}
		
		public override Item RandomItem()
		{
			return Reward.Weapon();
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
	
	public class CarpentersCraftsmanSatchel : BaseCraftsmanSatchel
	{
		[Constructable]
		public CarpentersCraftsmanSatchel() : base()
		{			
			if ( Items.Count < 2 && 0.5 > Utility.RandomDouble() )
				AddItem( Reward.CarpRecipe() );				
			
			if ( 0.01 > Utility.RandomDouble() )
				AddItem( Reward.CarpRunic() );
		}
		
		public CarpentersCraftsmanSatchel( Serial serial ) : base( serial )
		{
		}
		
		public override Item RandomItem()
		{
			return Reward.Weapon();
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