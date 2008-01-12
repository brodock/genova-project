using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Server.Commands;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Accounting;
using MC = Server.MegaSpawnerSystem.MasterControl;


namespace Server.MegaSpawnerSystem
{
	public class EditAccountGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList EAGArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig, InactiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor, FlagTextColor;

		private int index;
		private bool AddAccount;
		private Access accessSwitch;
		private bool accountsMgmtSwitch, systemConfigSwitch;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor, flagTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor, InactiveTextEntryTextColor;

		private string accountName;
		private ArrayList MenuAccessList = new ArrayList();

		private int totalPages = 1;
		private Mobile gumpMobile;

		public EditAccountGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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

			if ( AddAccount )
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, "Add Account" ), false, false );
			else
				AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, String.Format( "Editting Account: \"{0}\"", accountName ) ), false, false );

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

			MenuAccessList = MC.GetMenuAccess( gumpMobile );

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( !CheckAccess() )
				return;

			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					SubmitAccount( info, false, 0 );

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

						SubmitAccount( info, false, 0 );

						break;
					}

					DisplayMessages = !DisplayMessages;

					SubmitAccount( info, false, 0 );

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window and also change the page number if specified in the upper right hand corner. To change the page number, delete the current page number and type in the page you want to go to.";

						SubmitAccount( info, false, 0 );

						break;
					}

					int intPage = 0;
					string checkPage = Convert.ToString( info.GetTextEntry( 0 ).Text ).ToLower();

					if ( checkPage == String.Format( "{0}/{1}", pg, totalPages ) )
					{
						SubmitAccount( info, false, 0 );

						break;
					}

					try{ intPage = Convert.ToInt32( checkPage ); }
					catch
					{
						MessagesTitle = "Refresh";
						Messages = "The page number must be an integer.";

						SubmitAccount( info, false, 0 );

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

					SubmitAccount( info, false, 0 );

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						SubmitAccount( info, false, 0 );

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					SubmitAccount( info, false, 0 );

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

					SubmitAccount( info, false, 0 );

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous account edit page.";

						SubmitAccount( info, false, 0 );

						break;
					}

					SubmitAccount( info, false, 1 );

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next account edit page.";

						SubmitAccount( info, false, 0 );

						break;
					}

					SubmitAccount( info, false, 2 );

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						SubmitAccount( info, false, 0 );

						break;
					}

					SubmitAccount( info, false, 3 );

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						SubmitAccount( info, false, 0 );

						break;
					}

					SubmitAccount( info, false, 4 );

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						SubmitAccount( info, false, 0 );

						break;
					}

					if ( !AddAccount )
						MC.AccountEditors[index] = null;

					gumpMobile.SendGump( new AccountsManagementGump( gumpMobile, ArgsList ) );

					break;
				}
				case 2: // Delete Account
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Account Button";
						Messages = "That button will delete the account from the Mega Spawner System.";

						SubmitAccount( info, false, 0 );

						break;
					}

					if ( accountName == "default user" )
					{
						MessagesTitle = "Delete Account";
						Messages = "You may not delete the default user account.";

						SubmitAccount( info, false, 0 );

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteAccountGump( gumpMobile, ArgsList ) );

					break;
				}
				case 3: // Submit/Update Account
				{
					if ( Help )
					{
						if ( AddAccount )
						{
							MessagesTitle = "Help: Submit Account Button";
							Messages = "That button will add the account into the Mega Spawner System.";
						}
						else
						{
							MessagesTitle = "Help: Update Account Button";
							Messages = "That button will update changes to the account.";
						}

						SubmitAccount( info, false, 0 );

						break;
					}

					SubmitAccount( info, true, 0 );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new EditAccountGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[28] = cpg;
			PageInfoList[29] = pg;

			EAGArgsList[0] = index;
			EAGArgsList[1] = AddAccount;
			EAGArgsList[2] = accessSwitch;
			EAGArgsList[3] = accountsMgmtSwitch;
			EAGArgsList[4] = systemConfigSwitch;
			EAGArgsList[5] = accountName;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
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
			PersonalConfigList = (ArrayList)				ArgsList[28];
			EAGArgsList = (ArrayList)					ArgsList[29];

			cpg = (int) 							PageInfoList[28];
			pg = (int) 							PageInfoList[29];

			index = (int)							EAGArgsList[0];
			AddAccount = (bool)						EAGArgsList[1];
			accessSwitch = (Access)						EAGArgsList[2];
			accountsMgmtSwitch = (bool)					EAGArgsList[3];
			systemConfigSwitch = (bool)					EAGArgsList[4];
			accountName = (string)						EAGArgsList[5];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)				PersonalConfigList[2];
			InactiveTEBGTypeConfig = (BackgroundType)			PersonalConfigList[3];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)				PersonalConfigList[7];
			PageNumberTextColor = (int)					PersonalConfigList[8];
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

		private Access GetAccessLevel{ get{ return MC.GetAccessLevel( gumpMobile ); } }

		private bool CheckAccess()
		{
			MenuAccessList = MC.GetMenuAccess( gumpMobile );

			bool access = (bool) MenuAccessList[0];

			if ( !access )
			{
				MessagesTitle = "Access Denied";
				Messages = "You no longer have access to that option.";

				if ( !AddAccount )
					MC.AccountEditors[index] = null;

				SetArgsList();

				gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );
			}

			return access;
		}

		private void UpdatePage( int command )
		{
			switch ( command )
			{
				case 1:{ pg--; break; }
				case 2:{ pg++; break; }
				case 3:{ cpg--; break; }
				case 4:{ cpg++; break; }
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
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

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

			switch ( pg )
			{
				case 1:
				{
					string textColor = null;

					AddHtml( 101, 101, 100, 20, MC.ColorText( defaultTextColor, "Account Name:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 101, 190, 18 );
					AddTextEntry( 300, 101, 190, 15, ActiveTextEntryTextColor, 1, accountName );

					AddHtml( 101, 121, 100, 20, MC.ColorText( defaultTextColor, "Access Level:" ), false, false );

					if ( GetAccessLevel >= Access.HeadAdmin )
					{
						textColor = flagTextColor;
						AddImage( 100, 140, 9026 );
					}
					else
					{
						textColor = defaultTextColor;
						AddRadio( 100, 140, 9026, 9027, accessSwitch == Access.HeadAdmin, 1 );
					}

					AddHtml( 141, 141, 100, 20, MC.ColorText( textColor, "Head Admin" ), false, false );

					if ( GetAccessLevel >= Access.Admin )
					{
						textColor = flagTextColor;
						AddImage( 100, 160, 9026 );
					}
					else
					{
						textColor = defaultTextColor;
						AddRadio( 100, 160, 9026, 9027, accessSwitch == Access.Admin, 2 );
					}

					AddHtml( 141, 161, 100, 20, MC.ColorText( textColor, "Admin" ), false, false );

					AddHtml( 141, 181, 100, 20, MC.ColorText( defaultTextColor, "User" ), false, false );
					AddRadio( 100, 180, 9026, 9027, accessSwitch == Access.User, 3 );

					AddHtml( 141, 201, 100, 20, MC.ColorText( defaultTextColor, "None" ), false, false );
					AddRadio( 100, 200, 9026, 9027, accessSwitch == Access.None, 4 );

					AddHtml( 101, 241, 160, 20, MC.ColorText( defaultTextColor, "Admin Menu Access:" ), false, false );

					for ( int i = 0; i < MenuAccessList.Count; i++ )
					{
						bool access = (bool) MenuAccessList[i];

						if ( i == 0 && !access )
						{
							textColor = flagTextColor;
							AddImage( 100, 260, 9026 );
						}
						else if ( i == 0 )
						{
							textColor = defaultTextColor;
							AddCheck( 100, 260, 9026, 9027, accountsMgmtSwitch, 5 );
						}

						if ( i == 0 )
							AddHtml( 141, 261, 160, 20, MC.ColorText( textColor, "Accounts Management" ), false, false );

						if ( i == 1 && !access )
						{
							textColor = flagTextColor;
							AddImage( 100, 280, 9026 );
						}
						else if ( i == 1 )
						{
							textColor = defaultTextColor;
							AddCheck( 100, 280, 9026, 9027, systemConfigSwitch, 6 );
						}

						if ( i == 1 )
							AddHtml( 141, 281, 160, 20, MC.ColorText( textColor, "System Configuration" ), false, false );
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
			if ( !AddAccount )
			{
				AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Account" ), false, false );
				AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

				AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Update Account" ), false, false );
			}
			else
			{
				AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Submit Account" ), false, false );
			}

			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );
		}

		private void SubmitAccount( RelayInfo info, bool submit, int command )
		{
			string oldMessages = Messages;
			string oldAccountName = accountName;

			if ( submit )
			{
				if ( AddAccount )
					MessagesTitle = "Submit Account";
				else
					MessagesTitle = "Update Account";

				Messages = null;
			}

			bool invalid = false;

			if ( pg == 1 )
			{
				accountName = info.GetTextEntry( 1 ).Text;

				int switchNum = MC.GetSwitchNum( info, 1, 4 );
				accessSwitch = (Access) switchNum + 1;

				accountsMgmtSwitch = info.IsSwitched( 5 );
				systemConfigSwitch = info.IsSwitched( 6 );
			}

// *************** First Check ***************

			if ( submit )
			{
				if ( oldAccountName == "default user" && accountName != oldAccountName.ToLower() )
				{
					invalid = true;
					Messages = "You cannot change the name of the default user account.";
				}
				else if ( oldAccountName != "default user" && accountName.ToLower() == "default user" )
				{
					invalid = true;
					Messages = "You cannot add default user as an account. The default user account is hard-coded and already exists.";
				}
				else if ( ( (Account) gumpMobile.Account ).Username.ToLower() == accountName )
				{
					invalid = true;
					Messages = "You cannot add your own account into the system.";
				}
				else if ( MC.AccountNotExist( accountName ) )
				{
					invalid = true;
					Messages = String.Format( "That account does not exist and cannot be {0}.", AddAccount ? "added" : "updated" );
				}
				else if ( MC.AccountBanned( accountName ) )
				{
					invalid = true;
					Messages = "That account is banned and cannot be added for security reasons.";
				}
				else if ( MC.AccountNoCommandAccess( accountName ) )
				{
					invalid = true;
					Messages = String.Format( "That account does not have access to the \"{0}MegaSpawner\" command and cannot be added.", CommandSystem.Prefix );
				}
			}

// *************** Final Check ***************

			if ( invalid )
			{
				UpdatePage( command );

				return;
			}

// *************** Applying Settings ***************

			if ( AddAccount && submit )
			{
				if ( MC.AccountExists( accountName ) )
				{
					Messages = "That account is already in the Mega Spawner System.";

					UpdatePage( command );

					return;
				}

				MC.AccountsList.Add( accountName );
				MC.AccessList.Add( accessSwitch );

				ArrayList List = new ArrayList();

				List.Add( accountsMgmtSwitch );
				List.Add( systemConfigSwitch );

				MC.AdminMenuAccessList.Add( List );
				MC.AccountEditors.Add( null );

				Messages = String.Format( "Account \"{0}\" has been added.", accountName );
			}
			else if ( submit )
			{
				if ( oldAccountName != accountName && MC.AccountExists( accountName ) )
				{
					Messages = "That account is already in the Mega Spawner System.";

					UpdatePage( command );

					return;
				}

				MC.AccountsList[index] = accountName;
				MC.AccessList[index] = accessSwitch;

				ArrayList List = new ArrayList();

				List.Add( accountsMgmtSwitch );
				List.Add( systemConfigSwitch );

				MC.AdminMenuAccessList[index] = List;
				MC.AccountEditors[index] = null;

				Messages = String.Format( "Account \"{0}\" has been updated.", accountName );
			}

			if ( submit )
			{
				MC.SaveAccountsAccess();

				if ( !AddAccount )
					MC.AccountEditors[index] = null;

				SetArgsList();

				gumpMobile.SendGump( new AccountsManagementGump( gumpMobile, ArgsList ) );
			}
			else
			{
				UpdatePage( command );
			}
		}
	}
}
