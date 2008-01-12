using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class FileMenuGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private string FileName;
		private ArrayList PageInfoList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList FMCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor;

		private static ArrayList SpawnerList = new ArrayList();

		private int offsetMin, offsetMax;

		private const int numOffset = 1000;
		private const int amtPerPg = 14;
		private const int listSpace = 308;
		private int totalPages;
		private Mobile gumpMobile;

		public FileMenuGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			ArgsList[34] = (bool) true;		// RefreshSpawnerLists

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
			AddHtml( 121, 81, 560, 20, MC.ColorText( titleTextColor, "File Menu" ), false, false );

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

			SetFMCheckBoxes();

			AddPages();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			SaveCheckBoxes( info );

			int index = (int) info.ButtonID - numOffset;

			if ( index >= 0 )
				FileName = (string) MC.FileImportList[index];

			switch ( info.ButtonID )
			{
				case -100: // Page Number
				{
					MessagesTitle = "Help: Page Number";
					Messages = "This is where the current page number out of total pages is shown. You may delete the page number and type in a page you would like to go to, then click refresh to jump to that page.\n\nFor Example:\nThe page number is 1/20, delete the 1/20 and type in 6, then click refresh. You will jump to page 6.";

					OpenGump();

					break;
				}
				case -101: // Files Loaded Into The Mega Spawner System
				{
					MessagesTitle = "Help: Files Loaded Into The Mega Spawner System";
					Messages = "This displays all files that are currently loaded into the Mega Spawner System.";

					OpenGump();

					break;
				}
				case -9: // Selection Checkbox
				{
					if ( Help )
					{
						MessagesTitle = "Help: Selection Checkbox Column";
						Messages = "This column of checkboxes allow you to select any amount loaded files, to perform certain commands on.\n\nFor Example: You select a couple loaded files, then choose [Unload Selected Files], you will unload the selected files only.";

						OpenGump();

						break;
					}

					int chkd=0, unchkd=0;

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( (bool) FMCheckBoxesList[i] )
							chkd++;
						else
							unchkd++;
					}

					for ( int i = offsetMin; i <= offsetMax; i++ )
					{
						if ( chkd > unchkd )
							FMCheckBoxesList[i] = (bool) false;
						else if ( unchkd > chkd )
							FMCheckBoxesList[i] = (bool) true;
						else
							FMCheckBoxesList[i] = (bool) false;
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

					ResetHideSpawnerList();

					FileName = "";

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Manual Load File
				{
					if ( Help )
					{
						MessagesTitle = "Help: Manual Load File Button";
						Messages = "That button will load a specified file if it is of a supported file type. Supported types are as follows:\n*.msf, *.mbk";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new LoadFileGump( gumpMobile, ArgsList, LoadType.Manual, "" ) );

					break;
				}
				case 2: // Unload Selected Files
				{
					if ( Help )
					{
						MessagesTitle = "Help: Unload Selected Files Button";
						Messages = "That button will unload all selected files from the Mega Spawner System.";

						OpenGump();

						break;
					}

					if ( MC.FileImportList.Count == 0 )
					{
						MessagesTitle = "Unload Selected Files";
						Messages = "There are no files currently loaded into the Mega Spawner System.";

						OpenGump();

						break;
					}

					bool found = false;

					for ( int i = 0; i < MC.FileImportList.Count; i++ )
					{
						if ( (bool) FMCheckBoxesList[i] )
						{
							found = true;

							break;
						}
					}

					if ( !found )
					{
						MessagesTitle = "Unload Selected Files";
						Messages = "There are no files selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmUnloadFilesGump( gumpMobile, ArgsList, 2 ) );

					break;
				}
				case 3: // Unload All Files
				{
					if ( Help )
					{
						MessagesTitle = "Help: Unload All Files Button";
						Messages = "That button will unload all files from the Mega Spawner System.";

						OpenGump();

						break;
					}

					if ( MC.FileImportList.Count == 0 )
					{
						MessagesTitle = "Unload All Files";
						Messages = "There are no files currently loaded into the Mega Spawner System.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new ConfirmUnloadFilesGump( gumpMobile, ArgsList, 1 ) );

					break;
				}
				case 4: // Open File Browser
				{
					if ( Help )
					{
						MessagesTitle = "Help: Open File Browser Button";
						Messages = "That button will open the file browser in which you may load files into the Mega Spawner System.";

						OpenGump();

						break;
					}

					PageInfoList[10] = 1;			// File Browser Gump Command Page
					PageInfoList[11] = 1;			// File Browser Gump Page

					SetArgsList();

					gumpMobile.SendGump( new FileBrowserGump( gumpMobile, ArgsList ) );

					break;
				}
				case 5: // Save Selected From Spawner List
				{
					if ( Help )
					{
						MessagesTitle = "Help: Save Selected From Spawner List Button";
						Messages = String.Format( "That button will save all selected Mega Spawners on the Mega Spawner list to a file with *.msf extension in your \"{0}\" directory.", MC.CropCoreDirectory( MC.SpawnerExportsDirectory ) );

						OpenGump();

						break;
					}

					CompileSpawnerList();
					CheckSpawners();

					if ( SpawnerList.Count == 0 )
					{
						MessagesTitle = "Save Selected From Spawner List";
						Messages = "There are no Mega Spawners on the Mega Spawner list.";

						OpenGump();

						break;
					}

					MC.CheckSpawners();

					bool found = false;

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
					{
						if ( (bool) MSGCheckBoxesList[i] )
							found = true;
					}

					if ( !found )
					{
						MessagesTitle = "Save Selected From Spawner List";
						Messages = "There are no Mega Spawners selected.";

						OpenGump();

						break;
					}

					gumpMobile.SendGump( new SaveFileGump( gumpMobile, ArgsList, SaveType.FromFileMenu, "" ) );

					break;
				}
				case 6: // Save All Spawners
				{
					if ( Help )
					{
						MessagesTitle = "Help: Save All Spawners Button";
						Messages = String.Format( "That button will save all Mega Spawners on the entire server to a file with *.msf extension in your \"{0}\" directory.", MC.CropCoreDirectory( MC.SpawnerExportsDirectory ) );

						OpenGump();

						break;
					}

					MC.CheckSpawners();

					if ( MC.SpawnerList.Count == 0 )
					{
						MessagesTitle = "Save All Spawners";
						Messages = "There are no Mega Spawners on the entire server.";

						OpenGump();

						break;
					}

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
						MSGCheckBoxesList[i] = (bool) true;

					gumpMobile.SendGump( new SaveFileGump( gumpMobile, ArgsList, SaveType.FromFileMenu, "" ) );

					break;
				}
				case 7: // New Spawner Workspace
				{
					if ( Help )
					{
						MessagesTitle = "Help: New Spawner Workspace Button";
						Messages = "That button will open a new Spawner Workspace allowing you to add spawners and set up a file to export. After you are done and choose to save your Spawner Workspace, all of the Mega Spawners will be removed.";

						OpenGump();

						break;
					}

					SetNewSpawnerWorkspaceList();

					FileName = "new";

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 10: // Create Backup File
				{
					if ( Help )
					{
						MessagesTitle = "Help: Create Backup File Button";
						Messages = String.Format( "That button will create a Mega Spawner backup file with *.mbk extension in your \"{0}\" directory.", MC.BackupDirectory );

						OpenGump();

						break;
					}

					MC.CheckSpawners();

					if ( MC.SpawnerList.Count == 0 )
					{
						MessagesTitle = "Create Backup File";
						Messages = "There are no Mega Spawners on the entire server.";

						OpenGump();

						break;
					}

					BackupSystem.SaveBackup( gumpMobile, ArgsList );

					break;
				}
				default:
				{
					if ( Help )
					{
						MessagesTitle = "Help: Filename Button";
						Messages = "That button will open the filename and allow you to edit the spawner list.";

						OpenGump();

						break;
					}

					CompileHideSpawnerList( FileName );

					PageInfoList[1] = 1;			// MegaSpawnerGumpPage

					SetArgsList();

					ArgsList[9] = "";			// DirectoryPath

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[6] = cpg;
			PageInfoList[7] = pg;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[7] = FileName;
			ArgsList[12] = PageInfoList;
			ArgsList[13] = MSGCheckBoxesList;
			ArgsList[15] = FMCheckBoxesList;
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
			PageInfoList = (ArrayList)					ArgsList[12];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			FMCheckBoxesList = (ArrayList)					ArgsList[15];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			cpg = (int) 							PageInfoList[6];
			pg = (int) 							PageInfoList[7];

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

			CheckArrayLists();

			for ( int i = offsetMin; i <= offsetMax; i++ )
				FMCheckBoxesList[i] = info.IsSwitched( i );
		}

		private void SetFMCheckBoxes()
		{
			for ( int i = 0; i < MC.FileImportList.Count; i++ )
				FMCheckBoxesList.Add( (bool) false );
		}

		private void CompileHideSpawnerList( string fileName )
		{
			MC.CheckSpawners();

			for ( int i = 0; i < MC.SpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

				if ( megaSpawner.Imported == fileName )
				{
					HideSpawnerList[i] = (bool) false;

					megaSpawner.FileEdit = gumpMobile;

					MC.AddToFileEdit( gumpMobile, megaSpawner );
				}
				else
				{
					HideSpawnerList[i] = (bool) true;
				}

				MSGCheckBoxesList.Add( (bool) false );
			}
		}

		private void ResetHideSpawnerList()
		{
			MC.CheckSpawners();

			for ( int i = 0; i < MC.SpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

				HideSpawnerList[i] = (bool) false;
			}
		}

		private void SetNewSpawnerWorkspaceList()
		{
			MC.CheckSpawners();

			for ( int i = 0; i < MC.SpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

				HideSpawnerList[i] = (bool) true;
			}
		}

		private void CompileSpawnerList()
		{
			SpawnerList.Clear();
			MC.CheckSpawners();

			for ( int i = 0; i < MC.SpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

				if ( !(bool) HideSpawnerList[i] )
					SpawnerList.Add( megaSpawner );
			}
		}

		private void CheckSpawners()
		{
			for ( int i = 0; i < SpawnerList.Count; i++ )
			{
				MegaSpawner megaSpawner = (MegaSpawner) SpawnerList[i];

				if ( megaSpawner.Deleted )
				{
					SpawnerList.RemoveAt(i);

					i--;
				}
			}
		}

		private void CheckArrayLists()
		{
			if ( MC.FileImportList.Count > FMCheckBoxesList.Count )
			{
				int diff = ( MC.FileImportList.Count - FMCheckBoxesList.Count );

				for ( int i = 0; i < diff; i++ )
					FMCheckBoxesList.Add( (bool) false );
			}

			SetArgsList();
		}

		private void GetOffsets()
		{
			offsetMin = ( pg - 1 ) * amtPerPg;
			offsetMax = ( pg * amtPerPg ) - 1;

			if ( offsetMax >= MC.FileImportList.Count )
				offsetMax = MC.FileImportList.Count - 1;
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = (int) ( ( MC.FileImportList.Count - 1 ) / amtPerPg ) + 1;

			string pageNum = String.Format( "{0}/{1}", pg, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			if ( MC.FileImportList.Count == 0 )
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 560, 330 );

				AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "There are no files on the import file list." ), false, false );
			}
			else
			{
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, 100, 438, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 540, 100, 98, 20 );

				AddButton( 640, 100, 211, 210, -9, GumpButtonType.Reply, 0 );

				if ( Help )
				{
					AddButton( 364, 100, 2104, 2103, -101, GumpButtonType.Reply, 0 );
				}

				AddHtml( 101, 101, 440, 20, MC.ColorText( defaultTextColor, "<center>File Name</center>" ), false, false );
				AddHtml( 541, 101, 100, 20, MC.ColorText( defaultTextColor, "<center>File Version</center>" ), false, false );

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
				MC.DisplayBackground( this, BackgroundTypeConfig, 100, listY, 438, 20 );
				MC.DisplayBackground( this, BackgroundTypeConfig, 540, listY, 98, 20 );

				string fileName = (string) MC.FileImportList[i];
				string fileVersion = (string) MC.FileImportVersionList[i];

				if ( fileName == "" || fileName == null )
					fileName = "Bad FileName";

				AddButton( 100, listY, 9904, 9905, i + numOffset, GumpButtonType.Reply, 0 );
				AddHtml( 141, listY + 1, 440, 20, MC.ColorText( defaultTextColor, fileName ), false, false );
				AddHtml( 541, listY + 1, 100, 20, MC.ColorText( defaultTextColor, String.Format( "<center>{0}</center>", fileVersion ) ), false, false );

				AddCheck( 640, listY, 210, 211, (bool) FMCheckBoxesList[i], i );

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
				case 2:{ AddButtonsPageTwo(); break; }
			}

			if ( cpg != 2 ) // Next Command Page
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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Manual Load File" ), false, false );
			AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Unload Selected Files" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Unload All Files" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Open File Browser" ), false, false );
			AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddHtml( 391, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Save Selected From Spawner List" ), false, false );
			AddButton( 350, 470, 9904, 9905, 5, GumpButtonType.Reply, 0 );

			AddHtml( 391, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Save All Spawners" ), false, false );
			AddButton( 350, 490, 9904, 9905, 6, GumpButtonType.Reply, 0 );
		}

		private void AddButtonsPageTwo()
		{
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "New Spawner Workspace" ), false, false );
			AddButton( 100, 450, 9904, 9905, 7, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Create Backup File" ), false, false );
			AddButton( 350, 450, 9904, 9905, 10, GumpButtonType.Reply, 0 );
		}
	}
}