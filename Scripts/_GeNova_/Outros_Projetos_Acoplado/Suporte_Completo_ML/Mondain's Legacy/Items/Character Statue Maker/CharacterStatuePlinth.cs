using System;
using Server;
using Server.Gumps;
using Server.Mobiles;

namespace Server.Items
{	
	public class CharacterStatuePlinth : Static
	{		
		public override int LabelNumber{ get{ return 1076201; } } // Character Statue
	
		private CharacterStatue m_Statue;
		
		public CharacterStatuePlinth( CharacterStatue statue ) : base( 0x32F2 )
		{
			m_Statue = statue;
			
			InvalidateHue();
		}

		public CharacterStatuePlinth( Serial serial ) : base( serial )
		{
		}    
		
		public override void OnAfterDelete()
		{
			base.OnAfterDelete();
			
			if ( m_Statue != null && !m_Statue.Deleted )
				m_Statue.Delete();
		}
		
		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Statue != null )
				from.SendGump( new CharacterPlinthGump( m_Statue ) );			
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
		
		public void InvalidateHue()
		{
			if ( m_Statue != null )
				Hue = 0xB8F + (int) m_Statue.StatueType * 4 + (int) m_Statue.Material;
		}
	}
}