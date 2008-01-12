using System;
using System.Collections;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	public class BaseTalismanSummon : BaseCreature
	{
		public override bool Commandable{ get{ return false; } }
		public override bool InitialInnocent{ get{ return true; } }
		public virtual bool IsInvulnerable{ get{ return true; } }
	
		public BaseTalismanSummon() : base( AIType.AI_Melee, FightMode.None, 10, 1, 0.2, 0.4 )
		{
		}

		public BaseTalismanSummon( Serial serial ) : base( serial )
		{
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && ControlMaster == from )
				list.Add( new TalismanReleaseEntry( this ) );
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