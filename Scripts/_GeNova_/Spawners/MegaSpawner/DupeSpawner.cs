using System;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Commands;


namespace Server.MegaSpawnerSystem
{
	public class DupeMS
	{
		public static void Initialize()
		{
			Register( "DupeMS", AccessLevel.GameMaster, new CommandEventHandler( DupeMS_OnCommand ) );
		}

		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
			CommandSystem.Register( command, access, handler );
		}

		[Usage( "DupeMS" )]
		[Description( "Dupes a MegaSpawner" )]
		public static void DupeMS_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile; 
			int count = 1;

			try
			{
				count = e.GetInt32( 0 );
			}
			catch{}

			if( count <= 0 )
				count = 1;

			from.Target = new InternalTarget( count );
		}

		private class InternalTarget : Target
		{
			private int count;

			public InternalTarget( int m_Count) : base( -1, true, TargetFlags.None )
			{
				count = m_Count;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				MegaSpawner ms = null;

				if( targeted is MegaSpawner )
					ms = (MegaSpawner) targeted;

				if( ms != null )
					ms.Dupe( from, count );
			}
		}
	}
}
