using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteSpawnersGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList MasterSpawnerList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private ArrayList SpawnerList = new ArrayList();
		private Mobile gumpMobile;

		public ConfirmDeleteSpawnersGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
		{
			gumpMobile = mobile;

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
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting All Spawners" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Selected Spawners" ), false, false ); break; }
				case 3: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Imported Spawners" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete all spawners?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete selected spawners?" ), false, false ); break; }
				case 3: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete imported spawners?" ), false, false ); break; }
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
						case 1:	{ MessagesTitle = "Delete All Spawners"; Messages = "You have chosen not to delete all Mega Spawners."; break; }
						case 2:	{ MessagesTitle = "Delete Selected Spawners"; Messages = "You have chosen not to delete all selected Mega Spawners."; break; }
						case 3:	{ MessagesTitle = "Delete Imported Spawners"; Messages = "You have chosen not to delete all imported spawners."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Spawners
				{
					if ( CheckProcess() )
						break;

					DeleteSpawners();

					SetArgsList();

					ArgsList[34] = (bool) true;		// RefreshSpawnerLists

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

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
			ArgsList[36] = MasterSpawnerList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)						ArgsList[2];
			Messages = (string)								ArgsList[4];
			HideSpawnerList = (ArrayList) 					ArgsList[6];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			MasterSpawnerList = (ArrayList)					ArgsList[36];

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

				gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );
			}

			return MC.CheckProcess();
		}

		private void DeleteSpawners()
		{
			SpawnerList.Clear();

			int exceptions = 0;

			for ( int i = 0; i < MasterSpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) MasterSpawnerList[i];

				if ( megaSpawner.Deleted )
				{
					MasterSpawnerList.RemoveAt( i );
					i--;
				}
				else
				{
					if ( ( megaSpawner.Workspace && megaSpawner.WorkspaceEditor != gumpMobile ) || ( megaSpawner.Editor != null && megaSpawner.Editor != gumpMobile ) || ( megaSpawner.FileEdit != null && megaSpawner.FileEdit != gumpMobile ) )
					{
						switch ( Select )
						{
							case 1:{ exceptions++; break; }
							case 2:{ if ( (bool) MSGCheckBoxesList[i] ) exceptions++; break; }
							case 3:{ if ( megaSpawner.Imported != "" ) exceptions++; break; }
						}
					}
					else
					{
						switch ( Select )
						{
							case 1:{ SpawnerList.Add( megaSpawner ); break; }
							case 2:
							{
								if ( (bool) MSGCheckBoxesList[i] )
									SpawnerList.Add( megaSpawner );

								break;
							}
							case 3:
							{
								if ( megaSpawner.Imported != "" )
									SpawnerList.Add( megaSpawner );

								break;
							}
						}

						MSGCheckBoxesList[i] = (bool) false;
					}
				}
			}

			int count = SpawnerList.Count;

			foreach ( MegaSpawner megaSpawner in SpawnerList )
			{
				if ( megaSpawner.Workspace && megaSpawner.WorkspaceEditor == gumpMobile )
				{
					megaSpawner.Workspace = false;
					megaSpawner.WorkspaceEditor = null;
				}

				if ( megaSpawner.FileEdit != null && megaSpawner.FileEdit == gumpMobile )
					megaSpawner.FileEdit = null;

				if ( megaSpawner.Editor != null && megaSpawner.Editor == gumpMobile )
					megaSpawner.Editor = null;

				megaSpawner.Delete();
			}

			for ( int i = 0; i < SpawnerList.Count; i++ )
				MSGCheckBoxesList.RemoveAt( 0 );

			SpawnerList.Clear();

			switch ( Select )
			{
				case 1:{ MessagesTitle = "Delete All Spawners"; break; }
				case 2:{ MessagesTitle = "Delete Selected Spawners"; break; }
				case 3:{ MessagesTitle = "Delete Imported Spawners"; break; }
			}

			Messages = String.Format( "{0} Mega Spawner{1} been deleted.", count, count == 1 ? " has" : "s have" );

			if ( exceptions > 0 )
				Messages = String.Format( "{0} There {1} {2} Mega Spawner{3} that could not be deleted, because {4} being editted by someone else.", Messages, exceptions == 1 ? "was" : "were", exceptions, exceptions == 1 ? "" : "s", exceptions == 1 ? "it is" : "they are" );

		}
	}
}