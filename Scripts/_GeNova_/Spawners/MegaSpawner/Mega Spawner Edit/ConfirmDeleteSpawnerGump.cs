using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteSpawnerGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList MSEGCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private bool fromSpawnerList;
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private Mobile gumpMobile;

		public ConfirmDeleteSpawnerGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 330, 90 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 290, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 230, 290, 20 );

			AddHtml( 221, 181, 270, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Spawner" ), false, false );
			AddHtml( 201, 201, 290, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete the spawner?" ), false, false );

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
					MessagesTitle = "Delete Spawner";
					Messages = "You have chosen not to delete the Mega Spawner.";

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Spawner
				{
					if ( CheckProcess() )
						break;

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
					{
						MegaSpawner megaspawner = (MegaSpawner) MC.SpawnerList[i];

						if ( megaspawner == megaSpawner )
						{
							if ( fromSpawnerList )
							{
								HideSpawnerList.RemoveAt( i );
								MSGCheckBoxesList.RemoveAt( i );
							}

							MC.SpawnerList.RemoveAt( i );
						}
					}

					megaSpawner.Workspace = false;
					megaSpawner.WorkspaceEditor = null;
					megaSpawner.Editor = null;
					megaSpawner.FileEdit = null;

					megaSpawner.Delete();

					if ( fromSpawnerList )
					{
						MessagesTitle = "Delete Spawner";
						Messages = "The Mega Spawner has now been deleted.";

						SetArgsList();

						ArgsList[34] = (bool) true;		// RefreshSpawnerLists

						gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );
					}
					else
					{
						gumpMobile.SendMessage( "The Mega Spawner has now been deleted." );
					}

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[13] = MSGCheckBoxesList;
			ArgsList[19] = megaSpawner;
			ArgsList[20] = fromSpawnerList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			HideSpawnerList = (ArrayList) 					ArgsList[6];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			megaSpawner = (MegaSpawner) 					ArgsList[19];
			fromSpawnerList = (bool) 					ArgsList[20];
			PersonalConfigList = (ArrayList)				ArgsList[28];

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