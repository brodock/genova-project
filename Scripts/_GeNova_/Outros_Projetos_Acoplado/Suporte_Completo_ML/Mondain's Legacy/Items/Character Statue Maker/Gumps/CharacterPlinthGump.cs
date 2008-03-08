using System;
using System.Globalization;
using Server;
using Server.Mobiles;

namespace Server.Gumps
{
	public class CharacterPlinthGump : Gump
	{	
		public CharacterPlinthGump( CharacterStatue statue ) : base( 60, 30 )
		{						
			Closable = true;
			Disposable = true;
			Dragable = true;
			Resizable = false;
		
			AddPage( 0 );			
			AddImage( 0, 0, 0x24F4 );
			AddHtml( 55, 50, 150, 20, statue.Name, false, false );
			AddHtml( 55, 75, 150, 20, statue.SculptedOn.ToString( "G", new CultureInfo("de-DE") ), false, false );
			AddHtmlLocalized( 55, 100, 150, 20, GetTypeNumber( statue.StatueType ), 0, false, false );
		}
		
		public int GetTypeNumber( StatueType type )
		{
			switch ( type )
			{
				case StatueType.Marble: return 1076181;
				case StatueType.Jade: return 1076180;
				case StatueType.Bronze: return 1076230;
				default: return 1076181;
			}
		}
	}
}