using System;
using System.IO;
using System.Xml;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class SearchAndSortGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private bool sortSearchCaseSensitive, sortSearchFlagged, sortSearchBadLocation, sortSearchDupeSpawners;
		private string sortSearchFor;
		private SortOrder sortOrder;
		private SortSearchType sortSearchType, sortType;

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor;

		private const int totalPages = 2;
		private RelayInfo relayInfo;
		private Mobile gumpMobile;

		public SearchAndSortGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

			MC.DisplayImage( this, StyleTypeConfig );

			AddButton( 630, 490, 4017, 4019, 0, GumpButtonType.Reply, 0 );
			AddHtml( 121, 81, 560, 20, MC.ColorText( titleTextColor, "Search And Sort" ), false, false );

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
				case -101: // Search
				{
					MessagesTitle = "Help: Search";
					Messages = "Type in the search criteria for your search into this field.";

					OpenGump();

					break;
				}
				case -102: // Search Terms
				{
					MessagesTitle = "Help: Search Terms";
					Messages = "This is where you can select how to perform the search. If \"None\" is selected, this means the search option is not active. You can select only one search term.";

					OpenGump();

					break;
				}
				case -103: // Search Options
				{
					MessagesTitle = "Help: Search Options";
					Messages = "You can select one or more options to use with your search.";

					OpenGump();

					break;
				}
				case -104: // Sort Terms
				{
					MessagesTitle = "Help: Sort Terms";
					Messages = "This is where you can select how to perform the sort. If \"None\" is selected, this means the sort option is not active. You can select only one sort term.";

					OpenGump();

					break;
				}
				case -105: // Sort Order
				{
					MessagesTitle = "Help: Sort Order";
					Messages = "You can select from ascending or descending order. Ascending would be for example from A-Z, where descending would be Z-A.";

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

						SubmitSort( info, false, 0 );

						break;
					}

					DisplayMessages = !DisplayMessages;

					SubmitSort( info, false, 0 );

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window and also change the page number if specified in the upper right hand corner. To change the page number, delete the current page number and type in the page you want to go to.";

						SubmitSort( info, false, 0 );

						break;
					}

					relayInfo = info;

					SubmitSort( info, false, 5 );

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						SubmitSort( info, false, 0 );

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					SubmitSort( info, false, 0 );

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

					SubmitSort( info, false, 0 );

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous page.";

						SubmitSort( info, false, 0 );

						break;
					}

					SubmitSort( info, false, 1 );

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next page.";

						SubmitSort( info, false, 0 );

						break;
					}

					SubmitSort( info, false, 2 );

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						SubmitSort( info, false, 0 );

						break;
					}

					SubmitSort( info, false, 3 );

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						SubmitSort( info, false, 0 );

						break;
					}

					SubmitSort( info, false, 4 );

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						SubmitSort( info, false, 0 );

						break;
					}

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 2: // Reset
				{
					if ( Help )
					{
						MessagesTitle = "Help: Reset Button";
						Messages = "That button will reset all search and sort options to default.";

						SubmitSort( info, false, 0 );

						break;
					}

					sortSearchFor = "";
					sortSearchType = SortSearchType.None;
					sortSearchCaseSensitive = false;
					sortType = SortSearchType.None;
					sortOrder = SortOrder.Ascending;

					OpenGump();

					break;
				}
				case 3: // Submit
				{
					if ( Help )
					{
						MessagesTitle = "Help: Submit Button";
						Messages = "That button will submit the criteria specified for search and sort to produce a new spawner list.";

						SubmitSort( info, false, 0 );

						break;
					}

					SubmitSort( info, true, 0 );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new SearchAndSortGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[36] = cpg;
			PageInfoList[37] = pg;

			PersonalConfigList[15] = sortSearchFor;
			PersonalConfigList[16] = sortSearchType;
			PersonalConfigList[17] = sortOrder;
			PersonalConfigList[18] = sortType;
			PersonalConfigList[19] = sortSearchCaseSensitive;
			PersonalConfigList[20] = sortSearchFlagged;
			PersonalConfigList[21] = sortSearchBadLocation;
			PersonalConfigList[22] = sortSearchDupeSpawners;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[28] = PersonalConfigList;
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

			cpg = (int) 							PageInfoList[36];
			pg = (int) 							PageInfoList[37];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)			PersonalConfigList[2];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)				PersonalConfigList[7];
			PageNumberTextColor = (int)					PersonalConfigList[8];
			ActiveTextEntryTextColor = (int)				PersonalConfigList[9];
			sortSearchFor = (string)					PersonalConfigList[15];
			sortSearchType = (SortSearchType)				PersonalConfigList[16];
			sortOrder = (SortOrder)						PersonalConfigList[17];
			sortType = (SortSearchType)					PersonalConfigList[18];
			sortSearchCaseSensitive = (bool)				PersonalConfigList[19];
			sortSearchFlagged = (bool)					PersonalConfigList[20];
			sortSearchBadLocation = (bool)				PersonalConfigList[21];
			sortSearchDupeSpawners = (bool)				PersonalConfigList[22];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
		}

		private void WriteConfig()
		{
			if ( !Directory.Exists( MC.SaveDirectory ) )
				Directory.CreateDirectory( MC.SaveDirectory );

			if ( !Directory.Exists( MC.PersonalConfigsDirectory ) )
				Directory.CreateDirectory( MC.PersonalConfigsDirectory );

			using ( StreamWriter sw = new StreamWriter( MC.GetPersonalFileName( gumpMobile ) ) )
			{
				XmlTextWriter xml = new XmlTextWriter( sw );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );

				xml.WriteStartElement( "YourPersonalConfig" );

				xml.WriteStartElement( "StyleType" );
				xml.WriteString( ( (int) PersonalConfigList[0] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "BackgroundType" );
				xml.WriteString( ( (int) PersonalConfigList[1] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "ActiveTextEntryBackgroundType" );
				xml.WriteString( ( (int) PersonalConfigList[2] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "InactiveTextEntryBackgroundType" );
				xml.WriteString( ( (int) PersonalConfigList[3] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "DefaultTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[4] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "TitleTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[5] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MessagesTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[6] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "CommandButtonsTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[7] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "PageNumberTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[8] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "ActiveTextEntryTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[9] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "InactiveTextEntryTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[10] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "DirectoryTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[11] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "FileTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[12] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "KnownFileTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[13] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "FlagTextColor" );
				xml.WriteString( ( (int) PersonalConfigList[14] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchOptions" );

				xml.WriteStartElement( "SortSearchFor" );
				xml.WriteString( sortSearchFor );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchType" );
				xml.WriteString( ( (int) sortSearchType ).ToString()  );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortOrder" );
				xml.WriteString( ( (int) sortOrder ).ToString()  );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortType" );
				xml.WriteString( ( (int) sortType ).ToString()  );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchCaseSensitive" );
				xml.WriteString( sortSearchCaseSensitive.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchFlagged" );
				xml.WriteString( sortSearchFlagged.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchBadLocation" );
				xml.WriteString( sortSearchBadLocation.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchDupeSpawners" );
				xml.WriteString( sortSearchDupeSpawners.ToString() );
				xml.WriteEndElement();

				xml.WriteEndElement();

				xml.WriteEndElement();

				xml.Close();
			}
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

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
			{
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );
				AddButton( 142, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
				AddButton( 184, 140, 2104, 2103, -102, GumpButtonType.Reply, 0 );
				AddButton( 191, 340, 2104, 2103, -103, GumpButtonType.Reply, 0 );
				AddButton( 472, 140, 2104, 2103, -104, GumpButtonType.Reply, 0 );
				AddButton( 470, 320, 2104, 2103, -105, GumpButtonType.Reply, 0 );
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

			switch ( pg )
			{
				case 1:
				{
					AddHtml( 101, 101, 100, 20, MC.ColorText( defaultTextColor, "Search:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 200, 101, 320, 18 );
					AddTextEntry( 200, 101, 320, 15, ActiveTextEntryTextColor, 1, sortSearchFor );

					AddHtml( 101, 141, 300, 20, MC.ColorText( defaultTextColor, "Search Terms:" ), false, false );

					AddHtml( 141, 161, 300, 20, MC.ColorText( defaultTextColor, "None" ), false, false );
					AddRadio( 100, 160, 9026, 9027, sortSearchType == SortSearchType.None, 1 );

					AddHtml( 141, 181, 300, 20, MC.ColorText( defaultTextColor, "Spawner Name" ), false, false );
					AddRadio( 100, 180, 9026, 9027, sortSearchType == SortSearchType.Name, 2 );

					AddHtml( 141, 201, 300, 20, MC.ColorText( defaultTextColor, "Facet" ), false, false );
					AddRadio( 100, 200, 9026, 9027, sortSearchType == SortSearchType.Facet, 3 );

					AddHtml( 141, 221, 300, 20, MC.ColorText( defaultTextColor, "Location" ), false, false );
					AddRadio( 100, 220, 9026, 9027, sortSearchType == SortSearchType.Location, 4 );

					AddHtml( 141, 241, 300, 20, MC.ColorText( defaultTextColor, "Location X-axis" ), false, false );
					AddRadio( 100, 240, 9026, 9027, sortSearchType == SortSearchType.LocationX, 5 );

					AddHtml( 141, 261, 300, 20, MC.ColorText( defaultTextColor, "Location Y-axis" ), false, false );
					AddRadio( 100, 260, 9026, 9027, sortSearchType == SortSearchType.LocationY, 6 );

					AddHtml( 141, 281, 300, 20, MC.ColorText( defaultTextColor, "Location Z-axis" ), false, false );
					AddRadio( 100, 280, 9026, 9027, sortSearchType == SortSearchType.LocationZ, 7 );

					AddHtml( 141, 301, 300, 20, MC.ColorText( defaultTextColor, "Entry Name" ), false, false );
					AddRadio( 100, 300, 9026, 9027, sortSearchType == SortSearchType.EntryName, 8 );

					AddHtml( 401, 141, 300, 20, MC.ColorText( defaultTextColor, "Search Options:" ), false, false );

					AddHtml( 441, 161, 300, 20, MC.ColorText( defaultTextColor, "Case Sensitive" ), false, false );
					AddCheck( 400, 160, 9026, 9027, sortSearchCaseSensitive, 9 );

					AddHtml( 441, 181, 300, 20, MC.ColorText( defaultTextColor, "Flagged Spawners" ), false, false );
					AddCheck( 400, 180, 9026, 9027, sortSearchFlagged, 10 );

					AddHtml( 441, 201, 300, 20, MC.ColorText( defaultTextColor, "Spawners In Bad Location" ), false, false );
					AddCheck( 400, 200, 9026, 9027, sortSearchBadLocation, 11 );

					AddHtml( 441, 221, 300, 20, MC.ColorText( defaultTextColor, "Duped Spawners" ), false, false );
					AddCheck( 400, 220, 9026, 9027, sortSearchDupeSpawners, 12 );

					break;
				}
				case 2:
				{
					AddGroup( 1 );

					AddHtml( 101, 101, 300, 20, MC.ColorText( defaultTextColor, "Sort Terms:" ), false, false );

					AddHtml( 141, 121, 300, 20, MC.ColorText( defaultTextColor, "None" ), false, false );
					AddRadio( 100, 120, 9026, 9027, sortType == SortSearchType.None, 1 );

					AddHtml( 141, 141, 300, 20, MC.ColorText( defaultTextColor, "Spawner Name" ), false, false );
					AddRadio( 100, 140, 9026, 9027, sortType == SortSearchType.Name, 2 );

					AddHtml( 141, 161, 300, 20, MC.ColorText( defaultTextColor, "Facet" ), false, false );
					AddRadio( 100, 160, 9026, 9027, sortType == SortSearchType.Facet, 3 );

					AddHtml( 141, 181, 300, 20, MC.ColorText( defaultTextColor, "Location" ), false, false );
					AddRadio( 100, 180, 9026, 9027, sortType == SortSearchType.Location, 4 );

					AddHtml( 141, 201, 300, 20, MC.ColorText( defaultTextColor, "Location X-axis" ), false, false );
					AddRadio( 100, 200, 9026, 9027, sortType == SortSearchType.LocationX, 5 );

					AddHtml( 141, 221, 300, 20, MC.ColorText( defaultTextColor, "Location Y-axis" ), false, false );
					AddRadio( 100, 220, 9026, 9027, sortType == SortSearchType.LocationY, 6 );

					AddHtml( 141, 241, 300, 20, MC.ColorText( defaultTextColor, "Location Z-axis" ), false, false );
					AddRadio( 100, 240, 9026, 9027, sortType == SortSearchType.LocationZ, 7 );

					AddGroup( 2 );

					AddHtml( 101, 281, 300, 20, MC.ColorText( defaultTextColor, "Sort Order:" ), false, false );

					AddHtml( 141, 301, 300, 20, MC.ColorText( defaultTextColor, "Ascending" ), false, false );
					AddRadio( 100, 300, 9026, 9027, sortOrder == SortOrder.Ascending, 8 );

					AddHtml( 141, 321, 300, 20, MC.ColorText( defaultTextColor, "Descending" ), false, false );
					AddRadio( 100, 320, 9026, 9027, sortOrder == SortOrder.Descending, 9 );

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
			AddStaticButtons();
		}

		private void AddStaticButtons()
		{
			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Reset" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Submit" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );
		}

		private void SubmitSort( RelayInfo info, bool submit, int command )
		{
			string oldMessages = Messages;

			if ( submit )
			{
				MessagesTitle = "Search And Sort";
				Messages = "Search And Sort options have been submitted.";
			}

			string checkSortSearchFor=null;

			if ( pg == 1 )
			{
				checkSortSearchFor = info.GetTextEntry( 1 ).Text;

				int switchNum = MC.GetSwitchNum( info, 1, 8 );
				sortSearchType = (SortSearchType) switchNum;

				sortSearchCaseSensitive = info.IsSwitched( 9 );
				sortSearchFlagged = info.IsSwitched( 10 );
				sortSearchBadLocation = info.IsSwitched( 11 );
				sortSearchDupeSpawners = info.IsSwitched( 12 );
			}
			else
			{
				checkSortSearchFor = sortSearchFor;
			}

			if ( pg == 2 )
			{
				int switchNum = MC.GetSwitchNum( info, 1, 7 );
				sortType = (SortSearchType) switchNum;

				switchNum = MC.GetSwitchNum( info, 8, 9 );

				switch( switchNum )
				{
					case 0:{ sortOrder = SortOrder.Ascending; break; }
					case 1:{ sortOrder = SortOrder.Descending; break; }
				}
			}

			if ( submit )
			{
				sortSearchFor = checkSortSearchFor;

				SetArgsList();

				ArgsList[34] = (bool) true;		// RefreshSpawnerLists

				WriteConfig();

				gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );
			}
			else
			{
				UpdatePage( command );
			}
		}
	}
}