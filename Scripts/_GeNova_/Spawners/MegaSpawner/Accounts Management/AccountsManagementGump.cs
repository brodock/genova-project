using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Accounting;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class AccountsManagementGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList AMGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList EAGArgsList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor, FlagTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor, flagTextColor;
		private int PageNumberTextColor;

		private int offsetMin, offsetMax;

		private ArrayList MenuAccessList = new ArrayList();

		private const int numOffset = 1000;
		private const int amtPerPg = 14;
		private const int listSpace = 308;
		private int totalPages;
		private Mobile gumpMobile;

		public AccountsManagementGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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

			AddHtml( 121, 81, 560, 20, MC.ColorText( titleTextColor, "Accounts Management" ), false, false );

			if ( !Help )
				AddButton( 597, 429, 2033, 2032, -5, GumpButtonType.Reply, 0 );
			else
				AddButton( 597, 429, 2032, 2033, -5, GumpButtonType.Reply, 0 );

			CheckAccounts();

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

			SetAMGCheckBoxes();

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( !CheckAccess() )
				return;

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
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount of accounts, to perform certain commands on.\n\nFor Example: You select a few spawners, then choose [Delete Selected Accounts], you will delete the selected accounts only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( i > 1 )
						{
							if ( (bool) AMGCheckBoxesList[i] )
								chkd++;
							else
								unchkd++;
						}
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( i > 1 )
						{
							if ( chkd > unchkd )
								AMGCheckBoxesList[i] = (bool) false;
							else if ( unchkd > chkd )
								AMGCheckBoxesList[i] = (bool) true;
							else
								AMGCheckBoxesList[i] = (bool) false;
						}
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
						Messages = "That button will close the gump and return to the previous gump.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Select All Accounts
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select All Accounts Button";
						Messages = "That button will select all accounts on the list.";

						OpenGump();

						break;
					}

					for ( int i = 0; i < MC.AccountsList.Count; i++ )
						AMGCheckBoxesList[i] = (bool) true;

					MessagesTitle = "Select All Accounts";
					Messages = "All accounts have been selected.";

					OpenGump();

					break;
				}
				case 2: // Deselect All Accounts
				{
					if ( Help )
					{
						MessagesTitle = "Help: Deselect All Accounts Button";
						Messages = "That button will deselect all accounts on the list.";

						OpenGump();

						break;
					}

					for ( int i = 0; i < MC.AccountsList.Count; i++ )
						AMGCheckBoxesList[i] = (bool) false;

					MessagesTitle = "Deselect All Accounts";
					Messages = "All accounts have been deselected.";

					OpenGump();

					break;
				}
				case 3: // Add Account
				{
					if ( Help )
					{
						MessagesTitle = "Help: Add Account Button";
						Messages = "That button will add an account to the Mega Spawner System.";

						OpenGump();

						break;
					}

					PageInfoList[28] = 1;					// EditAccountGumpCommandPage
					PageInfoList[29] = 1;					// EditAccountGumpPage

					EAGArgsList[0] = index;					// index
					EAGArgsList[1] = (bool) true;				// AddAccount
					EAGArgsList[2] = Access.User;				// accessSwitch
					EAGArgsList[3] = (bool) false;				// accountsMgmtSwitch
					EAGArgsList[4] = (bool) false;				// systemConfigSwitch
					EAGArgsList[5] = "";					// accountName

					SetArgsList();

					gumpMobile.SendGump( new EditAccountGump( gumpMobile, ArgsList ) );

					break;
				}
				case 4: // Delete Selected Accounts
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Selected Accounts Button";
						Messages = "That button will delete all selected accounts on the list.";

						OpenGump();

						break;
					}

					if ( MC.AccountsList.Count == 2 )
					{
						MessagesTitle = "Delete Selected Accounts";
						Messages = "There are no accounts on the list (excluding Master Admin and Default User).";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < MC.AccountsList.Count; i++ )
					{
						if ( (bool) AMGCheckBoxesList[i] )
						{
							found = true;

							break;
						}
					}

					if ( !found )
					{
						MessagesTitle = "Delete Selected Accounts";
						Messages = "There are no accounts selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteAccountsGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				case 5: // Delete All Accounts
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete All Accounts Button";
						Messages = "That button will delete all accounts on the list excluding the Master Admin and Default User accounts.";

						OpenGump();

						break;
					}

					if ( MC.SpawnerList.Count == 2 )
					{
						MessagesTitle = "Delete All Accounts";
						Messages = "There are no accounts on the list (excluding Master Admin and Default User).";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteAccountsGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				default: // Edit Account
				{
					if ( Help )
					{
						MessagesTitle = "Help: Edit Account Button";
						Messages = "That button will open the selected account and allow you to edit it.";

						OpenGump();

						break;
					}

					string accountName = (string) MC.AccountsList[index];
					Access accessLevel = (Access) MC.AccessList[index];

					if ( accountName == ( (Account) gumpMobile.Account ).Username )
					{
						MessagesTitle = "Edit Account";
						Messages = "You cannot edit your own account.";

						OpenGump();

						break;
					}

					if ( MC.GetAccessLevel( gumpMobile ) > Access.MasterAdmin && accountName == "default user" )
					{
						MessagesTitle = "Edit Account";
						Messages = "You do not have authorization to edit the default user account. Only the master admin can edit that account.";

						OpenGump();

						break;
					}

					if ( accessLevel <= MC.GetAccessLevel( gumpMobile ) )
					{
						MessagesTitle = "Edit Account";
						Messages = "You cannot edit an account with the same or higher access level than yours.";

						OpenGump();

						break;
					}

					Mobile editor = (Mobile) MC.AccountEditors[index];

					if ( editor != null )
					{
						MessagesTitle = "Edit Account";
						Messages = String.Format( "{0} is currently editting that account. You will have to wait until they are done.", editor.Name );

						OpenGump();

						break;
					}

					MC.AccountEditors[index] = gumpMobile;

					ArrayList List = MC.GetMenuAccess( index );

					PageInfoList[28] = 1;					// EditAccountGumpCommandPage
					PageInfoList[29] = 1;					// EditAccountGumpPage

					EAGArgsList[0] = index;					// index
					EAGArgsList[1] = (bool) false;				// AddAccount
					EAGArgsList[2] = accessLevel;				// accessSwitch
					EAGArgsList[3] = (bool) List[0];			// accountsMgmtSwitch
					EAGArgsList[4] = (bool) List[1];			// systemConfigSwitch
					EAGArgsList[5] = (string) MC.AccountsList[index];	// accountName

					SetArgsList();

					gumpMobile.SendGump( new EditAccountGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new AccountsManagementGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[26] = cpg;
			PageInfoList[27] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[18] = AMGCheckBoxesList;
			ArgsList[29] = EAGArgsList;
		}

		private void GetArgsList()
		{
			Help = (bool)							ArgsList[0];
			DisplayMessages = (bool)					ArgsList[1];
			MessagesTitle = (string)					ArgsList[2];
			OldMessagesTitle = (string)					ArgsList[3];
			Messages = (string)						ArgsList[4];
			OldMessages = (string)						ArgsList[5];
			PageInfoList = (ArrayList)					ArgsList[12];
			AMGCheckBoxesList = (ArrayList)					ArgsList[18];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			EAGArgsList = (ArrayList)					ArgsList[29];

			cpg = (int) 							PageInfoList[26];
			pg = (int) 							PageInfoList[27];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)				PersonalConfigList[7];
			PageNumberTextColor = (int)					PersonalConfigList[8];
			FlagTextColor = (TextColor)					PersonalConfigList[14];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
			flagTextColor = MC.GetTextColor( FlagTextColor );
		}

		private bool CheckAccess()
		{
			MenuAccessList = MC.GetMenuAccess( gumpMobile );

			bool access = (bool) MenuAccessList[0];

			if ( !access )
			{
				MessagesTitle = "Access Denied";
				Messages = "You no longer have access to that option.";

				SetArgsList();

				gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );
			}

			return access;
		}

		private void SaveCheckBoxes( RelayInfo info )
		{
			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
				AMGCheckBoxesList[i] = info.IsSwitched( i );
		}

		private void SetAMGCheckBoxes()
		{
			for ( int i = 0; i < MC.AccountsList.Count; i++ )
				AMGCheckBoxesList.Add( (bool) false );
		}

		private void CheckAccounts()
		{
			bool found = false;
			bool accountNotExist = false;
			bool banned = false;

			for ( int i = 0; i < MC.AccountsList.Count; i++ )
			{
				string accountName = (string) MC.AccountsList[i];

				if ( MC.AccountNotExist( accountName ) )
				{
					if ( !accountNotExist && OldMessages != Messages )
						Messages = String.Format( "{0}:\n{1}\n\nThe following accounts have been detected as non-existant accounts:\n{2}", MessagesTitle, Messages, MC.AccountsList[i].ToString() );
					else if ( !accountNotExist )
						Messages = String.Format( "The following accounts have been detected as non-existant accounts:\n{0}", MC.AccountsList[i].ToString() );
					else
						Messages = String.Format( "{0}, {1}", Messages, MC.AccountsList[i].ToString() );

					MessagesTitle = "Account Errors Detected";

					accountNotExist = true;
					found = true;
				}
			}

			for ( int i = 0; i < MC.AccountsList.Count; i++ )
			{
				string accountName = (string) MC.AccountsList[i];

				if ( MC.AccountBanned( accountName ) )
				{
					MessagesTitle = "Account Errors Detected";

					if ( !banned && accountNotExist )
						Messages = String.Format( "{0}\nThe following accounts have been detected as banned accounts:\n{1}", Messages, MC.AccountsList[i].ToString() );
					else if ( !banned && !accountNotExist && OldMessages != Messages )
						Messages = String.Format( "{0}:\n{1}\n\nThe following accounts have been detected as banned accounts:\n{2}", MessagesTitle, Messages, MC.AccountsList[i].ToString() );
					else if ( !banned && !accountNotExist )
						Messages = String.Format( "The following accounts have been detected as banned accounts:\n{0}", MC.AccountsList[i].ToString() );
					else
						Messages = String.Format( "{0}, {1}", Messages, MC.AccountsList.ToString() );

					found = true;
				}
			}

			if ( found )
				Messages = String.Format( "{0}\n\nThose accounts listed either do not exist or are banned. They have been flagged a different color. You should take care of the possible security risks.", Messages );

			OldMessages = Messages;
		}

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= MC.AccountsList.Count )
				offsetMax = MC.AccountsList.Count - 1;
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( MC.AccountsList.Count - 1 ) / amtPerPg ) + 1;

			if ( pg > totalPages )
				pg = totalPages;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( MC.AccountsList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no accounts to display." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 338, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 440, 100, 200, 20 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
//					AddButton( 193, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
				}

				AddHtml( 101, 101, 340, 20, MC.ColorText( defaultTextColor, "<center>Account Name</center>" ), false, false );
				AddHtml( 441, 101, 200, 20, MC.ColorText( defaultTextColor, "<center>Access Level</center>" ), false, false );
			}

			SetPage();
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
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 338, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 440, listY, 200, 20 );

				string accountName = (string) MC.AccountsList[i];
				string textColor = defaultTextColor;

				if ( MC.AccountNotExist( accountName ) || MC.AccountBanned( accountName ) )
					textColor = flagTextColor;

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );
				AddHtml( 141, listY + 1, 520, 20, MC.ColorText( textColor, accountName ), false, false );

				string AccessLevel = null;
				Access accessLevel = (Access) MC.AccessList[i];

				switch ( accessLevel )
				{
					case Access.MasterAdmin:{ AccessLevel = "Master Admin"; break; }
					case Access.HeadAdmin:{ AccessLevel = "Head Admin"; break; }
					case Access.Admin:{ AccessLevel = "Admin"; break; }
					case Access.User:{ AccessLevel = "User"; break; }
					case Access.None:{ AccessLevel = "None"; break; }
				}

				AddHtml( 441, listY + 1, 200, 20, MC.ColorText( textColor, String.Format( "<center>{0}</center>", AccessLevel ) ), false, false );

				if ( i > 1 )
					AddCheck( 640, listY, 210, 211, (bool) AMGCheckBoxesList[i], i );
				else
					AddImage( 640, listY, 210 );

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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select All Accounts" ), false, false );
			AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Deselect All Accounts" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Add Account" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Selected Accounts" ), false, false );
			AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete All Accounts" ), false, false );
			AddButton( 350, 470, 9904, 9905, 5, GumpButtonType.Reply, 0 );
		}
	}
}