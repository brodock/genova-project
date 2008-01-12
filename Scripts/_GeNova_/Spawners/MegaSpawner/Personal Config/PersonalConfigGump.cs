using System;
using System.IO;
using System.Xml;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Gumps;
using Server.Accounting;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class PersonalConfigGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList PCGArgsList = new ArrayList();
		private ArrayList PCGSetList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig, InactiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor, FlagTextColor;

		private StyleType styleTypeSwitch;
		private BackgroundType backgroundTypeSwitch, activeTEBGTypeSwitch, inactiveTEBGTypeSwitch;
		private TextColor defaultTextColorSwitch, titleTextColorSwitch, messagesTextColorSwitch, commandButtonsTextColorSwitch, flagTextColorSwitch;

		private bool StyleConfigClicked, TextColorConfigClicked;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor, InactiveTextEntryTextColor, DirectoryTextColor, FileTextColor, KnownFileTextColor;

		private int totalPages, pgOffset, pgNum;
		private RelayInfo relayInfo;
		private string ErrorLog = null;
		private int totalErrors;
		private Mobile gumpMobile;

		public PersonalConfigGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, "Your Personal Configuration" ), false, false );

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

						SubmitConfig( info, false, 0 );

						break;
					}

					DisplayMessages = !DisplayMessages;

					SubmitConfig( info, false, 0 );

					break;
				}
				case -7: // Refresh
				{
					if ( Help )
					{
						MessagesTitle = "Help: Refresh Button";
						Messages = "That button will refresh the current window and also change the page number if specified in the upper right hand corner. To change the page number, delete the current page number and type in the page you want to go to.";

						SubmitConfig( info, false, 0 );

						break;
					}

					relayInfo = info;

					SubmitConfig( info, false, 5 );

					break;
				}
				case -6: // Clear Messages
				{
					if ( Help )
					{
						MessagesTitle = "Help: Clear Messages Button";
						Messages = "That button will clear all the messages in the messages window.";

						SubmitConfig( info, false, 0 );

						break;
					}

					MessagesTitle = "Messages";
					Messages = null;

					SubmitConfig( info, false, 0 );

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

					SubmitConfig( info, false, 0 );

					break;
				}
				case -4: // Previous Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Page Button";
						Messages = "That button will switch to the previous page.";

						SubmitConfig( info, false, 0 );

						break;
					}

					SubmitConfig( info, false, 1 );

					break;
				}
				case -3: // Next Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Page Button";
						Messages = "That button will switch to the next page.";

						SubmitConfig( info, false, 0 );

						break;
					}

					SubmitConfig( info, false, 2 );

					break;
				}
				case -2: // Previous Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Previous Command Page Button";
						Messages = "That button will switch to the previous command list page.";

						SubmitConfig( info, false, 0 );

						break;
					}

					SubmitConfig( info, false, 3 );

					break;
				}
				case -1: // Next Command Page
				{
					if ( Help )
					{
						MessagesTitle = "Help: Next Command Page Button";
						Messages = "That button will switch to the next command list page.";

						SubmitConfig( info, false, 0 );

						break;
					}

					SubmitConfig( info, false, 4 );

					break;
				}
				case 0: // Close Gump
				{
					if ( Help )
					{
						MessagesTitle = "Help: Close Button";
						Messages = "That button will close the gump and return to the previous gump.";

						SubmitConfig( info, false, 0 );

						break;
					}

					gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Style Configuration
				{
					if ( Help )
					{
						MessagesTitle = "Help: Style Configuration Button";
						Messages = "That button will open the style configuration which allows you to choose from many different style settings.";

						SubmitConfig( info, false, 0 );

						break;
					}

					ResetSelectionClicks();

					pg = 2;
					StyleConfigClicked = true;

					OpenGump();

					break;
				}
				case 2: // Reset Configurations To Defaults
				{
					if ( Help )
					{
						MessagesTitle = "Help: Reset Configurations To Defaults Button";
						Messages = "That button will reset all configurations to their default settings.";

						SubmitConfig( info, false, 0 );

						break;
					}

					MC.DefaultPersonalConfigCreate( gumpMobile, ArgsList );
					ArgsList = MC.LoadPersonalConfig( gumpMobile, ArgsList );

					PCGArgsList[0] = PersonalConfigList[0];			// styleTypeSwitch
					PCGArgsList[1] = PersonalConfigList[1];			// backgroundTypeSwitch
					PCGArgsList[2] = PersonalConfigList[2];			// activeTEBGTypeSwitch
					PCGArgsList[3] = PersonalConfigList[3];			// inactiveTEBGTypeSwitch
					PCGArgsList[4] = PersonalConfigList[4];			// defaultTextColorSwitch
					PCGArgsList[5] = PersonalConfigList[5];			// titleTextColorSwitch
					PCGArgsList[6] = PersonalConfigList[6];			// messagesTextColorSwitch
					PCGArgsList[7] = PersonalConfigList[7];			// commandButtonsTextColorSwitch
					PCGArgsList[8] = PersonalConfigList[14];		// flagTextColorSwitch

					GetArgsList();

					MessagesTitle = "Reset Configurations To Defaults";
					Messages = "All configurations have been reset to their default settings.";

					OpenGump();

					break;
				}
				case 3: // Save Configurations
				{
					if ( Help )
					{
						MessagesTitle = "Help: Save Configurations Button";
						Messages = "That button will save all configurations.";

						SubmitConfig( info, false, 0 );

						break;
					}

					SubmitConfig( info, true, 0 );

					break;
				}
				case 4: // Text Color Configuration
				{
					if ( Help )
					{
						MessagesTitle = "Help: Text Color Configuration Button";
						Messages = "That button will open the text color configuration which allows you to choose from many different text color settings.";

						SubmitConfig( info, false, 0 );

						break;
					}

					ResetSelectionClicks();

					pg = 5;
					TextColorConfigClicked = true;

					OpenGump();

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new PersonalConfigGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[20] = cpg;
			PageInfoList[21] = pg;

			PCGArgsList[0] = styleTypeSwitch;
			PCGArgsList[1] = backgroundTypeSwitch;
			PCGArgsList[2] = activeTEBGTypeSwitch;
			PCGArgsList[3] = inactiveTEBGTypeSwitch;
			PCGArgsList[4] = defaultTextColorSwitch;
			PCGArgsList[5] = titleTextColorSwitch;
			PCGArgsList[6] = messagesTextColorSwitch;
			PCGArgsList[7] = commandButtonsTextColorSwitch;
			PCGArgsList[8] = flagTextColorSwitch;

			PCGSetList[0] = StyleConfigClicked;
			PCGSetList[1] = TextColorConfigClicked;

			PersonalConfigList[0] = StyleTypeConfig;
			PersonalConfigList[1] = BackgroundTypeConfig;
			PersonalConfigList[2] = ActiveTEBGTypeConfig;
			PersonalConfigList[3] = InactiveTEBGTypeConfig;
			PersonalConfigList[4] = DefaultTextColor;
			PersonalConfigList[5] = TitleTextColor;
			PersonalConfigList[6] = MessagesTextColor;
			PersonalConfigList[7] = CommandButtonsTextColor;
			PersonalConfigList[8] = PageNumberTextColor;
			PersonalConfigList[9] = ActiveTextEntryTextColor;
			PersonalConfigList[10] = InactiveTextEntryTextColor;
			PersonalConfigList[11] = DirectoryTextColor;
			PersonalConfigList[12] = FileTextColor;
			PersonalConfigList[13] = KnownFileTextColor;
			PersonalConfigList[14] = FlagTextColor;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[26] = PCGArgsList;
			ArgsList[27] = PCGSetList;
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
			PCGArgsList = (ArrayList)					ArgsList[26];
			PCGSetList = (ArrayList)					ArgsList[27];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			cpg = (int) 							PageInfoList[20];
			pg = (int) 							PageInfoList[21];

			styleTypeSwitch = (StyleType)					PCGArgsList[0];
			backgroundTypeSwitch = (BackgroundType)				PCGArgsList[1];
			activeTEBGTypeSwitch = (BackgroundType)				PCGArgsList[2];
			inactiveTEBGTypeSwitch = (BackgroundType)			PCGArgsList[3];
			defaultTextColorSwitch = (TextColor)				PCGArgsList[4];
			titleTextColorSwitch = (TextColor)				PCGArgsList[5];
			messagesTextColorSwitch = (TextColor)				PCGArgsList[6];
			commandButtonsTextColorSwitch = (TextColor)			PCGArgsList[7];
			flagTextColorSwitch = (TextColor)				PCGArgsList[8];

			StyleConfigClicked = (bool)					PCGSetList[0];
			TextColorConfigClicked = (bool)					PCGSetList[1];

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
			DirectoryTextColor = (int)					PersonalConfigList[11];
			FileTextColor = (int)						PersonalConfigList[12];
			KnownFileTextColor = (int)					PersonalConfigList[13];
			FlagTextColor = (TextColor)					PersonalConfigList[14];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
		}

		private void ResetSelectionClicks()
		{
			StyleConfigClicked = false;
			TextColorConfigClicked = false;
		}

		private int CalcPageEntry()
		{
			int intPage = 0;
			string checkPage = Convert.ToString( relayInfo.GetTextEntry( 0 ).Text ).ToLower();

			if ( checkPage == String.Format( "{0}/{1}", pgNum, totalPages ) )
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
				return intPage + pgOffset;
			}
			else
			{
				MessagesTitle = "Refresh";
				Messages = "That page number does not exist.";

				return pg;
			}
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
				xml.WriteString( ( (int) styleTypeSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "BackgroundType" );
				xml.WriteString( ( (int) backgroundTypeSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "ActiveTextEntryBackgroundType" );
				xml.WriteString( ( (int) activeTEBGTypeSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "InactiveTextEntryBackgroundType" );
				xml.WriteString( ( (int) inactiveTEBGTypeSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "DefaultTextColor" );
				xml.WriteString( ( (int) defaultTextColorSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "TitleTextColor" );
				xml.WriteString( ( (int) titleTextColorSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MessagesTextColor" );
				xml.WriteString( ( (int) messagesTextColorSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "CommandButtonsTextColor" );
				xml.WriteString( ( (int) commandButtonsTextColorSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "PageNumberTextColor" );
				xml.WriteString( PageNumberTextColor.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "ActiveTextEntryTextColor" );
				xml.WriteString( ActiveTextEntryTextColor.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "InactiveTextEntryTextColor" );
				xml.WriteString( InactiveTextEntryTextColor.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "DirectoryTextColor" );
				xml.WriteString( DirectoryTextColor.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "FileTextColor" );
				xml.WriteString( FileTextColor.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "KnownFileTextColor" );
				xml.WriteString( KnownFileTextColor.ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "FlagTextColor" );
				xml.WriteString( ( (int) flagTextColorSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchOptions" );

				xml.WriteStartElement( "SortSearchFor" );
				xml.WriteString( (string) PersonalConfigList[15] );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchType" );
				xml.WriteString( ( (int) PersonalConfigList[16] ).ToString()  );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortOrder" );
				xml.WriteString( ( (int) PersonalConfigList[17] ).ToString()  );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortType" );
				xml.WriteString( ( (int) PersonalConfigList[18] ).ToString()  );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchCaseSensitive" );
				xml.WriteString( ( (bool) PersonalConfigList[19] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchFlagged" );
				xml.WriteString( ( (bool) PersonalConfigList[20] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SortSearchBadLocation" );
				xml.WriteString( ( (bool) PersonalConfigList[21] ).ToString() );
				xml.WriteEndElement();

				xml.WriteEndElement();

				xml.WriteEndElement();

				xml.Close();
			}
		}

		private string GetInnerText( XmlElement node )
		{
			if ( node == null )
				return "Error";

			return node.InnerText;
		}

		private void AddToErrorLog( Exception ex )
		{
			if ( ErrorLog == null )
				ErrorLog = String.Format( "\nError #{0}:\n{1}", totalErrors, ex );
			else
				ErrorLog = String.Format( "{0}\n\nError #{1}:\n{2}", ErrorLog, totalErrors, ex );
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
		}

		private void AddPages()
		{
			AddCommandButtons();

			totalPages = 0;
			pgNum = 0;

			if ( pg == 1 )
			{
				totalPages = 1;
				pgNum = pg;
			}

			if ( StyleConfigClicked )
			{
				pgOffset = 1;
				totalPages = 3;
			}

			if ( TextColorConfigClicked )
			{
				pgOffset = 4;
				totalPages = 7;
			}

			pgNum = pg - pgOffset;

			string pageNum = String.Format( "{0}/{1}", pgNum, totalPages );

			AddLabel( 540, 80, PageNumberTextColor, "Page:" );
			AddTextEntry( 580, 80, 90, 20, PageNumberTextColor, 0, pageNum );

			if ( Help )
				AddButton( 529, 85, 2104, 2103, -100, GumpButtonType.Reply, 0 );

			SetPage();
		}

		private void SetPage()
		{
			if ( totalPages > pgNum )
			{
				AddHtml( 431, 431, 100, 20, MC.ColorText( defaultTextColor, String.Format( "Page {0}", ( pgNum + 1 ) ) ), false, false );
				AddButton( 380, 430, 4005, 4007, -3, GumpButtonType.Reply, 0 );
			}

			if ( pgNum > 1 )
			{
				int length = ( ( pg - 1 ).ToString().Length - 1 ) * 10;

				int pageX = 280 - length;

				AddHtml( pageX + 1, 431, 100, 20, MC.ColorText( defaultTextColor, String.Format( "Page {0}", ( pgNum - 1 ) ) ), false, false );
				AddButton( 340, 430, 4014, 4016, -4, GumpButtonType.Reply, 0 );
			}

			switch ( pg )
			{
				case 1:
				{
					AddHtml( 100, 100, 560, 330, MC.ColorText( defaultTextColor, "Select a configuration from the command window below." ), false, false );

					break;
				}
				case 2: // Style Configuration
				{
					string pageInfo = "<center>Style Configuration</center>\nThis is where you can choose your personal gump style used in the Mega Spawner System. This is a personal setting and only affects you.";
					pageInfo = String.Format( "{0} To test out a configuration before saving, choose your setting and hit refresh.\n\nGo to page 2 to continue.", pageInfo );

					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, pageInfo ), false, true );

					break;
				}
				case 3:
				{
					AddGroup( 1 );

					AddHtml( 141, 101, 200, 20, MC.ColorText( defaultTextColor, "<center>Style</center>" ), false, false );

					AddHtml( 141, 121, 200, 20, MC.ColorText( defaultTextColor, "Original Black" ), false, false );
					AddRadio( 100, 120, 210, 211, styleTypeSwitch == StyleType.OriginalBlack, 1 );

					AddHtml( 141, 141, 200, 20, MC.ColorText( defaultTextColor, "Distro Spawner Gray" ), false, false );
					AddRadio( 100, 140, 210, 211, styleTypeSwitch == StyleType.DistroGray, 2 );

					AddHtml( 141, 161, 200, 20, MC.ColorText( defaultTextColor, "Marble" ), false, false );
					AddRadio( 100, 160, 210, 211, styleTypeSwitch == StyleType.Marble, 3 );

					AddHtml( 141, 181, 200, 20, MC.ColorText( defaultTextColor, "Original Black Style With Border" ), false, false );
					AddRadio( 100, 180, 210, 211, styleTypeSwitch == StyleType.BlackBorder, 4 );

					AddHtml( 141, 201, 200, 20, MC.ColorText( defaultTextColor, "Gray With Embroidered Borders" ), false, false );
					AddRadio( 100, 200, 210, 211, styleTypeSwitch == StyleType.GrayEmbroidered, 5 );

					AddHtml( 141, 221, 200, 20, MC.ColorText( defaultTextColor, "Offwhite" ), false, false );
					AddRadio( 100, 220, 210, 211, styleTypeSwitch == StyleType.Offwhite, 6 );

					AddHtml( 141, 241, 200, 20, MC.ColorText( defaultTextColor, "Offwhite With Border" ), false, false );
					AddRadio( 100, 240, 210, 211, styleTypeSwitch == StyleType.OffwhiteBorder, 7 );

					AddHtml( 141, 261, 200, 20, MC.ColorText( defaultTextColor, "Gray Multi Border" ), false, false );
					AddRadio( 100, 260, 210, 211, styleTypeSwitch == StyleType.GrayMultiBorder, 8 );

					AddHtml( 141, 281, 200, 20, MC.ColorText( defaultTextColor, "Dark Gray" ), false, false );
					AddRadio( 100, 280, 210, 211, styleTypeSwitch == StyleType.DarkGray, 9 );

					AddHtml( 141, 301, 200, 20, MC.ColorText( defaultTextColor, "Scroll" ), false, false );
					AddRadio( 100, 300, 210, 211, styleTypeSwitch == StyleType.Scroll, 10 );

					AddGroup( 2 );

					AddHtml( 421, 101, 200, 20, MC.ColorText( defaultTextColor, "<center>Background</center>" ), false, false );

					AddHtml( 421, 121, 200, 20, MC.ColorText( defaultTextColor, "Alpha" ), false, false );
					AddRadio( 380, 120, 210, 211, backgroundTypeSwitch == BackgroundType.Alpha, 11 );

					AddHtml( 421, 141, 200, 20, MC.ColorText( defaultTextColor, "Black Alpha" ), false, false );
					AddRadio( 380, 140, 210, 211, backgroundTypeSwitch == BackgroundType.BlackAlpha, 12 );

					AddHtml( 421, 161, 200, 20, MC.ColorText( defaultTextColor, "Black" ), false, false );
					AddRadio( 380, 160, 210, 211, backgroundTypeSwitch == BackgroundType.Black, 13 );

					AddHtml( 421, 181, 200, 20, MC.ColorText( defaultTextColor, "Gray" ), false, false );
					AddRadio( 380, 180, 210, 211, backgroundTypeSwitch == BackgroundType.Gray, 14 );

					AddHtml( 421, 201, 200, 20, MC.ColorText( defaultTextColor, "Marble" ), false, false );
					AddRadio( 380, 200, 210, 211, backgroundTypeSwitch == BackgroundType.Marble, 15 );

					AddHtml( 421, 221, 200, 20, MC.ColorText( defaultTextColor, "Off White" ), false, false );
					AddRadio( 380, 220, 210, 211, backgroundTypeSwitch == BackgroundType.Offwhite, 16 );

					AddHtml( 421, 241, 200, 20, MC.ColorText( defaultTextColor, "Dark Gray" ), false, false );
					AddRadio( 380, 240, 210, 211, backgroundTypeSwitch == BackgroundType.DarkGray, 17 );

					AddHtml( 421, 261, 200, 20, MC.ColorText( defaultTextColor, "Scroll" ), false, false );
					AddRadio( 380, 260, 210, 211, backgroundTypeSwitch == BackgroundType.Scroll, 18 );

					break;
				}
				case 4:
				{
					AddGroup( 1 );

					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 100, 100, 280, 20 );
					AddHtml( 141, 101, 200, 20, MC.ColorText( defaultTextColor, "<center>Active Text Entry Background</center>" ), false, false );

					AddHtml( 141, 121, 200, 20, MC.ColorText( defaultTextColor, "Default Active" ), false, false );
					AddRadio( 100, 120, 210, 211, activeTEBGTypeSwitch == BackgroundType.ActiveTextEntry, 9 );

					AddHtml( 141, 141, 200, 20, MC.ColorText( defaultTextColor, "Alpha" ), false, false );
					AddRadio( 100, 140, 210, 211, activeTEBGTypeSwitch == BackgroundType.Alpha, 1 );

					AddHtml( 141, 161, 200, 20, MC.ColorText( defaultTextColor, "Black Alpha" ), false, false );
					AddRadio( 100, 160, 210, 211, activeTEBGTypeSwitch == BackgroundType.BlackAlpha, 2 );

					AddHtml( 141, 181, 200, 20, MC.ColorText( defaultTextColor, "Black" ), false, false );
					AddRadio( 100, 180, 210, 211, activeTEBGTypeSwitch == BackgroundType.Black, 3 );

					AddHtml( 141, 201, 200, 20, MC.ColorText( defaultTextColor, "Gray" ), false, false );
					AddRadio( 100, 200, 210, 211, activeTEBGTypeSwitch == BackgroundType.Gray, 4 );

					AddHtml( 141, 221, 200, 20, MC.ColorText( defaultTextColor, "Marble" ), false, false );
					AddRadio( 100, 220, 210, 211, activeTEBGTypeSwitch == BackgroundType.Marble, 5 );

					AddHtml( 141, 241, 200, 20, MC.ColorText( defaultTextColor, "Off White" ), false, false );
					AddRadio( 100, 240, 210, 211, activeTEBGTypeSwitch == BackgroundType.Offwhite, 6 );

					AddHtml( 141, 261, 200, 20, MC.ColorText( defaultTextColor, "Dark Gray" ), false, false );
					AddRadio( 100, 260, 210, 211, activeTEBGTypeSwitch == BackgroundType.DarkGray, 7 );

					AddHtml( 141, 281, 200, 20, MC.ColorText( defaultTextColor, "Scroll" ), false, false );
					AddRadio( 100, 280, 210, 211, activeTEBGTypeSwitch == BackgroundType.Scroll, 8 );

					AddGroup( 2 );

					MC.DisplayBackground( this, InactiveTEBGTypeConfig, 380, 100, 280, 20 );
					AddHtml( 421, 101, 200, 20, MC.ColorText( defaultTextColor, "<center>Inactive Text Entry Background</center>" ), false, false );

					AddHtml( 421, 121, 200, 20, MC.ColorText( defaultTextColor, "Default Inactive" ), false, false );
					AddRadio( 380, 120, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.InactiveTextEntry, 19 );

					AddHtml( 421, 141, 200, 20, MC.ColorText( defaultTextColor, "Alpha" ), false, false );
					AddRadio( 380, 140, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.Alpha, 10 );

					AddHtml( 421, 161, 200, 20, MC.ColorText( defaultTextColor, "Black Alpha" ), false, false );
					AddRadio( 380, 160, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.BlackAlpha, 11 );

					AddHtml( 421, 181, 200, 20, MC.ColorText( defaultTextColor, "Black" ), false, false );
					AddRadio( 380, 180, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.Black, 12 );

					AddHtml( 421, 201, 200, 20, MC.ColorText( defaultTextColor, "Gray" ), false, false );
					AddRadio( 380, 200, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.Gray, 13 );

					AddHtml( 421, 221, 200, 20, MC.ColorText( defaultTextColor, "Marble" ), false, false );
					AddRadio( 380, 220, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.Marble, 14 );

					AddHtml( 421, 241, 200, 20, MC.ColorText( defaultTextColor, "Off White" ), false, false );
					AddRadio( 380, 240, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.Offwhite, 15 );

					AddHtml( 421, 261, 200, 20, MC.ColorText( defaultTextColor, "Dark Gray" ), false, false );
					AddRadio( 380, 260, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.DarkGray, 16 );

					AddHtml( 421, 281, 200, 20, MC.ColorText( defaultTextColor, "Scroll" ), false, false );
					AddRadio( 380, 280, 210, 211, inactiveTEBGTypeSwitch == BackgroundType.Scroll, 17 );

					break;
				}
				case 5: // Text Color Configuration
				{
					string pageInfo = "<center>Text Color Configuration</center>\nThis is where you can choose from different text colors that are used across the gumps. This is a personal setting and only affects you.";
					pageInfo = String.Format( "{0} To test out a configuration before saving, choose your setting and hit refresh.\n\nGo to page 2 to continue.", pageInfo );

					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, pageInfo ), false, true );

					break;
				}
				case 6:
				{
					AddHtml( 101, 101, 560, 20, MC.ColorText( defaultTextColor, "<center>Default Text Color</center>" ), false, false );
					AddTextColorsPage( defaultTextColorSwitch );

					break;
				}
				case 7:
				{
					AddHtml( 101, 101, 560, 20, MC.ColorText( defaultTextColor, "<center>Title Text Color</center>" ), false, false );
					AddTextColorsPage( titleTextColorSwitch );

					break;
				}
				case 8:
				{
					AddHtml( 101, 101, 560, 20, MC.ColorText( defaultTextColor, "<center>Messages Text Color</center>" ), false, false );
					AddTextColorsPage( messagesTextColorSwitch );

					break;
				}
				case 9:
				{
					AddHtml( 101, 101, 560, 20, MC.ColorText( defaultTextColor, "<center>Command Buttons Text Color</center>" ), false, false );
					AddTextColorsPage( commandButtonsTextColorSwitch );

					break;
				}
				case 10:
				{
					AddHtml( 101, 101, 560, 20, MC.ColorText( defaultTextColor, "<center>Flag Text Color</center>" ), false, false );
					AddTextColorsPage( flagTextColorSwitch );

					break;
				}
				case 11:
				{
					AddHtml( 101, 101, 560, 20, MC.ColorText( defaultTextColor, "<center>Misc Text Colors</center>" ), false, false );

					AddLabel( 100, 120, PageNumberTextColor, "Page Number Text Color:" );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 121, 100, 18 );
					AddTextEntry( 300, 120, 300, 15, ActiveTextEntryTextColor, 1, PageNumberTextColor.ToString() );

					AddLabel( 100, 140, ActiveTextEntryTextColor, "Active Text Entry Text Color:" );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 141, 100, 18 );
					AddTextEntry( 300, 140, 300, 15, ActiveTextEntryTextColor, 2, ActiveTextEntryTextColor.ToString() );

					AddLabel( 100, 160, InactiveTextEntryTextColor, "Inactive Text Entry Text Color:" );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 161, 100, 18 );
					AddTextEntry( 300, 160, 300, 15, ActiveTextEntryTextColor, 3, InactiveTextEntryTextColor.ToString() );

					AddLabel( 100, 180, DirectoryTextColor, "Directory Text Color:" );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 181, 100, 18 );
					AddTextEntry( 300, 180, 300, 15, ActiveTextEntryTextColor, 4, DirectoryTextColor.ToString() );

					AddLabel( 100, 200, FileTextColor, "File Text Color:" );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 201, 100, 18 );
					AddTextEntry( 300, 200, 300, 15, ActiveTextEntryTextColor, 5, FileTextColor.ToString() );

					AddLabel( 100, 220, KnownFileTextColor, "Known File Text Color:" );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 221, 100, 18 );
					AddTextEntry( 300, 220, 300, 15, ActiveTextEntryTextColor, 6, KnownFileTextColor.ToString() );

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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Style Configuration" ), false, false );

			if ( StyleConfigClicked )
				AddButton( 100, 450, 5541, 5542, 1, GumpButtonType.Reply, 0 );
			else
				AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Text Color Configuration" ), false, false );

			if ( TextColorConfigClicked )
				AddButton( 350, 450, 5541, 5542, 4, GumpButtonType.Reply, 0 );
			else
				AddButton( 350, 450, 9904, 9905, 4, GumpButtonType.Reply, 0 );

			AddStaticButtons();
		}

		private void AddStaticButtons()
		{
			AddHtml( 141, 471, 300, 20, MC.ColorText( commandButtonsTextColor, "Reset Configurations To Defaults" ), false, false );
			AddButton( 100, 470, 9904, 9905, 2, GumpButtonType.Reply, 0 );

			AddHtml( 141, 491, 300, 20, MC.ColorText( commandButtonsTextColor, "Save Configurations" ), false, false );
			AddButton( 100, 490, 9904, 9905, 3, GumpButtonType.Reply, 0 );
		}

		private void AddTextColorsPage( TextColor textColorSwitch )
		{
			AddHtml( 101, 121, 160, 20, MC.ColorText( defaultTextColor, "<center>Purples</center>" ), false, false );

			AddHtml( 141, 141, 100, 20, MC.ColorText( MC.TextColorLightPurple, "Light Purple" ), false, false );
			AddRadio( 100, 140, 210, 211, textColorSwitch == TextColor.LightPurple, 1 );

			AddHtml( 141, 161, 100, 20, MC.ColorText( MC.TextColorPurple, "Purple" ), false, false );
			AddRadio( 100, 160, 210, 211, textColorSwitch == TextColor.Purple, 2 );

			AddHtml( 141, 181, 100, 20, MC.ColorText( MC.TextColorDarkPurple, "Dark Purple" ), false, false );
			AddRadio( 100, 180, 210, 211, textColorSwitch == TextColor.DarkPurple, 3 );

			AddHtml( 101, 221, 160, 20, MC.ColorText( defaultTextColor, "<center>Blues</center>" ), false, false );

			AddHtml( 141, 241, 100, 20, MC.ColorText( MC.TextColorLightBlue, "Light Blue" ), false, false );
			AddRadio( 100, 240, 210, 211, textColorSwitch == TextColor.LightBlue, 4 );

			AddHtml( 141, 261, 100, 20, MC.ColorText( MC.TextColorBlue, "Blue" ), false, false );
			AddRadio( 100, 260, 210, 211, textColorSwitch == TextColor.Blue, 5 );

			AddHtml( 141, 281, 100, 20, MC.ColorText( MC.TextColorDarkBlue, "Dark Blue" ), false, false );
			AddRadio( 100, 280, 210, 211, textColorSwitch == TextColor.DarkBlue, 6 );

			AddHtml( 101, 321, 160, 20, MC.ColorText( defaultTextColor, "<center>Reds</center>" ), false, false );

			AddHtml( 141, 341, 100, 20, MC.ColorText( MC.TextColorLightRed, "Light Red" ), false, false );
			AddRadio( 100, 340, 210, 211, textColorSwitch == TextColor.LightRed, 7 );

			AddHtml( 141, 361, 100, 20, MC.ColorText( MC.TextColorRed, "Red" ), false, false );
			AddRadio( 100, 360, 210, 211, textColorSwitch == TextColor.Red, 8 );

			AddHtml( 141, 381, 100, 20, MC.ColorText( MC.TextColorDarkRed, "Dark Red" ), false, false );
			AddRadio( 100, 380, 210, 211, textColorSwitch == TextColor.DarkRed, 9 );

			AddHtml( 301, 121, 160, 20, MC.ColorText( defaultTextColor, "<center>Greens</center>" ), false, false );

			AddHtml( 341, 141, 100, 20, MC.ColorText( MC.TextColorLightGreen, "Light Green" ), false, false );
			AddRadio( 300, 140, 210, 211, textColorSwitch == TextColor.LightGreen, 10 );

			AddHtml( 341, 161, 100, 20, MC.ColorText( MC.TextColorGreen, "Green" ), false, false );
			AddRadio( 300, 160, 210, 211, textColorSwitch == TextColor.Green, 11 );

			AddHtml( 341, 181, 100, 20, MC.ColorText( MC.TextColorDarkGreen, "Dark Green" ), false, false );
			AddRadio( 300, 180, 210, 211, textColorSwitch == TextColor.DarkGreen, 12 );

			AddHtml( 301, 221, 160, 20, MC.ColorText( defaultTextColor, "<center>Yellows</center>" ), false, false );

			AddHtml( 341, 241, 100, 20, MC.ColorText( MC.TextColorLightYellow, "Light Yellow" ), false, false );
			AddRadio( 300, 240, 210, 211, textColorSwitch == TextColor.LightYellow, 13 );

			AddHtml( 341, 261, 100, 20, MC.ColorText( MC.TextColorYellow, "Yellow" ), false, false );
			AddRadio( 300, 260, 210, 211, textColorSwitch == TextColor.Yellow, 14 );

			AddHtml( 341, 281, 100, 20, MC.ColorText( MC.TextColorDarkYellow, "Dark Yellow" ), false, false );
			AddRadio( 300, 280, 210, 211, textColorSwitch == TextColor.DarkYellow, 15 );

			AddHtml( 501, 121, 160, 20, MC.ColorText( defaultTextColor, "<center>Grayscale</center>" ), false, false );

			AddHtml( 541, 141, 100, 20, MC.ColorText( MC.TextColorWhite, "White" ), false, false );
			AddRadio( 500, 140, 210, 211, textColorSwitch == TextColor.White, 16 );

			AddHtml( 541, 161, 100, 20, MC.ColorText( MC.TextColorLightGray, "Light Gray" ), false, false );
			AddRadio( 500, 160, 210, 211, textColorSwitch == TextColor.LightGray, 17 );

			AddHtml( 541, 181, 130, 20, MC.ColorText( MC.TextColorMediumLightGray, "Medium Light Gray" ), false, false );
			AddRadio( 500, 180, 210, 211, textColorSwitch == TextColor.MediumLightGray, 18 );

			AddHtml( 541, 201, 100, 20, MC.ColorText( MC.TextColorMediumGray, "Medium Gray" ), false, false );
			AddRadio( 500, 200, 210, 211, textColorSwitch == TextColor.MediumGray, 19 );

			AddHtml( 541, 221, 100, 20, MC.ColorText( MC.TextColorDarkGray, "Dark Gray" ), false, false );
			AddRadio( 500, 220, 210, 211, textColorSwitch == TextColor.DarkGray, 20 );

			AddHtml( 541, 241, 100, 20, MC.ColorText( MC.TextColorBlack, "Black" ), false, false );
			AddRadio( 500, 240, 210, 211, textColorSwitch == TextColor.Black, 21 );
		}

		private void SubmitConfig( RelayInfo info, bool submit, int command )
		{
			int settingNum = 0;

			string oldMessages = Messages;

			if ( submit )
			{
				MessagesTitle = "Save Configurations";
				Messages = null;
			}

			if ( StyleConfigClicked )
				settingNum = 1;

			if ( TextColorConfigClicked )
				settingNum = 2;

			switch ( settingNum )
			{
				case 0:
				{
					Messages = oldMessages;

					if ( submit )
						Messages = "Your Personal Configurations have been saved.";

					UpdatePage( command );
					OpenGump();

					break;
				}
				case 1:
				{
					int switchNum = 0;

					switch ( pg )
					{
						case 3:
						{
							switchNum = MC.GetSwitchNum( info, 1, 10 );
							styleTypeSwitch = (StyleType) switchNum;

							switchNum = MC.GetSwitchNum( info, 11, 18 );
							backgroundTypeSwitch = (BackgroundType) switchNum;

							break;
						}
						case 4:
						{
							switchNum = MC.GetSwitchNum( info, 1, 9 );
							activeTEBGTypeSwitch = (BackgroundType) switchNum;

							switchNum = MC.GetSwitchNum( info, 10, 19 );
							inactiveTEBGTypeSwitch = (BackgroundType) switchNum;

							break;
						}
					}

					StyleTypeConfig = styleTypeSwitch;
					BackgroundTypeConfig = backgroundTypeSwitch;
					ActiveTEBGTypeConfig = activeTEBGTypeSwitch;
					InactiveTEBGTypeConfig = inactiveTEBGTypeSwitch;

					WriteConfig();

					if ( submit )
					{
						Messages = "Your Personal Configurations have been saved.";

						SetArgsList();

						gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

						break;
					}
					else
					{
						Messages = oldMessages;

						UpdatePage( command );
					}

					OpenGump();

					break;
				}
				case 2:
				{
					int switchNum = MC.GetSwitchNum( info, 1, 21 );

					switch ( pg )
					{
						case 6:{ defaultTextColorSwitch = (TextColor) switchNum; break; }
						case 7:{ titleTextColorSwitch = (TextColor) switchNum; break; }
						case 8:{ messagesTextColorSwitch = (TextColor) switchNum; break; }
						case 9:{ commandButtonsTextColorSwitch = (TextColor) switchNum; break; }
						case 10:{ flagTextColorSwitch = (TextColor) switchNum; break; }
						case 11:
						{
							bool invalid = false;

							bool fPageNumberTextColor=false, fActiveTextEntryTextColor=false, fInactiveTextEntryTextColor=false, fDirectoryTextColor=false, fFileTextColor=false, fKnownFileTextColor=false;

							string checkPageNumberTextColor=null, checkActiveTextEntryTextColor=null, checkInactiveTextEntryTextColor=null, checkDirectoryTextColor=null, checkFileTextColor=null, checkKnownFileTextColor=null;

							int intPageNumberTextColor=0, intActiveTextEntryTextColor=0, intInactiveTextEntryTextColor=0, intDirectoryTextColor=0, intFileTextColor=0, intKnownFileTextColor=0;

							TextRelay textInput;

							textInput = info.GetTextEntry( 1 );
							checkPageNumberTextColor = Convert.ToString( textInput.Text );

							textInput = info.GetTextEntry( 2 );
							checkActiveTextEntryTextColor = Convert.ToString( textInput.Text );

							textInput = info.GetTextEntry( 2 );
							checkInactiveTextEntryTextColor = Convert.ToString( textInput.Text );

							textInput = info.GetTextEntry( 4 );
							checkDirectoryTextColor = Convert.ToString( textInput.Text );

							textInput = info.GetTextEntry( 5 );
							checkFileTextColor = Convert.ToString( textInput.Text );

							textInput = info.GetTextEntry( 6 );
							checkKnownFileTextColor = Convert.ToString( textInput.Text );

							try{ intPageNumberTextColor = Convert.ToInt32( checkPageNumberTextColor ); }
							catch
							{
								invalid = true;
								fPageNumberTextColor = true;
								Messages = String.Format( "{0}Invalid input for [Page Number Text Color]. You must specify an integer only.\n", Messages );
							}

							try{ intActiveTextEntryTextColor = Convert.ToInt32( checkActiveTextEntryTextColor ); }
							catch
							{
								invalid = true;
								fActiveTextEntryTextColor = true;
								Messages = String.Format( "{0}Invalid input for [Active Text Entry Text Color]. You must specify an integer only.\n", Messages );
							}

							try{ intInactiveTextEntryTextColor = Convert.ToInt32( checkInactiveTextEntryTextColor ); }
							catch
							{
								invalid = true;
								fInactiveTextEntryTextColor = true;
								Messages = String.Format( "{0}Invalid input for [Inactive Text Entry Text Color]. You must specify an integer only.\n", Messages );
							}

							try{ intDirectoryTextColor = Convert.ToInt32( checkDirectoryTextColor ); }
							catch
							{
								invalid = true;
								fDirectoryTextColor = true;
								Messages = String.Format( "{0}Invalid input for [Directory Text Color]. You must specify an integer only.\n", Messages );
							}

							try{ intFileTextColor = Convert.ToInt32( checkFileTextColor ); }
							catch
							{
								invalid = true;
								fFileTextColor = true;
								Messages = String.Format( "{0}Invalid input for [File Text Color]. You must specify an integer only.\n", Messages );
							}

							try{ intKnownFileTextColor = Convert.ToInt32( checkKnownFileTextColor ); }
							catch
							{
								invalid = true;
								fKnownFileTextColor = true;
								Messages = String.Format( "{0}Invalid input for [Known File Text Color]. You must specify an integer only.\n", Messages );
							}

							if ( invalid )
							{
								if ( !fPageNumberTextColor )
									PageNumberTextColor = intPageNumberTextColor;

								if ( !fActiveTextEntryTextColor )
									ActiveTextEntryTextColor = intActiveTextEntryTextColor;

								if ( !fInactiveTextEntryTextColor )
									InactiveTextEntryTextColor = intInactiveTextEntryTextColor;

								if ( !fDirectoryTextColor )
									DirectoryTextColor = intDirectoryTextColor;

								if ( !fFileTextColor )
									FileTextColor = intFileTextColor;

								if ( !fKnownFileTextColor )
									KnownFileTextColor = intKnownFileTextColor;

								if ( !submit )
								{
									Messages = oldMessages;

									UpdatePage( command );
								}

								OpenGump();

								return;
							}

							PageNumberTextColor = intPageNumberTextColor;
							ActiveTextEntryTextColor = intActiveTextEntryTextColor;
							InactiveTextEntryTextColor = intInactiveTextEntryTextColor;
							DirectoryTextColor = intDirectoryTextColor;
							FileTextColor = intFileTextColor;
							KnownFileTextColor = intKnownFileTextColor;

							break;
						}
					}

					DefaultTextColor = defaultTextColorSwitch;
					TitleTextColor = titleTextColorSwitch;
					MessagesTextColor = messagesTextColorSwitch;
					CommandButtonsTextColor = commandButtonsTextColorSwitch;
					FlagTextColor = flagTextColorSwitch;

					WriteConfig();

					if ( submit )
					{
						Messages = "Your Personal Configurations have been saved.";

						SetArgsList();

						gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) );

						break;
					}
					else
					{
						Messages = oldMessages;

						UpdatePage( command );
					}

					OpenGump();

					break;
				}
			}
		}
	}
}