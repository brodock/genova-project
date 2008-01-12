using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteAccountsGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList AMGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private Mobile gumpMobile;

		public ConfirmDeleteAccountsGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
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
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete All Accounts" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete Selected Accounts" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete all accounts?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete selected accounts?" ), false, false ); break; }
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
						case 1: { MessagesTitle = "Delete All Accounts"; Messages = "You have chosen not to delete all accounts in the system."; break; }
						case 2: { MessagesTitle = "Delete Selected Accounts"; Messages = "You have chosen not to delete selected accounts in the system."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new AccountsManagementGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Accounts
				{
					switch ( Select )
					{
						case 1: // Delete All Accounts
						{
							int count = 0;

							for ( int i = 2; i < MC.AccountsList.Count; i++ )
							{
								Access accessLevel = (Access) MC.AccessList[i];

								if ( accessLevel > MC.GetAccessLevel( gumpMobile ) )
								{
									count++;

									MC.AccountsList.RemoveAt( i );
									MC.AccessList.RemoveAt( i );
									MC.AdminMenuAccessList.RemoveAt( i );
									MC.AccountEditors.RemoveAt( i );
									AMGCheckBoxesList.RemoveAt( i );

									i--;
								}
							}

							AMGCheckBoxesList.Clear();
							MC.SaveAccountsAccess();

							MessagesTitle = "Delete All Accounts";

							if ( count > 0 )
								Messages = String.Format( "{0} account{1} in the system {2} been deleted. Master Admin, Default User, and any account with equal to or higher accesslevel than yours has been excluded.", count, count == 1 ? "" : "s", count == 1 ? "has" : "have" );
							else
								Messages = "No accounts have been deleted. You do not have authorization to delete any of the accounts in the system.";

							break;
						}
						case 2: // Delete Selected Accounts
						{
							int count = 0;

							for ( int i = 2; i < MC.AccountsList.Count; i++ )
							{
								Access accessLevel = (Access) MC.AccessList[i];

								if ( (bool) AMGCheckBoxesList[i] && ( accessLevel > MC.GetAccessLevel( gumpMobile ) ) )
								{
									count++;

									MC.AccountsList.RemoveAt( i );
									MC.AccessList.RemoveAt( i );
									MC.AdminMenuAccessList.RemoveAt( i );
									MC.AccountEditors.RemoveAt( i );
									AMGCheckBoxesList.RemoveAt( i );

									i--;
								}
							}

							MC.SaveAccountsAccess();

							MessagesTitle = "Delete Selected Accounts";

							if ( count > 0 )
								Messages = String.Format( "{0} selected account{1} in the system {2} been deleted. Master Admin, Default User, and any account with equal to or higher accesslevel than yours has been excluded.", count, count == 1 ? "" : "s", count == 1 ? "has" : "have" );
							else
								Messages = "No accounts have been deleted. You do not have authorization to delete any of the selected accounts in the system.";

							break;
						}
					}

					SetArgsList();

					gumpMobile.SendGump( new AccountsManagementGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[18] = AMGCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			AMGCheckBoxesList = (ArrayList)					ArgsList[18];
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