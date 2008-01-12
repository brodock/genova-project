using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class SettingsGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList SettingsCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private ArrayList AVSArgsList = new ArrayList();
		private ArrayList AVSSetList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

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

		public SettingsGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, "Mega Spawner Settings" ), false, false );

			if ( !Help )
				AddButton( 597, 429, 2033, 2032, -5, GumpButtonType.Reply, 0 );
			else
				AddButton( 597, 429, 2032, 2033, -5, GumpButtonType.Reply, 0 );

			CheckSettings();

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

			SetSettingsCheckBoxes();

			AddHtml( 151, 431, 60, 20, MC.ColorText( defaultTextColor, "Refresh" ), false, false );
			AddButton( 120, 433, 2118, 2117, -7, GumpButtonType.Reply, 0 );

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			SaveCheckBoxes( info );

			int index = (int) info.ButtonID - numOffset;

			ArrayList setting = new ArrayList();

			if ( index >= 0 )
				setting = (ArrayList) megaSpawner.SettingsList[index];

			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					OpenGump();

					break;
				}
				case -101: // Setting
				{
					MessagesTitle = "Help: Setting Column";
					Messages = "This column lists all the settings.";

					OpenGump();

					break;
				}
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount of settings, to perform certain commands on.\n\nFor Example: You select a few settings, then choose [Delete Selected Settings], you will delete the selected settings only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( (bool) SettingsCheckBoxesList[i] )
							chkd++;
						else
							unchkd++;
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( chkd > unchkd )
							SettingsCheckBoxesList[i] = (bool) false;
						else if ( unchkd > chkd )
							SettingsCheckBoxesList[i] = (bool) true;
						else
							SettingsCheckBoxesList[i] = (bool) false;
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

					SetArgsList();

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

						SetArgsList();
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

					SetArgsList();

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

					SetArgsList();

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

					SetArgsList();

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

					SetArgsList();

					OpenGump();

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						OpenGump();

						break;
					}

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Selected Settings
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Selected Settings Button";
						Messages = "That button will delete all selected settings.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( megaSpawner.SettingsList.Count == 0 )
					{
						MessagesTitle = "Delete Selected Settings";
						Messages = "There are no settings to delete.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
					{
						if ( (bool) SettingsCheckBoxesList[i] )
							found = true;
					}

					if ( !found )
					{
						MessagesTitle = "Delete Selected Settings";
						Messages = "There are no settings selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteSettingsGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				case 2: // Delete All Settings
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete All Settings Button";
						Messages = "That button will delete all settings on the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( megaSpawner.SettingsList.Count == 0 )
					{
						MessagesTitle = "Delete All Settings";
						Messages = "There are no settings to delete.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteSettingsGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				case 3: // Add New Setting
				{
					if ( Help )
					{
						MessagesTitle = "Help: Add New Setting Button";
						Messages = "That button will open a gump to add a new setting to the Mega Spawner.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					PageInfoList[14] = 1;							// AddViewSettingsGumpCommandPage
					PageInfoList[15] = 1;							// AddViewSettingsGumpPage

					AVSArgsList[0] = (bool) true;					// AddSetting
					AVSArgsList[1] = index;							// index
					AVSArgsList[3] = (bool) false;					// spawnGroupSwitch
					AVSArgsList[4] = (bool) true;					// eventAmbushSwitch
					AVSArgsList[5] = SpawnType.Regular;				// spawnTypeSwitch
					AVSArgsList[6] = 10;							// spawnRange
					AVSArgsList[7] = 10;							// walkRange
					AVSArgsList[8] = 1;								// amount
					AVSArgsList[9] = 0;								// minDelayHour
					AVSArgsList[10] = 5;							// minDelayMinute
					AVSArgsList[11] = 0;							// minDelaySecond
					AVSArgsList[12] = 0;							// maxDelayHour
					AVSArgsList[13] = 10;							// maxDelayMinute
					AVSArgsList[14] = 0;							// maxDelaySecond
					AVSArgsList[15] = 10;							// eventRange
					AVSArgsList[16] = 0;							// beginTimeBasedHour
					AVSArgsList[17] = 0;							// beginTimeBasedMinute
					AVSArgsList[18] = 0;							// endTimeBasedHour
					AVSArgsList[19] = 0;							// endTimeBasedMinute
					AVSArgsList[20] = "";							// keyword
					AVSArgsList[21] = (bool) false;					// caseSensitiveSwitch
					AVSArgsList[22] = "";							// entryName
					AVSArgsList[23] = "";							// addItem
					AVSArgsList[24] = -1;							// entryIndex
					AVSArgsList[25] = 1;							// minStackAmount
					AVSArgsList[26] = 1;							// maxStackAmount
					AVSArgsList[27] = new ArrayList();				// InsideItemList
					AVSArgsList[29] = 0;							// minDespawnHour
					AVSArgsList[30] = 30;							// minDespawnMinute
					AVSArgsList[31] = 0;							// minDespawnSecond
					AVSArgsList[32] = 1;							// maxDespawnHour
					AVSArgsList[33] = 0;							// maxDespawnMinute
					AVSArgsList[34] = 0;							// maxDespawnSecond
					AVSArgsList[35] = (bool) false;					// despawnSwitch
					AVSArgsList[36] = (bool) false;					// despawnGroupSwitch
					AVSArgsList[37] = (bool) true;					// despawnTimeExpireSwitch

					AVSSetList[0] = (bool) false;					// OverrideIndividualEntriesClicked
					AVSSetList[1] = (bool) false;					// AddItemClicked

					SetArgsList();

					ArgsList[8] = new ArrayList();					// setting

					gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 4: // Select All Settings
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select All Settings Button";
						Messages = "That button will select all settings on the Mega Spawner setting list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.SettingsList.Count == 0 )
					{
						MessagesTitle = "Select All Settings";
						Messages = "There are no settings to select.";

						OpenGump();

						break;
					}

					SettingsCheckBoxesList = new ArrayList( megaSpawner.SettingsList.Count );

					for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
						SettingsCheckBoxesList.Add( (bool) true );

					MessagesTitle = "Select All Settings";
					Messages = "All settings have been selected.";

					OpenGump();

					break;
				}
				case 5: // Deselect All Settings
				{
					if ( Help )
					{
						MessagesTitle = "Help: Deselect All Settings Button";
						Messages = "That button will deselect all settings on the Mega Spawner setting list.";

						OpenGump();

						break;
					}

					if ( megaSpawner.SettingsList.Count == 0 )
					{
						MessagesTitle = "Deselect All Settings";
						Messages = "There are no settings to deselect.";

						OpenGump();

						break;
					}

					SettingsCheckBoxesList = new ArrayList( megaSpawner.SettingsList.Count );

					for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
						SettingsCheckBoxesList.Add( (bool) false );

					MessagesTitle = "Deselect All Settings";
					Messages = "All settings have been deselected.";

					OpenGump();

					break;
				}
				default: // Setting Edit
				{
					if ( Help )
					{
						MessagesTitle = "Help: Setting Edit Button";
						Messages = "That button will open the setting and allow you to edit it.";

						OpenGump();

						break;
					}

					if ( setting[0] is Setting )
					{
						switch( (Setting) setting[0] )
						{
							case Setting.OverrideIndividualEntries:{ AVSArgsList[24] = -1; break; }
							case Setting.AddItem:{ AVSArgsList[24] = setting[2]; break; }
							case Setting.AddContainer:{ AVSArgsList[24] = setting[2]; break; }
						}
					}

					PageInfoList[14] = 1;								// AddViewSettingsGumpCommandPage
					PageInfoList[15] = 0;								// AddViewSettingsGumpPage

					AVSArgsList[0] = (bool) false;						// AddSetting
					AVSArgsList[1] = index;								// index

					AVSSetList[0] = (bool) false;						// OverrideIndividualEntriesClicked
					AVSSetList[1] = (bool) false;						// AddItemClicked

					SetArgsList();

					ArgsList[8] = setting;								// setting

					gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[12] = cpg;
			PageInfoList[13] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[17] = SettingsCheckBoxesList;
			ArgsList[19] = megaSpawner;
			ArgsList[22] = AVSArgsList;
			ArgsList[23] = AVSSetList;
		}

		private void GetArgsList()
		{
			Help = (bool)											ArgsList[0];
			DisplayMessages = (bool)								ArgsList[1];
			MessagesTitle = (string)								ArgsList[2];
			OldMessagesTitle = (string)								ArgsList[3];
			Messages = (string)										ArgsList[4];
			OldMessages = (string)									ArgsList[5];
			PageInfoList = (ArrayList)								ArgsList[12];
			SettingsCheckBoxesList = (ArrayList)					ArgsList[17];
			megaSpawner = (MegaSpawner)								ArgsList[19];
			AVSArgsList = (ArrayList)								ArgsList[22];
			AVSSetList = (ArrayList)								ArgsList[23];
			PersonalConfigList = (ArrayList)						ArgsList[28];

			cpg = (int)												PageInfoList[12];
			pg = (int)												PageInfoList[13];

			StyleTypeConfig = (StyleType)							PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)					PersonalConfigList[1];
			DefaultTextColor = (TextColor)							PersonalConfigList[4];
			TitleTextColor = (TextColor)							PersonalConfigList[5];
			MessagesTextColor = (TextColor)							PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)					PersonalConfigList[7];
			PageNumberTextColor = (int)								PersonalConfigList[8];
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

		private void SaveCheckBoxes( RelayInfo info )
		{
			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
				SettingsCheckBoxesList[i] = info.IsSwitched( i );
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

		private void SetSettingsCheckBoxes()
		{
			for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
				SettingsCheckBoxesList.Add( (bool) false );
		}

		private void CheckSettings()
		{
			bool found = false;
			bool badSetting = false;

			for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
			{
				ArrayList setting = (ArrayList) megaSpawner.SettingsList[i];

				if ( !MC.VerifySetting( setting ) )
				{
					if ( !badSetting && OldMessages != Messages )
						Messages = String.Format( "{0}:\n{1}\n\nThe following settings have been detected as bad settings:\n{2}", MessagesTitle, Messages, MC.GetSettingInfo( megaSpawner, setting ) );
					else if ( !badSetting )
						Messages = String.Format( "The following settings have been detected as bad settings:\n{0}", MC.GetSettingInfo( megaSpawner, setting ) );
					else
						Messages = String.Format( "{0}, {1}", Messages, MC.GetSettingInfo( megaSpawner, setting ) );

					MessagesTitle = "Bad Settings Detected";

					badSetting = true;
					found = true;
				}
			}

			if ( found )
				Messages = String.Format( "{0}\n\nThose settings listed have been detected as bad settings. They have been flagged a different color.", Messages );

			OldMessages = Messages;
		}

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= megaSpawner.SettingsList.Count )
				offsetMax = megaSpawner.SettingsList.Count - 1;
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( megaSpawner.SettingsList.Count - 1 ) / amtPerPg ) + 1;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( megaSpawner.SettingsList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no settings." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 20 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
					AddButton( 130, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
				}

				AddHtml( 141, 101, 300, 20, MC.ColorText( defaultTextColor, "Setting" ), false, false );
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

			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
			{
				string textColor = defaultTextColor;

				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 560, 20 );

				ArrayList setting = (ArrayList) megaSpawner.SettingsList[i];

				if ( !MC.VerifySetting( setting ) )
					textColor = flagTextColor;

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );
				AddHtml( 141, listY + 1, 520, 20, MC.ColorText( textColor, MC.GetSettingInfo( megaSpawner, setting ) ), false, false );

				AddCheck( 640, listY, 210, 211, (bool) SettingsCheckBoxesList[i], i );

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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Selected Settings" ), false, false );
			AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete All Settings" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Add New Setting" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select All Settings" ), false, false );
			AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Deselect All Settings" ), false, false );
			AddButton( 350, 470, 9904, 9905, 5, GumpButtonType.Reply, 0 );
		}
	}
}