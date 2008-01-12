using System;
using System.Xml;
using Server;
using Server.Mobiles;
using Server.Regions;

namespace Server.Regions
{
	public class MondainTownRegion : MondainRegion
	{
		public MondainTownRegion( XmlElement xml, Map map, Region parent ) : base( xml, map, parent )
		{			
		}
		
		public override bool OnBeginSpellCast( Mobile m, ISpell s )
		{
			if ( !s.OnCastInTown( this ) )
			{
				m.SendLocalizedMessage( 500946 ); // You cannot cast this in town!
				
				return false;
			}
			
			return base.OnBeginSpellCast( m, s );
		}
	}
}