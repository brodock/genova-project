using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteEntryGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList MSEGCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private ArrayList ESGArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private int index;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private Mobile gumpMobile;

		public ConfirmDeleteEntryGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 330, 90 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 290, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 230, 290, 20 );

			AddHtml( 221, 181, 270, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Entry" ), false, false );
			AddHtml( 201, 201, 290, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete the entry?" ), false, false );

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
					MessagesTitle = "Delete Entry";
					Messages = "You have chosen not to delete the entry.";

					SetArgsList();

					gumpMobile.SendGump( new EditSpawnGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Entry
				{
					if ( CheckProcess() )
						break;

					MSEGCheckBoxesList.RemoveAt( index );
					megaSpawner.DeleteEntry( index );

					MessagesTitle = "Delete Entry";
					Messages = "Entry has been removed.";

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ESGArgsList[1] = index;

			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[14] = MSEGCheckBoxesList;
			ArgsList[19] = megaSpawner;
			ArgsList[21] = ESGArgsList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			MSEGCheckBoxesList = (ArrayList)				ArgsList[14];
			megaSpawner = (MegaSpawner) 					ArgsList[19];
			ESGArgsList = (ArrayList)					ArgsList[21];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			index = (int) 							ESGArgsList[1];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
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