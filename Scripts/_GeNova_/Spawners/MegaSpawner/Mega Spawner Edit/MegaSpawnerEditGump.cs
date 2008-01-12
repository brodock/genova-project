using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class MegaSpawnerEditGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList MSEGCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private bool fromSpawnerList;
		private ArrayList ESGArgsList = new ArrayList();
		private ArrayList AVSArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private FromGump FromWhere;

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor, FlagTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor, flagTextColor;
		private int PageNumberTextColor;

		private int offsetMin, offsetMax;

		private const int numOffset = 1000;
		private const int amtPerPg = 14;
		private const int listSpace = 308;
		private int totalPages;
		private Mobile gumpMobile;

		public MegaSpawnerEditGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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

			MC.DisplayStyle( this, StyleTypeConfig, 110, 60, 540, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 111, 61, 538, 18 );

			AddHtml( 121, 61, 540, 20, MC.ColorText( titleTextColor, String.Format( "\"{0}\"", megaSpawner.Name ) ), false, false );

			if ( megaSpawner.RootParent != null )
			{
				Point3D location = new Point3D();
				Map map = null;

				if ( megaSpawner.RootParent is Container )
				{
					location = ( (Container) megaSpawner.RootParent ).Location;
					map = ( (Container) megaSpawner.RootParent ).Map;
				}
				else if ( megaSpawner.RootParent is Mobile )
				{
					location = ( (Mobile) megaSpawner.RootParent ).Location;
					map = ( (Mobile) megaSpawner.RootParent ).Map;
				}

				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "[{0}] Inside {1} | {2} | {3}", megaSpawner.Active ? "Active" : "Inactive", megaSpawner.RootParent.GetType().Name, location, map ) ), false, false );
			}
			else if ( megaSpawner.ContainerSpawn != null )
			{
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "[{0}] Bound To {1} | {2} | {3}", megaSpawner.Active ? "Active" : "Inactive", megaSpawner.ContainerSpawn.GetType().Name, megaSpawner.Location, megaSpawner.Map ) ), false, false );
			}
			else
			{
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "[{0}] | {1} | {2}", megaSpawner.Active ? "Active" : "Inactive", megaSpawner.Location.ToString(), megaSpawner.Map ) ), false, false );
			}

			if ( !Help )
				AddButton( 597, 429, 2033, 2032, -5, GumpButtonType.Reply, 0 );
			else
				AddButton( 597, 429, 2032, 2033, -5, GumpButtonType.Reply, 0 );

			CheckEntryErrors();

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

			SetMSEGCheckBoxes();

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			SaveCheckBoxes( info );

			int index = (int) info.ButtonID - numOffset;

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
					Messages = "This column lists the name of each entry.";

					OpenGump();

					break;
				}
				case -102: // Spawn Type
				{
					MessagesTitle = "Help: Spawn Type Column";
					Messages = "This column lists the spawn type of each entry. Current spawn types are: Regular, Proximity, Game Time, Real Time, and Speech. If any of the spawn types are set to spawn in a group, (Grp) will appear next to the spawn type.";

					OpenGump();

					break;
				}
				case -103: // Respawn Time
				{
					MessagesTitle = "Help: Respawn Time Column";
					Messages = "This column lists the respawn time of each entry for when the next time the entry will be respawned. When you see \"Waiting...\", this means the entry is waiting for something to happen before respawning. This could mean that it is a proximity spawn waiting to be triggered, or a regular spawner that is full and is waiting for some of the spawned entries to be killed, tamed, etc...";

					OpenGump();

					break;
				}
				case -104: // Active
				{
					MessagesTitle = "Help: Active Column";
					Messages = "This column lists whether each entry is on or off";

					OpenGump();

					break;
				}
				case -105: // Count
				{
					MessagesTitle = "Help: Count Column";
					Messages = "This column lists how many is spawned out of a max amount for each entry. An example would be: 6/6. This means that 6 out of 6 are spawned for that entry, which means the entry is fully spawned. If Override Individual Entries is turned on, then only the number of spawned entries for each entry will be shown. An example would be: orc entry that shows 4. This means that out of the total amount to spawn, there are four orcs that are spawned.";

					OpenGump();

					break;
				}
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount of entries, to perform certain commands on.\n\nFor Example: You select a few entries, then choose [Respawn Selected], you will respawn the selected entries only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( (bool) MSEGCheckBoxesList[i] )
							chkd++;
						else
							unchkd++;
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( chkd > unchkd )
							MSEGCheckBoxesList[i] = (bool) false;
						else if ( unchkd > chkd )
							MSEGCheckBoxesList[i] = (bool) true;
						else
							MSEGCheckBoxesList[i] = (bool) false;
					}

					OpenGump();

					break;
				}
				case -8: // Display Messages
				{
					if ( Help )
					{
						if ( DisplayMessages )
						{
							MessagesTitle = "Help: Minimize Messages Button";
							Messages = "That button will minimize the messages window.";
						}
						else
						{
							MessagesTitle = "Help: Restore Messages Button";
							Messages = "That button will restore the messages window.";
						}

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

						if ( fromSpawnerList )
							Messages = "That button will close the gump and return to the previous gump.";
						else
							Messages = "That button will close the gump.";

						OpenGump();

						break;
					}

					if ( FromWhere == FromGump.AddViewSettings )
					{
						MessagesTitle = "Select Entry";
						Messages = "You have cancelled the entry selection. You must select an entry to continue.";

						SetArgsList();

						ArgsList[30] = 0;				// FromWhere

						gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );

						break;
					}

					megaSpawner.Editor = null;

					MC.RemoveEditor( gumpMobile, megaSpawner );

					if ( fromSpawnerList )
						gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Edit Settings
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Edit Settings Button";
						Messages = "That button will allow you to edit settings for the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					PageInfoList[12] = 1;			// SettingsGumpCommandPage
					PageInfoList[13] = 1;			// SettingsGumpPage

					SetArgsList();

					ArgsList[17] = new ArrayList();		// SettingsCheckBoxesList
					ArgsList[19] = megaSpawner;		// megaSpawner

					gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 2: // Set Spawner Name
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Set Spawner Name Button";
						Messages = "That button will set the name of the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					SetArgsList();

					gumpMobile.SendGump( new SetSpawnerNameGump( gumpMobile, ArgsList ) );

					break;
				}
				case 3: // Add Entry
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Add Entry Button";
						Messages = "That button will add another entry to the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					PageInfoList[4] = 1;									// EditSpawnGumpCommandPage
					PageInfoList[5] = 1;									// EditSpawnGumpPage

					ESGArgsList[0] = (bool) true;							// AddToSpawner
					ESGArgsList[1] = index;									// index
					ESGArgsList[2] = (bool) true;							// activatedSwitch
					ESGArgsList[3] = (bool) false;							// spawnGroupSwitch
					ESGArgsList[4] = (bool) true;							// eventAmbushSwitch
					ESGArgsList[5] = SpawnType.Regular;						// spawnTypeSwitch
					ESGArgsList[6] = "";									// entryType
					ESGArgsList[7] = 10;									// spawnRange
					ESGArgsList[8] = 10;									// walkRange
					ESGArgsList[9] = 1;										// amount
					ESGArgsList[10] = 0;									// minDelayHour
					ESGArgsList[11] = 5;									// minDelayMinute
					ESGArgsList[12] = 0;									// minDelaySecond
					ESGArgsList[13] = 0;									// maxDelayHour
					ESGArgsList[14] = 10;									// maxDelayMinute
					ESGArgsList[15] = 0;									// maxDelaySecond
					ESGArgsList[16] = 10;									// eventRange
					ESGArgsList[17] = 0;									// beginTimeBasedHour
					ESGArgsList[18] = 0;									// beginTimeBasedMinute
					ESGArgsList[19] = 0;									// endTimeBasedHour
					ESGArgsList[20] = 0;									// endTimeBasedMinute
					ESGArgsList[21] = "";									// keyword
					ESGArgsList[22] = (bool) false;							// caseSensitiveSwitch
					ESGArgsList[23] = 1;									// minStackAmount
					ESGArgsList[24] = 1;									// maxStackAmount
					ESGArgsList[25] = (bool) true;							// movableSwitch
					ESGArgsList[26] = 0;									// minDespawnHour
					ESGArgsList[27] = 30;									// minDespawnMinute
					ESGArgsList[28] = 0;									// minDespawnSecond
					ESGArgsList[29] = 1;									// maxDespawnHour
					ESGArgsList[30] = 0;									// maxDespawnMinute
					ESGArgsList[31] = 0;									// maxDespawnSecond
					ESGArgsList[32] = (bool) false;							// despawnSwitch
					ESGArgsList[33] = (bool) false;							// despawnGroupSwitch
					ESGArgsList[34] = (bool) true;							// despawnTimeExpireSwitch

					SetArgsList();

					gumpMobile.SendGump( new EditSpawnGump( gumpMobile, ArgsList ) );

					break;
				}
				case 4: // Delete Selected Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Delete Selected Entries Button";
						Messages = "That button will delete all selected entries.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					bool found = false;

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
					{
						if ( (bool) MSEGCheckBoxesList[i] )
							found = true;
					}

					if ( !found )
					{
						MessagesTitle = "Delete Selected Entries";
						Messages = "There are no entries selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteEntriesGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				case 5: // Delete All Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Delete All Entries Button";
						Messages = "That button will delete all entries on the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Delete All Entries";
						Messages = "There are no entries to delete.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteEntriesGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				case 6: // Delete Spawner
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Delete Spawner Button";
						Messages = "That button will delete the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					gumpMobile.SendGump( new ConfirmDeleteSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 7: // Select Regular Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Select Regular Entries Button";
						Messages = "That button will select all regular spawn type entries on the Mega Spawner entry list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Select Regular Entries";
						Messages = "There are no entries to select.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
					{
						if ( (SpawnType) megaSpawner.SpawnTypeList[i] == SpawnType.Regular )
						{
							found = true;

							MSEGCheckBoxesList[i] = (bool) true;
						}
						else
						{
							MSEGCheckBoxesList[i] = (bool) false;
						}
					}

					MessagesTitle = "Select Regular Entries";

					if ( !found )
						Messages = "There are no regular spawn type entries to select.";
					else
						Messages = "All regular spawn type entries have been selected.";

					OpenGump();

					break;
				}
				case 8: // Select All Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Select All Entries Button";
						Messages = "That button will select all entries on the Mega Spawner entry list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Select All Entries";
						Messages = "There are no entries to select.";

						OpenGump();

						break;
					}

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
						MSEGCheckBoxesList[i] = (bool) true;

					MessagesTitle = "Select All Entries";
					Messages = "All entries have been selected.";

					OpenGump();

					break;
				}
				case 9: // Deselect All Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Deselect All Entries Button";
						Messages = "That button will deselect all entries on the Mega Spawner entry list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Deselect All Entries";
						Messages = "There are no entries to deselect.";

						OpenGump();

						break;
					}

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
						MSEGCheckBoxesList[i] = (bool) false;

					MessagesTitle = "Deselect All Entries";
					Messages = "All entries have been deselected.";

					OpenGump();

					break;
				}
				case 10: // Select Proximity Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Select Proximity Entries Button";
						Messages = "That button will select all proximity spawn type entries on the Mega Spawner entry list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Select Proximity Entries";
						Messages = "There are no entries to select.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
					{
						if ( (SpawnType) megaSpawner.SpawnTypeList[i] == SpawnType.Proximity )
						{
							found = true;

							MSEGCheckBoxesList[i] = (bool) true;
						}
						else
						{
							MSEGCheckBoxesList[i] = (bool) false;
						}
					}

					MessagesTitle = "Select Proximity Entries";

					if ( !found )
						Messages = "There are no proximity spawn type entries to select.";
					else
						Messages = "All proximity spawn type entries have been selected.";

					OpenGump();

					break;
				}
				case 11: // Select Game Time Based Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Select Game Time Based Entries Button";
						Messages = "That button will select all Game time based spawn type entries on the Mega Spawner entry list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Select Game Time Based Entries";
						Messages = "There are no entries to select.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
					{
						if ( (SpawnType) megaSpawner.SpawnTypeList[i] == SpawnType.GameTimeBased )
						{
							found = true;

							MSEGCheckBoxesList[i] = (bool) true;
						}
						else
						{
							MSEGCheckBoxesList[i] = (bool) false;
						}
					}

					MessagesTitle = "Select UO Time Based Entries";

					if ( !found )
						Messages = "There are no game time based spawn type entries to select.";
					else
						Messages = "All game time based spawn type entries have been selected.";

					OpenGump();

					break;
				}
				case 12: // Select Real Time Based Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Select Real Time Based Entries Button";
						Messages = "That button will select all real time based spawn type entries on the Mega Spawner entry list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Select Real Time Based Entries";
						Messages = "There are no entries to select.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
					{
						if ( (SpawnType) megaSpawner.SpawnTypeList[i] == SpawnType.RealTimeBased )
						{
							found = true;

							MSEGCheckBoxesList[i] = (bool) true;
						}
						else
						{
							MSEGCheckBoxesList[i] = (bool) false;
						}
					}

					MessagesTitle = "Select Real Time Based Entries";

					if ( !found )
						Messages = "There are no real time based spawn type entries to select.";
					else
						Messages = "All real time based spawn type entries have been selected.";

					OpenGump();

					break;
				}
				case 13: // Respawn Spawner
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Respawn Spawner Button";
						Messages = "That button will respawn all entries on the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Respawn Spawner";
						Messages = "There are no entries to respawn.";

						OpenGump();

						break;
					}

					MessagesTitle = "Respawn Spawner";

					if ( megaSpawner.Active && !megaSpawner.Workspace )
					{
						megaSpawner.Respawn();

						Messages = "The Mega Spawner has now been respawned.";
					}
					else if ( megaSpawner.Workspace )
					{
						Messages = "The Mega Spawner is a part of a Spawner Workspace and cannot be respawned.";
					}
					else
					{
						Messages = "The Mega Spawner is not currently active and cannot be respawned.";
					}

					OpenGump();

					break;
				}
				case 14: // Bring All To Home
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Bring All To Home Button";
						Messages = "That button will bring all creatures on the Mega Spawner to their home location. This does not work for items, for they don't wander away like creatures.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Bring All To Home";
						Messages = "There are no entries to bring to home.";

						OpenGump();

						break;
					}

					MessagesTitle = "Bring All To Home";

					if ( megaSpawner.Active )
					{
						megaSpawner.BringToHome();

						Messages = "All creatures are now at their home location.";
					}
					else
					{
						Messages = "The Mega Spawner is not currently active, therefore, there are no creatures to bring to home.";
					}

					OpenGump();

					break;
				}
				case 15: // Respawn Selected Entries
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Respawn Selected Entries Button";
						Messages = "That button will respawn selected entries on the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Respawn Selected Entries";
						Messages = "There are no entries to respawn.";

						OpenGump();

						break;
					}

					bool found = false;

					MessagesTitle = "Respawn Selected Entries";

					if ( megaSpawner.Active )
					{
						for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
						{
							if ( (bool) MSEGCheckBoxesList[i] )
							{
								found = true;

								megaSpawner.Respawn( i );
							}
						}

						if ( found )
							Messages = "The selected entries have now been respawned.";
						else
							Messages = "There are no entries selected.";
					}
					else
					{
						Messages = "The Mega Spawner is not currently active and cannot be respawned.";
					}

					OpenGump();

					break;
				}
				case 16: // Bring Selected Entries To Home
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Bring Selected Entries To Home Button";
						Messages = "That button will bring selected creatures on the Mega Spawner to their home location. This does not work for items, for they don't wander away like creatures.";

						OpenGump();

						break;
					}

					if ( megaSpawner.EntryList.Count == 0 )
					{
						MessagesTitle = "Bring Selected Entries To Home";
						Messages = "There are no entries to bring to home.";

						OpenGump();

						break;
					}

					bool found = false;

					MessagesTitle = "Bring Selected Entries To Home";

					if ( megaSpawner.Active )
					{
						for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
						{
							if ( (bool) MSEGCheckBoxesList[i] )
							{
								found = true;

								megaSpawner.BringToHome( i );
							}
						}

						if ( found )
							Messages = "All selected creatures are now at their home location.";
						else
							Messages = "There are no entries selected.";
					}
					else
					{
						Messages = "The Mega Spawner is not currently active, therefore, there are no creatures to bring to home.";
					}

					OpenGump();

					break;
				}
				case 19: // Toggle Spawner On/Off
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						if ( megaSpawner.Active )
						{
							MessagesTitle = "Help: Turn Off Button";
							Messages = "That button will turn off the Mega Spawner.";
						}
						else
						{
							MessagesTitle = "Help: Turn On Button";
							Messages = "That button will turn on the Mega Spawner.";
						}

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( megaSpawner.Active )
					{
						megaSpawner.Active = false;

						MessagesTitle = "Turn Off Spawner";
						Messages = "The Mega Spawner has now been turned off.";
					}
					else
					{
						megaSpawner.Active = true;

						MessagesTitle = "Turn On Spawner";
						Messages = "The Mega Spawner has now been turned on.";
					}

					OpenGump();

					break;
				}
				case 20: // Go To Location
				{
					if ( CheckSelecting() )
						break;

					if ( Help )
					{
						MessagesTitle = "Help: Go To Location Button";
						Messages = "That button will move you to the Mega Spawner's location.";

						OpenGump();

						break;
					}

					gumpMobile.Map = megaSpawner.Map;

					MessagesTitle = "Go To Location";

					if ( megaSpawner.RootParent is Container || megaSpawner.RootParent is Mobile )
					{
						Point3D location = new Point3D();

						if ( megaSpawner.RootParent is Container )
							location = ( (Container) megaSpawner.RootParent ).Location;
						else if ( megaSpawner.RootParent is Mobile )
							location = ( (Mobile) megaSpawner.RootParent ).Location;

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
				default: // Entry Edit
				{
					if ( Help )
					{
						MessagesTitle = "Help: Entry Edit Button";
						Messages = "That button will open the selected entry and allow you to edit it's settings.";

						OpenGump();

						break;
					}

					if ( FromWhere == FromGump.AddViewSettings )
					{
						string entryType = (string) megaSpawner.EntryList[index];

						if ( !MC.IsContainerOrMobile( entryType ) )
						{
							MessagesTitle = "Select Entry";
							Messages = "You must select a mobile or container entry only.";

							OpenGump();

							break;
						}

						if ( !MC.IsMobileHasBackpack( entryType ) )
						{
							MessagesTitle = "Select Entry";
							Messages = "You must select a mobile that has a backpack.";

							OpenGump();

							break;
						}

						AVSArgsList[24] = index;		// index

						SetArgsList();

						gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );

						break;
					}

					int minDelayHour, minDelayMin, minDelaySec, maxDelayHour, maxDelayMin, maxDelaySec, beginTimeBasedHour, beginTimeBasedMinute, endTimeBasedHour, endTimeBasedMinute, minDespawnHour, minDespawnMin, minDespawnSec, maxDespawnHour, maxDespawnMin, maxDespawnSec;

					minDelayHour = (int) megaSpawner.MinDelayList[index] / 3600;
					minDelayMin = ( (int) megaSpawner.MinDelayList[index] - ( minDelayHour * 3600 ) ) / 60;
					minDelaySec = ( (int) megaSpawner.MinDelayList[index] - ( minDelayHour * 3600 ) - ( minDelayMin * 60 ) );
					maxDelayHour = (int) megaSpawner.MaxDelayList[index] / 3600;
					maxDelayMin = ( (int) megaSpawner.MaxDelayList[index] - ( maxDelayHour * 3600 ) ) / 60;
					maxDelaySec = ( (int) megaSpawner.MaxDelayList[index] - ( maxDelayHour * 3600 ) - ( maxDelayMin * 60 ) );

					minDespawnHour = (int) megaSpawner.MinDespawnList[index] / 3600;
					minDespawnMin = ( (int) megaSpawner.MinDespawnList[index] - ( minDespawnHour * 3600 ) ) / 60;
					minDespawnSec = ( (int) megaSpawner.MinDespawnList[index] - ( minDespawnHour * 3600 ) - ( minDespawnMin * 60 ) );
					maxDespawnHour = (int) megaSpawner.MaxDespawnList[index] / 3600;
					maxDespawnMin = ( (int) megaSpawner.MaxDespawnList[index] - ( maxDespawnHour * 3600 ) ) / 60;
					maxDespawnSec = ( (int) megaSpawner.MaxDespawnList[index] - ( maxDespawnHour * 3600 ) - ( maxDespawnMin * 60 ) );

					beginTimeBasedHour = (int) megaSpawner.BeginTimeBasedList[index] / 60;
					beginTimeBasedMinute = (int) megaSpawner.BeginTimeBasedList[index] - ( beginTimeBasedHour * 60 );
					endTimeBasedHour = (int) megaSpawner.EndTimeBasedList[index] / 60;
					endTimeBasedMinute = (int) megaSpawner.EndTimeBasedList[index] - ( endTimeBasedHour * 60 );

					PageInfoList[4] = 1;													// EditSpawnGumpCommandPage
					PageInfoList[5] = 1;													// EditSpawnGumpPage

					ESGArgsList[0] = (bool) false;											// AddToSpawner
					ESGArgsList[1] = index;													// index
					ESGArgsList[2] = (bool) megaSpawner.ActivatedList[index];				// activatedSwitch
					ESGArgsList[3] = (bool) megaSpawner.GroupSpawnList[index];				// spawnGroupSwitch
					ESGArgsList[4] = (bool) megaSpawner.EventAmbushList[index];				// eventAmbushSwitch
					ESGArgsList[5] = (int) megaSpawner.SpawnTypeList[index];				// spawnTypeSwitch
					ESGArgsList[6] = (string) megaSpawner.EntryList[index];					// entryType
					ESGArgsList[7] = (int) megaSpawner.SpawnRangeList[index];				// spawnRange
					ESGArgsList[8] = (int) megaSpawner.WalkRangeList[index];				// walkRange
					ESGArgsList[9] = (int) megaSpawner.AmountList[index];					// amount
					ESGArgsList[10] = minDelayHour;											// minDelayHour
					ESGArgsList[11] = minDelayMin;											// minDelayMinute
					ESGArgsList[12] = minDelaySec;											// minDelaySecond
					ESGArgsList[13] = maxDelayHour;											// maxDelayHour
					ESGArgsList[14] = maxDelayMin;											// maxDelayMinute
					ESGArgsList[15] = maxDelaySec;											// maxDelaySecond
					ESGArgsList[16] = (int) megaSpawner.EventRangeList[index];				// eventRange
					ESGArgsList[17] = (int) beginTimeBasedHour;								// beginTimeBasedHour
					ESGArgsList[18] = (int) beginTimeBasedMinute;							// beginTimeBasedMinute
					ESGArgsList[19] = (int) endTimeBasedHour;								// endTimeBasedHour
					ESGArgsList[20] = (int) endTimeBasedMinute;								// endTimeBasedMinute
					ESGArgsList[21] = (string) megaSpawner.EventKeywordList[index];			// keyword
					ESGArgsList[22] = (bool) megaSpawner.KeywordCaseSensitiveList[index];	// caseSensitiveSwitch
					ESGArgsList[23] = (int) megaSpawner.MinStackAmountList[index];			// minStackAmount
					ESGArgsList[24] = (int) megaSpawner.MaxStackAmountList[index];			// maxStackAmount
					ESGArgsList[25] = (bool) megaSpawner.MovableList[index];				// movableSwitch
					ESGArgsList[26] = minDespawnHour;										// minDespawnHour
					ESGArgsList[27] = minDespawnMin;										// minDespawnMinute
					ESGArgsList[28] = minDespawnSec;										// minDespawnSecond
					ESGArgsList[29] = maxDespawnHour;										// maxDespawnHour
					ESGArgsList[30] = maxDespawnMin;										// maxDespawnMinute
					ESGArgsList[31] = maxDespawnSec;										// maxDespawnSecond
					ESGArgsList[32] = (bool) megaSpawner.DespawnList[index];				// despawnSwitch
					ESGArgsList[33] = (bool) megaSpawner.DespawnGroupList[index];			// despawnGroupSwitch
					ESGArgsList[34] = (bool) megaSpawner.DespawnTimeExpireList[index];		// despawnTimeExpireSwitch

					SetArgsList();

					gumpMobile.SendGump( new EditSpawnGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[2] = cpg;
			PageInfoList[3] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[14] = MSEGCheckBoxesList;
			ArgsList[19] = megaSpawner;
			ArgsList[20] = fromSpawnerList;
			ArgsList[21] = ESGArgsList;
			ArgsList[22] = AVSArgsList;
			ArgsList[30] = FromWhere;
		}

		private void GetArgsList()
		{
			Help = (bool)												ArgsList[0];
			DisplayMessages = (bool)									ArgsList[1];
			MessagesTitle = (string)									ArgsList[2];
			OldMessagesTitle = (string)									ArgsList[3];
			Messages = (string)											ArgsList[4];
			OldMessages = (string)										ArgsList[5];
			PageInfoList = (ArrayList)									ArgsList[12];
			MSEGCheckBoxesList = (ArrayList)							ArgsList[14];
			megaSpawner = (MegaSpawner) 								ArgsList[19];
			fromSpawnerList = (bool) 									ArgsList[20];
			ESGArgsList = (ArrayList)									ArgsList[21];
			AVSArgsList = (ArrayList)									ArgsList[22];
			PersonalConfigList = (ArrayList)							ArgsList[28];
			FromWhere = (FromGump)										ArgsList[30];

			cpg = (int)	 												PageInfoList[2];
			pg = (int) 													PageInfoList[3];

			StyleTypeConfig = (StyleType)								PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)						PersonalConfigList[1];
			DefaultTextColor = (TextColor)								PersonalConfigList[4];
			TitleTextColor = (TextColor)								PersonalConfigList[5];
			MessagesTextColor = (TextColor)								PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)						PersonalConfigList[7];
			PageNumberTextColor = (int)									PersonalConfigList[8];
			FlagTextColor = (TextColor)									PersonalConfigList[14];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
			flagTextColor = MC.GetTextColor( FlagTextColor );
		}

		private bool CheckSelecting()
		{
			if ( FromWhere == FromGump.AddViewSettings )
			{
				MessagesTitle = "Command Not Allowed";
				Messages = "You are not allowed to use that command while you are selecting an entry to use for a setting.";

				OpenGump();

				return true;
			}

			return false;
		}

		private void SaveCheckBoxes( RelayInfo info )
		{
			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
				MSEGCheckBoxesList[i] = info.IsSwitched( i );
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

		private void SetMSEGCheckBoxes()
		{
			for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
				MSEGCheckBoxesList.Add( (bool) false );
		}

		private void CheckEntryErrors()
		{
			bool found = false;

			for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
			{
				string checkEntryType = Convert.ToString( ScriptCompiler.FindTypeByName( megaSpawner.EntryList[i].ToString() ) );

				if ( checkEntryType == "" )
				{
					if ( !found && OldMessages != Messages )
						Messages = String.Format( "{0}:\n{1}\n\nThe following entries have been detected as bad entries:\n{2}", MessagesTitle, Messages, megaSpawner.EntryList[i].ToString() );
					else if ( !found )
						Messages = String.Format( "The following entries have been detected as bad entries:\n{0}", megaSpawner.EntryList[i].ToString() );
					else
						Messages = String.Format( "{0}, {1}", Messages, megaSpawner.EntryList[i].ToString() );

					MessagesTitle = "Entry Errors Detected";

					found = true;
				}
			}

			if ( found )
				Messages = String.Format( "{0}\n\nThose entries listed do not exist on this server. They have been flagged a different color. Perhaps you have imported a file that has mobile or item scripts that you do not have.", Messages );

			OldMessages = Messages;
		}

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= megaSpawner.EntryList.Count )
				offsetMax = megaSpawner.EntryList.Count - 1;
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( megaSpawner.EntryList.Count - 1 ) / amtPerPg ) + 1;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( megaSpawner.EntryList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no entries on this Mega Spawner." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 178, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 280, 100, 118, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 400, 100, 98, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 500, 100, 58, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 560, 100, 78, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 640, 100, 20, 20 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
					AddButton( 183, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
					AddButton( 333, 100, 2104, 2103, -102, GumpButtonType.Reply, 0 );
					AddButton( 444, 100, 2104, 2103, -103, GumpButtonType.Reply, 0 );
					AddButton( 524, 100, 2104, 2103, -104, GumpButtonType.Reply, 0 );
					AddButton( 594, 100, 2104, 2103, -105, GumpButtonType.Reply, 0 );
				}

				AddHtml( 101, 101, 180, 20, MC.ColorText( defaultTextColor, "<center>Name</center>" ), false, false );
				AddHtml( 281, 101, 120, 20, MC.ColorText( defaultTextColor, "<center>Spawn Type</center>" ), false, false );
				AddHtml( 401, 101, 100, 20, MC.ColorText( defaultTextColor, "<center>Respawn Time</center>" ), false, false );
				AddHtml( 501, 101, 60, 20, MC.ColorText( defaultTextColor, "<center>Active</center>" ), false, false );
				AddHtml( 561, 101, 80, 20, MC.ColorText( defaultTextColor, "<center>Count</center>" ), false, false );

				SetPage();
			}
		}

		private void SetPage()
		{
			int respawnHour, respawnMin, respawnSec, countTime, calcTime, firstSpawnTime;
			DateTime firstSpawnCounter;

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

			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 178, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 280, listY, 118, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 400, listY, 98, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 500, listY, 58, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 560, listY, 78, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 640, listY, 20, 20 );

				string spawnType=null, count=null;
				string checkEntryType = Convert.ToString( ScriptCompiler.FindTypeByName( megaSpawner.EntryList[i].ToString() ) );
				string textColor = defaultTextColor;
				bool badEntry = false;

				if ( checkEntryType == "" )
				{
					badEntry = true;
					textColor = flagTextColor;
				}

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );
				AddHtml( 141, listY + 1, 138, 20, MC.ColorText( textColor, megaSpawner.EntryList[i].ToString() ), false, false );

				SpawnType spawntype = SpawnType.Regular;
				bool groupspawn = false;

				if ( megaSpawner.OverrideIndividualEntries )
				{
					spawntype = megaSpawner.OverrideSpawnType;
					groupspawn = megaSpawner.OverrideGroupSpawn;

					firstSpawnCounter = MegaSpawnerOverride.FindFirstSpawnCounter( megaSpawner );
					firstSpawnTime = MegaSpawnerOverride.FindFirstSpawnTime( megaSpawner );
				}
				else
				{
					spawntype = (SpawnType) megaSpawner.SpawnTypeList[i];
					groupspawn = (bool) megaSpawner.GroupSpawnList[i];

					firstSpawnCounter = megaSpawner.FindFirstSpawnCounter( i );
					firstSpawnTime = megaSpawner.FindFirstSpawnTime( i );
				}

				switch ( spawntype )
				{
					case SpawnType.Regular: { spawnType = "Regular"; break; }
					case SpawnType.Proximity: { spawnType = "Proximity"; break; }
					case SpawnType.GameTimeBased: { spawnType = "Game Time"; break; }
					case SpawnType.RealTimeBased: { spawnType = "Real Time"; break; }
					case SpawnType.Speech: { spawnType = "Speech"; break; }
				}

				if ( groupspawn )
					spawnType = String.Format( "{0}(Grp)", spawnType );

				AddHtml( 281, listY + 1, 118, 20, MC.ColorText( textColor, String.Format( "<center>{0}</center>", spawnType ) ), false, false );

				countTime = ( DateTime.Now.Hour - firstSpawnCounter.Hour ) * 3600;
				countTime = countTime + ( DateTime.Now.Minute - firstSpawnCounter.Minute ) * 60;
				countTime = countTime + ( DateTime.Now.Second - firstSpawnCounter.Second );

				calcTime = ( firstSpawnTime - countTime ) + megaSpawner.SaveAdjust( i );

				respawnHour = calcTime / 3600;
				respawnMin = ( calcTime - ( respawnHour * 3600 ) ) / 60;
				respawnSec = ( calcTime - ( respawnHour * 3600 ) - ( respawnMin * 60 ) );

				if ( respawnHour < 0 )
					respawnHour = 0;

				if ( respawnMin < 0 )
					respawnMin = 0;

				if ( respawnSec < 0 )
					respawnSec = 0;

				int entryCount = megaSpawner.CountEntries( i );

				if ( megaSpawner.OverrideIndividualEntries )
				{
					if ( !(bool) megaSpawner.ActivatedList[i] || badEntry )
						AddHtml( 401, listY + 1, 98, 20, MC.ColorText( textColor, "<center>N/A</center>" ), false, false );
					else if ( ( respawnHour <= 0 && respawnMin <= 0 && respawnSec <= 0 ) || entryCount >= megaSpawner.OverrideAmount || ( megaSpawner.OverrideGroupSpawn && entryCount != 0 ) )
						AddHtml( 401, listY + 1, 98, 20, MC.ColorText( textColor, "<center>Waiting...</center>" ), false, false );
					else
						AddHtml( 401, listY + 1, 98, 20, MC.ColorText( textColor, String.Format( "<center>{0}h {1}m {2}s</center>", respawnHour, respawnMin, respawnSec ) ), false, false );
				}
				else
				{
					if ( !(bool) megaSpawner.ActivatedList[i] || badEntry )
						AddHtml( 401, listY + 1, 98, 20, MC.ColorText( textColor, "<center>N/A</center>" ), false, false );
					else if ( ( respawnHour <= 0 && respawnMin <= 0 && respawnSec <= 0 ) || entryCount >= (int) megaSpawner.AmountList[i] || ( (bool) megaSpawner.GroupSpawnList[i] && entryCount != 0 ) )
						AddHtml( 401, listY + 1, 98, 20, MC.ColorText( textColor, "<center>Waiting...</center>" ), false, false );
					else
						AddHtml( 401, listY + 1, 98, 20, MC.ColorText( textColor, String.Format( "<center>{0}h {1}m {2}s</center>", respawnHour, respawnMin, respawnSec ) ), false, false );
				}

				AddHtml( 501, listY + 1, 58, 20, MC.ColorText( textColor, String.Format( "<center>{0}</center>", megaSpawner.ActivatedList[i].ToString() ) ), false, false );

				if ( megaSpawner.OverrideIndividualEntries )
					count = entryCount.ToString();
				else
					count = String.Format( "{0}/{1}", entryCount, megaSpawner.AmountList[i] );

				AddHtml( 561, listY + 1, 78, 20, MC.ColorText( textColor, String.Format( "<center>{0}</center>", count ) ), false, false );

				AddCheck( 640, listY, 210, 211, (bool) MSEGCheckBoxesList[i], i );

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
			}

			if ( cpg != 4 ) // Next Command Page
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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Edit Settings" ), false, false );
			AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Set Spawner Name" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Add Entry" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Selected Entries" ), false, false );
			AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete All Entries" ), false, false );
			AddButton( 350, 470, 9904, 9905, 5, GumpButtonType.Reply, 0 );

			AddHtml( 391, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Spawner" ), false, false );
			AddButton( 350, 490, 9904, 9905, 6, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageTwo()
		{
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Regular Entries" ), false, false );
			AddButton( 100, 450, 9904, 9905, 7, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Select All Entries" ), false, false );
			AddButton( 100, 470, 9904, 9905, 8, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Deselect All Entries" ), false, false );
			AddButton( 100, 490, 9904, 9905, 9, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Proximity Entries" ), false, false );
			AddButton( 350, 450, 9904, 9905, 10, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Game Time Based Entries" ), false, false );
			AddButton( 350, 470, 9904, 9905, 11, GumpButtonType.Reply, 0 );

			AddHtml( 391, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Select Real Time Based Entries" ), false, false );
			AddButton( 350, 490, 9904, 9905, 12, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageThree()
		{
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Respawn Spawner" ), false, false );
			AddButton( 100, 450, 9904, 9905, 13, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Bring All To Home" ), false, false );
			AddButton( 100, 470, 9904, 9905, 14, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Respawn Selected Entries" ), false, false );
			AddButton( 350, 450, 9904, 9905, 15, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Bring Selected Entries To Home" ), false, false );
			AddButton( 350, 470, 9904, 9905, 16, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageFour()
		{
			if ( !megaSpawner.Active )
				AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Turn On Spawner" ), false, false );
			else
				AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Turn Off Spawner" ), false, false );

			AddButton( 100, 450, 9904, 9905, 19, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Go To Location" ), false, false );
			AddButton( 350, 450, 9904, 9905, 20, GumpButtonType.Reply, 0 );
		}
	}
}