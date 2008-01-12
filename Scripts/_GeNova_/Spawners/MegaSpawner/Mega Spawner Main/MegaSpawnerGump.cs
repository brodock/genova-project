using System;
using System.IO;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;
using Server.Commands;


namespace Server.MegaSpawnerSystem
{
	public class MegaSpawnerCommand
	{
		public static void Initialize()
		{
			Register( "MegaSpawner", new CommandEventHandler( MegaSpawner_OnCommand ) );
		}

		public static void Register( string command, CommandEventHandler handler )
		{
			CommandSystem.Register( command, MC.AccessLevelReq, handler );
		}

		[Usage( "MegaSpawner" )]
		[Description( "Opens the Mega Spawner System." )]
		public static void MegaSpawner_OnCommand( CommandEventArgs e )
		{
			Mobile mobile = e.Mobile;

			if ( MC.CheckMSUser( mobile ) )
			{
				mobile.SendMessage( "You are currently using the Mega Spawner System. You may not log in more than once." );

				return;
			}
			else if ( MC.GetAccessLevel( mobile ) == Access.None )
			{
				mobile.SendMessage( "You do not have authorization to use that command." );

				return;
			}

			MC.AddMSUser( mobile );

			ArrayList ArgsList = MC.CompileDefaultArgsList();
			ArgsList = MC.LoadPersonalConfig( mobile, ArgsList );

			mobile.SendGump( new MegaSpawnerGump( mobile, ArgsList ) );
		}
	}

	public class MegaSpawnerGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private string FileName;
		private bool Changed;
		private ArrayList ChangedSpawnerList = new ArrayList();
		private ArrayList PageInfoList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PCGArgsList = new ArrayList();
		private ArrayList PCGSetList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private string sortSearchFor;
		private SortOrder sortOrder;
		private SortSearchType sortSearchType, sortType;
		private bool RefreshSpawnerLists, sortSearchCaseSensitive, sortSearchFlagged, sortSearchBadLocation, sortSearchDupeSpawners, GotoLocation;
		private ArrayList SpawnerList = new ArrayList();
		private ArrayList MasterSpawnerList = new ArrayList();
		private ArrayList ModHideSpawnerList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor;

		private int offsetMin, offsetMax;

		private const int numOffset = 1000;
		private const int amtPerPg = 14;
		private const int listSpace = 308;
		private int totalPages;
		private Mobile gumpMobile;

		public MegaSpawnerGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 80, 80, 600, 452 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 450, 249, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 350, 450, 279, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 630, 450, 30, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 470, 249, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 350, 470, 279, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 630, 470, 30, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 490, 249, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 350, 490, 279, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 630, 490, 30, 20 );

			MC.DisplayImage( this, StyleTypeConfig );

			AddButton( 630, 490, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			if ( FileName == "" )
			{
				AddHtml( 121, 81, 560, 20, MC.ColorText( titleTextColor, "Mega Spawners On The Spawner List" ), false, false );
			}
			else
			{
				MC.DisplayStyle( this, StyleTypeConfig, 110, 60, 540, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 111, 61, 538, 18 );

				AddHtml( 121, 61, 540, 20, MC.ColorText( titleTextColor, "Mega Spawners From File:" ), false, false );
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "\"{0}\"", FileName ) ), false, false );
			}

			if ( !Help )
				AddButton( 597, 429, 2033, 2032, -5, GumpButtonType.Reply, 0 );
			else
				AddButton( 597, 429, 2032, 2033, -5, GumpButtonType.Reply, 0 );

			if ( DisplayMessages )
			{
				MC.DisplayStyle( this, StyleTypeConfig, 80, 540, 600, 122 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 560, 560, 80 );

				AddHtml( 121, 541, 540, 20, MC.ColorText( titleTextColor, MessagesTitle ), false, false );
				AddHtml( 100, 560, 560, 80, MC.ColorText( messagesTextColor, Messages ), false, true );
				AddHtml( 531, 641, 100, 20, MC.ColorText( defaultTextColor, "Clear Messages" ), false, false );

				AddButton( 630, 640, 4017, 4019, -6, GumpButtonType.Reply, 0 );
				AddButton( 371, 645, 2436, 2435, -8, GumpButtonType.Reply, 0 );
			}
			else
			{
				MC.DisplayStyle( this, StyleTypeConfig, 80, 530, 600, 20 );

				AddButton( 371, 535, 2438, 2437, -8, GumpButtonType.Reply, 0 );
			}

			OldMessagesTitle = MessagesTitle;
			OldMessages = Messages;

			AddHtml( 151, 431, 60, 20, MC.ColorText( defaultTextColor, "Refresh" ), false, false );
			AddButton( 120, 433, 2118, 2117, -7, GumpButtonType.Reply, 0 );

			if ( RefreshSpawnerLists )
			{
				CompileSpawnerList();

				RefreshSpawnerLists = false;
			}

			CheckMSGCheckBoxesList();

			SetPageInfo();
			GetOffsets();

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			SaveCheckBoxes( info );

			int index = (int) info.ButtonID - numOffset;
			MegaSpawner megaSpawner = null;

			if ( index >= 0 )
				megaSpawner = CheckSelection( index );

			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					OpenGump();

					break;
				}
				case -101: // Name
				{
					MessagesTitle = "Help: Name Column";
					Messages = "This column lists the name of each Mega Spawner.";

					OpenGump();

					break;
				}
				case -102: // Facet
				{
					MessagesTitle = "Help: Facet Column";
					Messages = "This column lists the facet of each Mega Spawner.";

					OpenGump();

					break;
				}
				case -103: // Location
				{
					MessagesTitle = "Help: Location Column";
					Messages = "This column lists the location of each Mega Spawner.";

					OpenGump();

					break;
				}
				case -104: // Active
				{
					MessagesTitle = "Help: Active Column";
					Messages = "This column lists whether each Mega Spawner is on or off.";

					OpenGump();

					break;
				}
				case -105: // Entries
				{
					MessagesTitle = "Help: Entries Column";
					Messages = "This column lists the total number of entries of each Mega Spawner.";

					OpenGump();

					break;
				}
				case -106: // Spawns
				{
					MessagesTitle = "Help: Spawns Column";
					Messages = "This column lists the total number of spawned entries of each Mega Spawner.";

					OpenGump();

					break;
				}
				case -50: // Toggle Go To Spawner Location
				{
					if ( Help )
					{
						MessagesTitle = "Help: Toggle Go To Spawner Location Button";
						Messages = "That button will toggle on/off whether the Mega Spawner buttons will edit the chosen Mega Spawner or go to it's location.";

						OpenGump();

						break;
					}

					GotoLocation = !GotoLocation;

					MessagesTitle = "Go To Spawner Location Toggle";

					if ( GotoLocation )
						Messages = "Go To Spawner Location has been turned on. Clicking the Mega Spawner buttons will result in to going to their location.";
					else
						Messages = "Go To Spawner Location has been turned off. Clicking the Mega Spawner buttons will result in to editting the Mega Spawner.";

					OpenGump();

					break;
				}
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount of Mega Spawners, to perform certain commands on.\n\nFor Example: You select a few Mega Spawners, then choose [Delete Selected Spawners], you will delete the selected Mega Spawners only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
							chkd++;
						else
							unchkd++;
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( chkd > unchkd )
							MSGCheckBoxesList[i] = (bool) false;
						else if ( unchkd > chkd )
							MSGCheckBoxesList[i] = (bool) true;
						else
							MSGCheckBoxesList[i] = (bool) false;
					}

					OpenGump();

					break;
				}
				case -8: // Display Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Minimize Messages Button";
						Messages = "That button will minimize the messages window. Clicking it again once minimized will restore the window.";

						OpenGump();

						break;
					}

					DisplayMessages = !DisplayMessages;

					OpenGump();

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window and also change the page number if specified in the upper right hand corner. To change the page number, delete the current page number and type in the page you want to go to.";

						OpenGump();

						break;
					}

					RefreshSpawnerLists = true;

					int intPage = 0;
					string checkPage = Convert.ToString( info.GetTextEntry( 0 ).Text ).ToLower();

					if ( checkPage == String.Format( "{0}/{1}", pg, totalPages ) )
					{
						OpenGump();

						break;
					}

					try{ intPage = Convert.ToInt32( checkPage ); }
					catch
					{
						MessagesTitle = "Refresh";
						Messages = "The page number must be an integer.";

						OpenGump();

						break;
					}

					if ( intPage > 0 && intPage <= totalPages )
					{
						pg = intPage;
					}
					else
					{
						MessagesTitle = "Refresh";
						Messages = "That page number does not exist.";
					}

					OpenGump();

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						OpenGump();

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					OpenGump();

					break;
				}
				case -5: // Help
				{
					if ( Help )
					{
						MessagesTitle = "Help";
						Messages = "Help is now off.";

						Help = false;
					}
					else
					{
						MessagesTitle = "Help";
						Messages = "Help is now on. Click on the button you wish to receive help on. If any buttons appear when you click help, those are help buttons you may click as well. You may click on as many buttons as you wish, and you must click the help button again to turn off help.";

						Help = true;
					}

					OpenGump();

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous page.";

						OpenGump();

						break;
					}

					pg--;

					OpenGump();

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next page.";

						OpenGump();

						break;
					}

					pg++;

					OpenGump();

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						OpenGump();

						break;
					}

					cpg--;

					OpenGump();

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						OpenGump();

						break;
					}

					cpg++;

					OpenGump();

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump.";

						OpenGump();

						break;
					}

					if ( FileName != "" )
					{
						SetArgsList();

						if ( FileName == "new" && SpawnerList.Count != 0 )
						{
							gumpMobile.SendGump( new ConfirmAbandonChangesGump( gumpMobile, ArgsList, SpawnerList, 1 ) );
						}
						else if ( Changed )
						{
							gumpMobile.SendGump( new ConfirmAbandonChangesGump( gumpMobile, ArgsList, SpawnerList, 2 ) );
						}
						else
						{
							MC.RemoveFileEdit( gumpMobile );

							gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );
						}
					}
					else
					{
						MC.RemoveMSUser( gumpMobile );
					}

					break;
				}
				case 1: // Admin Menu
				{
					if ( Help )
					{
						MessagesTitle = "Help: Admin Menu Button";
						Messages = "That button will open the administrator menu.";

						OpenGump();

						break;
					}

					PageInfoList[24] = 1;			// AdminMenuGumpCommandPage
					PageInfoList[25] = 1;			// AdminMenuGumpPage 

					SetArgsList();

					gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );

					break;
				}
				case 2: // Edit Your Personal Configuration
				{
					if ( Help )
					{
						MessagesTitle = "Help: Edit Your Personal Configuration Button";
						Messages = "That button will open your personal configuration.";

						OpenGump();

						break;
					}

					PageInfoList[20] = 1;					// PersonalConfigGumpCommandPage
					PageInfoList[21] = 1;					// PersonalConfigGumpPage

					PCGArgsList[0] = PersonalConfigList[0];			// styleTypeSwitch
					PCGArgsList[1] = PersonalConfigList[1];			// backgroundTypeSwitch
					PCGArgsList[2] = PersonalConfigList[2];			// activeTEBGTypeSwitch
					PCGArgsList[3] = PersonalConfigList[3];			// inactiveTEBGTypeSwitch
					PCGArgsList[4] = PersonalConfigList[4];			// defaultTextColorSwitch
					PCGArgsList[5] = PersonalConfigList[5];			// titleTextColorSwitch
					PCGArgsList[6] = PersonalConfigList[6];			// messagesTextColorSwitch
					PCGArgsList[7] = PersonalConfigList[7];			// commandButtonsTextColorSwitch
					PCGArgsList[8] = PersonalConfigList[14];		// flagTextColorSwitch

					PCGSetList[0] = (bool) false;				// StyleConfigClicked
					PCGSetList[1] = (bool) false;				// TextColorConfigClicked

					SetArgsList();

					gumpMobile.SendGump( new PersonalConfigGump( gumpMobile, ArgsList ) );

					break;
				}
				case 3: // Add Spawner
				{
					if ( Help )
					{
						MessagesTitle = "Help: Add Spawner Button";
						Messages = "That button will show a target cursor in which you are able to place a new Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					gumpMobile.SendMessage( "Target a location for the new Mega Spawner." );
					gumpMobile.Target = new AddSpawnerTarget( gumpMobile, ArgsList, pg, totalPages );

					break;
				}
				case 4: // Delete Selected Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Selected Spawners Button";
						Messages = "That button will delete all selected Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Delete Selected Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
						{
							found = true;

							break;
						}
					}

					if ( !found )
					{
						MessagesTitle = "Delete Selected Spawners";
						Messages = "There are no Mega Spawners selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteSpawnersGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				case 5: // Delete Imported Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Imported Spawners Button";
						Messages = "That button will delete all imported Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( MasterSpawnerList.Count == 0 )
					{
						MessagesTitle = "Delete Imported Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( !(bool) HideSpawnerList[i] )
						{
							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							if ( megaspawner.Imported != "" )
							{
								found = true;

								break;
							}
						}
					}

					if ( !found )
					{
						MessagesTitle = "Delete Imported Spawners";
						Messages = "There are no imported Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteSpawnersGump( gumpMobile, ArgsList, 3 ) );

					break;
				}
				case 6: // Delete All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete All Spawners Button";
						Messages = "That button will delete all Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( MasterSpawnerList.Count == 0 )
					{
						MessagesTitle = "Delete All Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteSpawnersGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				case 7: // Select Imported Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select Imported Spawners Button";
						Messages = "That button will select all imported Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Select Imported Spawners";
						Messages = "There are no Mega Spawners to select.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

						if ( !(bool) ModHideSpawnerList[i] )
						{
							if ( !MC.CheckBadFileName( megaspawner.Imported ) )
							{
								count++;

								MSGCheckBoxesList[i] = (bool) true;
							}
							else
							{
								MSGCheckBoxesList[i] = (bool) false;
							}
						}
					}

					MessagesTitle = "Select Imported Spawners";

					if ( count == 0 )
						Messages = "There are no imported spawners to select.";
					else
						Messages = String.Format( "{0} imported spawner{1} been selected.", count, count == 1 ? " has": "s have" );

					OpenGump();

					break;
				}
				case 8: // Select All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select All Spawners Button";
						Messages = "That button will select all Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Select All Spawners";
						Messages = "There are no Mega Spawners to select.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( !(bool) ModHideSpawnerList[i] )
						{
							count++;

							MSGCheckBoxesList[i] = (bool) true;
						}
					}

					MessagesTitle = "Select All Spawners";
					Messages = String.Format( "{0} Mega Spawner{1} been selected.", count, count == 1 ? " has": "s have" );

					OpenGump();

					break;
				}
				case 9: // Deselect All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Deselect All Spawners Button";
						Messages = "That button will deselect all Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Deselect All Spawners";
						Messages = "There are no Mega Spawners to deselect.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( !(bool) HideSpawnerList[i] )
						{
							count++;

							MSGCheckBoxesList[i] = (bool) false;
						}
					}

					MessagesTitle = "Deselect All Spawners";
					Messages = String.Format( "{0} Mega Spawner{1} been deselected.", count, count == 1 ? " has" : "s have" );

					OpenGump();

					break;
				}
				case 10: // Select Active Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select Active Spawners Button";
						Messages = "That button will select all active Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Select Active Spawners";
						Messages = "There are no Mega Spawners to select.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( !(bool) ModHideSpawnerList[i] )
						{
							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							if ( megaspawner.Active )
							{
								count++;

								MSGCheckBoxesList[i] = (bool) true;
							}
							else
							{
								MSGCheckBoxesList[i] = (bool) false;
							}
						}
					}

					MessagesTitle = "Select Active Spawners";

					if ( count == 0 )
						Messages = "There are no active Mega Spawners to select.";
					else
						Messages = String.Format( "{0} active Mega Spawner{1} been selected.", count, count == 1 ? " has" : "s have" );

					OpenGump();

					break;
				}
				case 11: // Select Inactive Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select Inactive Spawners Button";
						Messages = "That button will select all inactive Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Select Inactive Spawners";
						Messages = "There are no Mega Spawners to select.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( !(bool) ModHideSpawnerList[i] )
						{
							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							if ( !megaspawner.Active )
							{
								count++;

								MSGCheckBoxesList[i] = (bool) true;
							}
							else
							{
								MSGCheckBoxesList[i] = (bool) false;
							}
						}
					}

					MessagesTitle = "Select Inactive Spawners";

					if ( count == 0 )
						Messages = "There are no inactive Mega Spawners to select.";
					else
						Messages = String.Format( "{0} inactive Mega Spawner{1} been selected.", count, count == 1 ? " has" : "s have" );

					OpenGump();

					break;
				}
				case 13: // Respawn All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Respawn All Spawners Button";
						Messages = "That button respawns all entries on all Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Respawn All Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					CheckSpawners();

					foreach ( MegaSpawner megaspawner in SpawnerList )
						megaspawner.Respawn();

					MessagesTitle = "Respawn All Spawners";
					Messages = String.Format( "{0} Mega Spawner{1} been respawned.", SpawnerList.Count, SpawnerList.Count == 1 ? " has" : "s have" );

					OpenGump();

					break;
				}
				case 14: // Bring All To Home
				{
					if ( Help )
					{
						MessagesTitle = "Help: Bring All To Home Button";
						Messages = "That button will bring all Mega Spawner's creatures to their home location on the spawner list. This does not work for items, for they don't wander away like creatures.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Bring All To Home";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					CheckSpawners();

					foreach ( MegaSpawner megaspawner in SpawnerList )
						megaspawner.BringToHome();

					MessagesTitle = "Bring All To Home";
					Messages = String.Format( "{0} Mega Spawner's creatures have been brought to their Mega Spawner's location.", SpawnerList.Count );

					OpenGump();

					break;
				}
				case 15: // Respawn Selected Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Respawn Selected Spawners Button";
						Messages = "That button respawns all entries on selected Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Respawn Selected Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
						{
							count++;

							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							megaspawner.Respawn();
						}
					}

					MessagesTitle = "Respawn Selected Spawners";

					if ( count > 0 )
						Messages = String.Format( "{0} Mega Spawner{1} been respawned.", count, count == 1 ? " has" : "s have" );
					else
						Messages = "There are no Mega Spawners selected.";

					OpenGump();

					break;
				}
				case 16: // Bring Selected To Home
				{
					if ( Help )
					{
						MessagesTitle = "Help: Bring Selected To Home Button";
						Messages = "That button will bring all selected spawners creatures to their home Mega Spawner location. This does not work for items, for they don't wander away like creatures.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Bring Selected To Home";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
						{
							count++;

							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							megaspawner.BringToHome();
						}
					}

					MessagesTitle = "Bring Selected To Home";

					if ( count > 0 )
						Messages = String.Format( "{0} Mega Spawner's creatures have been brought to their Mega Spawner's location.", count );
					else
						Messages = "There are no Mega Spawners selected.";

					OpenGump();

					break;
				}
				case 19: // Turn On All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Turn On All Spawners Button";
						Messages = "That button will turn on all Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Turn On All Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					CheckSpawners();

					foreach ( MegaSpawner megaspawner in SpawnerList )
						megaspawner.Active = true;

					MessagesTitle = "Turn On All Spawners";
					Messages = String.Format( "{0} Mega Spawner{1} been turned on.", SpawnerList.Count, SpawnerList.Count == 1 ? " has" : "s have " );

					OpenGump();

					break;
				}
				case 20: // Turn Off All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Turn Off All Spawners Button";
						Messages = "That button will turn off all Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Turn Off All Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					CheckSpawners();

					foreach ( MegaSpawner megaspawner in SpawnerList )
						megaspawner.Active = false;

					MessagesTitle = "Turn Off All Spawners";
					Messages = String.Format( "{0} Mega Spawner{1} been turned off.", SpawnerList.Count, SpawnerList.Count == 1 ? " has" : "s have " );

					OpenGump();

					break;
				}
				case 21: // Turn On Selected Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Turn On Selected Spawners Button";
						Messages = "That button will turn on selected Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Turn On Selected Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
						{
							count++;

							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							megaspawner.Active = true;
						}
					}

					MessagesTitle = "Turn On Selected Spawners";

					if ( count > 0 )
						Messages = String.Format( "{0} selected Mega Spawner{1} been turned on.", count, count == 1 ? " has" : "s have " );
					else
						Messages = "There are no Mega Spawners selected.";

					OpenGump();

					break;
				}
				case 22: // Turn Off Selected Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Turn Off Selected Spawners Button";
						Messages = "That button will turn off selected Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Turn Off Selected Spawners";
						Messages = "There are no Mega Spawners on the spawner list.";

						OpenGump();

						break;
					}

					int count = 0;

					for ( int i = 0; i < MasterSpawnerList.Count; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
						{
							count++;

							MegaSpawner megaspawner = (MegaSpawner) MasterSpawnerList[i];

							megaspawner.Active = false;
						}
					}

					MessagesTitle = "Turn Off Selected Spawners";

					if ( count > 0 )
						Messages = String.Format( "{0} selected Mega Spawner{1} been turned off.", count, count == 1 ? " has" : "s have " );
					else
						Messages = "There are no Mega Spawners selected.";

					OpenGump();

					break;
				}
				case 25: // File Menu / Save File / Save Workspace
				{
					if ( Help )
					{
						if ( FileName == "new " )
						{
							MessagesTitle = "Help: Save Workspace Button";
							Messages = "That button will save your Spawner Workspace to a file.";
						}
						else if ( FileName != "" )
						{
							MessagesTitle = "Help: Save File Button";
							Messages = String.Format( "That button will save your spawner list to file: {0}", FileName );
						}
						else
						{
							MessagesTitle = "Help: File Menu Button";
							Messages = "That button will show a list of all imported files. You may import more files, export files, etc...";
						}

						OpenGump();

						break;
					}

					if ( FileName != "" )
					{
						if ( SpawnerList.Count == 0 )
						{
							MessagesTitle = FileName == "new" ? "Save Workspace" : "Save File";

							Messages = String.Format( "There are no Mega Spawners on the {0} list.", FileName == "new" ? "Spawner Workspace" : "File Edit" );

							OpenGump();

							break;
						}

						CheckSpawners();

						for ( int i = 0; i < MasterSpawnerList.Count; i++ )
						{
							if ( !(bool) HideSpawnerList[i] )
								MSGCheckBoxesList[i] = (bool) true;
						}

						Changed = false;

						if ( FileName == "new" )
							gumpMobile.SendGump( new SaveFileGump( gumpMobile, ArgsList, SaveType.Workspace, "" ) );
						else
							gumpMobile.SendGump( new SaveFileGump( gumpMobile, ArgsList, SaveType.FileEdit, FileName ) );

						break;
					}

					PageInfoList[6] = 1;				// FileMenuGumpCommandPage
					PageInfoList[7] = 1;				// FileMenuGumpPage

					SetArgsList();

					ArgsList[15] = new ArrayList();		// FMCheckBoxesList

					gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );

					break;
				}
				case 26: // Conversion Utility
				{
					if ( Help )
					{
						MessagesTitle = "Help: Conversion Utility Button";
						Messages = "That button will allow you to convert spawners of different types to a Mega Spawner.";

						OpenGump();

						break;
					}

					PageInfoList[8] = 1;				// ConversionUtilityGumpCommandPage
					PageInfoList[9] = 1;				// ConversionUtilityGumpPage 

					SetArgsList();

					ArgsList[16] = new ArrayList();		// CUGCheckBoxesList

					gumpMobile.SendGump( new ConversionUtilityGump( gumpMobile, ArgsList ) );

					break;
				}
				case 27: // Plug-In System
				{
					if ( Help )
					{
						MessagesTitle = "Help: Plug-In System Button";
						Messages = "That button will show you a list of all plug-ins loaded into the system and allow you to run them.";

						OpenGump();

						break;
					}

					PageInfoList[16] = 1;				// PlugInsGumpCommandPage
					PageInfoList[17] = 1;				// PlugInsGumpPage 

					SetArgsList();

					gumpMobile.SendGump( new PlugInsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 28: // Search And Sort
				{
					if ( Help )
					{
						MessagesTitle = "Help: Plug-In System Button";
						Messages = "That button will show you a list of all plug-ins loaded into the system and allow you to run them.";

						OpenGump();

						break;
					}

					PageInfoList[36] = 1;				// SearchAndSortGumpCommandPage
					PageInfoList[37] = 1;				// SearchAndSortGumpPage 

					SetArgsList();

					gumpMobile.SendGump( new SearchAndSortGump( gumpMobile, ArgsList ) );

					break;
				}
				default: // Mega Spawner Edit
				{
					if ( Help )
					{
						MessagesTitle = "Help: Mega Spawner Edit Button";
						Messages = "That button will open the selected Mega Spawner, and allow you to edit the settings.";

						OpenGump();

						break;
					}

					if ( megaSpawner == null )
					{
						MessagesTitle = "Mega Spawner Edit";
						Messages = "Command failed. That Mega Spawner has been deleted.";

						OpenGump();

						break;
					}

					if ( GotoLocation )
					{
						gumpMobile.Map = megaSpawner.Map;

						MessagesTitle = "Go To Location";

						Point3D location = new Point3D();

						if ( megaSpawner.RootParent is Container )
							location = ( (Container) megaSpawner.RootParent ).Location;
						else if ( megaSpawner.RootParent is Mobile )
							location = ( (Mobile) megaSpawner.RootParent ).Location;

						if ( megaSpawner.RootParent is Container || megaSpawner.RootParent is Mobile )
						{
							Messages = String.Format( "You have been moved to the Mega Spawner's location {0}. The Mega Spawner is located inside of {1}: {2}", location, megaSpawner.RootParent is Container ? "container" : "mobile", megaSpawner.RootParent.GetType().Name );

							gumpMobile.Location = location;
						}
						else
						{
							Messages = String.Format( "You have been moved to the Mega Spawner's location {0}.", megaSpawner.Location );

							gumpMobile.Location = megaSpawner.Location;
						}

						OpenGump();

						break;
					}

					if ( megaSpawner.Editor != null && megaSpawner.Editor != gumpMobile && megaSpawner.Editor.NetState != null && MC.IsLoggedIn( megaSpawner.Editor ) )
					{
						MessagesTitle = "Mega Spawner Edit";
						Messages = String.Format( "That Mega Spawner is currently being editted by {0}. You must wait for them to finish before you may edit that Mega Spawner.", megaSpawner.Editor.Name );

						OpenGump();

						break;
					}
					else if ( megaSpawner.FileEdit != null && megaSpawner.FileEdit != gumpMobile && megaSpawner.FileEdit.NetState != null && MC.IsLoggedIn( megaSpawner.Editor ) )
					{
						MessagesTitle = "Mega Spawner Edit";
						Messages = String.Format( "That Mega Spawner is currently a part of a file being editted by {0}. You must wait for them to finish before you may edit that Mega Spawner.", megaSpawner.FileEdit.Name );

						OpenGump();

						break;
					}

					megaSpawner.Editor = gumpMobile;

					MC.AddEditor( gumpMobile, megaSpawner );

					PageInfoList[3] = 1;				// MegaSpawnerEditGumpPage

					SetArgsList();

					ArgsList[14] = new ArrayList();		// MSEGCheckBoxesList
					ArgsList[19] = megaSpawner;			// megaSpawner
					ArgsList[20] = (bool) true;			// fromSpawnerList
					ArgsList[30] = 0;					// FromWhere

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[0] = cpg;
			PageInfoList[1] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[7] = FileName;
			ArgsList[10] = Changed;
			ArgsList[11] = ChangedSpawnerList;
			ArgsList[12] = PageInfoList;
			ArgsList[13] = MSGCheckBoxesList;
			ArgsList[26] = PCGArgsList;
			ArgsList[27] = PCGSetList;
			ArgsList[34] = RefreshSpawnerLists;
			ArgsList[35] = SpawnerList;
			ArgsList[36] = MasterSpawnerList;
			ArgsList[37] = ModHideSpawnerList;
			ArgsList[38] = GotoLocation;
		}

		private void GetArgsList()
		{
			Help = (bool)									ArgsList[0];
			DisplayMessages = (bool)						ArgsList[1];
			MessagesTitle = (string)						ArgsList[2];
			OldMessagesTitle = (string)						ArgsList[3];
			Messages = (string)								ArgsList[4];
			OldMessages = (string)							ArgsList[5];
			HideSpawnerList = (ArrayList)					ArgsList[6];
			FileName = (string)								ArgsList[7];
			Changed = (bool)								ArgsList[10];
			ChangedSpawnerList = (ArrayList)				ArgsList[11];
			PageInfoList = (ArrayList)						ArgsList[12];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			PCGArgsList = (ArrayList)						ArgsList[26];
			PCGSetList = (ArrayList)						ArgsList[27];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			RefreshSpawnerLists = (bool)					ArgsList[34];
			SpawnerList = (ArrayList)						ArgsList[35];
			MasterSpawnerList = (ArrayList)					ArgsList[36];
			ModHideSpawnerList = (ArrayList)				ArgsList[37];
			GotoLocation = (bool)						ArgsList[38];

			cpg = (int)										PageInfoList[0];
			pg = (int)										PageInfoList[1];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)			PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)			PersonalConfigList[7];
			PageNumberTextColor = (int)						PersonalConfigList[8];
			sortSearchFor = (string)						PersonalConfigList[15];
			sortSearchType = (SortSearchType)				PersonalConfigList[16];
			sortOrder = (SortOrder)							PersonalConfigList[17];
			sortType = (SortSearchType)						PersonalConfigList[18];
			sortSearchCaseSensitive = (bool)				PersonalConfigList[19];
			sortSearchFlagged = (bool)						PersonalConfigList[20];
			sortSearchBadLocation = (bool)					PersonalConfigList[21];
			sortSearchDupeSpawners = (bool)					PersonalConfigList[22];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
		}

		private void SetPageInfo()
		{
			totalPages = (int) ( ( SpawnerList.Count - 1 ) / amtPerPg ) + 1;

			if ( pg > totalPages )
				pg = totalPages;
		}

		private void SaveCheckBoxes( RelayInfo info )
		{
			CheckHideSpawnerList();
			CheckMSGCheckBoxesList();

			GetOffsets();

			if ( SpawnerList.Count == 0 )
				return;

			int j = offsetMin;

			for ( int i = 0; i < MasterSpawnerList.Count; i++ )
			{
				MegaSpawner fromCompare = (MegaSpawner) MasterSpawnerList[i];
				MegaSpawner toCompare = (MegaSpawner) SpawnerList[j];

				if ( fromCompare == toCompare )
				{
					if ( !toCompare.Deleted )
					{
						MSGCheckBoxesList[i] = info.IsSwitched( i );
						j++;
					}
				}

				if ( j > offsetMax )
					break;
			}
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

				OpenGump();
			}

			return MC.CheckProcess();
		}

		private void CompileSpawnerList()
		{
			MC.CheckSpawners();

			ModHideSpawnerList = MC.DupeList( HideSpawnerList );

			MasterSpawnerList.Clear();
			SpawnerList.Clear();

			if ( sortType != SortSearchType.None )
			{
				for ( int i = 0; i < MC.SpawnerList.Count; i++ )
				{
					MasterSpawnerList.Add( MC.SpawnerList[i] );
				}

				MasterSpawnerList = MC.Sort( MasterSpawnerList, sortOrder, sortType );
			}

			int count = MasterSpawnerList.Count;

			if ( sortType == SortSearchType.None )
				count = MC.SpawnerList.Count;

			for ( int i = 0; i < count; i++ )
			{
				MegaSpawner megaSpawner;

				if ( sortType == SortSearchType.None )
				{
					MasterSpawnerList.Add( MC.SpawnerList[i] );
					megaSpawner = (MegaSpawner) MC.SpawnerList[i];
				}
				else
				{
					megaSpawner = (MegaSpawner) MasterSpawnerList[i];
				}

				CheckHideSpawnerList();

				if ( !(bool) HideSpawnerList[i] )
				{
					bool addToList=false, flagged=false, badloc=false;

					if ( sortSearchFlagged )
					{
						for ( int j = 0; j < megaSpawner.EntryList.Count; j++ )
						{
							string checkEntryType = Convert.ToString( ScriptCompiler.FindTypeByName( megaSpawner.EntryList[j].ToString() ) );

							if ( checkEntryType == "" )
							{
								flagged = true;

								break;
							}
						}
					}

					if ( sortSearchBadLocation )
					{
						if ( !megaSpawner.Map.CanSpawnMobile( megaSpawner.Location ) )
							badloc = true;
					}

					if ( ( ( sortSearchFlagged && flagged ) || !sortSearchFlagged ) && ( ( sortSearchBadLocation && badloc ) || !sortSearchBadLocation ) )
						addToList = true;

					if ( sortSearchType != SortSearchType.None && addToList )
					{
						switch ( sortSearchType )
						{
							case SortSearchType.Name:
							{
								if ( sortSearchCaseSensitive )
								{
									if ( megaSpawner.Name.IndexOf( sortSearchFor ) >= 0 )
										SpawnerList.Add( megaSpawner );
								}
								else
								{
									if ( megaSpawner.Name.ToLower().IndexOf( sortSearchFor.ToLower() ) >= 0 )
										SpawnerList.Add( megaSpawner );
								}

								break;
							}
							case SortSearchType.Facet:
							{
								if ( megaSpawner.Map.ToString().ToLower().IndexOf( sortSearchFor.ToLower() ) >= 0 )
									SpawnerList.Add( megaSpawner );

								break;
							}
							case SortSearchType.Location:
							{
								if ( megaSpawner.Location.ToString().IndexOf( sortSearchFor ) >= 0 )
									SpawnerList.Add( megaSpawner );

								break;
							}
							case SortSearchType.LocationX:
							{
								if ( megaSpawner.Location.X.ToString() == sortSearchFor )
									SpawnerList.Add( megaSpawner );

								break;
							}
							case SortSearchType.LocationY:
							{
								if ( megaSpawner.Location.Y.ToString() == sortSearchFor )
									SpawnerList.Add( megaSpawner );

								break;
							}
							case SortSearchType.LocationZ:
							{
								if ( megaSpawner.Location.Z.ToString() == sortSearchFor )
									SpawnerList.Add( megaSpawner );

								break;
							}
							case SortSearchType.EntryName:
							{
								if ( sortSearchCaseSensitive )
								{
									for ( int j = 0; j < megaSpawner.EntryList.Count; j++ )
									{
										if ( ( (string) megaSpawner.EntryList[j] ).IndexOf( sortSearchFor ) >= 0 )
										{
											SpawnerList.Add( megaSpawner );

											break;
										}
									}
								}
								else
								{
									for ( int j = 0; j < megaSpawner.EntryList.Count; j++ )
									{
										if ( ( (string) megaSpawner.EntryList[j] ).ToLower().IndexOf( sortSearchFor.ToLower() ) >= 0 )
										{
											SpawnerList.Add( megaSpawner );

											break;
										}
									}
								}

								break;
							}
						}
					}
					else if ( addToList )
					{
						SpawnerList.Add( megaSpawner );
					}
					else
					{
						ModHideSpawnerList[i] = (bool) true;
					}
				}
			}

			if ( sortSearchDupeSpawners )
			{
				ArrayList DupeList = MC.CompileSameCriteriaList();

				for ( int i = 0; i < SpawnerList.Count; i++ )
				{
					MegaSpawner fromCompare = (MegaSpawner) SpawnerList[i];

					bool found = false;

					for ( int j = 0; j < DupeList.Count; j++ )
					{
						MegaSpawner toCompare = (MegaSpawner) DupeList[j];

						if ( fromCompare == toCompare )
						{
							found = true;

							break;
						}
					}

					if ( !found )
					{
						SpawnerList.RemoveAt( i );
						i--;
					}
				}
			}
		}

		private void CheckSpawners()
		{
			for ( int i = 0; i < SpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) SpawnerList[i];

				if ( megaSpawner.Deleted )
				{
					SpawnerList.RemoveAt( i );

					i--;
				}
			}

			for ( int i = 0; i < MasterSpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) MasterSpawnerList[i];

				if ( megaSpawner.Deleted )
				{
					MasterSpawnerList.RemoveAt( i );
					HideSpawnerList.RemoveAt( i );
					MSGCheckBoxesList.RemoveAt( i );

					i--;
				}
			}
		}

		private void CheckHideSpawnerList()
		{
			if ( MasterSpawnerList.Count > HideSpawnerList.Count )
			{
				int diff = ( MasterSpawnerList.Count - HideSpawnerList.Count );

				for ( int i = 0; i < diff; i++ )
				{
					if ( FileName != "" )
						HideSpawnerList.Add( (bool) true );
					else
						HideSpawnerList.Add( (bool) false );
				}
			}
		}

		private void CheckMSGCheckBoxesList()
		{
			if ( MasterSpawnerList.Count > MSGCheckBoxesList.Count )
			{
				int diff = ( MasterSpawnerList.Count - MSGCheckBoxesList.Count );

				for ( int i = 0; i < diff; i++ )
					MSGCheckBoxesList.Add( (bool) false );
			}

			if ( MSGCheckBoxesList.Count > MasterSpawnerList.Count )
			{
				int diff = ( MSGCheckBoxesList.Count - MasterSpawnerList.Count );

				for ( int i = 0; i < diff; i++ )
					MSGCheckBoxesList.RemoveAt( MSGCheckBoxesList.Count - 1 );
			}

			SetArgsList();
		}

		private MegaSpawner CheckSelection( int index )
		{
			MegaSpawner megaSpawner = (MegaSpawner) MasterSpawnerList[index];

			if ( megaSpawner.Deleted )
				return null;
			else
				return megaSpawner;
		}

		private Access GetAccessLevel{ get{ return MC.GetAccessLevel( gumpMobile ); } }

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= SpawnerList.Count )
				offsetMax = SpawnerList.Count - 1;
		}

		private void AddPages()
		{
			AddCommandButtons();

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( SpawnerList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				if ( FileName == "" )
					AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no Mega Spawners on the spawner list." ), false, false );
				else if ( FileName != "new" )
					AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, String.Format( "There are no Mega Spawners from file: {0}", FileName ) ), false, false );
				else
					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, "<center>New Spawner Workspace</center>\nYou may add as many Mega Spawners as you like, set them up how you want to set them up, then save them to a file. Mega Spawners created by \"New Spawner Workspace\" will be removed from the world after saving them to a file. The whole purpose for this feature is to allow you to design a Mega Spawner File (*.msf) to export. You can always load the file into your Mega Spawner System after creation." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 198, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 300, 100, 53, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 355, 100, 113, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 470, 100, 58, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 530, 100, 58, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 590, 100, 48, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 640, 100, 20, 20 );

				if ( !GotoLocation )
					AddButton( 100, 100, 9904, 9905, -50, GumpButtonType.Reply, 0 );
				else
					AddButton( 100, 100, 5540, 5542, -50, GumpButtonType.Reply, 0 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
					AddButton( 193, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
					AddButton( 321, 100, 2104, 2103, -102, GumpButtonType.Reply, 0 );
					AddButton( 406, 100, 2104, 2103, -103, GumpButtonType.Reply, 0 );
					AddButton( 493, 100, 2104, 2103, -104, GumpButtonType.Reply, 0 );
					AddButton( 554, 100, 2104, 2103, -105, GumpButtonType.Reply, 0 );
					AddButton( 609, 100, 2104, 2103, -106, GumpButtonType.Reply, 0 );
				}

				AddHtml( 101, 101, 200, 20, MC.ColorText( defaultTextColor, "<center>Name</center>" ), false, false );
				AddHtml( 301, 101, 55, 20, MC.ColorText( defaultTextColor, "<center>Facet</center>" ), false, false );
				AddHtml( 356, 101, 115, 20, MC.ColorText( defaultTextColor, "<center>Location</center>" ), false, false );
				AddHtml( 471, 101, 60, 20, MC.ColorText( defaultTextColor, "<center>Active</center>" ), false, false );
				AddHtml( 531, 101, 60, 20, MC.ColorText( defaultTextColor, "<center>Entries</center>" ), false, false );
				AddHtml( 591, 101, 50, 20, MC.ColorText( defaultTextColor, "<center>Spawns</center>" ), false, false );

				SetPage();
			}
		}

		private void SetPage()
		{
			int listY = 122;
			int spaceLeft = listSpace;

			if ( totalPages > pg )
			{
				AddHtml( 431, 431, 100, 20, MC.ColorText( defaultTextColor, String.Format( "Page {0}", ( pg + 1 ) ) ), false, false );
				AddButton( 380, 430, 4005, 4007, -3, GumpButtonType.Reply, 0 );
			}

			if ( pg > 1 )
			{
				int length = ( ( pg - 1 ).ToString().Length - 1 ) * 10;

				int pageX = 280 - length;

				AddHtml( pageX + 1, 431, 100, 20, MC.ColorText( defaultTextColor, String.Format( "Page {0}", ( pg - 1 ) ) ), false, false );
				AddButton( 340, 430, 4014, 4016, -4, GumpButtonType.Reply, 0 );
			}

			CheckSpawners();

			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) SpawnerList[i];

				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 198, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 300, listY, 53, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 355, listY, 113, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 470, listY, 58, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 530, listY, 58, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 590, listY, 48, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 640, listY, 20, 20 );

				int index = 0;

				for ( int j = offsetMin; j < MasterSpawnerList.Count; j++ )
				{
					MegaSpawner ms = (MegaSpawner) MasterSpawnerList[j];

					index = j;

					if ( megaSpawner == ms )
						break;
				}

				if ( ( megaSpawner.Workspace && megaSpawner.WorkspaceEditor != gumpMobile ) || megaSpawner.Editor != null || megaSpawner.FileEdit != null )
					AddButton( 100, listY, 5540, 5542, index + numOffset, GumpButtonType.Reply, 0 );
				else
					AddButton( 100, listY, 9904, 9905, index + numOffset, GumpButtonType.Reply, 0 );

				AddHtml( 141, listY + 1, 158, 20, MC.ColorText( defaultTextColor, megaSpawner.Name ), false, false );

				AddHtml( 301, listY + 1, 53, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", megaSpawner.Map ) ), false, false );

				Point3D location = new Point3D();

				if ( megaSpawner.RootParent is Container )
					location = ( (Container) megaSpawner.RootParent ).Location;
				else if ( megaSpawner.RootParent is Mobile )
					location = ( (Mobile) megaSpawner.RootParent ).Location;
				else
					location = megaSpawner.Location;

				AddHtml( 356, listY + 1, 113, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", location ) ), false, false );
				AddHtml( 471, listY + 1, 58, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", megaSpawner.Active.ToString() ) ), false, false );
				AddHtml( 531, listY + 1, 58, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", megaSpawner.EntryList.Count ) ), false, false );
				AddHtml( 591, listY + 1, 48, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", megaSpawner.CountEntries() ) ), false, false );
				AddCheck( 640, listY, 210, 211, (bool) MSGCheckBoxesList[index], index );

				listY += 21;
				spaceLeft -= 21;
			}

			MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 560, spaceLeft );
		}

		private void AddCommandButtons()
		{
			switch ( cpg )
			{
				case 1:{ AddButtonsPageOne(); break; }
				case 2:{ AddButtonsPageTwo(); break; }
				case 3:{ AddButtonsPageThree(); break; }
				case 4:{ AddButtonsPageFour(); break; }
				case 5:{ AddButtonsPageFive(); break; }
			}

			if ( cpg != 5 ) // Next Command Page
			{
				AddHtml( 441, 511, 150, 20, MC.ColorText( defaultTextColor, String.Format( "Command Page {0}", ( (int) cpg + 1 ) ) ), false, false );
				AddButton( 390, 510, 4005, 4007, -1, GumpButtonType.Reply, 0 );
			}

			AddHtml( 361, 511, 30, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", ( (int) cpg ) ) ), false, false );

			if ( cpg != 1 ) // Previous Command Page
			{
				AddHtml( 211, 511, 150, 20, MC.ColorText( defaultTextColor, String.Format( "Command Page {0}", ( (int) cpg - 1) ) ), false, false );
				AddButton( 330, 510, 4014, 4016, -2, GumpButtonType.Reply, 0 );
			}
		}

		private void AddButtonsPageOne()
		{
			if ( GetAccessLevel <= Access.Admin )
			{
				AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Admin Menu" ), false, false );
				AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );
			}

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Edit Your Personal Configuration" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Add Spawner" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Selected Spawners" ), false, false );
			AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Imported Spawners" ), false, false );
			AddButton( 350, 470, 9904, 9905, 5, GumpButtonType.Reply, 0 );

			AddHtml( 391, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete All Spawners" ), false, false );
			AddButton( 350, 490, 9904, 9905, 6, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageTwo()
		{
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Imported Spawners" ), false, false );
			AddButton( 100, 450, 9904, 9905, 7, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Select All Spawners" ), false, false );
			AddButton( 100, 470, 9904, 9905, 8, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Deselect All Spawners" ), false, false );
			AddButton( 100, 490, 9904, 9905, 9, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Active Spawners" ), false, false );
			AddButton( 350, 450, 9904, 9905, 10, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Inactive Spawners" ), false, false );
			AddButton( 350, 470, 9904, 9905, 11, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageThree()
		{
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Respawn All Spawners" ), false, false );
			AddButton( 100, 450, 9904, 9905, 13, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Bring All To Home" ), false, false );
			AddButton( 100, 470, 9904, 9905, 14, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Respawn Selected Spawners" ), false, false );
			AddButton( 350, 450, 9904, 9905, 15, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Bring Selected To Home" ), false, false );
			AddButton( 350, 470, 9904, 9905, 16, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageFour()
		{
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Turn On All Spawners" ), false, false );
			AddButton( 100, 450, 9904, 9905, 19, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Turn Off All Spawners" ), false, false );
			AddButton( 100, 470, 9904, 9905, 20, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Turn On Selected Spawners" ), false, false );
			AddButton( 350, 450, 9904, 9905, 21, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Turn Off Selected Spawners" ), false, false );
			AddButton( 350, 470, 9904, 9905, 22, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageFive()
		{
			if ( FileName == "new" )
				AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Save Workspace" ), false, false );
			else if ( FileName != "" )
				AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Save File" ), false, false );
			else
				AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "File Menu" ), false, false );

			AddButton( 100, 450, 9904, 9905, 25, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Conversion Utility" ), false, false );
			AddButton( 100, 470, 9904, 9905, 26, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Plug-In System" ), false, false );
			AddButton( 100, 490, 9904, 9905, 27, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Search And Sort" ), false, false );
			AddButton( 350, 450, 9904, 9905, 28, GumpButtonType.Reply, 0 );
		}

		private class AddSpawnerTarget : Target
		{
			private ArrayList ArgsList = new ArrayList();

			private string MessagesTitle = null;
			private string Messages = null;
			private ArrayList HideSpawnerList = new ArrayList();
			private string FileName = null;
			private bool Changed = false;
			private ArrayList ChangedSpawnerList = new ArrayList();
			private ArrayList PageInfoList = new ArrayList();
			private ArrayList MSGCheckBoxesList = new ArrayList();

			private MegaSpawner megaSpawner = null;
			private Mobile mobile;
			private int pg, totalPages;

			public AddSpawnerTarget( Mobile owner, ArrayList argsList, int page, int totalpages ) : base( -1, true, TargetFlags.None )
			{
				mobile = owner;
				ArgsList = argsList;
				MessagesTitle = (string)		ArgsList[2];
				Messages = (string)			ArgsList[4];
				HideSpawnerList = (ArrayList)		ArgsList[6];
				FileName = (string)			ArgsList[7];
				Changed = (bool)			ArgsList[10];
				ChangedSpawnerList = (ArrayList)	ArgsList[11];
				PageInfoList = (ArrayList)		ArgsList[12];
				MSGCheckBoxesList = (ArrayList)		ArgsList[13];

				pg = page;
				totalPages = totalpages;
			}

			protected override void OnTarget( Mobile mobile, object o )
			{
				IPoint3D target = (IPoint3D) o;
				Point3D point3D;

				if ( target != null )
				{
					megaSpawner = new MegaSpawner();

					if ( target is Container )
						megaSpawner.ContainerSpawn = (Container) target;

					if ( target is Item )
						target = ( (Item) target ).GetWorldTop();
					else if ( target is Mobile )
						target = ( (Mobile) target ).Location;

					point3D = new Point3D( target.X, target.Y, target.Z );

					megaSpawner.MoveToWorld( point3D, mobile.Map );
				}
			}

			protected override void OnTargetFinish( Mobile mobile )
			{
				if ( megaSpawner != null )
				{
					HideSpawnerList.Add( (bool) false );
					MSGCheckBoxesList.Add( (bool) false );

					if ( FileName != "" )
					{
						if ( FileName != "new" )
						{
							Changed = true;
							ChangedSpawnerList.Add( megaSpawner );

							MC.AddToFileEdit( mobile, megaSpawner );
						}
						else
						{
							megaSpawner.Workspace = true;
							megaSpawner.WorkspaceEditor = mobile;

							MC.AddToWorkspace( mobile, megaSpawner );
						}
					}

					MessagesTitle = "Add Spawner Button";
					Messages = String.Format( "Mega Spawner placed at location: {0}.", megaSpawner.Location.ToString() );

					if ( megaSpawner.ContainerSpawn != null )
						Messages = String.Format( "{0} The Mega Spawner is bound to a container: {1}.", Messages, megaSpawner.ContainerSpawn.GetType().Name );

					if ( pg != totalPages )
						pg = totalPages;

					PageInfoList[1] = pg;

					ArgsList[6] = HideSpawnerList;
					ArgsList[7] = FileName;
					ArgsList[10] = Changed;
					ArgsList[11] = ChangedSpawnerList;
					ArgsList[12] = PageInfoList;
					ArgsList[13] = MSGCheckBoxesList;
					ArgsList[34] = (bool) true;
				}
				else
				{
					MessagesTitle = "Add Spawner Button";
					Messages = "You have cancelled placement of a new Mega Spawner.";
				}

				ArgsList[2] = MessagesTitle;
				ArgsList[4] = Messages;

				mobile.SendGump( new MegaSpawnerGump( mobile, ArgsList ) );
			}
		}
	}
}
