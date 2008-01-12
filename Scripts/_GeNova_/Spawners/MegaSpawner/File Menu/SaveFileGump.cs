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
	public class SaveFileGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;
		private int ActiveTextEntryTextColor;

		private string FileName;
		private SaveType saveType;
		private ArrayList SpawnerList = new ArrayList();
		private Mobile gumpMobile;

		public SaveFileGump( Mobile mobile, ArrayList argsList, SaveType savetype, string filename ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			saveType = savetype;
			FileName = filename;

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 340, 120 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 300, 40 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 260, 300, 20 );

			AddHtml( 221, 181, 280, 20, MC.ColorText( titleTextColor, "Save File" ), false, false );

			switch ( saveType )
			{
				case SaveType.FileEdit:
				{
					AddHtml( 201, 201, 280, 20, MC.ColorText( defaultTextColor, "Are you sure you want to save file:" ), false, false );
					AddHtml( 201, 221, 280, 20, MC.ColorText( defaultTextColor, FileName ), false, false );

					break;
				}
				default:
				{
					AddHtml( 201, 201, 280, 20, MC.ColorText( defaultTextColor, "Type in a filename without an extension:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 200, 220, 300, 18 );
					AddTextEntry( 200, 220, 300, 15, ActiveTextEntryTextColor, 0, "" );

					break;
				}
			}

			AddHtml( 441, 261, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 400, 260, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 261, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 260, 4023, 4025, 1, GumpButtonType.Reply, 0 );

			CompileSpawnerList();
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					MessagesTitle = "Save File";
					Messages = "You have chosen not to save to a file.";

					SetArgsList();

					switch ( saveType )
					{
						case SaveType.FromFileMenu:{ gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) ); break; }
						default: { gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) ); break; }
					}

					break;
				}
				case 1: // Save File
				{
					string fileName = null;

					switch ( saveType )
					{
						case SaveType.FileEdit:{ fileName = FileName; break; }
						default:
						{
							TextRelay textInput = info.GetTextEntry( 0 );
							fileName = String.Format( "{0}.msf", Convert.ToString( textInput.Text ) );

							if ( fileName == ".msf" )
							{
								gumpMobile.SendMessage( "That filename is invalid. Please type in another filename." );

								OpenGump();

								break;
							}

							break;
						}
					}

					Save( fileName );

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			gumpMobile.SendGump( new SaveFileGump( gumpMobile, ArgsList, saveType, FileName ) );
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[13] = MSGCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			ActiveTEBGTypeConfig = (BackgroundType)				PersonalConfigList[2];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
			ActiveTextEntryTextColor = (int)				PersonalConfigList[9];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
		}

		private void Save( string fileName )
		{
			try
			{
				if ( !Directory.Exists( MC.SaveDirectory ) )
					Directory.CreateDirectory( MC.SaveDirectory );

				if ( !Directory.Exists( MC.SpawnerExportsDirectory ) )
					Directory.CreateDirectory( MC.SpawnerExportsDirectory );

				StreamWriter sw = new StreamWriter( Path.Combine( MC.SpawnerExportsDirectory, fileName ) );

				XmlTextWriter xml = new XmlTextWriter( sw );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );

				xml.WriteStartElement( "MegaSpawners" );
				xml.WriteAttributeString( "count", SpawnerList.Count.ToString() );

				xml.WriteStartElement( "Version" );
				xml.WriteString( MC.Version );
				xml.WriteEndElement();

				int delay = 30;
				bool fullForce = false;

				if ( fullForce )
				{
					foreach ( MegaSpawner megaSpawner in SpawnerList )
						ExportSpawner( xml, megaSpawner );

					xml.WriteEndElement();

					xml.Close();
				}
				else
				{
					foreach ( MegaSpawner megaSpawner in SpawnerList )
						new SaveSpawnerTimer( xml, megaSpawner, false, delay ).Start();

					new SaveSpawnerTimer( xml, null, true, delay + 50 ).Start();
				}

				MessagesTitle = "Save File";
				Messages = String.Format( "{0} Mega Spawner{1} now been saved to filename: \"{2}\"", SpawnerList.Count, SpawnerList.Count == 1 ? " has" : "s have", fileName );

				switch ( saveType )
				{
					case SaveType.Workspace:
					{
						foreach ( MegaSpawner megaSpawner in SpawnerList )
							megaSpawner.Delete();

						Messages = String.Format( "Spawner Workspace has been saved and all Mega Spawners that were created are now deleted. {0}", Messages );

						break;
					}
					case SaveType.FileEdit:
					{
						foreach ( MegaSpawner megaSpawner in SpawnerList )
						{
							megaSpawner.Workspace = false;
							megaSpawner.WorkspaceEditor = null;
						}

						Messages = String.Format( "You chose to save the file you were editting. This file will be saved to the {0} directory. {1}", MC.CropCoreDirectory( MC.SpawnerExportsDirectory ), Messages );

						break;
					}
				}

				SetArgsList();

				switch ( saveType )
				{
					case SaveType.FileEdit: { gumpMobile.SendGump( new MegaSpawnerGump( gumpMobile, ArgsList ) ); break; }
					default:{ gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) ); break; }
				}
			}
			catch
			{
				gumpMobile.SendMessage( "That filename is invalid. Please type in another filename." );

				OpenGump();
			}
		}

		private class SaveSpawnerTimer : Timer
		{ 
			private XmlTextWriter xml;
			private MegaSpawner megaSpawner;
			private bool complete;

			public SaveSpawnerTimer( XmlTextWriter writer, MegaSpawner ms, bool done, int delay ) : base( TimeSpan.FromMilliseconds( delay ) )
			{
				xml = writer;
				megaSpawner = ms;
				complete = done;
			}

			protected override void OnTick()
			{
				if ( !complete )
				{
					ExportSpawner( xml, megaSpawner );
				}
				else
				{
					xml.WriteEndElement();

					xml.Close();
				}
			}
		}

		private static void ExportSpawner( XmlTextWriter xml, MegaSpawner megaSpawner )
		{
			xml.WriteStartElement( "MegaSpawner" );

			xml.WriteStartElement( "Name" );
			xml.WriteString( megaSpawner.Name );
			xml.WriteEndElement();

			xml.WriteStartElement( "Active" );
			xml.WriteString( megaSpawner.Active.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "Map" );
			xml.WriteString( megaSpawner.Map.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "Location" );
			xml.WriteString( megaSpawner.Location.ToString() );
			xml.WriteEndElement();

			xml.WriteStartElement( "EntryLists" );
			xml.WriteAttributeString( "count", megaSpawner.EntryList.Count.ToString() );

			for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
			{
				xml.WriteStartElement( "EntryList" );

				xml.WriteStartElement( "EntryType" );
				xml.WriteString( megaSpawner.EntryList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SpawnRange" );
				xml.WriteString( megaSpawner.SpawnRangeList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "WalkRange" );
				xml.WriteString( megaSpawner.WalkRangeList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "Amount" );
				xml.WriteString( megaSpawner.AmountList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MinDelay" );
				xml.WriteString( megaSpawner.MinDelayList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MaxDelay" );
				xml.WriteString( megaSpawner.MaxDelayList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "SpawnType" );
				xml.WriteString( ( (int) megaSpawner.SpawnTypeList[i] ).ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "Activated" );
				xml.WriteString( megaSpawner.ActivatedList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "EventRange" );
				xml.WriteString( megaSpawner.EventRangeList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "EventKeyword" );
				xml.WriteString( megaSpawner.EventKeywordList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "KeywordCaseSensitive" );
				xml.WriteString( megaSpawner.KeywordCaseSensitiveList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "EventAmbush" );
				xml.WriteString( megaSpawner.EventAmbushList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "BeginTimeBased" );
				xml.WriteString( megaSpawner.BeginTimeBasedList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "EndTimeBased" );
				xml.WriteString( megaSpawner.EndTimeBasedList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "GroupSpawn" );
				xml.WriteString( megaSpawner.GroupSpawnList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MinStackAmount" );
				xml.WriteString( megaSpawner.MinStackAmountList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MaxStackAmount" );
				xml.WriteString( megaSpawner.MaxStackAmountList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "Movable" );
				xml.WriteString( megaSpawner.MovableList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MinDespawn" );
				xml.WriteString( megaSpawner.MinDespawnList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "MaxDespawn" );
				xml.WriteString( megaSpawner.MaxDespawnList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "Despawn" );
				xml.WriteString( megaSpawner.DespawnList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "DespawnGroup" );
				xml.WriteString( megaSpawner.DespawnGroupList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteStartElement( "DespawnTimeExpire" );
				xml.WriteString( megaSpawner.DespawnTimeExpireList[i].ToString() );
				xml.WriteEndElement();

				xml.WriteEndElement();
			}

			xml.WriteEndElement();

			xml.WriteStartElement( "SettingsList" );
			xml.WriteAttributeString( "count", megaSpawner.SettingsList.Count.ToString() );

			for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
			{
				ArrayList setting = (ArrayList) megaSpawner.SettingsList[i];

				switch ( (Setting) setting[0] )
				{
					case Setting.OverrideIndividualEntries:
					{
						xml.WriteStartElement( "OverrideIndividualEntries" );

						xml.WriteStartElement( "SpawnRange" );
						xml.WriteString( setting[1].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "WalkRange" );
						xml.WriteString( setting[2].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "Amount" );
						xml.WriteString( setting[3].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MinDelay" );
						xml.WriteString( setting[4].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MaxDelay" );
						xml.WriteString( setting[5].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "GroupSpawn" );
						xml.WriteString( setting[6].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EventAmbush" );
						xml.WriteString( setting[7].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "SpawnType" );
						xml.WriteString( ( (int) setting[8] ).ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EventKeyword" );
						xml.WriteString( setting[9].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "CaseSensitive" );
						xml.WriteString( setting[10].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EventRange" );
						xml.WriteString( setting[11].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "BeginTimeBasedHour" );
						xml.WriteString( setting[12].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "BeginTimeBasedMinute" );
						xml.WriteString( setting[13].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EndTimeBasedHour" );
						xml.WriteString( setting[14].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EndTimeBasedMinute" );
						xml.WriteString( setting[15].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MinDespawn" );
						xml.WriteString( setting[16].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MaxDespawn" );
						xml.WriteString( setting[17].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "Despawn" );
						xml.WriteString( setting[18].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "DespawnGroup" );
						xml.WriteString( setting[19].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "DespawnTimeExpire" );
						xml.WriteString( setting[20].ToString() );
						xml.WriteEndElement();

						xml.WriteEndElement();

						break;
					}
					case Setting.AddItem:
					{
						xml.WriteStartElement( "AddItemSetting" );

						xml.WriteStartElement( "EntryName" );
						xml.WriteString( setting[1].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EntryIndex" );
						xml.WriteString( setting[2].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "AddItem" );
						xml.WriteString( setting[3].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MinStackAmount" );
						xml.WriteString( setting[4].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MaxStackAmount" );
						xml.WriteString( setting[5].ToString() );
						xml.WriteEndElement();

						xml.WriteEndElement();

						break;
					}
					case Setting.AddContainer:
					{
						xml.WriteStartElement( "AddContainerSetting" );

						xml.WriteStartElement( "EntryName" );
						xml.WriteString( setting[1].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "EntryIndex" );
						xml.WriteString( setting[2].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "AddItem" );
						xml.WriteString( setting[3].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MinStackAmount" );
						xml.WriteString( setting[4].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "MaxStackAmount" );
						xml.WriteString( setting[5].ToString() );
						xml.WriteEndElement();

						xml.WriteStartElement( "ContainerItems" );

						for ( int j = 6; j < setting.Count; j++ )
						{
							xml.WriteStartElement( "ContainerItem" );

							ArrayList ItemsList = (ArrayList) setting[j];

							xml.WriteStartElement( "AddItem" );
							xml.WriteString( ItemsList[0].ToString() );
							xml.WriteEndElement();

							xml.WriteStartElement( "MinStackAmount" );
							xml.WriteString( ItemsList[1].ToString() );
							xml.WriteEndElement();

							xml.WriteStartElement( "MaxStackAmount" );
							xml.WriteString( ItemsList[2].ToString() );
							xml.WriteEndElement();

							xml.WriteEndElement();
						}

						xml.WriteEndElement();

						xml.WriteEndElement();

						break;
					}
				}
			}

			xml.WriteEndElement();

			xml.WriteEndElement();
		}

		private void CompileSpawnerList()
		{
			MC.CheckSpawners();

			for ( int i = 0; i < MC.SpawnerList.Count; i++ )
			{
				if ( (bool) MSGCheckBoxesList[i] )
					SpawnerList.Add( (MegaSpawner) MC.SpawnerList[i] );
			}
		}
	}
}