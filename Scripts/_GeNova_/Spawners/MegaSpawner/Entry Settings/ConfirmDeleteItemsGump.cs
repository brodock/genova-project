using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteItemsGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList AVSArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList AITCCheckBoxesList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private ArrayList InsideItemList = new ArrayList();
		
		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private Mobile gumpMobile;

		public ConfirmDeleteItemsGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
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
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete All Items" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete Selected Items" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete all items?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete selected items?" ), false, false ); break; }
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
						case 1: { MessagesTitle = "Delete All Items"; Messages = "You have chosen not to delete all items."; break; }
						case 2: { MessagesTitle = "Delete Selected Items"; Messages = "You have chosen not to delete selected items."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new AddItemsToContainerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Items
				{
					switch ( Select )
					{
						case 1: // Delete All Items
						{
							InsideItemList.Clear();
							AITCCheckBoxesList.Clear();

							MessagesTitle = "Delete All Items";
							Messages = "All of the items have been deleted.";

							break;
						}
						case 2: // Delete Selected Items
						{
							for ( int i = 0; i < InsideItemList.Count; i++ )
							{
								if ( (bool) AITCCheckBoxesList[i] )
								{
									InsideItemList.RemoveAt( i );
									AITCCheckBoxesList.RemoveAt( i );

									i--;
								}
							}

							MessagesTitle = "Delete Selected Items";
							Messages = "All selected items have been deleted.";

							break;
						}
					}

					SetArgsList();

					gumpMobile.SendGump( new AddItemsToContainerGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			AVSArgsList[27] = InsideItemList;

			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[22] = AVSArgsList;
			ArgsList[31] = AITCCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			AVSArgsList = (ArrayList)					ArgsList[22];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			AITCCheckBoxesList = (ArrayList)				ArgsList[31];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];

			InsideItemList = (ArrayList)					AVSArgsList[27];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
		}
	}
}