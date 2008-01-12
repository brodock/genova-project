using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class EditSpawnGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList MSEGCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private ArrayList ESGArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList SEGArgsList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig, InactiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor, FlagTextColor;

		private bool AddToSpawner;
		private int index;
		private SpawnType spawnTypeSwitch;
		private bool activatedSwitch, spawnGroupSwitch, eventAmbushSwitch, caseSensitiveSwitch, movableSwitch, despawnSwitch, despawnGroupSwitch, despawnTimeExpireSwitch;
		private string entryType, oldEntryType, keyword;
		private int spawnRange, walkRange, amount, minDelayHour, minDelayMin, minDelaySec, maxDelayHour, maxDelayMin, maxDelaySec, eventRange, beginTimeBasedHour, beginTimeBasedMinute, endTimeBasedHour, endTimeBasedMinute, minStackAmount, maxStackAmount, minDespawnHour, minDespawnMin, minDespawnSec, maxDespawnHour, maxDespawnMin, maxDespawnSec;
		private bool bEntryType, bSpawnRange, bWalkRange, bAmount, bMinDelayHour, bMinDelayMin, bMinDelaySec, bMaxDelayHour, bMaxDelayMin, bMaxDelaySec, bEventRange, bBeginTimeBased, bEndTimeBased, bMinStackAmount, bMaxStackAmount, bMinDespawnHour, bMinDespawnMin, bMinDespawnSec, bMaxDespawnHour, bMaxDespawnMin, bMaxDespawnSec;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor, flagTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor, InactiveTextEntryTextColor;

		private string beginTimeBased, endTimeBased, beginAMPM, endAMPM;

		private const int totalPages = 2;
		private RelayInfo relayInfo;
		private Mobile gumpMobile;

		public EditSpawnGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

			MC.DisplayImage( this, StyleTypeConfig );

			AddButton( 630, 490, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			if ( AddToSpawner )
			{
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "Add Entry {0}", megaSpawner.OverrideIndividualEntries ? "(Overridden)" : "" ) ), false, false );
			}
			else
			{
				MC.DisplayStyle( this, StyleTypeConfig, 110, 60, 540, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 111, 61, 538, 18 );

				AddHtml( 121, 61, 540, 20, MC.ColorText( titleTextColor, String.Format( "\"{0}\" {1}", entryType, CheckBadEntry() ? "(Bad Entry)" : "" ) ), false, false );
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "Editting Entry {0}", megaSpawner.OverrideIndividualEntries ? "(Overridden)" : "" ) ), false, false );
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
					Messages = "This is where you enter an entry type to add to the spawner.\n\nFor Example:\nYou type in: orc\nThis will add an orc entry to the spawner for spawning orcs.";

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
					Messages = "This is where you specify the keyword for a speech event trigger. This keyword is case sensitive only if the [Case Sensitive] checkbox is checked.";

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
				case -10: // Search
				{
					if ( Help )
					{
						MessagesTitle = "Help: Search Button";
						Messages = "That button will search for all entry types that match the text you typed in for [Entry Type]. This will allow you to type in a partial name and search for any existing entry types that match your search criteria. If nothing is typed in for [Entry Type], then a list of all existing constructable types will be shown.";

						SubmitEntry( info, false, 0 );

						break;
					}

					PageInfoList[22] = 1;					// Search Entry Gump Command Page
					PageInfoList[23] = 1;					// Search Entry Gump Page

					ArgsList[30] = FromGump.EditSpawn;			// FromWhere

					SEGArgsList[0] = SearchType.Any;			// searchType

					SubmitEntry( info, false, 6 );

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

						SubmitEntry( info, false, 0 );

						break;
					}

					DisplayMessages = !DisplayMessages;

					SubmitEntry( info, false, 0 );

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window and also change the page number if specified in the upper right hand corner. To change the page number, delete the current page number and type in the page you want to go to.";

						SubmitEntry( info, false, 0 );

						break;
					}

					relayInfo = info;

					SubmitEntry( info, false, 5 );

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						SubmitEntry( info, false, 0 );

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					SubmitEntry( info, false, 0 );

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

					SubmitEntry( info, false, 0 );

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous page.";

						SubmitEntry( info, false, 0 );

						break;
					}

					SubmitEntry( info, false, 1 );

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next page.";

						SubmitEntry( info, false, 0 );

						break;
					}

					SubmitEntry( info, false, 2 );

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						SubmitEntry( info, false, 0 );

						break;
					}

					SubmitEntry( info, false, 3 );

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						SubmitEntry( info, false, 0 );

						break;
					}

					SubmitEntry( info, false, 4 );

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						SubmitEntry( info, false, 0 );

						break;
					}

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
				case 2: // Delete Entry
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Entry Button";
						Messages = "That button will delete the entry from the Mega Spawner.";

						SubmitEntry( info, false, 0 );

						break;
					}

					if ( CheckProcess() )
						break;

					gumpMobile.SendGump( new ConfirmDeleteEntryGump( gumpMobile, ArgsList ) );

					break;
				}
				case 3: // Submit/Update Entry
				{
					if ( Help )
					{
						if ( AddToSpawner )
						{
							MessagesTitle = "Help: Submit Entry Button";
							Messages = "That button will add the entry to the Mega Spawner.";
						}
						else
						{
							MessagesTitle = "Help: Update Entry Button";
							Messages = "That button will update changes to the entry.";
						}

						SubmitEntry( info, false, 0 );

						break;
					}

					if ( CheckProcess() )
						SubmitEntry( info, false, 7 );

					SubmitEntry( info, true, 0 );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new EditSpawnGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[4] = cpg;
			PageInfoList[5] = pg;

			ESGArgsList[0] = AddToSpawner;
			ESGArgsList[1] = index;
			ESGArgsList[2] = activatedSwitch;
			ESGArgsList[3] = spawnGroupSwitch;
			ESGArgsList[4] = eventAmbushSwitch;
			ESGArgsList[5] = spawnTypeSwitch;
			ESGArgsList[6] = entryType;
			ESGArgsList[7] = spawnRange;
			ESGArgsList[8] = walkRange;
			ESGArgsList[9] = amount;
			ESGArgsList[10] = minDelayHour;
			ESGArgsList[11] = minDelayMin;
			ESGArgsList[12] = minDelaySec;
			ESGArgsList[13] = maxDelayHour;
			ESGArgsList[14] = maxDelayMin;
			ESGArgsList[15] = maxDelaySec;
			ESGArgsList[16] = eventRange;
			ESGArgsList[17] = beginTimeBasedHour;
			ESGArgsList[18] = beginTimeBasedMinute;
			ESGArgsList[19] = endTimeBasedHour;
			ESGArgsList[20] = endTimeBasedMinute;
			ESGArgsList[21] = keyword;
			ESGArgsList[22] = caseSensitiveSwitch;
			ESGArgsList[23] = minStackAmount;
			ESGArgsList[24] = maxStackAmount;
			ESGArgsList[25] = movableSwitch;
			ESGArgsList[26] = minDespawnHour;
			ESGArgsList[27] = minDespawnMin;
			ESGArgsList[28] = minDespawnSec;
			ESGArgsList[29] = maxDespawnHour;
			ESGArgsList[30] = maxDespawnMin;
			ESGArgsList[31] = maxDespawnSec;
			ESGArgsList[32] = despawnSwitch;
			ESGArgsList[33] = despawnGroupSwitch;
			ESGArgsList[34] = despawnTimeExpireSwitch;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[14] = MSEGCheckBoxesList;
			ArgsList[19] = megaSpawner;
			ArgsList[21] = ESGArgsList;
			ArgsList[32] = SEGArgsList;
		}

		private void GetArgsList()
		{
			Help = (bool)									ArgsList[0];
			DisplayMessages = (bool)						ArgsList[1];
			MessagesTitle = (string)						ArgsList[2];
			OldMessagesTitle = (string)						ArgsList[3];
			Messages = (string)								ArgsList[4];
			OldMessages = (string)							ArgsList[5];
			PageInfoList = (ArrayList)						ArgsList[12];
			MSEGCheckBoxesList = (ArrayList)				ArgsList[14];
			megaSpawner = (MegaSpawner)						ArgsList[19];
			ESGArgsList = (ArrayList)						ArgsList[21];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			SEGArgsList = (ArrayList)						ArgsList[32];

			cpg = (int)	 									PageInfoList[4];
			pg = (int) 										PageInfoList[5];

			AddToSpawner = (bool) 							ESGArgsList[0];
			index = (int) 									ESGArgsList[1];
			activatedSwitch = (bool) 						ESGArgsList[2];
			spawnGroupSwitch = (bool) 						ESGArgsList[3];
			eventAmbushSwitch = (bool) 						ESGArgsList[4];
			spawnTypeSwitch = (SpawnType) 					ESGArgsList[5];

			if ( !bEntryType )
				entryType = (string) 						ESGArgsList[6];

			if ( !bSpawnRange )
				spawnRange = (int) 							ESGArgsList[7];

			if ( !bWalkRange )
				walkRange = (int) 							ESGArgsList[8];

			if ( !bAmount )
				amount = (int)								ESGArgsList[9];

			if ( !bMinDelayHour )
				minDelayHour = (int) 						ESGArgsList[10];

			if ( !bMinDelayMin )
				minDelayMin = (int) 						ESGArgsList[11];

			if ( !bMinDelaySec )
				minDelaySec = (int) 						ESGArgsList[12];

			if ( !bMaxDelayHour )
				maxDelayHour = (int) 						ESGArgsList[13];

			if ( !bMaxDelayMin )
				maxDelayMin = (int) 						ESGArgsList[14];

			if ( !bMaxDelaySec )
				maxDelaySec = (int) 						ESGArgsList[15];

			if ( !bEventRange )
				eventRange = (int) 							ESGArgsList[16];

			if ( !bBeginTimeBased )
			{
				beginTimeBasedHour = (int) 					ESGArgsList[17];
				beginTimeBasedMinute = (int) 				ESGArgsList[18];
			}

			if ( !bEndTimeBased )
			{
				endTimeBasedHour = (int) 					ESGArgsList[19];
				endTimeBasedMinute = (int) 					ESGArgsList[20];
			}

			keyword = (string) 								ESGArgsList[21];
			caseSensitiveSwitch = (bool)					ESGArgsList[22];

			if ( !bMinStackAmount )
				minStackAmount = (int)						ESGArgsList[23];

			if ( !bMaxStackAmount )
				maxStackAmount = (int)						ESGArgsList[24];

			movableSwitch = (bool)							ESGArgsList[25];

			if ( !bMinDespawnHour )
				minDespawnHour = (int) 						ESGArgsList[26];

			if ( !bMinDespawnMin )
				minDespawnMin = (int) 						ESGArgsList[27];

			if ( !bMinDespawnSec )
				minDespawnSec = (int) 						ESGArgsList[28];

			if ( !bMaxDespawnHour )
				maxDespawnHour = (int) 						ESGArgsList[29];

			if ( !bMaxDespawnMin )
				maxDespawnMin = (int) 						ESGArgsList[30];

			if ( !bMaxDespawnSec )
				maxDespawnSec = (int) 						ESGArgsList[31];

			despawnSwitch = (bool)							ESGArgsList[32];
			despawnGroupSwitch = (bool)						ESGArgsList[33];
			despawnTimeExpireSwitch = (bool)				ESGArgsList[34];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)			PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)			PersonalConfigList[2];
			InactiveTEBGTypeConfig = (BackgroundType)		PersonalConfigList[3];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)			PersonalConfigList[7];
			PageNumberTextColor = (int)						PersonalConfigList[8];
			ActiveTextEntryTextColor = (int)				PersonalConfigList[9];
			InactiveTextEntryTextColor = (int)				PersonalConfigList[10];
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

		private bool CheckBadEntry()
		{
			if ( !AddToSpawner )
			{
				string checkEntryType = Convert.ToString( ScriptCompiler.FindTypeByName( entryType ) );

				if ( checkEntryType == "" )
					return true;
			}

			return false;
		}

		private void CalcTimeBased()
		{
			if ( megaSpawner.OverrideIndividualEntries )
			{
				beginTimeBasedHour = megaSpawner.OverrideBeginTimeBasedHour;
				beginTimeBasedMinute = megaSpawner.OverrideBeginTimeBasedMinute;
				endTimeBasedHour = megaSpawner.OverrideEndTimeBasedHour;
				endTimeBasedMinute = megaSpawner.OverrideEndTimeBasedMinute;
			}

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

			if ( checkPage == String.Format( "{0}/{1}", pg, totalPages ) )
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
				return intPage;
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
				case 6:{ SEGArgsList[1] = entryType; SetArgsList(); gumpMobile.SendGump( new SearchEntryGump( gumpMobile, ArgsList ) ); break; }
				case 7:{ OpenGump(); break; }
			}

			if ( command < 6 )
				OpenGump();
		}

		private void AddPages()
		{
			AddCommandButtons();

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
			{
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

				switch ( pg )
				{
					case 1:
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
					case 2:
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

			string overrideTextColor = flagTextColor;

			if ( !megaSpawner.OverrideIndividualEntries )
				overrideTextColor = defaultTextColor;

			switch ( pg )
			{
				case 1:
				{
					AddHtml( 101, 101, 100, 20, MC.ColorText( defaultTextColor, "Entry Type:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 101, 190, 18 );
					AddTextEntry( 300, 101, 190, 15, ActiveTextEntryTextColor, 1, entryType );

					AddHtml( 101, 281, 160, 20, MC.ColorText( defaultTextColor, "Stack Amount Between:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 281, 60, 18 );
					AddTextEntry( 300, 281, 60, 15, ActiveTextEntryTextColor, 11, minStackAmount.ToString() );

					AddHtml( 371, 281, 40, 20, MC.ColorText( defaultTextColor, "And" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 401, 281, 60, 18 );
					AddTextEntry( 401, 281, 60, 15, ActiveTextEntryTextColor, 12, maxStackAmount.ToString() );

					AddHtml( 541, 101, 50, 20, MC.ColorText( defaultTextColor, "Search" ), false, false );
					AddButton( 500, 103, 2118, 2117, -10, GumpButtonType.Reply, 0 );

					AddHtml( 101, 321, 300, 20, MC.ColorText( defaultTextColor, "Spawn Options:" ), false, false );

					AddHtml( 141, 341, 300, 20, MC.ColorText( defaultTextColor, "Activated" ), false, false );
					AddCheck( 100, 340, 210, 211, activatedSwitch, 1 );

					AddHtml( 141, 401, 300, 20, MC.ColorText( defaultTextColor, "Movable" ), false, false );
					AddCheck( 100, 400, 210, 211, movableSwitch, 4 );

					AddHtml( 101, 121, 300, 20, MC.ColorText( overrideTextColor, "Spawn Range:" ), false, false );
					AddHtml( 101, 141, 300, 20, MC.ColorText( overrideTextColor, "Walk Range:" ), false, false );
					AddHtml( 101, 161, 300, 20, MC.ColorText( overrideTextColor, "Amount To Spawn:" ), false, false );

					AddHtml( 101, 181, 300, 20, MC.ColorText( overrideTextColor, "Min Delay To Next Spawn:" ), false, false );
					AddHtml( 366, 181, 300, 20, MC.ColorText( overrideTextColor, "Hours" ), false, false );
					AddHtml( 481, 181, 300, 20, MC.ColorText( overrideTextColor, "Minutes" ), false, false );
					AddHtml( 606, 181, 300, 20, MC.ColorText( overrideTextColor, "Seconds" ), false, false );
					AddHtml( 101, 201, 300, 20, MC.ColorText( overrideTextColor, "Max Delay To Next Spawn:" ), false, false );
					AddHtml( 366, 201, 300, 20, MC.ColorText( overrideTextColor, "Hours" ), false, false );
					AddHtml( 481, 201, 300, 20, MC.ColorText( overrideTextColor, "Minutes" ), false, false );
					AddHtml( 606, 201, 300, 20, MC.ColorText( overrideTextColor, "Seconds" ), false, false );

					AddHtml( 101, 221, 300, 20, MC.ColorText( overrideTextColor, "Min Delay To Despawn:" ), false, false );
					AddHtml( 366, 221, 300, 20, MC.ColorText( overrideTextColor, "Hours" ), false, false );
					AddHtml( 481, 221, 300, 20, MC.ColorText( overrideTextColor, "Minutes" ), false, false );
					AddHtml( 606, 221, 300, 20, MC.ColorText( overrideTextColor, "Seconds" ), false, false );
					AddHtml( 101, 241, 300, 20, MC.ColorText( overrideTextColor, "Max Delay To Despawn:" ), false, false );
					AddHtml( 366, 241, 300, 20, MC.ColorText( overrideTextColor, "Hours" ), false, false );
					AddHtml( 481, 241, 300, 20, MC.ColorText( overrideTextColor, "Minutes" ), false, false );
					AddHtml( 606, 241, 300, 20, MC.ColorText( overrideTextColor, "Seconds" ), false, false );

					AddHtml( 141, 361, 300, 20, MC.ColorText( overrideTextColor, "Spawn In Group" ), false, false );
					AddHtml( 141, 381, 300, 20, MC.ColorText( overrideTextColor, "Event Ambush" ), false, false );
					AddHtml( 341, 341, 300, 20, MC.ColorText( overrideTextColor, "Use Despawn" ), false, false );
					AddHtml( 341, 361, 300, 20, MC.ColorText( overrideTextColor, "Despawn In Group" ), false, false );

					if ( megaSpawner.OverrideIndividualEntries )
					{
						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 120, 60, 18 );
						AddLabelCropped( 300, 120, 60, 20, InactiveTextEntryTextColor, megaSpawner.OverrideSpawnRange.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 140, 60, 18 );
						AddLabelCropped( 300, 140, 60, 20, InactiveTextEntryTextColor, megaSpawner.OverrideWalkRange.ToString() );

						MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 161, 60, 18 );
						AddTextEntry( 300, 161, 60, 15, ActiveTextEntryTextColor, 4, amount.ToString() );

						minDelayHour = (int) megaSpawner.OverrideMinDelay / 3600;
						minDelayMin = ( (int) megaSpawner.OverrideMinDelay - ( minDelayHour * 3600 ) ) / 60;
						minDelaySec = ( (int) megaSpawner.OverrideMinDelay - ( minDelayHour * 3600 ) - ( minDelayMin * 60 ) );
						maxDelayHour = (int) megaSpawner.OverrideMaxDelay / 3600;
						maxDelayMin = ( (int) megaSpawner.OverrideMaxDelay - ( maxDelayHour * 3600 ) ) / 60;
						maxDelaySec = ( (int) megaSpawner.OverrideMaxDelay - ( maxDelayHour * 3600 ) - ( maxDelayMin * 60 ) );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 180, 60, 18 );
						AddLabelCropped( 300, 180, 60, 20, InactiveTextEntryTextColor, minDelayHour.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 415, 180, 60, 18 );
						AddLabelCropped( 415, 180, 60, 20, InactiveTextEntryTextColor, minDelayMin.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 540, 180, 60, 18 );
						AddLabelCropped( 540, 180, 60, 20, InactiveTextEntryTextColor, minDelaySec.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 200, 60, 18 );
						AddLabelCropped( 300, 200, 60, 20, InactiveTextEntryTextColor, maxDelayHour.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 415, 200, 60, 18 );
						AddLabelCropped( 415, 200, 60, 20, InactiveTextEntryTextColor, maxDelayMin.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 540, 200, 60, 18 );
						AddLabelCropped( 540, 200, 60, 20, InactiveTextEntryTextColor, maxDelaySec.ToString() );

						minDespawnHour = (int) megaSpawner.OverrideMinDespawn / 3600;
						minDespawnMin = ( (int) megaSpawner.OverrideMinDespawn - ( minDespawnHour * 3600 ) ) / 60;
						minDespawnSec = ( (int) megaSpawner.OverrideMinDespawn - ( minDespawnHour * 3600 ) - ( minDespawnMin * 60 ) );
						maxDespawnHour = (int) megaSpawner.OverrideMaxDespawn / 3600;
						maxDespawnMin = ( (int) megaSpawner.OverrideMaxDespawn - ( maxDespawnHour * 3600 ) ) / 60;
						maxDespawnSec = ( (int) megaSpawner.OverrideMaxDespawn - ( maxDespawnHour * 3600 ) - ( maxDespawnMin * 60 ) );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 220, 60, 18 );
						AddLabelCropped( 300, 220, 60, 20, InactiveTextEntryTextColor, minDespawnHour.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 415, 220, 60, 18 );
						AddLabelCropped( 415, 220, 60, 20, InactiveTextEntryTextColor, minDespawnMin.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 540, 220, 60, 18 );
						AddLabelCropped( 540, 220, 60, 20, InactiveTextEntryTextColor, minDespawnSec.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 300, 240, 60, 18 );
						AddLabelCropped( 300, 240, 60, 20, InactiveTextEntryTextColor, maxDespawnHour.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 415, 240, 60, 18 );
						AddLabelCropped( 415, 240, 60, 20, InactiveTextEntryTextColor, maxDespawnMin.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 540, 240, 60, 18 );
						AddLabelCropped( 540, 240, 60, 20, InactiveTextEntryTextColor, maxDespawnSec.ToString() );

						if ( megaSpawner.OverrideGroupSpawn )
							AddImage( 100, 360, 211 );
						else
							AddImage( 100, 360, 210 );

						if ( megaSpawner.OverrideEventAmbush )
							AddImage( 100, 380, 211 );
						else
							AddImage( 100, 380, 210 );

						if ( megaSpawner.OverrideDespawn )
							AddImage( 300, 340, 211 );
						else
							AddImage( 300, 340, 210 );

						if ( megaSpawner.OverrideDespawnGroup )
							AddImage( 300, 360, 211 );
						else
							AddImage( 300, 360, 210 );
					}
					else
					{
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
						AddTextEntry( 300, 221, 60, 15, ActiveTextEntryTextColor, 13, minDespawnHour.ToString() );

						MC.DisplayBackground( this, ActiveTEBGTypeConfig, 415, 221, 60, 18 );
						AddTextEntry( 415, 221, 60, 15, ActiveTextEntryTextColor, 14, minDespawnMin.ToString() );

						MC.DisplayBackground( this, ActiveTEBGTypeConfig, 540, 221, 60, 18 );
						AddTextEntry( 540, 221, 60, 15, ActiveTextEntryTextColor, 15, minDespawnSec.ToString() );

						MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 241, 60, 18 );
						AddTextEntry( 300, 241, 60, 15, ActiveTextEntryTextColor, 16, maxDespawnHour.ToString() );

						MC.DisplayBackground( this, ActiveTEBGTypeConfig, 415, 241, 60, 18 );
						AddTextEntry( 415, 241, 60, 15, ActiveTextEntryTextColor, 17, maxDespawnMin.ToString() );

						MC.DisplayBackground( this, ActiveTEBGTypeConfig, 540, 241, 60, 18 );
						AddTextEntry( 540, 241, 60, 15, ActiveTextEntryTextColor, 18, maxDespawnSec.ToString() );

						AddCheck( 100, 360, 210, 211, spawnGroupSwitch, 2 );
						AddCheck( 100, 380, 210, 211, eventAmbushSwitch, 3 );
						AddCheck( 300, 340, 210, 211, despawnSwitch, 5 );
						AddCheck( 300, 360, 210, 211, despawnGroupSwitch, 6 );
					}

					break;
				}
				case 2:
				{
					AddHtml( 101, 101, 300, 20, MC.ColorText( defaultTextColor, "Spawn Type:" ), false, false );
					AddHtml( 126, 121, 300, 20, MC.ColorText( overrideTextColor, "Regular" ), false, false );
					AddHtml( 126, 141, 300, 20, MC.ColorText( overrideTextColor, "Proximity" ), false, false );
					AddHtml( 126, 161, 300, 20, MC.ColorText( overrideTextColor, "Game Time Based" ), false, false );
					AddHtml( 126, 181, 300, 20, MC.ColorText( overrideTextColor, "Real Time Based" ), false, false );
					AddHtml( 126, 201, 300, 20, MC.ColorText( overrideTextColor, "Speech" ), false, false );
					AddHtml( 241, 201, 300, 20, MC.ColorText( overrideTextColor, "Keyword:" ), false, false );
					AddHtml( 266, 241, 120, 20, MC.ColorText( overrideTextColor, "Case Sensitive" ), false, false );
					AddHtml( 101, 381, 300, 20, MC.ColorText( overrideTextColor, "Event Range:" ), false, false );
					AddHtml( 101, 401, 300, 20, MC.ColorText( overrideTextColor, "All Time Based Will Spawn Between:" ), false, false );
					AddHtml( 451, 401, 300, 20, MC.ColorText( overrideTextColor, "And" ), false, false );
					AddHtml( 391, 381, 300, 20, MC.ColorText( overrideTextColor, "Despawn After Time Expire" ), false, false );

					if ( megaSpawner.OverrideIndividualEntries )
					{
						CalcTimeBased();

						if ( megaSpawner.OverrideSpawnType == SpawnType.Regular )
							AddImage( 100, 120, 9027 );
						else
							AddImage( 100, 120, 9026 );

						if ( megaSpawner.OverrideSpawnType == SpawnType.Proximity )
							AddImage( 100, 140, 9027 );
						else
							AddImage( 100, 140, 9026 );

						if ( megaSpawner.OverrideSpawnType == SpawnType.GameTimeBased )
							AddImage( 100, 160, 9027 );
						else
							AddImage( 100, 160, 9026 );

						if ( megaSpawner.OverrideSpawnType == SpawnType.RealTimeBased )
							AddImage( 100, 180, 9027 );
						else
							AddImage( 100, 180, 9026 );

						if ( megaSpawner.OverrideSpawnType == SpawnType.Speech )
							AddImage( 100, 200, 9027 );
						else
							AddImage( 100, 200, 9026 );

						if ( megaSpawner.OverrideCaseSensitive )
							AddImage( 240, 240, 211 );
						else
							AddImage( 240, 240, 210 );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 320, 201, 320, 38 );
						AddLabelCropped( 320, 201, 320, 40, InactiveTextEntryTextColor, megaSpawner.OverrideEventKeyword );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 210, 381, 60, 18 );
						AddLabelCropped( 210, 381, 60, 20, InactiveTextEntryTextColor, megaSpawner.OverrideEventRange.ToString() );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 350, 401, 80, 18 );
						AddLabelCropped( 350, 401, 80, 20, InactiveTextEntryTextColor, beginTimeBased );

						MC.DisplayBackground( this, InactiveTEBGTypeConfig, 490, 401, 80, 18 );
						AddLabelCropped( 490, 401, 80, 20, InactiveTextEntryTextColor, endTimeBased );

						if ( megaSpawner.OverrideDespawnTimeExpire )
							AddImage( 350, 380, 211 );
						else
							AddImage( 350, 380, 210 );
					}
					else
					{
						CalcTimeBased();

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
					}

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
			if ( !AddToSpawner )
			{
				AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Entry" ), false, false );
				AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

				AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Update Entry" ), false, false );
			}
			else
			{
				AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Submit Entry" ), false, false );
			}

			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );
		}

		private void SubmitEntry( RelayInfo info, bool submit, int command )
		{
			oldEntryType = entryType;

			string oldMessages = Messages;
			bool oldActivated = false;

			if ( submit )
			{
				if ( AddToSpawner )
					MessagesTitle = "Submit Entry";
				else
					MessagesTitle = "Update Entry";

				Messages = null;
			}

			bool invalid = false;

			bool fEntryType=false, fSpawnRange=false, fWalkRange=false, fAmount=false, fMinDelayHour=false, fMinDelayMin=false, fMinDelaySec=false, fMaxDelayHour=false, fMaxDelayMin=false, fMaxDelaySec=false, fEventRange=false, fBeginTimeBased=false, fEndTimeBased=false, fMinStackAmount=false, fMaxStackAmount=false, fMinDespawnHour=false, fMinDespawnMin=false, fMinDespawnSec=false, fMaxDespawnHour=false, fMaxDespawnMin=false, fMaxDespawnSec=false;

			string checkEntryType=null, checkSpawnRange=null, checkWalkRange=null, checkAmount=null, checkMinDelayHour=null, checkMinDelayMin=null, checkMinDelaySec=null, checkMaxDelayHour=null, checkMaxDelayMin=null, checkMaxDelaySec=null, checkEventRange=null, checkBeginTimeBased=null, checkEndTimeBased=null, checkMinStackAmount=null, checkMaxStackAmount=null, checkMinDespawnHour=null, checkMinDespawnMin=null, checkMinDespawnSec=null, checkMaxDespawnHour=null, checkMaxDespawnMin=null, checkMaxDespawnSec=null;

			string stringEntryType=null, stringBeginTimeBasedAMPM=null, stringEndTimeBasedAMPM=null;
			int intSpawnRange=0, intWalkRange=0, intAmount=0, intMinDelayHour=0, intMinDelayMin=0, intMinDelaySec=0, intMaxDelayHour=0, intMaxDelayMin=0, intMaxDelaySec=0, intEventRange=0, intBeginTimeBasedHour=0, intBeginTimeBasedMinute=0, intEndTimeBasedHour=0, intEndTimeBasedMinute=0, intMinStackAmount=0, intMaxStackAmount=0, intMinDespawnHour=0, intMinDespawnMin=0, intMinDespawnSec=0, intMaxDespawnHour=0, intMaxDespawnMin=0, intMaxDespawnSec=0;

			if ( pg == 1 )
			{
				checkEntryType  = info.GetTextEntry( 1 ).Text;
				checkMinStackAmount = info.GetTextEntry( 11 ).Text;
				checkMaxStackAmount = info.GetTextEntry( 12 ).Text;

				oldActivated = activatedSwitch;
				activatedSwitch = info.IsSwitched( 1 );
				movableSwitch = info.IsSwitched( 4 );
			}
			else
			{
				checkEntryType = entryType;
				checkMinStackAmount = minStackAmount.ToString();
				checkMaxStackAmount = maxStackAmount.ToString();
			}

			if ( !megaSpawner.OverrideIndividualEntries )
			{
				if ( pg == 1 )
				{
					spawnGroupSwitch = info.IsSwitched( 2 );
					eventAmbushSwitch = info.IsSwitched( 3 );
					despawnSwitch = info.IsSwitched( 5 );
					despawnGroupSwitch = info.IsSwitched( 6 );

					checkSpawnRange = info.GetTextEntry( 2 ).Text;
					checkWalkRange = info.GetTextEntry( 3 ).Text;
					checkAmount = info.GetTextEntry( 4 ).Text;
					checkMinDelayHour = info.GetTextEntry( 5 ).Text;
					checkMinDelayMin = info.GetTextEntry( 6 ).Text;
					checkMinDelaySec = info.GetTextEntry( 7 ).Text;
					checkMaxDelayHour = info.GetTextEntry( 8 ).Text;
					checkMaxDelayMin = info.GetTextEntry( 9 ).Text;
					checkMaxDelaySec = info.GetTextEntry( 10 ).Text;
					checkMinDespawnHour = info.GetTextEntry( 13 ).Text;
					checkMinDespawnMin = info.GetTextEntry( 14 ).Text;
					checkMinDespawnSec = info.GetTextEntry( 15 ).Text;
					checkMaxDespawnHour = info.GetTextEntry( 16 ).Text;
					checkMaxDespawnMin = info.GetTextEntry( 17 ).Text;
					checkMaxDespawnSec = info.GetTextEntry( 18 ).Text;
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

				if ( pg == 2 )
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
			}
			else
			{
				if ( pg == 1 )
				{
					checkAmount = info.GetTextEntry( 4 ).Text;
				}

				spawnTypeSwitch = SpawnType.Regular;
				spawnGroupSwitch = false;
				eventAmbushSwitch = true;

				checkSpawnRange = "10";
				checkWalkRange = "10";
				checkMinDelayHour = "0";
				checkMinDelayMin = "5";
				checkMinDelaySec = "0";
				checkMaxDelayHour = "0";
				checkMaxDelayMin = "10";
				checkMaxDelaySec = "0";
				checkEventRange = "10";
				checkBeginTimeBased = "12:00 AM";
				checkEndTimeBased = "12:00 AM";
				keyword = "";
				checkMinDespawnHour = "0";
				checkMinDespawnMin = "30";
				checkMinDespawnSec = "0";
				checkMaxDespawnHour = "1";
				checkMaxDespawnMin = "0";
				checkMaxDespawnSec = "0";
			}

// *************** First Check ***************

			Type type = ScriptCompiler.FindTypeByName( checkEntryType );

			if ( type != null )
				stringEntryType = type.Name;

			if ( type == null || MC.IsBadEntryType( checkEntryType ) )
			{
				invalid = true;
				bEntryType = false;
				fEntryType = true;
				Messages = String.Format( "{0}Invalid input for [Entry Type]. You must specify a valid type.\n", Messages );
			}

			if ( AddToSpawner && submit && megaSpawner.OverrideIndividualEntries )
			{
				for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
				{
					if ( checkEntryType.ToLower() == ( (string) megaSpawner.EntryList[i] ).ToLower() )
					{
						invalid = true;
						bEntryType = false;
						fEntryType = true;
						Messages = String.Format( "{0}Invalid input for [Entry Type]. That entry already exists. You may not have duplicate entries when the Mega Spawner is overridden. Please choose another entry.\n", Messages );
					}
				}
			}

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
				if ( !fEntryType || command == 6 )
				{
					bEntryType = true;
					entryType = checkEntryType;
				}

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

			if ( stringEntryType != null )
				entryType = stringEntryType;
			else
				entryType = checkEntryType;

			spawnRange = intSpawnRange;
			walkRange = intWalkRange;
			amount = intAmount;
			eventRange = intEventRange;
			minStackAmount = intMinStackAmount;
			maxStackAmount = intMaxStackAmount;

			if ( AddToSpawner && submit )
			{
				megaSpawner.EntryList.Add( entryType );
				megaSpawner.SpawnRangeList.Add( spawnRange );
				megaSpawner.WalkRangeList.Add( walkRange );
				megaSpawner.AmountList.Add( amount );
				megaSpawner.MinDelayList.Add( calcMinDelay );
				megaSpawner.MaxDelayList.Add( calcMaxDelay );
				megaSpawner.SpawnTypeList.Add( spawnTypeSwitch );
				megaSpawner.ActivatedList.Add( activatedSwitch );
				megaSpawner.EventRangeList.Add( eventRange );
				megaSpawner.EventKeywordList.Add( keyword );
				megaSpawner.KeywordCaseSensitiveList.Add( caseSensitiveSwitch );
				megaSpawner.TriggerEventNowList.Add( (bool) true );
				megaSpawner.EventAmbushList.Add( eventAmbushSwitch );
				megaSpawner.BeginTimeBasedList.Add( calcBeginTimeBased );
				megaSpawner.EndTimeBasedList.Add( calcEndTimeBased );
				megaSpawner.GroupSpawnList.Add( spawnGroupSwitch );
				megaSpawner.MinStackAmountList.Add( minStackAmount );
				megaSpawner.MaxStackAmountList.Add( maxStackAmount );
				megaSpawner.MovableList.Add( movableSwitch );
				megaSpawner.MinDespawnList.Add( calcMinDespawn );
				megaSpawner.MaxDespawnList.Add( calcMaxDespawn );
				megaSpawner.DespawnList.Add( despawnSwitch );
				megaSpawner.DespawnGroupList.Add( despawnGroupSwitch );
				megaSpawner.DespawnTimeExpireList.Add( despawnTimeExpireSwitch );

				megaSpawner.SpawnedEntries.Add( new ArrayList() );
				megaSpawner.LastMovedList.Add( new ArrayList() );

				MSEGCheckBoxesList.Add( (bool) false );

				Messages = String.Format( "\"{0}\" entry has been added.", entryType );
			}
			else if ( submit )
			{
				if ( oldEntryType != entryType )
					megaSpawner.DeleteEntries( index );

				megaSpawner.EntryList[index] = entryType;
				megaSpawner.SpawnRangeList[index] = spawnRange;
				megaSpawner.WalkRangeList[index] = walkRange;
				megaSpawner.AmountList[index] = amount;
				megaSpawner.MinDelayList[index] = calcMinDelay;
				megaSpawner.MaxDelayList[index] = calcMaxDelay;
				megaSpawner.SpawnTypeList[index] = spawnTypeSwitch;
				megaSpawner.ActivatedList[index] = activatedSwitch;
				megaSpawner.EventRangeList[index] = eventRange;
				megaSpawner.EventKeywordList[index] = keyword;
				megaSpawner.KeywordCaseSensitiveList[index] = caseSensitiveSwitch;
				megaSpawner.TriggerEventNowList[index] = (bool) true;
				megaSpawner.EventAmbushList[index] = eventAmbushSwitch;
				megaSpawner.BeginTimeBasedList[index] = calcBeginTimeBased;
				megaSpawner.EndTimeBasedList[index] = calcEndTimeBased;
				megaSpawner.GroupSpawnList[index] = spawnGroupSwitch;
				megaSpawner.MinStackAmountList[index] = minStackAmount;
				megaSpawner.MaxStackAmountList[index] = maxStackAmount;
				megaSpawner.MovableList[index] = movableSwitch;
				megaSpawner.MinDespawnList[index] = calcMinDespawn;
				megaSpawner.MaxDespawnList[index] = calcMaxDespawn;
				megaSpawner.DespawnList[index] = despawnSwitch;
				megaSpawner.DespawnGroupList[index] = despawnGroupSwitch;
				megaSpawner.DespawnTimeExpireList[index] = despawnTimeExpireSwitch;

				if ( !megaSpawner.OverrideIndividualEntries )
				{
					megaSpawner.RemoveRespawnEntries( index );
					megaSpawner.RecompileSettings( index );
				}

				if ( entryType == oldEntryType )
					Messages = String.Format( "\"{0}\" entry has been updated.", oldEntryType );
				else
					Messages = String.Format( "\"{0}\" entry has been updated and changed to \"{1}\" entry.", oldEntryType, entryType );
			}

			if ( submit )
			{
				if ( megaSpawner.OverrideIndividualEntries )
				{
					MegaSpawnerOverride.DeleteEntries( megaSpawner );
					MegaSpawnerOverride.RemoveRespawnEntries( megaSpawner );
					megaSpawner.OverrideSpawnedEntries.Clear();
					megaSpawner.OverrideLastMovedList.Clear();

					for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
					{
						megaSpawner.OverrideRespawnEntryList.Add( "" );
						megaSpawner.OverrideRespawnTimeList.Add( 0 );
						megaSpawner.OverrideSpawnCounterList.Add( DateTime.Now );
						megaSpawner.OverrideSpawnTimeList.Add( 0 );
						megaSpawner.OverrideDespawnTimeList.Add( 0 );
					}
				}
				else
				{
					ArrayList respawnEntryList = new ArrayList();
					ArrayList respawnTimeList = new ArrayList();
					ArrayList spawnCounterList = new ArrayList();
					ArrayList spawnTimeList = new ArrayList();
					ArrayList respawnOnSaveList = new ArrayList();
					ArrayList despawnTimeList = new ArrayList();

					for ( int i = 0; i < amount; i++ )
					{
						respawnEntryList.Add( entryType );
						respawnTimeList.Add( 0 );
						spawnCounterList.Add( DateTime.Now );
						spawnTimeList.Add( 0 );
						respawnOnSaveList.Add( (bool) false );
						despawnTimeList.Add( 0 );
					}

					if ( AddToSpawner )
					{
						megaSpawner.RespawnEntryList.Add( respawnEntryList );
						megaSpawner.RespawnTimeList.Add( respawnTimeList );
						megaSpawner.SpawnCounterList.Add( spawnCounterList );
						megaSpawner.SpawnTimeList.Add( spawnTimeList );
						megaSpawner.RespawnOnSaveList.Add( respawnOnSaveList );
						megaSpawner.DespawnTimeList.Add( despawnTimeList );
					}
					else
					{
						megaSpawner.RespawnEntryList[index] = respawnEntryList;
						megaSpawner.RespawnTimeList[index] = respawnTimeList;
						megaSpawner.SpawnCounterList[index] = spawnCounterList;
						megaSpawner.SpawnTimeList[index] = spawnTimeList;
						megaSpawner.RespawnOnSaveList[index] = respawnOnSaveList;
						megaSpawner.DespawnTimeList[index] = despawnTimeList;
					}
				}

				if ( megaSpawner.Active )
				{
					if ( oldActivated && !activatedSwitch )
						megaSpawner.DeleteEntries( index );

					if ( !megaSpawner.OverrideIndividualEntries )
					{
						if ( !AddToSpawner )
							megaSpawner.Respawn( index );
						else
							megaSpawner.Respawn( megaSpawner.EntryList.Count - 1 );
					}
					else
					{
						megaSpawner.Respawn();
					}

					megaSpawner.CheckProximityType();
					megaSpawner.CheckSpeechType();
				}

				SetArgsList();

				gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );
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
		}
	}
}