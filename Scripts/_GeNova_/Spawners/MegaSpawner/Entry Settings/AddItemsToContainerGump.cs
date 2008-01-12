using System;
using System.IO;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Targeting;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class AddItemsToContainerGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList AVSArgsList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList AITCCheckBoxesList = new ArrayList();
		private ArrayList SEGArgsList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig, InactiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private string addItem;
		private ArrayList InsideItemList = new ArrayList();

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor, InactiveTextEntryTextColor;

		private string itemToAdd;
		private int minStackAmount, maxStackAmount;
		private bool addingToList;

		private int offsetMin, offsetMax;

		private const int numOffset = 1000;
		private const int amtPerPg = 14;
		private const int listSpace = 308;
		private int totalPages;
		private RelayInfo relayInfo;
		private Mobile gumpMobile;

		public AddItemsToContainerGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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

			AddHtml( 121, 81, 560, 20, MC.ColorText( titleTextColor, String.Format( "Add {0} Item{1} To \"{2}\"", InsideItemList.Count, InsideItemList.Count == 1 ? "" : "s", addItem ) ), false, false );

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

			GetOffsets();
			SetAITCCheckBoxes();

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
				case -101: // Item
				{
					MessagesTitle = "Help: Item Column";
					Messages = "This column lists the items that will be added to the container.";

					OpenGump();

					break;
				}
				case -102: // Min Stack Amount
				{
					MessagesTitle = "Help: Min Stack Amount Column";
					Messages = "This column lists each item's minimum stack amount. A random stack amount will be created between values of [Min Stack Amount] and [Max Stack Amount] if the item being added is stackable.";

					OpenGump();

					break;
				}
				case -103: // Max Stack Amount
				{
					MessagesTitle = "Help: Max Stack Amount Column";
					Messages = "This column lists each item's maximum stack amount. A random stack amount will be created between values of [Min Stack Amount] and [Max Stack Amount] if the item being added is stackable.";

					OpenGump();

					break;
				}
				case -104: // Entry #
				{
					MessagesTitle = "Help: Entry # Column";
					Messages = "This column lists the entry # for each item.";

					OpenGump();

					break;
				}
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount of items, to perform certain commands on.\n\nFor Example: You select entry #1, then choose [Delete Selected Items], you will delete the selected item(entry #1) only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( (bool) AITCCheckBoxesList[i] )
							chkd++;
						else
							unchkd++;
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( chkd > unchkd )
							AITCCheckBoxesList[i] = (bool) false;
						else if ( unchkd > chkd )
							AITCCheckBoxesList[i] = (bool) true;
						else
							AITCCheckBoxesList[i] = (bool) false;
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

						SubmitItems( info, false, 0 );

						break;
					}

					DisplayMessages = !DisplayMessages;

					SubmitItems( info, false, 0 );

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window and also change the page number if specified in the upper right hand corner. To change the page number, delete the current page number and type in the page you want to go to.";

						SubmitItems( info, false, 0 );

						break;
					}

					relayInfo = info;

					SubmitItems( info, false, 5 );

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						SubmitItems( info, false, 0 );

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					SubmitItems( info, false, 0 );

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

					SubmitItems( info, false, 0 );

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous page.";

						SubmitItems( info, false, 0 );

						break;
					}

					pg--;

					SubmitItems( info, false, 0 );

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next page.";

						SubmitItems( info, false, 0 );

						break;
					}

					pg++;

					SubmitItems( info, false, 0 );

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						SubmitItems( info, false, 0 );

						break;
					}

					cpg--;

					SubmitItems( info, false, 0 );

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						SubmitItems( info, false, 0 );

						break;
					}

					cpg++;

					SubmitItems( info, false, 0 );

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						SubmitItems( info, false, 0 );

						break;
					}

					SetArgsList();

					gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Add Item
				{
					if ( Help )
					{
						MessagesTitle = "Help: Add Item Button";
						Messages = "That button will add another item to the item list.";

						SubmitItems( info, false, 0 );

						break;
					}

					if ( InsideItemList.Count >= 125 )
					{
						MessagesTitle = "Add Item";
						Messages = "You may not add anymore items. Item amount cannot exceed 125 items.";

						SubmitItems( info, false, 0 );

						break;
					}

					ArrayList List = new ArrayList();
					List.Add( "" );
					List.Add( 1 );
					List.Add( 1 );
					InsideItemList.Add( List );

					AITCCheckBoxesList.Add( (bool) false );

					addingToList = true;

					SubmitItems( info, false, 0 );

					addingToList = false;

					break;
				}
				case 2: // Delete All Items
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete All Items Button";
						Messages = "That button will delete all items on the item list.";

						SubmitItems( info, false, 0 );

						break;
					}

					if ( InsideItemList.Count == 0 )
					{
						MessagesTitle = "Delete All Items";
						Messages = "There are no items to delete.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteItemsGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				case 3: // Submit Items
				{
					if ( Help )
					{
						MessagesTitle = "Help: Submit Items Button";
						Messages = "That button will submit the items list to the setting you are editting.";

						SubmitItems( info, false, 0 );

						break;
					}

					SubmitItems( info, true, 0 );

					break;
				}
				case 4: // Select All Items
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select All Items Button";
						Messages = "That button will select all items on the item list.";

						SubmitItems( info, false, 0 );

						break;
					}

					for ( int i = 0; i < InsideItemList.Count; i++ )
						AITCCheckBoxesList[i] = (bool) true;

					SubmitItems( info, false, 0 );

					break;
				}
				case 5: // Deselect All Items
				{
					if ( Help )
					{
						MessagesTitle = "Help: Deselect All Items Button";
						Messages = "That button will deselect all items on the item list.";

						SubmitItems( info, false, 0 );

						break;
					}

					for ( int i = 0; i < InsideItemList.Count; i++ )
						AITCCheckBoxesList[i] = (bool) false;

					SubmitItems( info, false, 0 );

					break;
				}
				case 6: // Delete Selected Items
				{
					if ( Help )
					{
						MessagesTitle = "Help: Delete Selected Items Button";
						Messages = "That button will deleted selected items on the item list.";

						SubmitItems( info, false, 0 );

						break;
					}

					if ( InsideItemList.Count == 0 )
					{
						MessagesTitle = "Delete Selected Items";
						Messages = "There are no items to delete.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < InsideItemList.Count; i++ )
					{
						if ( (bool) AITCCheckBoxesList[i] )
							found = true;
					}

					if ( !found )
					{
						MessagesTitle = "Delete Selected Items";
						Messages = "There are no items selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmDeleteItemsGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				default: // Search
				{
					if ( Help )
					{
						MessagesTitle = "Help: Search Button";
						Messages = "That button will search for all entry types that match the text you typed in. This will allow you to type in a partial name and search for any existing entry types that match your search criteria.";

						SubmitItems( info, false, 0 );

						break;
					}

					string searchFor = info.GetTextEntry( ( index * 3 ) + 1 ).Text;

					PageInfoList[22] = 1;				// SearchEntryGumpCommandPage
					PageInfoList[23] = 1;				// SearchEntryGumpPage

					AVSArgsList[27] = InsideItemList;		// InsideItemList
					AVSArgsList[28] = index;			// insideIndex

					SEGArgsList[0] = SearchType.ItemType;		// searchType
					SEGArgsList[1] = searchFor;			// SearchFor

					SetArgsList();

					ArgsList[30] = FromGump.AddItemsToContainer;	// FromWhere

					gumpMobile.SendGump( new SearchEntryGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new AddItemsToContainerGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[30] = cpg;
			PageInfoList[31] = pg;

			AVSArgsList[23] = addItem;
			AVSArgsList[27] = InsideItemList;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[22] = AVSArgsList;
			ArgsList[31] = AITCCheckBoxesList;
			ArgsList[32] = SEGArgsList;
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
			AVSArgsList = (ArrayList)					ArgsList[22];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			AITCCheckBoxesList = (ArrayList)				ArgsList[31];
			SEGArgsList = (ArrayList)					ArgsList[32];

			cpg = (int)							PageInfoList[30];
			pg = (int)							PageInfoList[31];

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

			addItem = (string)						AVSArgsList[23];
			InsideItemList = (ArrayList)					AVSArgsList[27];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
		}

		private void SaveCheckBoxes( RelayInfo info )
		{
			for ( int i = offsetMin; i <= offsetMax; i++ )
				AITCCheckBoxesList[i] = info.IsSwitched( i );
		}

		private void SetAITCCheckBoxes()
		{
			for ( int i = 0; i < InsideItemList.Count; i++ )
				AITCCheckBoxesList.Add( (bool) false );
		}

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= InsideItemList.Count )
				offsetMax = InsideItemList.Count - 1;
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
			}

			if ( command < 6 )
				OpenGump();
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( InsideItemList.Count - 1 ) / amtPerPg ) + 1;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( InsideItemList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no items on the item list." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 228, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 330, 100, 118, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 450, 100, 118, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 570, 100, 68, 20 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
					AddButton( 209, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
					AddButton( 384, 100, 2104, 2103, -102, GumpButtonType.Reply, 0 );
					AddButton( 504, 100, 2104, 2103, -103, GumpButtonType.Reply, 0 );
					AddButton( 599, 100, 2104, 2103, -104, GumpButtonType.Reply, 0 );
				}

				AddHtml( 101, 101, 230, 20, MC.ColorText( defaultTextColor, "<center>Item</center>" ), false, false );
				AddHtml( 331, 101, 120, 20, MC.ColorText( defaultTextColor, "<center>Min Stack Amount</center>" ), false, false );
				AddHtml( 451, 101, 120, 20, MC.ColorText( defaultTextColor, "<center>Max Stack Amount</center>" ), false, false );
				AddHtml( 571, 101, 70, 20, MC.ColorText( defaultTextColor, "<center>Entry #</center>" ), false, false );

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
				ArrayList List = (ArrayList) InsideItemList[i];
				itemToAdd = (string) List[0];
				minStackAmount = (int) List[1];
				maxStackAmount = (int) List[2];

				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 228, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 330, listY, 118, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 450, listY, 118, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 570, listY, 68, 20 );

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );
				MC.DisplayBackground( this, ActiveTEBGTypeConfig, 140, listY + 1, 188, 18 );
				AddTextEntry( 140, listY + 1, 188, 15, ActiveTextEntryTextColor, ( i * 3 ) + 1, itemToAdd );

				MC.DisplayBackground( this, ActiveTEBGTypeConfig, 330, listY + 1, 118, 18 );
				AddTextEntry( 330, listY + 1, 118, 15, ActiveTextEntryTextColor, ( i * 3 ) + 2, minStackAmount.ToString() );

				MC.DisplayBackground( this, ActiveTEBGTypeConfig, 450, listY + 1, 118, 18 );
				AddTextEntry( 450, listY + 1, 118, 15, ActiveTextEntryTextColor, ( i * 3 ) + 3, maxStackAmount.ToString() );

				AddHtml( 571, listY + 1, 68, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", i ) ), false, false );

				AddCheck( 640, listY, 210, 211, (bool) AITCCheckBoxesList[i], i );

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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Add Item" ), false, false );
			AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete All Items" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Submit Items" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select All Items" ), false, false );
			AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Deselect All Items" ), false, false );
			AddButton( 350, 470, 9904, 9905, 5, GumpButtonType.Reply, 0 );

			AddHtml( 391, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Delete Selected Items" ), false, false );
			AddButton( 350, 490, 9904, 9905, 6, GumpButtonType.Reply, 0 );
		}

		private void SubmitItems( RelayInfo info, bool submit, int command )
		{
			string oldMessages = Messages;

			if ( submit )
			{
				MessagesTitle = "Submit Items";
				Messages = null;
			}

			bool invalid = false;
			TextRelay textInput;

			for ( int i = offsetMin; i <= offsetMax; i++ )
			{
				ArrayList List = (ArrayList) InsideItemList[i];
				int intCheck = 0;

				textInput = info.GetTextEntry( ( i * 3 ) + 1 );
				List[0] = textInput.Text.ToLower();
				Type checkType = ScriptCompiler.FindTypeByName( (string) List[0] );

				if ( checkType != null )
					List[0] = checkType.Name;

				for ( int j = 2; j <= 3; j++ )
				{
					textInput = info.GetTextEntry( ( i * 3 ) + j );

					try
					{
						intCheck = Convert.ToInt32( textInput.Text );

						if ( intCheck <= 0 )
						{
							invalid = true;

							Messages = String.Format( "{0}Entry #{1} is invalid. {2} must be greater than 0.\n", Messages, i, j == 2 ? "[Min Stack Amount]" : "[Max Stack Amount]" );
						}
						else
						{
							if ( j == 3 && (int) List[1] > intCheck )
							{
								invalid = true;
								List[1] = List[2];

								Messages = String.Format( "{0}Entry #{1} is invalid. [Max Stack Amount] must be greater than or equal to [Min Stack Amount].\n", Messages, i );
							}
							else
							{
								List[j - 1] = intCheck;
							}
						}
					}
					catch
					{
						invalid = true;
						Messages = String.Format( "{0}Entry #{1} is invalid. {2} must be integer only.\n", Messages, i, j == 2 ? "[Min Stack Amount]" : "[Max Stack Amount]" );
					}
				}

				InsideItemList[i] = List;
			}

			bool nullCheck=false, itemCheck=false;

			for ( int i = 0; i < InsideItemList.Count; i++ )
			{
				ArrayList List = (ArrayList) InsideItemList[i];
                string check;
				Type type = ScriptCompiler.FindTypeByName( (string) List[0] );

				if ( type != null )
					check = type.Name;

				if ( type == null )
				{
					invalid = true;

					if ( Messages == null )
						Messages = String.Format( "The following is a list of incorrect entry types:\n#{0} {1}", i, (string) List[0] );
					else if ( !nullCheck )
						Messages = String.Format( "{0}\nThe following is a list of incorrect entry types:\n#{1} {2}", Messages, i, (string) List[0] );
					else
						Messages = String.Format( "{0}, #{1} {2}", Messages, i, (string) List[0] );

					nullCheck = true;
				}
			}

			for ( int i = 0; i < InsideItemList.Count; i++ )
			{
				ArrayList List = (ArrayList) InsideItemList[i];
				string check = "";
				Type type = ScriptCompiler.FindTypeByName( (string) List[0] );

				if ( type != null )
					check = type.Name;

				if ( check != "" && !MC.IsItem( check ) )
				{
					invalid = true;

					if ( Messages == null )
						Messages = String.Format( "The following is a list of entry types that are not items:\n#{0} {1}", i, (string) List[0] );
					else if ( !itemCheck )
						Messages = String.Format( "{0}\nThe following is a list of entry types that are not items:\n#{1} {2}", Messages, i, (string) List[0] );
					else
						Messages = String.Format( "{0}, #{1} {2}", Messages, i, (string) List[0] );

					itemCheck = true;
				}
			}

			if ( invalid )
			{
				if ( !submit )
					Messages = oldMessages;

				UpdatePage( command );

				return;
			}

			if ( submit )
			{
				SetArgsList();

				gumpMobile.SendGump( new AddViewSettingsGump( gumpMobile, ArgsList ) );
			}
			else
			{
				Messages = oldMessages;

				UpdatePage( command );
			}
		}
	}
}