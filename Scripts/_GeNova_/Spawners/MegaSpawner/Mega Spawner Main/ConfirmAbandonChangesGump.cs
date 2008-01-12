using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmAbandonChangesGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private string FileName;
		private bool Changed;
		private ArrayList ChangedSpawnerList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private ArrayList SpawnerList = new ArrayList();
		private Mobile gumpMobile;

		public ConfirmAbandonChangesGump( Mobile mobile, ArrayList argsList, ArrayList spawnerList, int select ) : base( 0,0 )
		{
			gumpMobile = mobile;
			SpawnerList = spawnerList;

			Select = select;
			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 380, 90 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 340, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 230, 340, 20 );

			switch ( Select )
			{
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Abandon Spawner Workspace" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, String.Format( "Confirmation Of Abandon Changes To File: {0}", FileName ) ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to abandon Spawner Workspace?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to abandon changes to file?" ), false, false ); break; }
			}

			AddHtml( 491, 231, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 450, 230, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 231, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 230, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					switch ( Select )
					{
						case 1:	{ MessagesTitle = "Abandon Spawner Workspace"; Messages = "You have chosen not to abandon your Spawner Workspace."; break; }
						case 2:	{ MessagesTitle = "Abandon Changes To File"; Messages = String.Format( "You have chosen not to abandon changes to file: {0}.", FileName ); break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Abandon Workspace/Changes
				{
					switch ( Select )
					{
						case 1: // Abandon Spawner Workspace
						{
							foreach ( MegaSpawner ms in SpawnerList )
								ms.Delete();

							MC.RemoveWorkspace( gumpMobile );

							MessagesTitle = "Close Spawner Workspace";
							Messages = "You have abandoned your Spawner Workspace. All added Mega Spawners were deleted and nothing was saved.";

							SetArgsList();

							break;
						}
						case 2: // Abandon Changes To File
						{
							foreach ( MegaSpawner ms in SpawnerList )
								ms.FileEdit = null;

							foreach ( MegaSpawner ms in ChangedSpawnerList )
								ms.Delete();

							MC.RemoveFileEdit( gumpMobile );

							Changed = false;
							ChangedSpawnerList.Clear();

							MessagesTitle = String.Format( "Close File {0}", FileName );
							Messages = String.Format( "You have abandoned changes you have made to file {0}. All Mega Spawners were deleted and nothing was saved.", FileName );

							SetArgsList();

							break;
						}
					}

					SetArgsList();

					gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[7] = FileName;
			ArgsList[10] = Changed;
			ArgsList[11] = ChangedSpawnerList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			FileName = (string)						ArgsList[7];
			Changed = (bool)						ArgsList[10];
			ChangedSpawnerList = (ArrayList)				ArgsList[11];
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
	}
}