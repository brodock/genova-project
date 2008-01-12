using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteAccountGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList EAGArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private int index;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private Mobile gumpMobile;

		public ConfirmDeleteAccountGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 330, 90 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 290, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 230, 290, 20 );

			AddHtml( 221, 181, 270, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Account" ), false, false );
			AddHtml( 201, 201, 290, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete the account?" ), false, false );

			AddHtml( 441, 231, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 400, 230, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 231, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 230, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					MessagesTitle = "Delete Account";
					Messages = "You have chosen not to delete the account.";

					SetArgsList();

					gumpMobile.SendGump( new EditAccountGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Account
				{
					MC.AccountsList.RemoveAt( index );
					MC.AccessList.RemoveAt( index );
					MC.AdminMenuAccessList.RemoveAt( index );
					MC.AccountEditors.RemoveAt( index );

					MC.SaveAccountsAccess();

					MessagesTitle = "Delete Account";
					Messages = "Account has been removed.";

					SetArgsList();

					gumpMobile.SendGump( new AccountsManagementGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			EAGArgsList[0] = index;

			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[29] = EAGArgsList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			EAGArgsList = (ArrayList)					ArgsList[29];

			index = (int) 							EAGArgsList[0];

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