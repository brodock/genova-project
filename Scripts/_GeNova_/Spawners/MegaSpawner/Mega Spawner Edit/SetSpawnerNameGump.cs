using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class SetSpawnerNameGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;

		private MegaSpawner megaSpawner;
		private string defaultTextColor, titleTextColor;
		private int ActiveTextEntryTextColor;

		private Mobile gumpMobile;

		public SetSpawnerNameGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 330, 90 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 290, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 230, 290, 20 );

			AddHtml( 221, 181, 270, 20, MC.ColorText( titleTextColor, "Set Spawner Name" ), false, false );
			MC.DisplayBackground( this, ActiveTEBGTypeConfig, 200, 200, 290, 20 );
			AddTextEntry( 200, 200, 280, 20, ActiveTextEntryTextColor, 0, megaSpawner.Name );

			AddHtml( 441, 231, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 400, 230, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 231, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 230, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					MessagesTitle = "Set Spawner Name";
					Messages = "You have chosen not to name the Mega Spawner.";

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Set Spawner Name
				{
					if ( CheckProcess() )
						break;

					string name = Convert.ToString( info.GetTextEntry( 0 ).Text );

					if ( name == "" )
						name = "A Mega Spawner";

					megaSpawner.Name = name;

					MessagesTitle = "Set Spawner Name";
					Messages = String.Format( "The Mega Spawner's name has been set to \"{0}\".", name );

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[19] = megaSpawner;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			megaSpawner = (MegaSpawner) 					ArgsList[19];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)				PersonalConfigList[2];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			ActiveTextEntryTextColor = (int)				PersonalConfigList[9];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
		}

		private bool CheckProcess()
		{
			string msgsTitle, msgs;

			if ( MC.CheckProcess( out msgsTitle, out msgs ) )
			{
				if ( msgsTitle != null )
					MessagesTitle = msgsTitle;

				if ( msgs != null )
					Messages = msgs;

				SetArgsList();

				gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );
			}

			return MC.CheckProcess();
		}
	}
}