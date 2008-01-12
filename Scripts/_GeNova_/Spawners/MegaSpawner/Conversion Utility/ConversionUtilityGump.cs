using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConversionUtilityGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList CUGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor;

		public ArrayList SpawnerList = new ArrayList();

		private int offsetMin, offsetMax;

		private const int numOffset = 1000;
		private const int amtPerPg = 14;
		private const int listSpace = 308;
		private int totalPages;
		private Mobile gumpMobile;

		public ConversionUtilityGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			AddHtml( 121, 81, 560, 20, MC.ColorText( titleTextColor, "Conversion Utility" ), false, false );

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

			CompileSpawnerList();
			SetCUGCheckBoxes();

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			SaveCheckBoxes( info );

			int index = (int) info.ButtonID - numOffset;

			Item spawner = null;

			if ( index >= 0 )
				spawner = (Item) SpawnerList[index];

			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					OpenGump();

					break;
				}
				case -101: // Name
				{
					MessagesTitle = "Help: Name Column";
					Messages = "This column lists the name of each spawner.";

					OpenGump();

					break;
				}
				case -102: // Facet
				{
					MessagesTitle = "Help: Facet Column";
					Messages = "This column lists the facet of each spawner.";

					OpenGump();

					break;
				}
				case -103: // Location
				{
					MessagesTitle = "Help: Location Column";
					Messages = "This column lists the location of each spawner.";

					OpenGump();

					break;
				}
				case -104: // Spawner Type
				{
					MessagesTitle = "Help: Spawner Type Column";
					Messages = "This column lists the type of each spawner.";

					OpenGump();

					break;
				}
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount of spawners, to perform certain commands on.\n\nFor Example: You select a few spawners, then choose [Convert Selected Spawners], you will convert the selected spawners only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( (bool) CUGCheckBoxesList[i] )
							chkd++;
						else
							unchkd++;
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( chkd > unchkd )
							CUGCheckBoxesList[i] = (bool) false;
						else if ( unchkd > chkd )
							CUGCheckBoxesList[i] = (bool) true;
						else
							CUGCheckBoxesList[i] = (bool) false;
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

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Convert Selected Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Convert Selected Spawners Button";
						Messages = "That button will convert all selected spawners to Mega Spawners.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Convert Selected Spawners";
						Messages = "There are not any non-Mega Spawner spawners on the entire server.";

						OpenGump();

						break;
					}

					CheckSpawners();

					bool selected = false;

					for ( int i = 0; i < SpawnerList.Count; i++ )
					{
						if ( (bool) CUGCheckBoxesList[i] )
							selected = true;
					}

					if ( !selected )
					{
						MessagesTitle = "Convert Selected Spawners";
						Messages = "There are not any non-Mega Spawner spawners selected.";

						OpenGump();

						break;
					}

					ArgsList[33] = SpawnerList;			// ConvertSpawnersList

					gumpMobile.SendGump( new ConfirmConvertSpawnersGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				case 2: // Convert All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Convert All Spawners Button";
						Messages = "That button will convert all spawners to Mega Spawners.";

						OpenGump();

						break;
					}

					if ( CheckProcess() )
						break;

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Convert All Spawners";
						Messages = "There are not any non-Mega Spawner spawners on the entire server.";

						OpenGump();

						break;
					}

					CheckSpawners();

					ArgsList[33] = SpawnerList;			// ConvertSpawnersList

					gumpMobile.SendGump( new ConfirmConvertSpawnersGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				case 3: // Select All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Select All Spawners Button";
						Messages = "That button will select all spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Select All Spawners";
						Messages = "There are no spawners to select.";

						OpenGump();

						break;
					}

					CheckSpawners();

					CUGCheckBoxesList = new ArrayList( SpawnerList.Count );

					for ( int i = 0; i < SpawnerList.Count; i++ )
						CUGCheckBoxesList.Add( (bool) true );

					MessagesTitle = "Select All Spawners";
					Messages = "All spawners have been selected.";

					OpenGump();

					break;
				}
				case 4: // Deselect All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Deselect All Spawners Button";
						Messages = "That button will deselect all spawners on the spawner list.";

						OpenGump();

						break;
					}

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Deselect All Spawners";
						Messages = "There are no spawners to select.";

						OpenGump();

						break;
					}

					CheckSpawners();

					CUGCheckBoxesList = new ArrayList( SpawnerList.Count );

					for ( int i = 0; i < SpawnerList.Count; i++ )
						CUGCheckBoxesList.Add( (bool) false );

					MessagesTitle = "Deselect All Spawners";
					Messages = "All spawners have been deselected.";

					OpenGump();

					break;
				}
				default:
				{
					if ( Help )
					{
						MessagesTitle = "Help: Spawner Info Button";
						Messages = "That button will open the selected spawner and allow you to view information on it.";

						OpenGump();

						break;
					}


					OpenGump();
					gumpMobile.SendGump( new PropertiesGump( gumpMobile, spawner ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new ConversionUtilityGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[8] = cpg;
			PageInfoList[9] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[16] = CUGCheckBoxesList;
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
			CUGCheckBoxesList = (ArrayList)					ArgsList[16];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			cpg = (int) 							PageInfoList[8];
			pg = (int) 							PageInfoList[9];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)				PersonalConfigList[7];
			PageNumberTextColor = (int)					PersonalConfigList[8];
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
			GetOffsets();

			for ( int i = offsetMin; i <= offsetMax; i++ )
				CUGCheckBoxesList[i] = info.IsSwitched( i );
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

		private void SetCUGCheckBoxes()
		{
			for( int i = 0; i < SpawnerList.Count; i++ )
				CUGCheckBoxesList.Add( (bool) false );
		}

		private void CompileSpawnerList()
		{
			SpawnerList.Clear();

			foreach( Item item in World.Items.Values )
			{
				if ( item is Spawner && !item.Deleted )
					SpawnerList.Add( item );
			}
		}

		public void CheckSpawners()
		{
			for ( int i = 0; i < SpawnerList.Count; i++ )
			{
				Item spawner = (Item) SpawnerList[i];

				if ( spawner.Deleted )
				{
					SpawnerList.RemoveAt(i);
					i--;
				}
			}
		}

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= SpawnerList.Count )
				offsetMax = SpawnerList.Count - 1;
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( SpawnerList.Count - 1 ) / amtPerPg ) + 1;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( SpawnerList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no spawners on the entire server that are not Mega Spawners." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 198, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 300, 100, 53, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 355, 100, 113, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 470, 100, 168, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 640, 100, 20, 20 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
					AddButton( 193, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
					AddButton( 321, 100, 2104, 2103, -102, GumpButtonType.Reply, 0 );
					AddButton( 406, 100, 2104, 2103, -103, GumpButtonType.Reply, 0 );
					AddButton( 548, 100, 2104, 2103, -104, GumpButtonType.Reply, 0 );
				}

				AddHtml( 100, 100, 200, 20, MC.ColorText( defaultTextColor, "<center>Name</center>" ), false, false );
				AddHtml( 300, 100, 55, 20, MC.ColorText( defaultTextColor, "<center>Facet</center>" ), false, false );
				AddHtml( 355, 100, 115, 20, MC.ColorText( defaultTextColor, "<center>Location</center>" ), false, false );
				AddHtml( 470, 100, 170, 20, MC.ColorText( defaultTextColor, "<center>Spawner Type</center>" ), false, false );

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
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 198, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 300, listY, 53, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 355, listY, 113, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 470, listY, 168, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 640, listY, 20, 20 );

				Item spawner = (Item) SpawnerList[i];

				if ( spawner is Spawner )
					spawner = (Spawner) spawner;

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );
				AddHtml( 141, listY + 1, 200, 20, MC.ColorText( defaultTextColor, spawner.Name ), false, false );
				AddHtml( 301, listY + 1, 55, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", spawner.Map.ToString() ) ), false, false );
				AddHtml( 356, listY + 1, 115, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", spawner.Location.ToString() ) ), false, false );

				if ( spawner is Spawner )
					AddHtml( 471, listY + 1, 170, 20, MC.ColorText( defaultTextColor, "<center>Distro Spawner</center>" ), false, false );

				AddCheck( 640, listY, 210, 211, (bool) CUGCheckBoxesList[i], i );

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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Convert Selected Spawners" ), false, false );
			AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Convert All Spawners" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Select All Spawners" ), false, false );
			AddButton( 350, 450, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Deselect All Spawners" ), false, false );
			AddButton( 350, 470, 9904, 9905, 4, GumpButtonType.Reply, 0 );
		}
	}
}