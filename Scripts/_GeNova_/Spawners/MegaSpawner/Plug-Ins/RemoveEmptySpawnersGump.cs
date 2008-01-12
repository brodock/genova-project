using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class RemoveEmptySpawnersGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private string FileName;
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private Mobile gumpMobile;

		public static void Initialize()
		{
			string helpDesc = "That plug-in will remove all spawners that do not have any entries.";
			MC.RegisterPlugIn( new MC.OnCommand( RemoveEmptySpawners ), "RemoveEmptySpawners", "Remove All Empty Spawners", helpDesc );
		}

		public static void RemoveEmptySpawners( Mobile mobile, ArrayList argsList )
		{
			mobile.SendGump( new RemoveEmptySpawnersGump( mobile, argsList ) );
		}

		public RemoveEmptySpawnersGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 380, 110 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 340, 40 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 250, 340, 20 );

			AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Empty Spawners" ), false, false );

			AddHtml( 201, 201, 340, 40, MC.ColorText( defaultTextColor, "Are you sure you want to delete all empty spawners on the entire server?" ), false, false );

			AddHtml( 491, 251, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 450, 250, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 251, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 250, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					MessagesTitle = "Delete Empty Spawners";
					Messages = "You have chosen not to delete all empty spawners on the entire server.";

					SetArgsList();

					gumpMobile.SendGump( new PlugInsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Empty Spawners
				{
					int count = 0;

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
					{
						MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

						if ( megaSpawner.EntryList.Count == 0 )
						{
							count++;

							megaSpawner.Delete();
						}
					}

					MessagesTitle = "Delete Empty Spawners";

					if ( count > 0 )
						Messages = String.Format( "{0} empty Mega Spawner{1} been deleted.", count, count == 1 ? " has" : "s have" );
					else
						Messages = "There are no empty Mega Spawners on the entire server.";

					SetArgsList();

					ArgsList[34] = (bool) true;		// RefreshSpawnerLists

					gumpMobile.SendGump( new PlugInsGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[13] = MSGCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)						ArgsList[2];
			Messages = (string)								ArgsList[4];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)			PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
		}
	}
}