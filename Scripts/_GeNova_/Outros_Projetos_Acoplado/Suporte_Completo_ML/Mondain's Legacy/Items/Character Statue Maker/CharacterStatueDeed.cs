using System;
using Server;
using Server.Mobiles;
using Server.Targets;
using Server.Accounting;
using Server.Engines.VeteranRewards;

namespace Server.Items
{	
	public class CharacterStatueDeed : Item
	{
		public override int LabelNumber
		{ 
			get
			{ 
				if ( m_Statue != null )
				{
					switch ( m_Statue.StatueType )
					{
						case StatueType.Marble: return 1076189;
						case StatueType.Jade: return 1076188;
						case StatueType.Bronze: return 1076190;
					}
				}
					
				return 1076173; 
			} 
		}
		
		private CharacterStatue m_Statue;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public CharacterStatue Statue
		{
			get { return m_Statue; }	
			set { m_Statue = value; }	
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public StatueType StatueType
		{
			get
			{ 
				if ( m_Statue != null )
					return m_Statue.StatueType; 
				
				return StatueType.Marble;
			}
		}
		
		public CharacterStatueDeed( CharacterStatue statue ) : base( 0x14F0 )
		{
			m_Statue = statue;
		
			LootType = LootType.Blessed;
			Weight = 1.0;
		}

		public CharacterStatueDeed( Serial serial ) : base( serial )
		{
		}    
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			list.Add( 1076222 ); // 6th Year Veteran Reward
			
			if ( m_Statue != null )
				list.Add( 1076231, m_Statue.Name ); // Statue of ~1_Name~
		}   	
		
		public override void OnDoubleClick( Mobile from )
		{
			Account acct = from.Account as Account;
			
			if ( acct != null && from.AccessLevel == AccessLevel.Player )
			{	
				TimeSpan time = TimeSpan.FromDays( RewardSystem.RewardInterval.TotalDays * 6 ) - ( DateTime.Now - acct.Created );
				
				if ( time > TimeSpan.Zero )
				{
					from.SendLocalizedMessage( 1008126, true, Math.Ceiling( time.TotalDays / RewardSystem.RewardInterval.TotalDays ).ToString() ); // Your account is not old enough to use this item. Months until you can use this item : 
					return;
				}
			}
				
			if ( IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 1076194 ); // Select a place where you would like to put your statue.
				from.Target = new CharacterStatueTarget( this, StatueType );
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}		
		
		public override void OnDelete()
		{
			base.OnDelete();
			
			if ( m_Statue != null )
				m_Statue.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			
			writer.Write( (Mobile) m_Statue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_Statue = reader.ReadMobile() as CharacterStatue;
		}
	}
}