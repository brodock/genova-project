using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class AddViewSettingsGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList setting;
		private ArrayList PageInfoList = new ArrayList();
		private ArrayList SettingsCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private ArrayList AVSArgsList = new ArrayList();
		private ArrayList AVSSetList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList SEGArgsList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig, InactiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor, FlagTextColor;

		private bool AddSetting;
		private int index;
		private SpawnType spawnTypeSwitch;
		private bool spawnGroupSwitch, eventAmbushSwitch, caseSensitiveSwitch, despawnSwitch, despawnGroupSwitch, despawnTimeExpireSwitch;
		private string keyword;
		private int spawnRange, walkRange, amount, minDelayHour, minDelayMin, minDelaySec, maxDelayHour, maxDelayMin, maxDelaySec, eventRange, beginTimeBasedHour, beginTimeBasedMinute, endTimeBasedHour, endTimeBasedMinute, minDespawnHour, minDespawnMin, minDespawnSec, maxDespawnHour, maxDespawnMin, maxDespawnSec;
		private bool bEntryType, bSpawnRange, bWalkRange, bAmount, bMinDelayHour, bMinDelayMin, bMinDelaySec, bMaxDelayHour, bMaxDelayMin, bMaxDelaySec, bEventRange, bBeginTimeBased, bEndTimeBased, bMinDespawnHour, bMinDespawnMin, bMinDespawnSec, bMaxDespawnHour, bMaxDespawnMin, bMaxDespawnSec;
		private string entryName, addItem;
		private int entryIndex, minStackAmount, maxStackAmount;
		private bool bAddItem, bMinStackAmount, bMaxStackAmount;
		private ArrayList InsideItemList = new ArrayList();

		private bool OverrideIndividualEntriesClicked, AddItemClicked;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor, flagTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor, InactiveTextEntryTextColor;

		private string entryCheck;
		private ArrayList theSetting = new ArrayList();

		private int minDelay, maxDelay, minDespawn, maxDespawn;
		private int totalPages, pgOffset, pgNum;
		private string beginTimeBased, endTimeBased, beginAMPM, endAMPM;
		private RelayInfo relayInfo;
		private Mobile gumpMobile;

		public AddViewSettingsGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			MC.DisplayBackground( this, BackgroundTypeConfig, 350, 470, 279, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 630, 470, 30, 19 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 350, 490, 279, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 630, 490, 30, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 471, 248, 39 );

			MC.DisplayImage( this, StyleTypeConfig );

			AddButton( 630, 490, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			CompileSettingInfo();

			if ( AddSetting )
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, "Add Setting" ), false, false );
			else
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "View Setting{0}{1}", entryIndex > -1 ? String.Format( " For Entry #{0}", entryIndex ) : "", CheckError() ? " (Bad Setting)" : "" ) ) , false, false );

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

			if ( entryIndex > -1 )
				entryName = (string) megaSpawner.EntryList[entryIndex];

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					OpenGump();

					break;
				}
				case -101: // Entry Type
				{
					MessagesTitle = "Help: Entry Type";
					Messages = "You are overriding individual entries. To add entries, you must go back to the entry list and choose [Add Entry].";

					OpenGump();

					break;
				}
				case -102: // Spawn Range
				{
					MessagesTitle = "Help: Spawn Range";
					Messages = "This is where you enter the spawn range for which the entry will spawn in.";

					OpenGump();

					break;
				}
				case -103: // Walk Range
				{
					MessagesTitle = "Help: Walk Range";
					Messages = "This is where you enter the walk range for which the entry will roam.";

					OpenGump();

					break;
				}
				case -104: // Amount To Spawn
				{
					MessagesTitle = "Help: Amount To Spawn";
					Messages = "This is where you enter the amount to spawn for the entry.";

					OpenGump();

					break;
				}
				case -105: // Min Delay To Next Spawn
				{
					MessagesTitle = "Help: Min Delay To Next Spawn";
					Messages = "This is where you enter the minimum delay for the entry. The entry will wait to spawn between the minimum and maximum delay.";

					OpenGump();

					break;
				}
				case -106: // Max Delay To Next Spawn
				{
					MessagesTitle = "Help: Max Delay To Next Spawn";
					Messages = "This is where you enter the maximum delay for the entry. The entry will wait to spawn between the minimum and maximum delay.";

					OpenGump();

					break;
				}
				case -110: // Stack Amount Between
				{
					MessagesTitle = "Help: Stack Amount Between";
					Messages = "This is where you enter a minimum and maximum stack amount to randomly spawn between. This only applies to stackable items.";

					OpenGump();

					break;
				}
				case -111: // Activated
				{
					MessagesTitle = "Help: Activated";
					Messages = "If this checkbox is checked, the entry will be activated and can be spawned. If inactive, the entry will never spawn.";

					OpenGump();

					break;
				}
				case -112: // Spawn In Group
				{
					MessagesTitle = "Help: Spawn In Group";
					Messages = "If this checkbox is checked, the entry will spawn in a group. The total amount of this entry will spawn, and in order for respawn, all types of this entry must be killed, tamed, etc... before respawning. This is not the same as [Event Ambush]. Please see help button for [Event Ambush] for more information.";

					OpenGump();

					break;
				}
				case -113: // Event Ambush
				{
					MessagesTitle = "Help: Event Ambush";
					Messages = "If this checkbox is checked, the entry will spawn the entire amount at once only if there are no current spawned entries of the entry type. This only affects event triggered spawns such as proximity and speech. If unchecked, only one of the entry type will spawn at a time. This is not the same as [Spawn In Group], where all spawned entries of the entry type must be killed before respawn. This option will only spawn the total amount if there are no spawned entries of the entry type.";

					OpenGump();

					break;
				}
				case -114: // Movable
				{
					MessagesTitle = "Help: Movable";
					Messages = "If this checkbox is checked, then item entries can be picked up from the ground, and mobile entries are able to walk around. If this is not checked, item entries cannot be picked up from the ground (this is useful for spawning containers), and mobiles stand in one spot.";

					OpenGump();

					break;
				}
				case -201: // Spawn Type
				{
					MessagesTitle = "Help: Spawn Type";
					Messages = "This is where you select a spawn type for which you would like the entry to spawn.";

					OpenGump();

					break;
				}
				case -202: // Regular Spawn Type
				{
					MessagesTitle = "Help: Regular Spawn Type";
					Messages = "If this checkbox is checked, the entry will spawn as a regular or normal spawn with no extra guidelines.";

					OpenGump();

					break;
				}
				case -203: // Proximity Spawn Type
				{
					MessagesTitle = "Help: Proximity Spawn Type";
					Messages = "If this checkbox is checked, the entry will spawn only when a player comes within the event range specified.";

					OpenGump();

					break;
				}
				case -204: // Game Time Based Spawn Type
				{
					MessagesTitle = "Help: Game Time Based Spawn Type";
					Messages = "If this checkbox is checked, the entry will only spawn between the game times specified.";

					OpenGump();

					break;
				}
				case -205: // Real Time Based Spawn Type
				{
					MessagesTitle = "Help: Real Time Based Spawn Type";
					Messages = "If this checkbox is checked, the entry will only spawn between the real times specified.";

					OpenGump();

					break;
				}
				case -206: // Speech Spawn Type
				{
					MessagesTitle = "Help: Speech Spawn Type";
					Messages = "If this checkbox is checked, the entry will spawn only when a player comes within the event range specified and says the correct keyword.";

					OpenGump();

					break;
				}
				case -207: // Keyword
				{
					MessagesTitle = "Help: Keyword";
					Messages = "This is where you specify the keyword for a speech event trigger. This keyword is case sensitive!";

					OpenGump();

					break;
				}
				case -208: // Event Range
				{
					MessagesTitle = "Help: Event Range";
					Messages = "This is where you specify the range in which a certain event will occur.\n\nFor Example:\nA proximity spawner would use an event range as the range in which a player would trigger the spawner.";

					OpenGump();

					break;
				}
				case -209: // All Time Based Will Spawn Between
				{
					MessagesTitle = "Help: All Time Based Will Spawn Between";
					Messages = "This is where you specify the times for which any time based spawn type will spawn between.";

					OpenGump();

					break;
				}
				case -210: // Case Sensitive
				{
					MessagesTitle = "Help: Case Sensitive";
					Messages = "If this checkbox is checked, the keyword specified for a speech event trigger will be case sensitive.";

					OpenGump();

					break;
				}
				case -10: // Select Entry
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select Entry Button";
						Messages = "That button will allow you to select the entry you wish to add an item to.";

						SubmitSetting( info, false, 0 );

						break;
					}

					PageInfoList[2] = 1;					// MegaSpawnerEditGumpCommandPage
					PageInfoList[3] = 1;					// MegaSpawne EditGumpPage

					ArgsList[30] = FromGump.AddViewSettings;		// FromWhere

					SubmitSetting( info, false, 6 );

					break;
				}
				case -11: // Search
				{
					if ( Help )
					{
						MessagesTitle = "Help: Search Button";
						Messages = "That button will search for all entry types that match the text you typed in for [Item To Add]. This will allow you to type in a partial name and search for any existing entry types that match your search criteria.";

						SubmitSetting( info, false, 0 );

						break;
					}

					PageInfoList[22] = 1;					// MegaSpawnerEditGumpCommandPage
					PageInfoList[23] = 1;					// MegaSpawnerEditGumpPage

					ArgsList[30] = FromGump.AddViewSettings;		// FromWhere

					SEGArgsList[0] = SearchType.ItemType;			// searchType

					SubmitSetting( info, false, 7 );

					break;
				}
				case -12: // Edit Items
				{
					if ( Help )
					{
						MessagesTitle = "Help: Edit Items Button";
						Messages = "That button will allow you to edit items that you would like to be placed inside of this item only if it is a container.";

						SubmitSetting( info, false, 0 );

						break;
					}

					PageInfoList[30] = 1;					// AddItemsToContainerGumpCommandPage
					PageInfoList[31] = 1;					// AddItemsToContainerGumpPage

					AVSArgsList[27] = new ArrayList();			// InsideItemList

					ArgsList[31] = new ArrayList();				// AITCCheckBoxesList

					SubmitSetting( info, false, 9 );

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

						SubmitSetting( info, false, 0 );

						break;
					}

					DisplayMessages = !DisplayMessages;

					SubmitSetting( info, false, 0 );

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window.";

						SubmitSetting( info, false, 0 );

						break;
					}

					relayInfo = info;

					SubmitSetting( info, false, 5 );

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						SubmitSetting( info, false, 0 );

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					SubmitSetting( info, false, 0 );

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

					SubmitSetting( info, false, 0 );

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous page.";

						SubmitSetting( info, false, 0 );

						break;
					}

					SubmitSetting( info, false, 1 );

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next page.";

						SubmitSetting( info, false, 0 );

						break;
					}

					SubmitSetting( info, false, 2 );

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						SubmitSetting( info, false, 0 );

						break;
					}

					SubmitSetting( info, false, 3 );

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						SubmitSetting( info, false, 0 );

						break;
					}

					SubmitSetting( info, false, 4 );

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						SubmitSetting( info, false, 0 );

						break;
					}

					ArgsList[30] = 0;				// FromWhere

					SetArgsList();

					gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Override Individual Entries
				{
					if ( Help )
					{
						MessagesTitle = "Help: Override Individual Entries Button";
						Messages = "That button will set the Mega Spawner to override the amount, minimum delay, and maximum delay of each entry with a total amount, minimum delay, and maximum delay. This will force the Mega Spawner to act like a distro spawner in it's method for spawning.";

						SubmitSetting( info, false, 0 );

						break;
					}

					if ( AddSetting && CheckSetting( Setting.OverrideIndividualEntries ) )
					{
						MessagesTitle = "Choose Setting";
						Messages = "That setting already exists. You must return to the settings list and choose to edit that setting.";

						OpenGump();

						break;
					}

					ResetSelectionClicks();

					pg = 2;
					OverrideIndividualEntriesClicked = true;

					OpenGump();

					break;
				}
				case 2: // Delete Setting
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Setting Button";
						Messages = "That button will delete the setting from the Mega Spawner.";

						SubmitSetting( info, false, 0 );

						break;
					}

					if ( CheckProcess() )
						break;

					gumpMobile.SendGump( new ConfirmDeleteSettingGump( gumpMobile, ArgsList ) );

					break;
				}
				case 3: // Submit/Update Setting
				{
					if ( Help )
					{
						if ( AddSetting )
						{
							MessagesTitle = "Help: Submit Setting Button";
							Messages = "That button will add the setting to the Mega Spawner.";
						}
						else
						{
							MessagesTitle = "Help: Update Setting Button";
							Messages = "That button will update changes to the setting.";
						}

						SubmitSetting( info, false, 0 );

						break;
					}

					if ( CheckProcess() )
						SubmitSetting( info, false, 8 );
					else
						SubmitSetting( info, true, 0 );

					break;
				}
				case 4: // Add Item To Spawn Entry
				{
					if ( Help )
					{
						MessagesTitle = "Help: Add Item To Spawn Entry Button";
						Messages = "That button will allow you to add an item to a spawn entry that is a container or mobile. When the selected entry spawns, this item will be spawned with the entry.";

						SubmitSetting( info, false, 0 );

						break;
					}

					ResetSelectionClicks();

					pg = 5;
					AddItemClicked = true;

					OpenGump();

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[14] = cpg;
			PageInfoList[15] = pg;

			AVSArgsList[0] = AddSetting;
			AVSArgsList[1] = index;
			AVSArgsList[3] = spawnGroupSwitch;
			AVSArgsList[4] = eventAmbushSwitch;
			AVSArgsList[5] = spawnTypeSwitch;
			AVSArgsList[6] = spawnRange;
			AVSArgsList[7] = walkRange;
			AVSArgsList[8] = amount;
			AVSArgsList[9] = minDelayHour;
			AVSArgsList[10] = minDelayMin;
			AVSArgsList[11] = minDelaySec;
			AVSArgsList[12] = maxDelayHour;
			AVSArgsList[13] = maxDelayMin;
			AVSArgsList[14] = maxDelaySec;
			AVSArgsList[15] = eventRange;
			AVSArgsList[16] = beginTimeBasedHour;
			AVSArgsList[17] = beginTimeBasedMinute;
			AVSArgsList[18] = endTimeBasedHour;
			AVSArgsList[19] = endTimeBasedMinute;
			AVSArgsList[20] = keyword;
			AVSArgsList[21] = caseSensitiveSwitch;
			AVSArgsList[22] = entryName;
			AVSArgsList[23] = addItem;
			AVSArgsList[24] = entryIndex;
			AVSArgsList[25] = minStackAmount;
			AVSArgsList[26] = maxStackAmount;
			AVSArgsList[27] = InsideItemList;
			AVSArgsList[29] = minDespawnHour;
			AVSArgsList[30] = minDespawnMin;
			AVSArgsList[31] = minDespawnSec;
			AVSArgsList[32] = maxDespawnHour;
			AVSArgsList[33] = maxDespawnMin;
			AVSArgsList[34] = maxDespawnSec;
			AVSArgsList[35] = despawnSwitch;
			AVSArgsList[36] = despawnGroupSwitch;
			AVSArgsList[37] = despawnTimeExpireSwitch;

			AVSSetList[0] = OverrideIndividualEntriesClicked;
			AVSSetList[1] = AddItemClicked;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[8] = setting;
			ArgsList[12] = PageInfoList;
			ArgsList[17] = SettingsCheckBoxesList;
			ArgsList[19] = megaSpawner;
			ArgsList[22] = AVSArgsList;
			ArgsList[23] = AVSSetList;
			ArgsList[32] = SEGArgsList;
		}

		private void GetArgsList()
		{
			Help = (bool)												ArgsList[0];
			DisplayMessages = (bool)									ArgsList[1];
			MessagesTitle = (string)									ArgsList[2];
			OldMessagesTitle = (string)									ArgsList[3];
			Messages = (string)											ArgsList[4];
			OldMessages = (string)										ArgsList[5];
			setting = (ArrayList)										ArgsList[8];
			PageInfoList = (ArrayList)									ArgsList[12];
			SettingsCheckBoxesList = (ArrayList)						ArgsList[17];
			megaSpawner = (MegaSpawner)									ArgsList[19];
			AVSArgsList = (ArrayList)									ArgsList[22];
			AVSSetList = (ArrayList)									ArgsList[23];
			PersonalConfigList = (ArrayList)							ArgsList[28];
			SEGArgsList = (ArrayList)									ArgsList[32];

			cpg = (int)													PageInfoList[14];
			pg = (int)													PageInfoList[15];

			AddSetting = (bool) 										AVSArgsList[0];
			index = (int) 												AVSArgsList[1];
			spawnGroupSwitch = (bool) 									AVSArgsList[3];
			eventAmbushSwitch = (bool) 									AVSArgsList[4];
			spawnTypeSwitch = (SpawnType) 								AVSArgsList[5];

			if ( !bSpawnRange )
				spawnRange = (int) 										AVSArgsList[6];

			if ( !bWalkRange )
				walkRange = (int) 										AVSArgsList[7];

			if ( !bAmount )
				amount = (int)											AVSArgsList[8];

			if ( !bMinDelayHour )
				minDelayHour = (int) 									AVSArgsList[9];

			if ( !bMinDelayMin )
				minDelayMin = (int) 									AVSArgsList[10];

			if ( !bMinDelaySec )
				minDelaySec = (int) 									AVSArgsList[11];

			if ( !bMaxDelayHour )
				maxDelayHour = (int) 									AVSArgsList[12];

			if ( !bMaxDelayMin )
				maxDelayMin = (int) 									AVSArgsList[13];

			if ( !bMaxDelaySec )
				maxDelaySec = (int) 									AVSArgsList[14];

			if ( !bEventRange )
				eventRange = (int) 										AVSArgsList[15];

			if ( !bBeginTimeBased )
			{
				beginTimeBasedHour = (int) 								AVSArgsList[16];
				beginTimeBasedMinute = (int) 							AVSArgsList[17];
			}

			if ( !bEndTimeBased )
			{
				endTimeBasedHour = (int) 								AVSArgsList[18];
				endTimeBasedMinute = (int) 								AVSArgsList[19];
			}

			keyword = (string) 											AVSArgsList[20];
			caseSensitiveSwitch = (bool)								AVSArgsList[21];
			entryName = (string)										AVSArgsList[22];

			if ( !bAddItem )
				addItem = (string)										AVSArgsList[23];

			entryIndex = (int)											AVSArgsList[24];

			if ( !bMinStackAmount )
				minStackAmount = (int)									AVSArgsList[25];

			if ( !bMaxStackAmount )
				maxStackAmount = (int)									AVSArgsList[26];

			InsideItemList = (ArrayList)								AVSArgsList[27];

			if ( !bMinDespawnHour )
				minDespawnHour = (int) 									AVSArgsList[29];

			if ( !bMinDespawnMin )
				minDespawnMin = (int) 									AVSArgsList[30];

			if ( !bMinDespawnSec )
				minDespawnSec = (int) 									AVSArgsList[31];

			if ( !bMaxDespawnHour )
				maxDespawnHour = (int) 									AVSArgsList[32];

			if ( !bMaxDespawnMin )
				maxDespawnMin = (int) 									AVSArgsList[33];

			if ( !bMaxDespawnSec )
				maxDespawnSec = (int) 									AVSArgsList[34];

			despawnSwitch = (bool)										AVSArgsList[35];
			despawnGroupSwitch = (bool)									AVSArgsList[36];
			despawnTimeExpireSwitch = (bool)							AVSArgsList[37];

			OverrideIndividualEntriesClicked = (bool)					AVSSetList[0];
			AddItemClicked = (bool) 									AVSSetList[1];

			StyleTypeConfig = (StyleType)								PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)						PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)						PersonalConfigList[2];
			InactiveTEBGTypeConfig = (BackgroundType)					PersonalConfigList[3];
			DefaultTextColor = (TextColor)								PersonalConfigList[4];
			TitleTextColor = (TextColor)								PersonalConfigList[5];
			MessagesTextColor = (TextColor)								PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)						PersonalConfigList[7];
			PageNumberTextColor = (int)									PersonalConfigList[8];
			ActiveTextEntryTextColor = (int)							PersonalConfigList[9];
			InactiveTextEntryTextColor = (int)							PersonalConfigList[10];
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

		private void CompileSettingInfo()
		{
			if ( setting.Count == 0 )
				return;

			theSetting = setting;

			InsideItemList.Clear();

			try
			{
				switch( (Setting) setting[0] )
				{
					case Setting.OverrideIndividualEntries:
					{
						spawnRange = (int)						setting[1];
						walkRange = (int)						setting[2];
						amount = (int)							setting[3];
						minDelay = (int)						setting[4];
						maxDelay = (int)						setting[5];
						spawnGroupSwitch = (bool)				setting[6];
						eventAmbushSwitch = (bool)				setting[7];
						spawnTypeSwitch = (SpawnType)			setting[8];
						keyword = (string)						setting[9];
						caseSensitiveSwitch = (bool)			setting[10];
						eventRange = (int)						setting[11];
						beginTimeBasedHour = (int)				setting[12];
						beginTimeBasedMinute = (int)			setting[13];
						endTimeBasedHour = (int)				setting[14];
						endTimeBasedMinute = (int)				setting[15];
						minDespawn = (int)						setting[16];
						maxDespawn = (int)						setting[17];
						despawnSwitch = (bool)					setting[18];
						despawnGroupSwitch = (bool)				setting[19];
						despawnTimeExpireSwitch = (bool)		setting[20];

						pg = 3;
						OverrideIndividualEntriesClicked = true;

						minDelayHour = (int) minDelay / 3600;
						minDelayMin = ( (int) minDelay - ( minDelayHour * 3600 ) ) / 60;
						minDelaySec = ( (int) minDelay - ( minDelayHour * 3600 ) - ( minDelayMin * 60 ) );
						maxDelayHour = (int) maxDelay / 3600;
						maxDelayMin = ( (int) maxDelay - ( maxDelayHour * 3600 ) ) / 60;
						maxDelaySec = ( (int) maxDelay - ( maxDelayHour * 3600 ) - ( maxDelayMin * 60 ) );

						minDespawnHour = (int) minDespawn / 3600;
						minDespawnMin = ( (int) minDespawn - ( minDespawnHour * 3600 ) ) / 60;
						minDespawnSec = ( (int) minDespawn - ( minDespawnHour * 3600 ) - ( minDespawnMin * 60 ) );
						maxDespawnHour = (int) maxDespawn / 3600;
						maxDespawnMin = ( (int) maxDespawn - ( maxDespawnHour * 3600 ) ) / 60;
						maxDespawnSec = ( (int) maxDespawn - ( maxDespawnHour * 3600 ) - ( maxDespawnMin * 60 ) );

						setting = new ArrayList();

						break;
					}
					case Setting.AddItem:
					{
						SetItemInfo();

						pg = 6;
						AddItemClicked = true;

						setting = new ArrayList();

						break;
					}
					case Setting.AddContainer:
					{
						SetItemInfo();

						for ( int i = 6; i < setting.Count; i++ )
						{
							ArrayList ItemsList = (ArrayList) setting[i];

							InsideItemList.Add( ItemsList );
						}

						pg = 6;
						AddItemClicked = true;

						setting = new ArrayList();

						break;
					}
				}
			}
			catch( Exception ex )
			{
				MessagesTitle = "Update Setting";
				Messages = String.Format( "Error reading setting info:\n{0}", ex );
			}
		}

		private void SetItemInfo()
		{
			entryCheck = (string)						setting[1];
			entryIndex = (int)							setting[2];
			addItem = (string)							setting[3];
			minStackAmount = (int)						setting[4];
			maxStackAmount = (int)						setting[5];

		}

		private bool CheckSetting( Setting settingNum )
		{
			for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
			{
				ArrayList List = (ArrayList) megaSpawner.SettingsList[i];

				switch ( settingNum )
				{
					case Setting.OverrideIndividualEntries:
					{
						if ( (Setting) List[0] == Setting.OverrideIndividualEntries )
							return true;

						break;
					}
				}
			}

			return false;
		}

		private bool CheckError()
		{
			if ( !AddSetting && setting.Count != 0 && entryIndex > -1 && entryCheck.ToLower() != ( (string) megaSpawner.EntryList[entryIndex] ).ToLower() )
				return true;
			else if ( !MC.VerifySetting( theSetting ) )
				return true;
			else
				return false;
		}

		private void ResetSelectionClicks()
		{
			OverrideIndividualEntriesClicked = false;
			AddItemClicked = false;
		}

		private void CalcTimeBased()
		{
			int beginHour = beginTimeBasedHour, endHour = endTimeBasedHour;
			string beginAMPM=null, endAMPM=null;

			if ( beginTimeBasedHour >= 12 )
			{
				beginAMPM = "PM";
				beginHour -= 12;
			}
			else
				beginAMPM = "AM";

			if ( endTimeBasedHour >= 12 )
			{
				endAMPM = "PM";
				endHour -= 12;
			}
			else
				endAMPM = "AM";

			if ( beginHour == 0 )
				beginHour = 12;

			if ( endHour == 0 )
				endHour = 12;

			if ( beginTimeBasedMinute < 10 )
				beginTimeBased = String.Format( "{0}:0{1} {2}", beginHour, beginTimeBasedMinute, beginAMPM );
			else
				beginTimeBased = String.Format( "{0}:{1} {2}", beginHour, beginTimeBasedMinute, beginAMPM );

			if ( endTimeBasedMinute < 10 )
				endTimeBased = String.Format( "{0}:0{1} {2}", endHour, endTimeBasedMinute, endAMPM );
			else
				endTimeBased = String.Format( "{0}:{1} {2}", endHour, endTimeBasedMinute, endAMPM );
		}

		private int CalcPageEntry()
		{
			int intPage = 0;
			string checkPage = Convert.ToString( relayInfo.GetTextEntry( 0 ).Text ).ToLower();

			if ( checkPage == String.Format( "{0}/{1}", pgNum, totalPages ) )
			{
				return pg;
			}

			try{ intPage = Convert.ToInt32( checkPage ); }
			catch
			{
				MessagesTitle = "Refresh";
				Messages = "The page number must be an integer.";

				return pg;
			}

			if ( intPage > 0 && intPage <= totalPages )
			{
				return intPage + pgOffset;
			}
			else
			{
				MessagesTitle = "Refresh";
				Messages = "That page number does not exist.";

				return pg;
			}
		}

		private void UpdatePage( int command )
		{
			switch ( command )
			{
				case 1:{ pg--; break; }
				case 2:{ pg++; break; }
				case 3:{ cpg--; break; }
				case 4:{ cpg++; break; }
				case 5:{ pg = CalcPageEntry(); break; }
				case 6:{ SetArgsList(); gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) ); break; }
				case 7:{ SEGArgsList[1] = addItem; SetArgsList(); gumpMobile.SendGump( new SearchEntryGump( gumpMobile, ArgsList ) ); break; }
				case 8:{ OpenGump(); break; }
				case 9:
				{
					if ( addItem == null || !MC.IsContainer( addItem ) )
					{
						MessagesTitle = "Edit Items";
						Messages = "The [Item To Add] is not a container. Therefore, you cannot add items to it.";

						OpenGump();

						break;
					}

					SetArgsList();

					gumpMobile.SendGump( new AddItemsToContainerGump( gumpMobile, ArgsList ) );

					break;
				}
			}

			if ( command < 6 )
				OpenGump();
		}

		private void AddPages()
		{
			AddCommandButtons();

			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

			if ( Help )
			{
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

				switch ( pg )
				{
					case 3:
					{
						AddButton( 168, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
						AddButton( 178, 120, 2104, 2103, -102, GumpButtonType.Reply, 0 );
						AddButton( 171, 140, 2104, 2103, -103, GumpButtonType.Reply, 0 );
						AddButton( 208, 160, 2104, 2103, -104, GumpButtonType.Reply, 0 );
						AddButton( 257, 180, 2104, 2103, -105, GumpButtonType.Reply, 0 );
						AddButton( 260, 200, 2104, 2103, -106, GumpButtonType.Reply, 0 );

						AddButton( 239, 280, 2104, 2103, -110, GumpButtonType.Reply, 0 );
						AddButton( 194, 340, 2104, 2103, -111, GumpButtonType.Reply, 0 );
						AddButton( 231, 360, 2104, 2103, -112, GumpButtonType.Reply, 0 );
						AddButton( 221, 380, 2104, 2103, -113, GumpButtonType.Reply, 0 );
						AddButton( 183, 400, 2104, 2103, -114, GumpButtonType.Reply, 0 );

						break;
					}
					case 4:
					{
						AddButton( 171, 100, 2104, 2103, -201, GumpButtonType.Reply, 0 );
						AddButton( 166, 120, 2104, 2103, -202, GumpButtonType.Reply, 0 );
						AddButton( 179, 140, 2104, 2103, -203, GumpButtonType.Reply, 0 );
						AddButton( 228, 160, 2104, 2103, -204, GumpButtonType.Reply, 0 );
						AddButton( 222, 180, 2104, 2103, -205, GumpButtonType.Reply, 0 );
						AddButton( 164, 200, 2104, 2103, -206, GumpButtonType.Reply, 0 );
						AddButton( 310, 200, 2104, 2103, -207, GumpButtonType.Reply, 0 );
						AddButton( 175, 380, 2104, 2103, -208, GumpButtonType.Reply, 0 );
						AddButton( 316, 400, 2104, 2103, -209, GumpButtonType.Reply, 0 );
						AddButton( 350, 240, 2104, 2103, -210, GumpButtonType.Reply, 0 );

						break;
					}
				}
			}

			SetPage();
		}

		private void SetPage()
		{
			totalPages = 0;
			pgNum = 0;

			if ( pg == 1 )
			{
				totalPages = 1;
				pgNum = pg;
			}

			if ( OverrideIndividualEntriesClicked )
			{
				pgOffset = 1;
				totalPages = 3;
			}

			if ( AddItemClicked )
			{
				pgOffset = 4;
				totalPages = 2;
			}

			pgNum = pg - pgOffset;

			string pageNum = String.Format( "{0}/{1}", pgNum, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( totalPages > pgNum )
			{
				AddHtml( 431, 431, 100, 20, MC.ColorText( defaultTextColor, String.Format( "Page {0}", ( pgNum + 1 ) ) ), false, false );
				AddButton( 380, 430, 4005, 4007, -3, GumpButtonType.Reply, 0 );
			}

			if ( pgNum > 1 )
			{
				int length = ( ( pg - 1 ).ToString().Length - 1 ) * 10;

				int pageX = 280 - length;

				AddHtml( pageX + 1, 431, 100, 20, MC.ColorText( defaultTextColor, String.Format( "Page {0}", ( pgNum - 1 ) ) ), false, false );
				AddButton( 340, 430, 4014, 4016, -4, GumpButtonType.Reply, 0 );
			}

			string overrideTextColor = flagTextColor;

			if ( !megaSpawner.OverrideIndividualEntries )
				overrideTextColor = defaultTextColor;

			switch ( pg )
			{
				case 1:
				{
					AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "Select a setting from the command window below." ), false, false );

					break;
				}
				case 2: // Override Individual Entries
				{
					string pageInfo = "<center>Override Individual Entries</center>\nThis will allow you to override each entry's settings with a global setting for all entries.";
					pageInfo = String.Format( "{0} This will make the Mega Spawner act as a distro spawner in it's spawn method. All you need to do is configure all your settings here, and just add entries afterwards.", pageInfo );
					pageInfo = String.Format( "{0}\n\nGo to page 2 to continue.", pageInfo );

					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, pageInfo ), false, true );

					break;
				}
				case 3:
				{
					AddHtml( 101, 101, 100, 20, MC.ColorText( overrideTextColor, "Entry Type:" ), false, false );
					MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 101, 190, 18 );
					AddLabelCropped( 300, 101, 60, 20, InactiveTextEntryTextColor, "N/A" );

					AddHtml( 101, 281, 160, 20, MC.ColorText( overrideTextColor, "Stack Amount Between:" ), false, false );
					MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 281, 60, 18 );
					AddLabelCropped( 300, 281, 60, 15, InactiveTextEntryTextColor, "N/A" );

					AddHtml( 371, 281, 40, 20, MC.ColorText( overrideTextColor, "And" ), false, false );
					MC.DisplayBackground( this, InactiveTEBGTypeConfig, 401, 281, 60, 18 );
					AddLabelCropped( 401, 281, 60, 15, InactiveTextEntryTextColor, "N/A" );

					AddHtml( 101, 321, 300, 20, MC.ColorText( defaultTextColor, "Spawn Options:" ), false, false );

					AddHtml( 141, 341, 300, 20, MC.ColorText( overrideTextColor, "Activated" ), false, false );
					AddImage( 100, 340, 211 );

					AddHtml( 141, 401, 300, 20, MC.ColorText( overrideTextColor, "Movable" ), false, false );
					AddImage( 100, 400, 211 );

					AddHtml( 101, 121, 300, 20, MC.ColorText( defaultTextColor, "Spawn Range:" ), false, false );
					AddHtml( 101, 141, 300, 20, MC.ColorText( defaultTextColor, "Walk Range:" ), false, false );
					AddHtml( 101, 161, 300, 20, MC.ColorText( defaultTextColor, "Amount To Spawn:" ), false, false );

					AddHtml( 101, 181, 300, 20, MC.ColorText( defaultTextColor, "Min Delay To Next Spawn:" ), false, false );
					AddHtml( 366, 181, 300, 20, MC.ColorText( defaultTextColor, "Hours" ), false, false );
					AddHtml( 481, 181, 300, 20, MC.ColorText( defaultTextColor, "Minutes" ), false, false );
					AddHtml( 606, 181, 300, 20, MC.ColorText( defaultTextColor, "Seconds" ), false, false );
					AddHtml( 101, 201, 300, 20, MC.ColorText( defaultTextColor, "Max Delay To Next Spawn:" ), false, false );
					AddHtml( 366, 201, 300, 20, MC.ColorText( defaultTextColor, "Hours" ), false, false );
					AddHtml( 481, 201, 300, 20, MC.ColorText( defaultTextColor, "Minutes" ), false, false );
					AddHtml( 606, 201, 300, 20, MC.ColorText( defaultTextColor, "Seconds" ), false, false );

					AddHtml( 101, 221, 300, 20, MC.ColorText( defaultTextColor, "Min Delay To Despawn:" ), false, false );
					AddHtml( 366, 221, 300, 20, MC.ColorText( defaultTextColor, "Hours" ), false, false );
					AddHtml( 481, 221, 300, 20, MC.ColorText( defaultTextColor, "Minutes" ), false, false );
					AddHtml( 606, 221, 300, 20, MC.ColorText( defaultTextColor, "Seconds" ), false, false );
					AddHtml( 101, 241, 300, 20, MC.ColorText( defaultTextColor, "Max Delay To Despawn:" ), false, false );
					AddHtml( 366, 241, 300, 20, MC.ColorText( defaultTextColor, "Hours" ), false, false );
					AddHtml( 481, 241, 300, 20, MC.ColorText( defaultTextColor, "Minutes" ), false, false );
					AddHtml( 606, 241, 300, 20, MC.ColorText( defaultTextColor, "Seconds" ), false, false );

					AddHtml( 141, 361, 300, 20, MC.ColorText( defaultTextColor, "Spawn In Group" ), false, false );
					AddHtml( 141, 381, 300, 20, MC.ColorText( defaultTextColor, "Event Ambush" ), false, false );
					AddHtml( 341, 341, 300, 20, MC.ColorText( defaultTextColor, "Use Despawn" ), false, false );
					AddHtml( 341, 361, 300, 20, MC.ColorText( defaultTextColor, "Despawn In Group" ), false, false );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 121, 60, 18 );
					AddTextEntry( 300, 121, 60, 15, ActiveTextEntryTextColor, 2, spawnRange.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 141, 60, 18 );
					AddTextEntry( 300, 141, 60, 15, ActiveTextEntryTextColor, 3, walkRange.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 161, 60, 18 );
					AddTextEntry( 300, 161, 60, 15, ActiveTextEntryTextColor, 4, amount.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 181, 60, 18 );
					AddTextEntry( 300, 181, 60, 15, ActiveTextEntryTextColor, 5, minDelayHour.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 415, 181, 60, 18 );
					AddTextEntry( 415, 181, 60, 15, ActiveTextEntryTextColor, 6, minDelayMin.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 540, 181, 60, 18 );
					AddTextEntry( 540, 181, 60, 15, ActiveTextEntryTextColor, 7, minDelaySec.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 201, 60, 18 );
					AddTextEntry( 300, 201, 60, 15, ActiveTextEntryTextColor, 8, maxDelayHour.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 415, 201, 60, 18 );
					AddTextEntry( 415, 201, 60, 15, ActiveTextEntryTextColor, 9, maxDelayMin.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 540, 201, 60, 18 );
					AddTextEntry( 540, 201, 60, 15, ActiveTextEntryTextColor, 10, maxDelaySec.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 221, 60, 18 );
					AddTextEntry( 300, 221, 60, 15, ActiveTextEntryTextColor, 11, minDespawnHour.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 415, 221, 60, 18 );
					AddTextEntry( 415, 221, 60, 15, ActiveTextEntryTextColor, 12, minDespawnMin.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 540, 221, 60, 18 );
					AddTextEntry( 540, 221, 60, 15, ActiveTextEntryTextColor, 13, minDespawnSec.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 241, 60, 18 );
					AddTextEntry( 300, 241, 60, 15, ActiveTextEntryTextColor, 14, maxDespawnHour.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 415, 241, 60, 18 );
					AddTextEntry( 415, 241, 60, 15, ActiveTextEntryTextColor, 15, maxDespawnMin.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 540, 241, 60, 18 );
					AddTextEntry( 540, 241, 60, 15, ActiveTextEntryTextColor, 16, maxDespawnSec.ToString() );

					AddCheck( 100, 360, 210, 211, spawnGroupSwitch, 1 );
					AddCheck( 100, 380, 210, 211, eventAmbushSwitch, 2 );
					AddCheck( 300, 340, 210, 211, despawnSwitch, 3 );
					AddCheck( 300, 360, 210, 211, despawnGroupSwitch, 4 );

					break;
				}
				case 4:
				{
					CalcTimeBased();

					AddHtml( 101, 101, 300, 20, MC.ColorText( defaultTextColor, "Spawn Type:" ), false, false );
					AddHtml( 126, 121, 300, 20, MC.ColorText( defaultTextColor, "Regular" ), false, false );
					AddHtml( 126, 141, 300, 20, MC.ColorText( defaultTextColor, "Proximity" ), false, false );
					AddHtml( 126, 161, 300, 20, MC.ColorText( defaultTextColor, "Game Time Based" ), false, false );
					AddHtml( 126, 181, 300, 20, MC.ColorText( defaultTextColor, "Real Time Based" ), false, false );
					AddHtml( 126, 201, 300, 20, MC.ColorText( defaultTextColor, "Speech" ), false, false );
					AddHtml( 241, 201, 300, 20, MC.ColorText( defaultTextColor, "Keyword:" ), false, false );
					AddHtml( 266, 241, 120, 20, MC.ColorText( defaultTextColor, "Case Sensitive" ), false, false );
					AddHtml( 101, 381, 300, 20, MC.ColorText( defaultTextColor, "Event Range:" ), false, false );
					AddHtml( 101, 401, 300, 20, MC.ColorText( defaultTextColor, "All Time Based Will Spawn Between:" ), false, false );
					AddHtml( 451, 401, 300, 20, MC.ColorText( defaultTextColor, "And" ), false, false );
					AddHtml( 391, 381, 300, 20, MC.ColorText( defaultTextColor, "Despawn After Time Expire" ), false, false );

					AddRadio( 100, 120, 9026, 9027, spawnTypeSwitch == SpawnType.Regular, 1 );
					AddRadio( 100, 140, 9026, 9027, spawnTypeSwitch == SpawnType.Proximity, 2 );
					AddRadio( 100, 160, 9026, 9027, spawnTypeSwitch == SpawnType.GameTimeBased, 3 );
					AddRadio( 100, 180, 9026, 9027, spawnTypeSwitch == SpawnType.RealTimeBased, 4 );
					AddRadio( 100, 200, 9026, 9027, spawnTypeSwitch == SpawnType.Speech, 5 );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 320, 201, 320, 38 );
					AddTextEntry( 320, 201, 320, 40, ActiveTextEntryTextColor, 4, keyword );

					AddCheck( 240, 240, 9026, 9027, caseSensitiveSwitch, 6 );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 210, 381, 60, 18 );
					AddTextEntry( 210, 381, 60, 15, ActiveTextEntryTextColor, 1, eventRange.ToString() );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 350, 401, 80, 18 );
					AddTextEntry( 350, 401, 80, 15, ActiveTextEntryTextColor, 2, beginTimeBased );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 490, 401, 80, 18 );
					AddTextEntry( 490, 401, 80, 15, ActiveTextEntryTextColor, 3, endTimeBased );

					AddCheck( 350, 380, 9026, 9027, despawnTimeExpireSwitch, 7 );

					break;
				}
				case 5: // Add Item To Spawn Entry
				{
					string pageInfo = "<center>Add Item To Spawn Entry</center>\nThis will allow you to add a stackable or non-stackable item to a selected spawn entry. You can set the minimum and maximum stack amount for random stack spawning.";
					pageInfo = String.Format( "{0} If the spawn entry selected is a container, this item will spawn inside of the container. If the spawn entry is a mobile, it will spawn inside of the mobile's backpack.", pageInfo );
					pageInfo = String.Format( "{0} You may add as many items you wish to a selected spawn entry.\n\nGo to page 2 to continue.", pageInfo );

					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, pageInfo ), false, true );

					break;
				}
				case 6:
				{
					AddHtml( 101, 101, 300, 20, MC.ColorText( defaultTextColor, "Entry Type:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 101, 190, 18 );
					AddLabelCropped( 300, 101, 190, 15, ActiveTextEntryTextColor, entryName );

					AddHtml( 541, 101, 80, 20, MC.ColorText( defaultTextColor, "Select Entry" ), false, false );
					AddButton( 500, 103, 2118, 2117, -10, GumpButtonType.Reply, 0 );

					AddHtml( 101, 121, 300, 20, MC.ColorText( defaultTextColor, "Item To Add:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 121, 190, 18 );
					AddTextEntry( 300, 121, 190, 15, ActiveTextEntryTextColor, 1, addItem );

					AddHtml( 541, 121, 80, 20, MC.ColorText( defaultTextColor, "Search" ), false, false );
					AddButton( 500, 123, 2118, 2117, -11, GumpButtonType.Reply, 0 );

					AddHtml( 101, 141, 160, 20, MC.ColorText( defaultTextColor, "Stack Amount Between:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 141, 60, 18 );
					AddTextEntry( 300, 141, 60, 15, ActiveTextEntryTextColor, 2, minStackAmount.ToString() );

					AddHtml( 371, 141, 40, 20, MC.ColorText( defaultTextColor, "And" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 401, 141, 60, 18 );
					AddTextEntry( 401, 141, 60, 15, ActiveTextEntryTextColor, 3, maxStackAmount.ToString() );

					AddHtml( 101, 181, 560, 20, MC.ColorText( defaultTextColor, String.Format( "{0} item{1} to spawn inside [Item To Add] if it is a container.", InsideItemList.Count, InsideItemList.Count == 1 ? "" : "s" ) ), false, false );
					AddHtml( 141, 201, 80, 20, MC.ColorText( defaultTextColor, "Edit Items" ), false, false );
					AddButton( 100, 203, 2118, 2117, -12, GumpButtonType.Reply, 0 );

					break;
				}
			}
		}

		private void AddCommandButtons()
		{
			switch ( cpg )
			{
				case 1:{ AddButtonsPageOne(); break; }
			}

			if ( cpg != 1 ) // Next Command Page
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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Override Individual Entries" ), false, false );

			if ( OverrideIndividualEntriesClicked )
				AddButton( 100, 450, 5541, 5542, 1, GumpButtonType.Reply, 0 );
			else
				AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Add Item To Spawn Entry" ), false, false );

			if ( AddItemClicked )
				AddButton( 350, 450, 5541, 5542, 4, GumpButtonType.Reply, 0 );
			else
				AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddStaticButtons();
		}

		private void AddStaticButtons()
		{
			if ( !AddSetting )
			{
				AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Setting" ), false, false );
				AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

				AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Update Setting" ), false, false );
			}
			else
			{
				AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Submit Setting" ), false, false );
			}

			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );
		}

		private void SubmitSetting( RelayInfo info, bool submit, int command )
		{
			int settingNum = 0;

			string oldMessages = Messages;

			if ( submit )
			{
				if ( AddSetting )
					MessagesTitle = "Submit Setting";
				else
					MessagesTitle = "Update Setting";

				Messages = null;
			}

			if ( OverrideIndividualEntriesClicked )
				settingNum = 1;

			if ( AddItemClicked )
				settingNum = 2;

			switch ( settingNum )
			{
				case 0:
				{
					Messages = "You must select a setting before submitting.";

					UpdatePage( command );

					break;
				}
				case 1: // Override Individual Entries
				{
					bool invalid = false;

					bool fSpawnRange=false, fWalkRange=false, fAmount=false, fMinDelayHour=false, fMinDelayMin=false, fMinDelaySec=false, fMaxDelayHour=false, fMaxDelayMin=false, fMaxDelaySec=false, fEventRange=false, fBeginTimeBased=false, fEndTimeBased=false, fMinDespawnHour=false, fMinDespawnMin=false, fMinDespawnSec=false, fMaxDespawnHour=false, fMaxDespawnMin=false, fMaxDespawnSec=false;

					string checkSpawnRange=null, checkWalkRange=null, checkAmount=null, checkMinDelayHour=null, checkMinDelayMin=null, checkMinDelaySec=null, checkMaxDelayHour=null, checkMaxDelayMin=null, checkMaxDelaySec=null, checkEventRange=null, checkBeginTimeBased=null, checkEndTimeBased=null, checkMinDespawnHour=null, checkMinDespawnMin=null, checkMinDespawnSec=null, checkMaxDespawnHour=null, checkMaxDespawnMin=null, checkMaxDespawnSec=null;

					string stringBeginTimeBasedAMPM=null, stringEndTimeBasedAMPM=null;
					int intSpawnRange=0, intWalkRange=0, intAmount=0, intMinDelayHour=0, intMinDelayMin=0, intMinDelaySec=0, intMaxDelayHour=0, intMaxDelayMin=0, intMaxDelaySec=0, intEventRange=0, intBeginTimeBasedHour=0, intBeginTimeBasedMinute=0, intEndTimeBasedHour=0, intEndTimeBasedMinute=0, intMinDespawnHour=0, intMinDespawnMin=0, intMinDespawnSec=0, intMaxDespawnHour=0, intMaxDespawnMin=0, intMaxDespawnSec=0;

					if ( pg == 3 )
					{
						spawnGroupSwitch = info.IsSwitched( 1 );
						eventAmbushSwitch = info.IsSwitched( 2 );
						despawnSwitch = info.IsSwitched( 3 );
						despawnGroupSwitch = info.IsSwitched( 4 );

						checkSpawnRange = info.GetTextEntry( 2 ).Text;
						checkWalkRange = info.GetTextEntry( 3 ).Text;
						checkAmount = info.GetTextEntry( 4 ).Text;
						checkMinDelayHour = info.GetTextEntry( 5 ).Text;
						checkMinDelayMin = info.GetTextEntry( 6 ).Text;
						checkMinDelaySec = info.GetTextEntry( 7 ).Text;
						checkMaxDelayHour = info.GetTextEntry( 8 ).Text;
						checkMaxDelayMin = info.GetTextEntry( 9 ).Text;
						checkMaxDelaySec = info.GetTextEntry( 10 ).Text;
						checkMinDespawnHour = info.GetTextEntry( 11 ).Text;
						checkMinDespawnMin = info.GetTextEntry( 12 ).Text;
						checkMinDespawnSec = info.GetTextEntry( 13 ).Text;
						checkMaxDespawnHour = info.GetTextEntry( 14 ).Text;
						checkMaxDespawnMin = info.GetTextEntry( 15 ).Text;
						checkMaxDespawnSec = info.GetTextEntry( 16 ).Text;
					}
					else
					{
						checkSpawnRange = spawnRange.ToString();
						checkWalkRange = walkRange.ToString();
						checkAmount = amount.ToString();
						checkMinDelayHour = minDelayHour.ToString();
						checkMinDelayMin = minDelayMin.ToString();
						checkMinDelaySec = minDelaySec.ToString();
						checkMaxDelayHour = maxDelayHour.ToString();
						checkMaxDelayMin = maxDelayMin.ToString();
						checkMaxDelaySec = maxDelaySec.ToString();
						checkMinDespawnHour = minDespawnHour.ToString();
						checkMinDespawnMin = minDespawnMin.ToString();
						checkMinDespawnSec = minDespawnSec.ToString();
						checkMaxDespawnHour = maxDespawnHour.ToString();
						checkMaxDespawnMin = maxDespawnMin.ToString();
						checkMaxDespawnSec = maxDespawnSec.ToString();
					}

					if ( pg == 4 )
					{
						int switchNum = MC.GetSwitchNum( info, 1, 5 );
						spawnTypeSwitch = (SpawnType) switchNum;

						caseSensitiveSwitch = info.IsSwitched( 6 );
						despawnTimeExpireSwitch = info.IsSwitched( 7 );

						checkEventRange = info.GetTextEntry( 1 ).Text;
						checkBeginTimeBased = info.GetTextEntry( 2 ).Text;
						checkEndTimeBased = info.GetTextEntry( 3 ).Text;
						keyword = info.GetTextEntry( 4 ).Text;
					}
					else
					{
						CalcTimeBased();

						checkEventRange = eventRange.ToString();
						checkBeginTimeBased = beginTimeBased.ToString();
						checkEndTimeBased = endTimeBased.ToString();
					}

// *************** First Check ***************

					try{ intSpawnRange = Convert.ToInt32( checkSpawnRange ); }
					catch
					{
						invalid = true;
						bSpawnRange = false;
						fSpawnRange = true;
						Messages = String.Format( "{0}Invalid input for [Spawn Range]. You must specify an integer only.\n", Messages );
					}

					try{ intWalkRange = Convert.ToInt32( checkWalkRange ); }
					catch
					{
						invalid = true;
						bWalkRange = false;
						fWalkRange = true;
						Messages = String.Format( "{0}Invalid input for [Walk Range]. You must specify an integer only.\n", Messages );
					}

					try{ intAmount = Convert.ToInt32( checkAmount ); }
					catch
					{
						invalid = true;
						bAmount = false;
						fAmount = true;
						Messages = String.Format( "{0}Invalid input for [Amount To Spawn]. You must specify an integer only.\n", Messages );
					}

					try{ intMinDelayHour = Convert.ToInt32( checkMinDelayHour ); }
					catch
					{
						invalid = true;
						bMinDelayHour = false;
						fMinDelayHour = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn] [Hours]. You must specify an integer only.\n", Messages );
					}

					try{ intMinDelayMin = Convert.ToInt32( checkMinDelayMin ); }
					catch
					{
						invalid = true;
						bMinDelayMin = false;
						fMinDelayMin = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn] [Minutes]. You must specify an integer only.\n", Messages );
					}

					try{ intMinDelaySec = Convert.ToInt32( checkMinDelaySec ); }
					catch
					{
						invalid = true;
						bMinDelaySec = false;
						fMinDelaySec = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn] [Seconds]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxDelayHour = Convert.ToInt32( checkMaxDelayHour ); }
					catch
					{
						invalid = true;
						bMaxDelayHour = false;
						fMaxDelayHour = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Next Spawn] [Hours]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxDelayMin = Convert.ToInt32( checkMaxDelayMin ); }
					catch
					{
						invalid = true;
						bMaxDelayMin = false;
						fMaxDelayMin = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Next Spawn] [Minutes]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxDelaySec = Convert.ToInt32( checkMaxDelaySec ); }
					catch
					{
						invalid = true;
						bMaxDelaySec = false;
						fMaxDelaySec = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Next Spawn] [Seconds]. You must specify an integer only.\n", Messages );
					}

					try{ intEventRange = Convert.ToInt32( checkEventRange ); }
					catch
					{
						invalid = true;
						bEventRange = false;
						fEventRange = true;
						Messages = String.Format( "{0}Invalid input for [Event Range]. You must specify an integer only.\n", Messages );
					}

					string[] begin = checkBeginTimeBased.Split(':');
					string[] end = checkEndTimeBased.Split(':');
					string[] beginAMPM = null;
					string[] endAMPM = null;

					if ( begin.Length != 2 )
					{
						invalid = true;
						bBeginTimeBased = false;
						fBeginTimeBased = true;
						Messages = String.Format( "{0}Invalid format for [Begin Time Based]. Example of the correct format is: 12:00 PM.\n", Messages );
					}
					else
					{
						beginAMPM = begin[1].Split(' ');
					}

					if ( end.Length != 2 )
					{
						invalid = true;
						bEndTimeBased = false;
						fEndTimeBased = true;
						Messages = String.Format( "{0}Invalid format for [End Time Based]. Example of the correct format is: 12:00 PM.\n", Messages );
					}
					else
					{
						endAMPM = end[1].Split(' ');
					}

					try{ intBeginTimeBasedHour = Convert.ToInt32( begin[0] ); }
					catch
					{
						if ( !fBeginTimeBased )
						{
							invalid = true;
							bBeginTimeBased = false;
							fBeginTimeBased = true;
							Messages = String.Format( "{0}Invalid input for [Begin Time Based] [Hour]. You must specify an integer only.\n", Messages );
						}
					}

					try{ intBeginTimeBasedMinute = Convert.ToInt32( beginAMPM[0] ); }
					catch
					{
						if ( !fBeginTimeBased )
						{
							invalid = true;
							bBeginTimeBased = false;
							fBeginTimeBased = true;
							Messages = String.Format( "{0}Invalid input for [Begin Time Based] [Minute]. You must specify an integer only.\n", Messages );
						}
					}

					try{ intEndTimeBasedHour = Convert.ToInt32( end[0] ); }
					catch
					{
						if ( !fEndTimeBased )
						{
							invalid = true;
							bEndTimeBased = false;
							fEndTimeBased = true;
							Messages = String.Format( "{0}Invalid input for [End Time Based] [Hour]. You must specify an integer only.\n", Messages );
						}
					}

					try{ intEndTimeBasedMinute = Convert.ToInt32( endAMPM[0] ); }
					catch
					{
						if ( !fEndTimeBased )
						{
							invalid = true;
							bEndTimeBased = false;
							fEndTimeBased = true;
							Messages = String.Format( "{0}Invalid input for [End Time Based] [Minute]. You must specify an integer only.\n", Messages );
						}
					}

					if ( ( beginAMPM == null || beginAMPM.Length != 2 ) && !fBeginTimeBased )
					{
						invalid = true;
						bBeginTimeBased = false;
						fBeginTimeBased = true;
						Messages = String.Format( "{0}Invalid format for [Begin Time Based]. Example of the correct format is: 12:00 PM.\n", Messages );
					}
					else
						stringBeginTimeBasedAMPM = Convert.ToString( beginAMPM[1] ).ToUpper();

					if ( ( endAMPM == null || endAMPM.Length != 2 ) && !fBeginTimeBased )
					{
						invalid = true;
						bEndTimeBased = false;
						fEndTimeBased = true;
						Messages = String.Format( "{0}Invalid format for [End Time Based]. Example of the correct format is: 12:00 PM.\n", Messages );
					}
					else
						stringEndTimeBasedAMPM = Convert.ToString( endAMPM[1] ).ToUpper();

					try{ intMinDespawnHour = Convert.ToInt32( checkMinDespawnHour ); }
					catch
					{
						invalid = true;
						bMinDespawnHour = false;
						fMinDespawnHour = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn] [Hours]. You must specify an integer only.\n", Messages );
					}

					try{ intMinDespawnMin = Convert.ToInt32( checkMinDespawnMin ); }
					catch
					{
						invalid = true;
						bMinDespawnMin = false;
						fMinDespawnMin = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn] [Minutes]. You must specify an integer only.\n", Messages );
					}

					try{ intMinDespawnSec = Convert.ToInt32( checkMinDespawnSec ); }
					catch
					{
						invalid = true;
						bMinDespawnSec = false;
						fMinDespawnSec = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn] [Seconds]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxDespawnHour = Convert.ToInt32( checkMaxDespawnHour ); }
					catch
					{
						invalid = true;
						bMaxDespawnHour = false;
						fMaxDespawnHour = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Despawn] [Hours]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxDespawnMin = Convert.ToInt32( checkMaxDespawnMin ); }
					catch
					{
						invalid = true;
						bMaxDespawnMin = false;
						fMaxDespawnMin = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Despawn] [Minutes]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxDespawnSec = Convert.ToInt32( checkMaxDespawnSec ); }
					catch
					{
						invalid = true;
						bMaxDespawnSec = false;
						fMaxDespawnSec = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Despawn] [Seconds]. You must specify an integer only.\n", Messages );
					}

// *************** Second Check ***************

					if ( intSpawnRange < 0 && !fSpawnRange )
					{
						invalid = true;
						bSpawnRange = false;
						fSpawnRange = true;
						Messages = String.Format( "{0}Invalid input for [Spawn Range]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( intWalkRange < 0 && !fWalkRange )
					{
						invalid = true;
						bWalkRange = false;
						fWalkRange = true;
						Messages = String.Format( "{0}Invalid input for [Walk Range]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( intAmount <= 0 && !fAmount )
					{
						invalid = true;
						bAmount = false;
						fAmount = true;
						Messages = String.Format( "{0}Invalid input for [Amount]. Must be greater than 0.\n", Messages );
					}

					if ( intMinDelayHour < 0  && !fMinDelayHour )
					{
						invalid = true;
						bMinDelayHour = false;
						fMinDelayHour = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn] [Hours]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( ( intMinDelayMin < 0 || intMinDelayMin > 59 ) && !fMinDelayMin )
					{
						invalid = true;
						bMinDelayMin = false;
						fMinDelayMin = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn] [Minutes]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					if ( ( intMinDelaySec < 0 || intMinDelaySec > 59 )  && !fMinDelaySec )
					{
						invalid = true;
						bMinDelaySec = false;
						fMinDelaySec = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn] [Seconds]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					if ( intMaxDelayHour < 0 && !fMaxDelayHour )
					{
						invalid = true;
						bMaxDelayHour = false;
						fMaxDelayHour = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Next Spawn] [Hours]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( ( intMaxDelayMin < 0 || intMaxDelayMin > 59 ) && !fMaxDelayMin )
					{
						invalid = true;
						bMaxDelayMin = false;
						fMaxDelayMin = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Next Spawn] [Minutes]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					if ( ( intMaxDelaySec < 0 || intMaxDelaySec > 59 ) && !fMaxDelaySec )
					{
						invalid = true;
						bMaxDelaySec = false;
						fMaxDelaySec = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Next Spawn] [Seconds]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					int calcMinDelay=0, calcMaxDelay=0, calcBeginTimeBased=0, calcEndTimeBased=0;

					if ( !fMinDelayHour && !fMinDelayMin && !fMinDelaySec && !fMaxDelayHour && !fMaxDelayMin && !fMaxDelaySec )
					{
						calcMinDelay = intMinDelaySec + ( intMinDelayMin * 60 ) + ( intMinDelayHour * 3600 );
						calcMaxDelay = intMaxDelaySec + ( intMaxDelayMin * 60 ) + ( intMaxDelayHour * 3600 );
					}

					if ( calcMinDelay > calcMaxDelay )
					{
						invalid = true;
						bMinDelayHour = false;
						bMinDelayMin = false;
						bMinDelaySec = false;
						bMaxDelayHour = false;
						bMaxDelayMin = false;
						bMaxDelaySec = false;
						fMinDelayHour = true;
						fMinDelayMin = true;
						fMinDelaySec = true;
						fMaxDelayHour = true;
						fMaxDelayMin = true;
						fMaxDelaySec = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Next Spawn]. Must be less than or equal to [Max Delay To Next Spawn].\n", Messages );
					}

					if ( intEventRange < 0 && !fEventRange )
					{
						invalid = true;
						bEventRange = false;
						fEventRange = true;
						Messages = String.Format( "{0}Invalid input for [Event Range]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( ( intBeginTimeBasedHour > 12 || intBeginTimeBasedHour < 1 ) && !fBeginTimeBased )
					{
						invalid = true;
						bBeginTimeBased = false;
						fBeginTimeBased = true;
						Messages = String.Format( "{0}Invalid input for [Begin Time Based] [Hour]. Must be between 1 and 12.\n", Messages );
					}

					if ( ( intBeginTimeBasedMinute > 59 || intBeginTimeBasedMinute < 0 ) && !fBeginTimeBased )
					{
						invalid = true;
						bBeginTimeBased = false;
						fBeginTimeBased = true;
						Messages = String.Format( "{0}Invalid input for [Begin Time Based] [Minute]. Must be between 00 and 59.\n", Messages );
					}

					if ( ( stringBeginTimeBasedAMPM != "AM" && stringBeginTimeBasedAMPM != "PM" ) && !fBeginTimeBased )
					{
						invalid = true;
						bBeginTimeBased = false;
						fBeginTimeBased = true;
						Messages = String.Format( "{0}Invalid input for [Begin Time Based] [AM/PM]. Must be AM or PM only.\n", Messages );
					}

					if ( ( intEndTimeBasedHour > 12 || intEndTimeBasedHour < 1 ) && !fEndTimeBased )
					{
						invalid = true;
						bEndTimeBased = false;
						fEndTimeBased = true;
						Messages = String.Format( "{0}Invalid input for [End Time Based] [Hour]. Must be between 1 and 12.\n", Messages );
					}

					if ( ( intEndTimeBasedMinute > 59 || intEndTimeBasedMinute < 0 ) && !fEndTimeBased )
					{
						invalid = true;
						bEndTimeBased = false;
						fEndTimeBased = true;
						Messages = String.Format( "{0}Invalid input for [End Time Based] [Minute]. Must be between 00 and 59.\n", Messages );
					}

					if ( ( stringEndTimeBasedAMPM != "AM" && stringEndTimeBasedAMPM != "PM" ) && !fEndTimeBased )
					{
						invalid = true;
						bEndTimeBased = false;
						fEndTimeBased = true;
						Messages = String.Format( "{0}Invalid input for [End Time Based] [AM/PM]. Must be AM or PM only.\n", Messages );
					}

					if ( !fBeginTimeBased )
					{
						int hourCheck = intBeginTimeBasedHour;

						if ( stringBeginTimeBasedAMPM == "PM" && hourCheck < 12 )
							hourCheck += 12;

						if ( stringBeginTimeBasedAMPM == "AM" && hourCheck == 12 )
							hourCheck = 0;

						calcBeginTimeBased = ( hourCheck * 60 ) + intBeginTimeBasedMinute;
					}

					if ( !fEndTimeBased )
					{
						int hourCheck = intEndTimeBasedHour;

						if ( stringEndTimeBasedAMPM == "PM" && hourCheck < 12 )
							hourCheck += 12;

						if ( stringEndTimeBasedAMPM == "AM" && hourCheck == 12 )
							hourCheck = 0;

						calcEndTimeBased = ( hourCheck * 60 ) + intEndTimeBasedMinute;
					}

					if ( intMinDespawnHour < 0  && !fMinDespawnHour )
					{
						invalid = true;
						bMinDespawnHour = false;
						fMinDespawnHour = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn] [Hours]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( ( intMinDespawnMin < 0 || intMinDespawnMin > 59 ) && !fMinDespawnMin )
					{
						invalid = true;
						bMinDespawnMin = false;
						fMinDespawnMin = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn] [Minutes]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					if ( ( intMinDespawnSec < 0 || intMinDespawnSec > 59 )  && !fMinDespawnSec )
					{
						invalid = true;
						bMinDespawnSec = false;
						fMinDespawnSec = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn] [Seconds]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					if ( intMaxDespawnHour < 0 && !fMaxDespawnHour )
					{
						invalid = true;
						bMaxDespawnHour = false;
						fMaxDespawnHour = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Despawn] [Hours]. Must be greater than or equal to 0.\n", Messages );
					}

					if ( ( intMaxDespawnMin < 0 || intMaxDespawnMin > 59 ) && !fMaxDespawnMin )
					{
						invalid = true;
						bMaxDespawnMin = false;
						fMaxDespawnMin = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Despawn] [Minutes]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					if ( ( intMaxDespawnSec < 0 || intMaxDespawnSec > 59 ) && !fMaxDespawnSec )
					{
						invalid = true;
						bMaxDespawnSec = false;
						fMaxDespawnSec = true;
						Messages = String.Format( "{0}Invalid input for [Max Delay To Despawn] [Seconds]. Must be greater than or equal to 0 and less than 60.\n", Messages );
					}

					int calcMinDespawn=0, calcMaxDespawn=0;

					if ( !fMinDespawnHour && !fMinDespawnMin && !fMinDespawnSec && !fMaxDespawnHour && !fMaxDespawnMin && !fMaxDespawnSec )
					{
						calcMinDespawn = intMinDespawnSec + ( intMinDespawnMin * 60 ) + ( intMinDespawnHour * 3600 );
						calcMaxDespawn = intMaxDespawnSec + ( intMaxDespawnMin * 60 ) + ( intMaxDespawnHour * 3600 );
					}

					if ( calcMinDespawn > calcMaxDespawn )
					{
						invalid = true;
						bMinDespawnHour = false;
						bMinDespawnMin = false;
						bMinDespawnSec = false;
						bMaxDespawnHour = false;
						bMaxDespawnMin = false;
						bMaxDespawnSec = false;
						fMinDespawnHour = true;
						fMinDespawnMin = true;
						fMinDespawnSec = true;
						fMaxDespawnHour = true;
						fMaxDespawnMin = true;
						fMaxDespawnSec = true;
						Messages = String.Format( "{0}Invalid input for [Min Delay To Despawn]. Must be less than or equal to [Max Delay To Despawn].\n", Messages );
					}

// *************** Final Check ***************

					if ( invalid )
					{
						if ( !fSpawnRange )
						{
							bSpawnRange = true;
							spawnRange = intSpawnRange;
						}

						if ( !fWalkRange )
						{
							bWalkRange = true;
							walkRange = intWalkRange;
						}

						if ( !fAmount )
						{
							bAmount = true;
							amount = intAmount;
						}

						if ( !fMinDelayHour )
						{
							bMinDelayHour = true;
							minDelayHour = intMinDelayHour;
						}

						if ( !fMinDelayMin )
						{
							bMinDelayMin = true;
							minDelayMin = intMinDelayMin;
						}

						if ( !fMinDelaySec )
						{
							bMinDelaySec = true;
							minDelaySec = intMinDelaySec;
						}

						if ( !fMaxDelayHour )
						{
							bMaxDelayHour = true;
							maxDelayHour = intMaxDelayHour;
						}

						if ( !fMaxDelayMin )
						{
							bMaxDelayMin = true;
							maxDelayMin = intMaxDelayMin;
						}

						if ( !fMaxDelaySec )
						{
							bMaxDelaySec = true;
							maxDelaySec = intMaxDelaySec;
						}

						if ( !fEventRange )
						{
							bEventRange = true;
							eventRange = intEventRange;
						}

						if ( !fBeginTimeBased )
						{
							bBeginTimeBased = true;

							if ( stringBeginTimeBasedAMPM == "PM" && intBeginTimeBasedHour < 12 )
								intBeginTimeBasedHour += 12;

							if ( stringBeginTimeBasedAMPM == "AM" && intBeginTimeBasedHour == 12 )
								intBeginTimeBasedHour = 0;

							beginTimeBasedHour = intBeginTimeBasedHour;
							beginTimeBasedMinute = intBeginTimeBasedMinute;
						}

						if ( !fEndTimeBased )
						{
							bEndTimeBased = true;

							if ( stringEndTimeBasedAMPM == "PM" && intEndTimeBasedHour < 12 )
								intEndTimeBasedHour += 12;

							if ( stringEndTimeBasedAMPM == "AM" && intEndTimeBasedHour == 12 )
								intEndTimeBasedHour = 0;

							endTimeBasedHour = intEndTimeBasedHour;
							endTimeBasedMinute = intEndTimeBasedMinute;
						}

						if ( !fMinDespawnHour )
						{
							bMinDespawnHour = true;
							minDespawnHour = intMinDespawnHour;
						}

						if ( !fMinDespawnMin )
						{
							bMinDespawnMin = true;
							minDespawnMin = intMinDespawnMin;
						}

						if ( !fMinDespawnSec )
						{
							bMinDespawnSec = true;
							minDespawnSec = intMinDespawnSec;
						}

						if ( !fMaxDespawnHour )
						{
							bMaxDespawnHour = true;
							maxDespawnHour = intMaxDespawnHour;
						}

						if ( !fMaxDespawnMin )
						{
							bMaxDespawnMin = true;
							maxDespawnMin = intMaxDespawnMin;
						}
	
						if ( !fMaxDespawnSec )
						{
							bMaxDespawnSec = true;
							maxDespawnSec = intMaxDespawnSec;
						}

						if ( !submit )
							Messages = oldMessages;

						UpdatePage( command );

						return;
					}

// *************** Applying Settings ***************

					spawnRange = intSpawnRange;
					walkRange = intWalkRange;
					amount = intAmount;
					eventRange = intEventRange;

					if ( stringBeginTimeBasedAMPM == "PM" && intBeginTimeBasedHour < 12 )
						intBeginTimeBasedHour += 12;

					if ( stringBeginTimeBasedAMPM == "AM" && intBeginTimeBasedHour == 12 )
						intBeginTimeBasedHour = 0;

					beginTimeBasedHour = intBeginTimeBasedHour;
					beginTimeBasedMinute = intBeginTimeBasedMinute;

					if ( stringEndTimeBasedAMPM == "PM" && intEndTimeBasedHour < 12 )
						intEndTimeBasedHour += 12;

					if ( stringEndTimeBasedAMPM == "AM" && intEndTimeBasedHour == 12 )
						intEndTimeBasedHour = 0;

					endTimeBasedHour = intEndTimeBasedHour;
					endTimeBasedMinute = intEndTimeBasedMinute;

					ArrayList settingList = new ArrayList();

					settingList.Add( Setting.OverrideIndividualEntries );
					settingList.Add( spawnRange );
					settingList.Add( walkRange );
					settingList.Add( amount );
					settingList.Add( calcMinDelay );
					settingList.Add( calcMaxDelay );
					settingList.Add( spawnGroupSwitch );
					settingList.Add( eventAmbushSwitch );
					settingList.Add( spawnTypeSwitch );
					settingList.Add( keyword );
					settingList.Add( caseSensitiveSwitch );
					settingList.Add( eventRange );
					settingList.Add( beginTimeBasedHour );
					settingList.Add( beginTimeBasedMinute );
					settingList.Add( endTimeBasedHour );
					settingList.Add( endTimeBasedMinute );
					settingList.Add( calcMinDespawn );
					settingList.Add( calcMaxDespawn );
					settingList.Add( despawnSwitch );
					settingList.Add( despawnGroupSwitch );
					settingList.Add( despawnTimeExpireSwitch );

					if ( AddSetting && submit )
					{
						megaSpawner.SettingsList.Add( settingList );

						Messages = "Setting has been added.";

						SettingsCheckBoxesList.Add( (bool) false );
					}
					else if ( submit )
					{
						megaSpawner.SettingsList[index] = settingList;

						Messages = "Setting has been updated.";
					}

					if ( submit )
					{
						megaSpawner.DeleteEntries();

						MegaSpawnerOverride.RemoveRespawnEntries( megaSpawner );

						for ( int i = 0; i < amount; i++ )
						{
							megaSpawner.OverrideRespawnEntryList.Add( "" );
							megaSpawner.OverrideRespawnTimeList.Add( 0 );
							megaSpawner.OverrideSpawnCounterList.Add( DateTime.Now );
							megaSpawner.OverrideSpawnTimeList.Add( 0 );
							megaSpawner.OverrideDespawnTimeList.Add( 0 );
						}

						megaSpawner.OverrideSpawnedEntries.Clear();
						megaSpawner.OverrideLastMovedList.Clear();

						megaSpawner.CompileSettings();
						megaSpawner.Respawn();

						MegaSpawnerOverride.CheckDupedEntries( megaSpawner );

						ArgsList[30] = 0;				// FromWhere

						SetArgsList();

						gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );

						break;
					}
					else
					{
						minDelayHour = intMinDelayHour;
						minDelayMin = intMinDelayMin;
						minDelaySec = intMinDelaySec;
						maxDelayHour = intMaxDelayHour;
						maxDelayMin = intMaxDelayMin;
						maxDelaySec = intMaxDelaySec;

						minDespawnHour = intMinDespawnHour;
						minDespawnMin = intMinDespawnMin;
						minDespawnSec = intMinDespawnSec;
						maxDespawnHour = intMaxDespawnHour;
						maxDespawnMin = intMaxDespawnMin;
						maxDespawnSec = intMaxDespawnSec;

						if ( stringBeginTimeBasedAMPM == "PM" && intBeginTimeBasedHour < 12 )
							intBeginTimeBasedHour += 12;

						if ( stringBeginTimeBasedAMPM == "AM" && intBeginTimeBasedHour == 12 )
							intBeginTimeBasedHour = 0;

						beginTimeBasedHour = intBeginTimeBasedHour;
						beginTimeBasedMinute = intBeginTimeBasedMinute;

						if ( stringEndTimeBasedAMPM == "PM" && intEndTimeBasedHour < 12 )
							intEndTimeBasedHour += 12;

						if ( stringEndTimeBasedAMPM == "AM" && intEndTimeBasedHour == 12 )
							intEndTimeBasedHour = 0;

						endTimeBasedHour = intEndTimeBasedHour;
						endTimeBasedMinute = intEndTimeBasedMinute;

						Messages = oldMessages;

						UpdatePage( command );
					}

					break;
				}
				case 2: // Add Item To Spawn Entry
				{
					bool invalid = false;

					bool fAddItem=false, fMinStackAmount=false, fMaxStackAmount=false;

					string checkAddItem=null, checkMinStackAmount=null, checkMaxStackAmount=null;

					int intMinStackAmount=0, intMaxStackAmount=0;
					string stringAddItem=null;

					TextRelay textInput;

					if ( pg == 6 )
					{
						textInput = info.GetTextEntry( 1 );
						checkAddItem = Convert.ToString( textInput.Text );

						textInput = info.GetTextEntry( 2 );
						checkMinStackAmount = Convert.ToString( textInput.Text );

						textInput = info.GetTextEntry( 3 );
						checkMaxStackAmount = Convert.ToString( textInput.Text );
					}
					else
					{
						checkMinStackAmount = minStackAmount.ToString();
						checkMaxStackAmount = maxStackAmount.ToString();
					}

// *************** First Check ***************

					if ( entryName == "" )
					{
						invalid = true;
						Messages = String.Format( "{0}Invalid input for [Entry Name]. You must choose an entry to add the item to.\n", Messages );
					}

					if ( pg == 6 )
					{
						Type type = ScriptCompiler.FindTypeByName( checkAddItem );

						if ( type != null )
							stringAddItem = type.Name;

						if ( type == null || MC.IsBadEntryType( checkAddItem ) || MC.IsMobile( checkAddItem ) )
						{
							invalid = true;
							bAddItem = false;
							fAddItem = true;
							Messages = String.Format( "{0}Invalid input for [Item To Add]. You must specify a valid type. Type must be an item only. Specifying a mobile will result in an error.\n", Messages );
						}
					}

					try{ intMinStackAmount = Convert.ToInt32( checkMinStackAmount ); }
					catch
					{
						invalid = true;
						bMinStackAmount = false;
						fMinStackAmount = true;
						Messages = String.Format( "{0}Invalid input for [Stack Amount] [Minimum Amount]. You must specify an integer only.\n", Messages );
					}

					try{ intMaxStackAmount = Convert.ToInt32( checkMaxStackAmount ); }
					catch
					{
						invalid = true;
						bMaxStackAmount = false;
						fMaxStackAmount = true;
						Messages = String.Format( "{0}Invalid input for [Stack Amount] [Maximum Amount]. You must specify an integer only.\n", Messages );
					}

// *************** Second Check ***************

					if ( intMinStackAmount < 1 && !fMinStackAmount )
					{
						invalid = true;
						bMinStackAmount = false;
						fMinStackAmount = true;
						Messages = String.Format( "{0}Invalid input for [Stack Amount] [Minimum Amount]. Must be greater than 0.\n", Messages );
					}

					if ( intMaxStackAmount < 1 && !fMaxStackAmount )
					{
						invalid = true;
						bMaxStackAmount = false;
						fMaxStackAmount = true;
						Messages = String.Format( "{0}Invalid input for [Stack Amount] [Maximum Amount]. Must be greater than 0.\n", Messages );
					}

					if ( intMinStackAmount > intMaxStackAmount && !fMinStackAmount && !fMaxStackAmount )
					{
						invalid = true;
						bMinStackAmount = false;
						fMinStackAmount = true;
						bMaxStackAmount = false;
						fMaxStackAmount = true;
						Messages = String.Format( "{0}Invalid input for [Stack Amount]. [Minimum Amount] must be less than or equal to [Maximum Amount].\n", Messages );
					}

// *************** Final Check ***************

					if ( invalid )
					{
						if ( !fAddItem || command == 7 )
						{
							bAddItem = true;
							addItem = checkAddItem;
						}

						if ( !fMinStackAmount )
						{
							bMinStackAmount = true;
							minStackAmount = intMinStackAmount;
						}

						if ( !fMaxStackAmount )
						{
							bMaxStackAmount = true;
							maxStackAmount = intMaxStackAmount;
						}

						if ( !submit )
							Messages = oldMessages;

						UpdatePage( command );

						return;
					}

// *************** Applying Settings ***************

					if ( stringAddItem != null )
						addItem = stringAddItem;
					else
						addItem = checkAddItem;

					minStackAmount = intMinStackAmount;
					maxStackAmount = intMaxStackAmount;

					bool notContainer = false;

					ArrayList settingList = new ArrayList();

					settingList.Add( Setting.AddItem );
					settingList.Add( entryName );
					settingList.Add( entryIndex );
					settingList.Add( addItem );
					settingList.Add( minStackAmount );
					settingList.Add( maxStackAmount );

					if ( MC.IsContainer( addItem ) )
						settingList[0] = Setting.AddContainer;
					else
						settingList[0] = Setting.AddItem;

					if ( !MC.IsContainer( addItem ) && InsideItemList.Count > 0 )
					{
						notContainer = true;
					}
					else
					{
						for ( int i = 0; i < InsideItemList.Count; i++ )
						{
							ArrayList List = (ArrayList) InsideItemList[i];

							settingList.Add( List );
						}
					}

					if ( AddSetting && submit )
					{
						megaSpawner.SettingsList.Add( settingList );
						megaSpawner.CompileSettings();

						Messages = String.Format( "Setting has been added.{0}", notContainer ? " The item being added is not a container, therefore, the items list was cleared." : "" );

						SettingsCheckBoxesList.Add( (bool) false );
					}
					else if ( submit )
					{
						megaSpawner.SettingsList[index] = settingList;
						megaSpawner.CompileSettings();

						Messages = String.Format( "Setting has been updated.{0}", notContainer ? " The item being added is not a container, therefore, the items list was cleared." : "" );
					}

					if ( submit )
					{
						megaSpawner.SettingsList.Sort( new MC.SettingsSorter() );

						if ( !MC.IsContainer( addItem ) )
							InsideItemList.Clear();

						ArgsList[30] = 0;				// FromWhere

						SetArgsList();

						gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );

						break;
					}
					else
					{
						Messages = oldMessages;

						UpdatePage( command );
					}

					break;
				}
			}
		}
	}
}