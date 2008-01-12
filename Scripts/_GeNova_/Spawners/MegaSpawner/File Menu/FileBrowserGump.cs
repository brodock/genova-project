using System;
using System.Collections;
using System.IO;
using Server;
using Server.Mobiles;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class FileBrowserGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private string FileName;
		private ArrayList PageInfoList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor, DirectoryTextColor, FileTextColor, KnownFileTextColor;

		private string ErrorLog = null;
		private int amountOfSpawners, totalErrors;

		private string[] SubDirEntries, FileEntries;
		private ArrayList TotalEntries = new ArrayList();
		private string DirectoryPath;

		private const int numOffset = 1000;
		private const int amtPerPg = 13;
		private const int listSpace = 286;
		private int totalPages;
		private Mobile gumpMobile;

		public FileBrowserGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, "File Browser" ), false, false );

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

			GetDirectoryInfo();

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			int index = (int) info.ButtonID - numOffset;

			string entry = null;

			if ( index > 0 )
				entry = (string) TotalEntries[index];
			else if ( index == 0 )
				entry = GetPreviousDirectory( DirectoryPath );

			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					OpenGump();

					break;
				}
				case -101: // Directory
				{
					MessagesTitle = "Help: Directory";
					Messages = "This is the directory of which you are browsing using the file browser. Directory info that is previous to the base RunUO directory is cropped for protection purposes.";

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

					SetArgsList();

					gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );

					break;
				}
				default:
				{
					if ( Help )
					{
						MessagesTitle = "Help: Browse Button";
						Messages = "That button will open a subdirectory to browse or open a file to load.";

						OpenGump();

						break;
					}

					pg = 1;

					SetArgsList();

					Browse( entry, index );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new FileBrowserGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[10] = cpg;
			PageInfoList[11] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[7] = FileName;
			ArgsList[9] = DirectoryPath;
			ArgsList[12] = PageInfoList;
			ArgsList[13] = MSGCheckBoxesList;
		}

		private void GetArgsList()
		{
			Help = (bool)							ArgsList[0];
			DisplayMessages = (bool)					ArgsList[1];
			MessagesTitle = (string)					ArgsList[2];
			OldMessagesTitle = (string)					ArgsList[3];
			Messages = (string)						ArgsList[4];
			OldMessages = (string)						ArgsList[5];
			HideSpawnerList = (ArrayList)					ArgsList[6];
			FileName = (string)						ArgsList[7];
			DirectoryPath = (string)					ArgsList[9];
			PageInfoList = (ArrayList)					ArgsList[12];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			cpg = (int)							PageInfoList[10];
			pg = (int)							PageInfoList[11];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)				PersonalConfigList[7];
			PageNumberTextColor = (int)					PersonalConfigList[8];
			DirectoryTextColor = (int)					PersonalConfigList[11];
			FileTextColor = (int)						PersonalConfigList[12];
			KnownFileTextColor = (int)					PersonalConfigList[13];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
		}

		private void GetDirectoryInfo()
		{
			string path = Core.BaseDirectory;

			if ( DirectoryPath == "" )
			{
				SubDirEntries = Directory.GetDirectories( path );
				FileEntries = Directory.GetFiles( path );

				DirectoryPath = path;
			}
			else
			{
				SubDirEntries = Directory.GetDirectories( DirectoryPath );
				FileEntries = Directory.GetFiles( DirectoryPath );
			}

			TotalEntries.Add( "" );

			for ( int i = 0; i < SubDirEntries.Length; i++ )
				TotalEntries.Add( SubDirEntries[i] );

			for ( int i = 0; i < FileEntries.Length; i++ )
				TotalEntries.Add( FileEntries[i] );
		}

		private string CropDirectory( string entry )
		{
			return MC.CropDirectory( entry );
		}

		private string CropCoreDirectory( string entry )
		{
			return MC.CropCoreDirectory( entry );
		}

		private string GetPreviousDirectory( string entry )
		{
			return MC.GetPreviousDirectory( entry );
		}

		private void Browse( string entry, int index )
		{
			if ( index == 0 )
			{
				if ( !CheckPreventCoreCDUP( entry ) )
				{
					DirectoryPath = entry;
				}
				else
				{
					MessagesTitle = "File Browser";
					Messages = "You may not browse to the previous directory before the base RunUO directory.";
				}

				OpenGump();

				return;
			}

			for ( int i = 0; i < SubDirEntries.Length; i++ )
			{
				if ( entry == SubDirEntries[i] )
				{
					if ( !Directory.Exists( entry ) )
					{
						MessagesTitle = "File Browser";
						Messages = "That directory no longer exists.";
					}
					else
					{
						DirectoryPath = entry;
					}

					OpenGump();

					break;
				}
			}

			for ( int i = 0; i < FileEntries.Length; i++ )
			{
				if ( entry == FileEntries[i] )
				{
					if ( !File.Exists( entry ) )
					{
						MessagesTitle = "File Browser";
						Messages = "That file no longer exists.";

						OpenGump();

						break;
					}

					LoadType loadType = MC.GetLoadType( entry );

					if ( loadType == LoadType.Error )
					{
						MessagesTitle = "File Browser";
						Messages = "That file type is unknown and cannot be loaded.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new LoadFileGump( gumpMobile, ArgsList, loadType, (string) TotalEntries[index] ) );

					break;
				}
			}
		}

		private bool CheckPreventCoreCDUP( string entry )
		{
			string[] checkEntry = entry.Split('\\');
			string[] checkCore = Core.BaseDirectory.Split('\\');

			if ( checkEntry.Length < checkCore.Length )
				return true;

			return false;
		}

		private bool IsDirectory( int index )
		{
			if ( index <= SubDirEntries.Length )
				return true;
			else
				return false;
		}

		private bool IsKnownExtension( string entry )
		{
			return MC.IsKnownExtension( entry );
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( TotalEntries.Count - 2 ) / amtPerPg ) + 1;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( TotalEntries.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no files or directories to display." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 20 );

				if ( Help )
				{
					AddButton( 130, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
				}

				AddHtml( 140, 101, 520, 20, MC.ColorTextDir( defaultTextColor, String.Format( "Directory: {0}", CropCoreDirectory( DirectoryPath ) ) ), false, false );

				SetPage();
			}
		}

		private void SetPage()
		{
			int offsetMin, offsetMax;
			int listY = 144;
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

			MC.DisplayBackground( this, BackgroundTypeConfig, 100, 122, 560, 21 );

			AddLabelCropped( 140, 122, 520, 20, DirectoryTextColor, "CDUP (Go To Previous Directory)" );
			AddButton( 100, 122, 9904, 9905, numOffset, GumpButtonType.Reply, 0 );

			offsetMin = ( ( pg - 1 ) * amtPerPg ) + 1;
			offsetMax = ( pg * amtPerPg );

			if ( offsetMax >= TotalEntries.Count )
				offsetMax = TotalEntries.Count - 1;

			for ( int i = offsetMin; i <= offsetMax; i++ )
			{
				string entry = null;

				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 560, 20 );

				entry = CropDirectory( (string) TotalEntries[i] );

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );

				if ( IsDirectory( i ) )
					AddLabelCropped( 140, listY, 520, 20, DirectoryTextColor, entry );
				else if ( IsKnownExtension( entry ) )
					AddLabelCropped( 140, listY, 520, 20, KnownFileTextColor, entry );
				else
					AddLabelCropped( 140, listY, 520, 20, FileTextColor, entry );

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
		}
	}
}