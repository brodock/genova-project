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
	public class SystemConfigGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList PageInfoList = new ArrayList();
		private ArrayList SCGArgsList = new ArrayList();
		private ArrayList SCGSetList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private int cpg, pg;
		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor, MessagesTextColor, CommandButtonsTextColor;

		private TimerType timerTypeSwitch;
		private bool ConfigRead, debugSwitch, staffTriggerEventSwitch;

		private bool TimerConfigClicked, MiscConfigClicked;

		private bool Help, DisplayMessages;
		private string MessagesTitle, Messages, OldMessagesTitle, OldMessages;
		private string defaultTextColor, titleTextColor, messagesTextColor, commandButtonsTextColor;
		private int PageNumberTextColor, ActiveTextEntryTextColor;

		private ArrayList MenuAccessList = new ArrayList();

		private int totalPages, pgOffset, pgNum;
		private RelayInfo relayInfo;
		private string ErrorLog = null;
		private int totalErrors;
		private Mobile gumpMobile;

		private int[] TimerSetting = new int[]
		{
			1, 2, 4, 8, 12, 16, 20, 24, 28, 32, 36, 40, 44, 48
		};

		public SystemConfigGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
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
			AddHtml( 121, 81, 540, 20, MC.ColorText( titleTextColor, "Mega Spawner System Configuration" ), false, false );

			if ( !Help )
				AddButton( 597, 429, 2033, 2032, -5, GumpButtonType.Reply, 0 );
			else
				AddButton( 597, 429, 2032, 2033, -5, GumpButtonType.Reply, 0 );

			if ( !ConfigRead )
				ReadConfig();

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
			if ( !CheckAccess() )
				return;

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

					MC.EditSystemConfig = null;

					gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Misc Configuration
				{
					if ( Help )
					{
						MessagesTitle = "Help: Misc Configuration Button";
						Messages = "That button will open the misc configuration which allows you to customize misc settings.";

						SubmitConfig( info, false, 0 );

						break;
					}

					ResetSelectionClicks();

					pg = 2;
					MiscConfigClicked = true;

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

					DefaultConfigCreate();

					MessagesTitle = "Reset Configurations To Defaults";
					Messages = "All configurations have been reset to their default settings.";

					ResetSelectionClicks();

					pg = 1;

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
				case 4: // Timer Configuration
				{
					if ( Help )
					{
						MessagesTitle = "Help: Timer Configuration Button";
						Messages = "That button will open the timer configuration which allows you to choose many different timer settings.";

						SubmitConfig( info, false, 0 );

						break;
					}

					ResetSelectionClicks();

					pg = 4;
					TimerConfigClicked = true;

					OpenGump();

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new SystemConfigGump( gumpMobile, ArgsList ) );
		}

		private void SetArgsList()
		{
			PageInfoList[18] = cpg;
			PageInfoList[19] = pg;

			SCGArgsList[0] = ConfigRead;
			SCGArgsList[1] = timerTypeSwitch;
			SCGArgsList[2] = debugSwitch;
			SCGArgsList[3] = staffTriggerEventSwitch;

			SCGSetList[0] = TimerConfigClicked;
			SCGSetList[1] = MiscConfigClicked;

			ArgsList[0] = Help;
			ArgsList[1] = DisplayMessages;
			ArgsList[2] = MessagesTitle;
			ArgsList[3] = OldMessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[5] = OldMessages;
			ArgsList[12] = PageInfoList;
			ArgsList[24] = SCGArgsList;
			ArgsList[25] = SCGSetList;
		}

		private void GetArgsList()
		{
			Help = (bool)									ArgsList[0];
			DisplayMessages = (bool)						ArgsList[1];
			MessagesTitle = (string)						ArgsList[2];
			OldMessagesTitle = (string)						ArgsList[3];
			Messages = (string)								ArgsList[4];
			OldMessages = (string)							ArgsList[5];
			PageInfoList = (ArrayList)						ArgsList[12];
			SCGArgsList = (ArrayList)						ArgsList[24];
			SCGSetList = (ArrayList)						ArgsList[25];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			cpg = (int) 									PageInfoList[18];
			pg = (int) 										PageInfoList[19];

			ConfigRead = (bool)							SCGArgsList[0];
			timerTypeSwitch = (TimerType)					SCGArgsList[1];
			debugSwitch = (bool)							SCGArgsList[2];
			staffTriggerEventSwitch = (bool)					SCGArgsList[3];

			TimerConfigClicked = (bool)						SCGSetList[0];
			MiscConfigClicked = (bool)						SCGSetList[1];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)			PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)			PersonalConfigList[2];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			MessagesTextColor = (TextColor)					PersonalConfigList[6];
			CommandButtonsTextColor = (TextColor)			PersonalConfigList[7];
			PageNumberTextColor = (int)						PersonalConfigList[8];
			ActiveTextEntryTextColor = (int)				PersonalConfigList[9];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
			messagesTextColor = MC.GetTextColor( MessagesTextColor );
			commandButtonsTextColor = MC.GetTextColor( CommandButtonsTextColor );
		}

		private bool TimerSettingType( int delay )
		{
			return Array.IndexOf( TimerSetting, delay ) >= 0;
		}

		private bool CheckAccess()
		{
			MenuAccessList = MC.GetMenuAccess( gumpMobile );

			bool access = (bool) MenuAccessList[1];

			if ( !access )
			{
				MessagesTitle = "Access Denied";
				Messages = "You no longer have access to that option.";

				MC.EditSystemConfig = null;

				SetArgsList();

				gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );
			}

			return access;
		}

		private void ResetSelectionClicks()
		{
			MiscConfigClicked = false;
			TimerConfigClicked = false;
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

		private void SetConfigList()
		{
			MC.TimerTypeConfig = timerTypeSwitch;
			MC.StaffTriggerEvent = staffTriggerEventSwitch;

			SetArgsList();
		}

		private void ReadConfig()
		{
			if ( !Directory.Exists( MC.SaveDirectory ) )
				Directory.CreateDirectory( MC.SaveDirectory );

			try
			{
				XmlDocument xml = new XmlDocument();
				xml.Load( MC.ConfigFileName );

				XmlElement node = xml["MegaSpawnerSystemConfig"];

				try{ timerTypeSwitch = (TimerType) int.Parse( GetInnerText( node["TimerSetting"] ) ); }
				catch( Exception ex ){ totalErrors++; AddToErrorLog( ex ); }

				try{ staffTriggerEventSwitch = bool.Parse( GetInnerText( node["StaffTriggerEvent"] ) ); }
				catch( Exception ex ){ totalErrors++; AddToErrorLog( ex ); }
			}
			catch
			{
				DefaultConfigCreate();

				MessagesTitle = "Mega Spawner System Configuration";
				Messages = "Configuration file not found, creating default configuration...";
			}

			SetConfigList();

			ConfigRead = true;

			if ( totalErrors > 0 )
			{
				MessagesTitle = "Mega Spawner System Configuration";
				Messages = String.Format( "{0} error{1} been detected.\nError Log:\n {2}", totalErrors, totalErrors == 1 ? " has" : "s have", ErrorLog );
			}
		}

		private void WriteConfig()
		{
			if ( !Directory.Exists( MC.SaveDirectory ) )
				Directory.CreateDirectory( MC.SaveDirectory );

			using ( StreamWriter sw = new StreamWriter( MC.ConfigFileName ) )
			{
				XmlTextWriter xml = new XmlTextWriter( sw );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );

				xml.WriteStartElement( "MegaSpawnerSystemConfig" );

				xml.WriteStartElement( "TimerSetting" );
				xml.WriteString( ( (int) timerTypeSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "TimerDelay" );
				xml.WriteString( ( (int) MC.Delay ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "StaffTriggerEvent" );
				xml.WriteString( ( (bool) staffTriggerEventSwitch ).ToString() );
				xml.WriteEndElement();

				xml.WriteEndElement();

				xml.Close();
			}
		}

		private void DefaultConfigCreate()
		{
			using ( StreamWriter sw = new StreamWriter( MC.ConfigFileName ) )
			{
				XmlTextWriter xml = new XmlTextWriter( sw );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );

				xml.WriteStartElement( "MegaSpawnerSystemConfig" );

				xml.WriteStartElement( "TimerSetting" );
				xml.WriteString( ( (int) TimerType.Personal ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "TimerDelay" );
				xml.WriteString( "28" );
				xml.WriteEndElement();

				xml.WriteStartElement( "StaffTriggerEvent" );
				xml.WriteString( "False" );
				xml.WriteEndElement();

				xml.WriteEndElement();

				xml.Close();
			}

			ReadConfig();
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

		private void SetTimers()
		{
			switch ( timerTypeSwitch )
			{
				case TimerType.Master:
				{
					MC.StartTimer();

					break;
				}
				default:
				{
					MC.CheckSpawners();

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
					{
						MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];
						megaSpawner.Start();
					}

					break;
				}
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

			totalPages = 0;
			pgNum = 0;

			if ( pg == 1 )
			{
				totalPages = 1;
				pgNum = pg;
			}

			if ( MiscConfigClicked )
			{
				pgOffset = 1;
				totalPages = 2;
			}

			if ( TimerConfigClicked )
			{
				pgOffset = 3;
				totalPages = 2;
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
				case 2:
				{
					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, "<center>Misc Configuration</center>\nThis is where you can configure misc settings.\n\nGo to page 2 to continue." ), false, false );

					break;
				}
				case 3:
				{
					AddHtml( 141, 101, 300, 20, MC.ColorText( defaultTextColor, "Staff Can Trigger Events" ), false, false );
					AddCheck( 100, 100, 210, 211, staffTriggerEventSwitch, 1 );

					break;
				}
				case 4:
				{
					string pageInfo = "<center>Timer Configuration</center>\nThis is where you can adjust the timer in which the Mega Spawner System uses for spawning.";
					pageInfo = String.Format( "{0} You may choose to use the master timer or personal timer. There are many different options to choose from for timer settings.", pageInfo );
					pageInfo = String.Format( "{0}\n<center>Information</center>\nMaster Timer:\nThe master timer is designed to run all of the Mega Spawners with one timer. This option may use a little bit more CPU usage than", pageInfo );
					pageInfo = String.Format( "{0} the personal timer. You can select from many different delay settings.", pageInfo );
					pageInfo = String.Format( "{0}\n\nPersonal Timer:\nThe personal timer is designed to have each Mega Spawner use their own timer for spawn control. This option may use a little more system memory than the master timer.", pageInfo );
					pageInfo = String.Format( "{0} Like the master timer, you can select any of the delay options or choose your own.", pageInfo );
					pageInfo = String.Format( "{0}\n\nDelay Settings:\nYou can choose from any of the delay settings by clicking on the slider anywhere from \"Consistency\" to \"Save On CPU Usage\". The higher the delay setting you choose, the", pageInfo );
					pageInfo = String.Format( "{0} less CPU usage is used to control the Mega Spawners. However, the spawns will be less consistent and more likely to fall out of their minimum and maximum delay range.", pageInfo );
					pageInfo = String.Format( "{0} You can specify your own delay by choosing the \"Use Specified Delay\" option. There is also another personal timer option which is called \"Use Personal Timer(Random Entry Delay)\". With this option,", pageInfo );
					pageInfo = String.Format( "{0} the system will randomly choose one of the entries on each Mega Spawner to create a random delay based on their minimum and maximum delay range.", pageInfo );
					pageInfo = String.Format( "{0}\n\n<center>Recommendations</center>\nFast CPU and lots of ram:\nMaster Timer with a delay of 1.\n\nFast CPU and medium amount of ram:\nMaster Timer with delay of 12.\n\n", pageInfo );
					pageInfo = String.Format( "{0}Fast CPU and little amount of ram:\nMaster Timer with a delay of 40-48.\n\nMedium CPU and lots of ram:\nPersonal Timer with delay of 1-4.\n\n", pageInfo );
					pageInfo = String.Format( "{0}Medium CPU and medium amount of ram:\nPersonal Timer with a delay of 16-20.\n\nMedium CPU and little amount of ram:\nPersonal Timer with delay of 40-48.\n\n", pageInfo );
					pageInfo = String.Format( "{0}Slow CPU and lots of ram:\nPersonal Timer with a delay of 12-16.\n\nSlow CPU and medium amount of ram:\nPersonal Timer with delay of 40-48.\n\n", pageInfo );
					pageInfo = String.Format( "{0}Slow CPU and little amount of ram:\nPersonal Timer with a delay of 60+.", pageInfo );
					pageInfo = String.Format( "{0}\n\nThese recommendations can change at any time, as more benchmarking is done.", pageInfo );
					pageInfo = String.Format( "{0}\n\nGo to page 2 to continue.", pageInfo );

					AddHtml( 100, 100, 560, 330, MC.ColorText( messagesTextColor, pageInfo ), false, true );

					break;
				}
				case 5:
				{
					AddHtml( 101, 101, 300, 20, MC.ColorText( defaultTextColor, "Timer Setting:" ), false, false );
					AddHtml( 101, 121, 300, 20, MC.ColorText( defaultTextColor, "Consistency" ), false, false );
					AddHtml( 541, 121, 300, 20, MC.ColorText( defaultTextColor, "Save On CPU Usage" ), false, false );

					AddImageTiled( 100, 140, 560, 20, 2604 );

					AddGroup( 1 );

					AddImage( 119, 144, 9024 );
					AddRadio( 100, 140, 9020, 9021, MC.Delay == 1, 1 );

					AddImage( 160, 144, 9024 );
					AddRadio( 141, 140, 9020, 9021, MC.Delay == 2, 2 );

					AddImage( 201, 144, 9024 );
					AddRadio( 182, 140, 9020, 9021, MC.Delay == 4, 3 );

					AddImage( 243, 144, 9024 );
					AddRadio( 224, 140, 9020, 9021, MC.Delay == 8, 4 );

					AddImage( 284, 144, 9024 );
					AddRadio( 265, 140, 9020, 9021, MC.Delay == 12, 5 );

					AddImage( 325, 144, 9024 );
					AddRadio( 306, 140, 9020, 9021, MC.Delay == 16, 6 );

					AddImage( 367, 144, 9024 );
					AddRadio( 348, 140, 9020, 9021, MC.Delay == 20, 7 );

					AddImage( 408, 144, 9024 );
					AddRadio( 389, 140, 9020, 9021, MC.Delay == 24, 8 );

					AddImage( 449, 144, 9024 );
					AddRadio( 430, 140, 9020, 9021, MC.Delay == 28, 9 );

					AddImage( 491, 144, 9024 );
					AddRadio( 472, 140, 9020, 9021, MC.Delay == 32, 10 );

					AddImage( 532, 144, 9024 );
					AddRadio( 513, 140, 9020, 9021, MC.Delay == 36, 11 );

					AddImage( 573, 144, 9024 );
					AddRadio( 554, 140, 9020, 9021, MC.Delay == 40, 12 );

					AddImage( 615, 144, 9024 );
					AddRadio( 596, 140, 9020, 9021, MC.Delay == 44, 13 );

					AddRadio( 637, 140, 9020, 9021, MC.Delay == 48, 14 );

					AddHtml( 101, 181, 300, 20, MC.ColorText( defaultTextColor, "Timer Delay In Seconds:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 300, 181, 60, 18 );
					AddTextEntry( 300, 181, 60, 15, ActiveTextEntryTextColor, 1, MC.Delay.ToString() );

					AddHtml( 430, 181, 300, 20, MC.ColorText( defaultTextColor, "Use Specified Delay" ), false, false );
					AddRadio( 389, 180, 210, 211, !TimerSettingType( MC.Delay ), 15 );

					AddGroup( 2 );

					AddHtml( 141, 221, 300, 20, MC.ColorText( defaultTextColor, "Use Master Timer" ), false, false );
					AddHtml( 141, 241, 300, 20, MC.ColorText( defaultTextColor, "Use Personal Timer (Normal)" ), false, false );

					AddHtml( 141, 281, 300, 20, MC.ColorText( defaultTextColor, "Use Personal Timer (Random Entry Delay)" ), false, false );

					AddRadio( 100, 220, 210, 211, timerTypeSwitch == TimerType.Master, 16 );
					AddRadio( 100, 240, 210, 211, timerTypeSwitch == TimerType.Personal, 17 );

					AddRadio( 100, 280, 210, 211, timerTypeSwitch == TimerType.RandomEntryDelay, 18 );

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
			AddHtml( 141, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Misc Configuration" ), false, false );

			if ( MiscConfigClicked )
				AddButton( 100, 450, 5541, 5542, 1, GumpButtonType.Reply, 0 );
			else
				AddButton( 100, 450, 9904, 9905, 1, GumpButtonType.Reply, 0 );

			AddHtml( 391, 451, 300, 20, MC.ColorText( commandButtonsTextColor, "Timer Configuration" ), false, false );

			if ( TimerConfigClicked )
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

		private void SubmitConfig( RelayInfo info, bool submit, int command )
		{
			int settingNum = 0;

			string oldMessages = Messages;

			if ( submit )
			{
				MessagesTitle = "Save Configurations";
				Messages = null;
			}

			if ( MiscConfigClicked )
				settingNum = 1;

			if ( TimerConfigClicked )
				settingNum = 2;

			switch ( settingNum )
			{
				case 0:
				{
					Messages = oldMessages;

					if ( submit )
						Messages = "Mega Spawner System Configurations have been saved.";

					UpdatePage( command );

					break;
				}
				case 1:
				{
					bool invalid = false;

					switch ( pg )
					{
						case 3:
						{
							if ( invalid )
							{
								if ( !submit )
									Messages = oldMessages;

								UpdatePage( command );

								return;
							}

							staffTriggerEventSwitch = info.IsSwitched( 1 );

							break;
						}
					}

					SetConfigList();
					WriteConfig();

					if ( submit )
					{
						Messages = "Mega Spawner System Configurations have been saved.";

						SetArgsList();

						MC.EditSystemConfig = null;

						gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );

						break;
					}
					else
					{
						Messages = oldMessages;

						UpdatePage( command );
					}

					break;
				}
				case 2:
				{
					bool invalid = false;

					int switchNum = 0;

					switch ( pg )
					{
						case 5:
						{
							string checkDelay=null;
							int intDelay=0;

							TextRelay textInput = info.GetTextEntry( 1 );
							checkDelay = Convert.ToString( textInput.Text );

							switchNum = MC.GetSwitchNum( info, 16, 18 );

							switch( switchNum )
							{
								case 0:{ timerTypeSwitch = TimerType.Master; break; }
								case 1:{ timerTypeSwitch = TimerType.Personal; break; }
								case 2:{ timerTypeSwitch = TimerType.RandomEntryDelay; break; }
							}

							try{ intDelay = Convert.ToInt32( checkDelay ); }
							catch
							{
								invalid = true;
								Messages = String.Format( "{0}Invalid input for [Timer Delay In Seconds]. You must specify an integer only.\n", Messages );
							}

							if ( intDelay < 1 && !invalid )
							{
								invalid = true;
								Messages = String.Format( "{0}Invalid input for [Timer Delay In Seconds]. Must be greater than or equal to 1.\n", Messages );
							}

							if ( invalid )
							{
								if ( !submit )
									Messages = oldMessages;

								UpdatePage( command );

								return;
							}

							int beforeDelay = intDelay;

							switchNum = MC.GetSwitchNum( info, 1, 15 ) + 1;

							if ( switchNum == 1 )
								intDelay = 1;
							else if ( switchNum == 2 )
								intDelay = 2;
							else if ( switchNum <= 14 )
								intDelay = ( switchNum - 2 ) * 4;
							else if ( switchNum == 15 )
								intDelay = beforeDelay;

							MC.Delay = intDelay;

							break;
						}
					}

					SetConfigList();
					WriteConfig();
					SetTimers();

					if ( submit )
					{
						Messages = "Mega Spawner System Configurations have been saved.";

						SetArgsList();

						MC.EditSystemConfig = null;

						gumpMobile.SendGump( new AdminMenuGump( gumpMobile, ArgsList ) );

						break;
					}
					else
					{
						Messages = oldMessages;

						UpdatePage( command );
					}

					break;
				}
			}
		}
	}
}