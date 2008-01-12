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
	public class LoadFileGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig, ActiveTEBGTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;
		private int ActiveTextEntryTextColor;

		private string version;
		private string FileName;
		private LoadType loadType;
		private Mobile gumpMobile;

		public LoadFileGump( Mobile mobile, ArrayList argsList, LoadType loadtype, string filename ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			loadType = loadtype;
			FileName = filename;

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 340, 120 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 300, 40 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 260, 300, 20 );

			AddHtml( 221, 181, 280, 20, MC.ColorText( titleTextColor, "Load File" ), false, false );

			switch ( loadType )
			{
				case LoadType.Manual:
				{
					AddHtml( 201, 201, 280, 20, MC.ColorText( defaultTextColor, "Type in the filename to load:" ), false, false );
					MC.DisplayBackground( this, ActiveTEBGTypeConfig, 200, 220, 300, 18 );
					AddTextEntry( 200, 220, 300, 15, ActiveTextEntryTextColor, 0, "" );

					break;
				}
				case LoadType.FileBrowserMsf:
				{
					AddHtml( 201, 201, 280, 20, MC.ColorText( defaultTextColor, "Are you sure you want to load file:" ), false, false );
					AddHtml( 201, 221, 280, 20, MC.ColorText( defaultTextColor, MC.CropDirectory( FileName ) ), false, false );

					break;
				}
				case LoadType.FileBrowserMbk:
				{
					MC.DisplayStyle( this, StyleTypeConfig, 180, 160, 340, 20 );
					MC.DisplayBackground( this, BackgroundTypeConfig, 181, 161, 338, 18 );

					AddHtml( 201, 161, 280, 20, MC.ColorText( defaultTextColor, "Warning: All Mega Spawners will be removed." ), false, false );
					AddHtml( 201, 201, 280, 20, MC.ColorText( defaultTextColor, "Are you sure you want to load backup file:" ), false, false );
					AddHtml( 201, 221, 280, 20, MC.ColorText( defaultTextColor, MC.CropDirectory( FileName ) ), false, false );

					break;
				}
			}

			AddHtml( 441, 261, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 400, 260, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 261, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 260, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					MessagesTitle = "Load File";
					Messages = "You have chosen not to load a file.";

					SetArgsList();

					switch ( loadType )
					{
						case LoadType.Manual:{ gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) ); break; }
						default:{ gumpMobile.SendGump( new FileBrowserGump( gumpMobile, ArgsList ) ); break; }
					}

					break;
				}
				case 1: // Load File
				{
					string fileName = null;

					switch ( loadType )
					{
						case LoadType.Manual:
						{
							TextRelay textInput = info.GetTextEntry( 0 );
							fileName = Convert.ToString( textInput.Text ).ToLower();

							break;
						}
						default:{ fileName = FileName; break; }
					}

					if ( loadType == LoadType.FileBrowserMbk )
					{
						MC.CheckSpawners();

						if ( MC.SpawnerList.Count > 0 )
						{
							foreach ( MegaSpawner megaSpawner in MC.SpawnerList )
								megaSpawner.Delete();

							ArgsList[6] = new ArrayList();		// HideSpawnerList
							ArgsList[13] = new ArrayList();		// MSGCheckBoxesList

							MC.SpawnerList.Clear();
						}

						BackupSystem.LoadBackup( gumpMobile, ArgsList, fileName );
					}
					else
					{
						LoadFile( fileName );
					}

					break;
				}
			}
		}

		private void OpenGump()
		{
			SetArgsList();

			switch ( loadType )
			{
				case LoadType.Manual:{ gumpMobile.SendGump( new LoadFileGump( gumpMobile, ArgsList, loadType, FileName ) ); break; }
				default:{ gumpMobile.SendGump( new FileBrowserGump( gumpMobile, ArgsList ) ); break; }
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[13] = MSGCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			HideSpawnerList = (ArrayList)					ArgsList[6];
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

		private void LoadFile( string fileName )
		{
			if ( fileName == "" )
			{
				Messages = "That filename is invalid. Please try another filename.";

				if ( loadType == LoadType.Manual )
					gumpMobile.SendMessage( Messages );

				OpenGump();

				return;
			}
			else if ( !File.Exists( fileName ) )
			{
				Messages = "That file does not exist. Please try another filename.";

				if ( loadType == LoadType.Manual )
					gumpMobile.SendMessage( Messages );

				OpenGump();

				return;
			}
			else if ( MC.CheckFileExists( MC.CropDirectory( fileName ) ) )
			{
				Messages = "That file has already been loaded. Please try another filename.";

				if ( loadType == LoadType.Manual )
					gumpMobile.SendMessage( Messages );

				OpenGump();

				return;
			}

			if ( MC.FileExtensionIs( fileName, "msf" ) ) // Read Mega Spawner File (*.msf)
			{
				StreamReader r = null;

				try
				{
					StreamReader reader = File.OpenText( fileName );
					version = reader.ReadLine();

					r = reader;
				}
				catch( Exception ex )
				{
					Messages = String.Format( "A read error has been detected: {0}", ex );

					if ( loadType == LoadType.Manual )
						gumpMobile.SendMessage( Messages );

					OpenGump();
				}

				if ( version == "Mega Spawners" )
				{
					ImportOlderVersion.ImportOldVersion( gumpMobile, fileName, 1, r, ArgsList );
				}
				else if ( version == "Mega Spawners v2.0" )
				{
					ImportOlderVersion.ImportOldVersion( gumpMobile, fileName, 2, r, ArgsList );
				}
				else if ( version == "Mega Spawners v2.06" )
				{
					ImportOlderVersion.ImportOldVersion( gumpMobile, fileName, 3, r, ArgsList );
				}
				else if ( version.StartsWith( "<?xml" ) )
				{
					r.Close();

					ImportXml( fileName, loadType );
				}
				else
				{
					OpenGump();

					return;
				}
			}
			else
			{
				Messages = String.Format( "The file {0} is an unrecognized file type. Please {1} another filename.", MC.CropDirectory( fileName ), loadType == LoadType.Manual ? "type in" : "select" );

				if ( loadType == LoadType.Manual )
					gumpMobile.SendMessage( Messages );

				OpenGump();
			}
		}

		private void ImportXml( string fileName, LoadType loadType )
		{
			gumpMobile.SendMessage( "Importing spawners..." );

			XmlDocument xml = new XmlDocument();
			xml.Load( fileName );

			XmlElement spawners = xml["MegaSpawners"];
			version = GetInnerText( spawners["Version"] );

			if ( MC.IsValidVersion( version ) )
			{
				int delay = 0;

				new LoadSpawnersTimer( ArgsList, gumpMobile, version, spawners, fileName, true, delay ).Start();
			}
			else
			{
				Messages = String.Format( "Error reading file. Unknown version: {0}. Please {1} another filename.", version, loadType == LoadType.Manual ? "type in" : "select" );

				if ( loadType == LoadType.Manual )
					gumpMobile.SendMessage( Messages );

				OpenGump();

				return;
			}
		}

		private string GetInnerText( XmlElement node )
		{
			if ( node == null )
				return "Error";

			return node.InnerText;
		}

		private class LoadSpawnersTimer : Timer
		{ 
			private ArrayList ArgsList = new ArrayList();

			private ArrayList HideSpawnerList = new ArrayList();
			private ArrayList MSGCheckBoxesList = new ArrayList();

			private string MessagesTitle, Messages;

			private int count, total, amountOfSpawners, autoFailures, totalErrors, exceptions;
			private DateTime beginTime;
			private TimeSpan finishTime;

			private Mobile gumpMobile;
			private XmlElement spawners;
			private string fileName, version;
			private bool fullForce;
			private ArrayList SpawnerList = new ArrayList();

			public LoadSpawnersTimer( ArrayList argsList, Mobile mobile, string ver, XmlElement theSpawners, string filename, bool fullforce, int delay ) : base( TimeSpan.Zero, TimeSpan.FromMilliseconds( delay ) )
			{
				gumpMobile = mobile;
				version = ver;
				spawners = theSpawners;
				fileName = filename;
				fullForce = fullforce;

				ArgsList = argsList;
				GetArgsList();
			}

			protected override void OnTick()
			{
				if ( count == 0 )
				{
					total = spawners.GetElementsByTagName( "MegaSpawner" ).Count;

					beginTime = (DateTime) DateTime.Now;
				}

				XmlNode spawner = null;

				if ( !fullForce )
				{
					spawner = spawners.GetElementsByTagName( "MegaSpawner" )[count];

					ImportMegaSpawner( spawner, fileName );
				}
				else if ( count == 0 )
				{
					for ( int i = 0; i < total; i++ )
					{
						spawner = spawners.GetElementsByTagName( "MegaSpawner" )[i];

						ImportMegaSpawner( spawner, fileName );
					}

					count = total;
				}

				count++;

				if ( count >= total )
				{
					Stop();

					finishTime = DateTime.Now - beginTime;

			gumpMobile.SendMessage( "Spawner import complete." );
			gumpMobile.SendMessage( "Checking for duplicate spawners..." );

			MessagesTitle = "Import Spawners";

			if ( amountOfSpawners > 0 )
			{
				Messages = String.Format( "File type identified as a Mega Spawner v{0} file. {1} Mega Spawner{2} imported. The import process took {3} second{4}.", version, amountOfSpawners, amountOfSpawners == 1 ? "" : "s", (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );
			}
			else
			{
				Messages = String.Format( "File type identified as a Mega Spawner v{0} file. No Mega Spawners were imported due to errors in the file. The import process took {1} second{2}.", version, (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );

				MC.FileImportRemove( MC.CropDirectory( fileName ) );
			}

			if ( amountOfSpawners > 1 )
			{
				DateTime beginDupeTime = DateTime.Now;

				DupeSpawnerCheck();

				TimeSpan finishDupeTime = DateTime.Now - beginDupeTime;

				Messages = String.Format( "{0} The duped spawner check process took {1} second{2}.", Messages, (int) finishDupeTime.TotalSeconds, (int) finishDupeTime.TotalSeconds == 1 ? "" : "s" );
			}

			if ( totalErrors > 0 )
				Messages = String.Format( "{0} {1} error{2} been detected.", Messages, totalErrors, totalErrors == 1 ? " has" : "s have" );

			if ( exceptions > 0 )
				Messages = String.Format( "{0} {1} exception{2} been detected. This is a result of an older file export missing certain data. This is not a concern. Any missing data has been replaced by default data.", Messages, exceptions, exceptions == 1 ? " has" : "s have" );

			if ( autoFailures > 0 )
				Messages = String.Format( "{0} {1} auto Z placement failure{2} been detected. The import file contains Mega Spawners configured for auto Z placement. There was a problem and placement has failed. Those Mega Spawners have been deleted.", Messages, autoFailures, autoFailures == 1 ? " has" : "s have" );

			MC.CheckFileImportList( fileName );

			SetArgsList();

			gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );
				}
			}

			private void ImportMegaSpawner( XmlNode node, string fileName )
			{
				int errors = 0;

				amountOfSpawners++;

				Map map = null;
				Point3D location = new Point3D();

				MegaSpawner megaSpawner = new MegaSpawner();

				megaSpawner.Imported = MC.CropDirectory( fileName.ToLower() );
				megaSpawner.ImportVersion = version;
				megaSpawner.Editor = null;
				megaSpawner.Workspace = false;

				double ver;

				try{ ver = Convert.ToDouble( version ); }
				catch{ ver = 0.0; }

				try{ megaSpawner.Name = GetInnerText( node["Name"] ); }
				catch{ errors++; totalErrors++; }

				try{ megaSpawner.Active = bool.Parse( GetInnerText( node["Active"] ) ); }
				catch{ errors++; totalErrors++; }

				try{ map = Map.Parse( GetInnerText( node["Map"] ) ); }
				catch{ errors++; totalErrors++; }

				try{ location = Point3D.Parse( GetInnerText( node["Location"] ) ); }
				catch{ errors++; totalErrors++; }

				XmlElement entryLists = null;

				try{ entryLists = node["EntryLists"]; }
				catch{ errors++; totalErrors++; }

				if ( entryLists != null )
				{
					int cnt = 0;

					try
					{
						foreach ( XmlElement entry in entryLists.GetElementsByTagName( "EntryList" ) )
						{
							string entryCompare = null;

							try{ entryCompare = GetInnerText( entry["EntryType"] ); }
							catch{ errors++; totalErrors++; }

							int entryCount = 0;

							for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
							{
								string entryType = (string) megaSpawner.EntryList[i];

								if ( entryType.ToLower() == entryCompare.ToLower() )
									entryCount++;
							}

							if ( entryCount == 0 )
								megaSpawner.EntryList.Add( entryCompare );

							try{ megaSpawner.SpawnRangeList.Add( int.Parse( GetInnerText( entry["SpawnRange"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.WalkRangeList.Add( int.Parse( GetInnerText( entry["WalkRange"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try
							{
								int amount = int.Parse( GetInnerText( entry["Amount"] ) );

								if ( amount == 0 )
									amount = 1;

								megaSpawner.AmountList.Add( amount );
							}
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.MinDelayList.Add( int.Parse( GetInnerText( entry["MinDelay"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.MaxDelayList.Add( int.Parse( GetInnerText( entry["MaxDelay"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.SpawnTypeList.Add( int.Parse( GetInnerText( entry["SpawnType"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.ActivatedList.Add( bool.Parse( GetInnerText( entry["Activated"] ) ) );	}
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.EventRangeList.Add( int.Parse( GetInnerText( entry["EventRange"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.EventKeywordList.Add( GetInnerText( entry["EventKeyword"] ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.KeywordCaseSensitiveList.Add( bool.Parse( GetInnerText( entry["KeywordCaseSensitive"] ) ) ); }
							catch{ megaSpawner.KeywordCaseSensitiveList.Add( (bool) false ); exceptions++; }

							megaSpawner.TriggerEventNowList.Add( (bool) true );

							try{ megaSpawner.EventAmbushList.Add( bool.Parse( GetInnerText( entry["EventAmbush"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.BeginTimeBasedList.Add( int.Parse( GetInnerText( entry["BeginTimeBased"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.EndTimeBasedList.Add( int.Parse( GetInnerText( entry["EndTimeBased"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.GroupSpawnList.Add( bool.Parse( GetInnerText( entry["GroupSpawn"] ) ) ); }
							catch{ errors++; totalErrors++; }

							try{ megaSpawner.MinStackAmountList.Add( int.Parse( GetInnerText( entry["MinStackAmount"] ) ) ); }
							catch{ megaSpawner.MinStackAmountList.Add( 1 ); exceptions++; }

							try{ megaSpawner.MaxStackAmountList.Add( int.Parse( GetInnerText( entry["MaxStackAmount"] ) ) ); }
							catch{ megaSpawner.MaxStackAmountList.Add( 1 ); exceptions++; }

							if ( ver < 3.2 )
							{
								try{ megaSpawner.MovableList.Add( bool.Parse( GetInnerText( entry["ItemMovable"] ) ) ); }
								catch{ megaSpawner.MovableList.Add( (bool) true ); exceptions++; }
							}
							else
							{
								try{ megaSpawner.MovableList.Add( bool.Parse( GetInnerText( entry["Movable"] ) ) ); }
								catch{ megaSpawner.MovableList.Add( (bool) true ); exceptions++; }
							}

							if ( ver >= 3.5 )
							{
								try{ megaSpawner.MinDespawnList.Add( int.Parse( GetInnerText( entry["MinDespawn"] ) ) ); }
								catch{ errors++; totalErrors++;}

								try{ megaSpawner.MaxDespawnList.Add( int.Parse( GetInnerText( entry["MaxDespawn"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ megaSpawner.DespawnList.Add( bool.Parse( GetInnerText( entry["Despawn"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ megaSpawner.DespawnGroupList.Add( bool.Parse( GetInnerText( entry["DespawnGroup"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ megaSpawner.DespawnTimeExpireList.Add( bool.Parse( GetInnerText( entry["DespawnTimeExpire"] ) ) ); }
								catch{ errors++; totalErrors++; }
							}
							else
							{
								megaSpawner.MinDespawnList.Add( 1800 );
								megaSpawner.MaxDespawnList.Add( 3600 );
								megaSpawner.DespawnList.Add( (bool) false );
								megaSpawner.DespawnGroupList.Add( (bool) false );
								megaSpawner.DespawnTimeExpireList.Add( (bool) true );
							}

							cnt++;
						}
					}
					catch{ errors++; totalErrors++; }
				}

				XmlElement settingsList = null;

				try{ settingsList = node["SettingsList"]; }
				catch{ errors++; totalErrors++; }

				if ( settingsList != null )
				{
					if ( ver >= 3.6 )
					{
						try
						{
							foreach ( XmlElement setting in settingsList.GetElementsByTagName( "OverrideIndividualEntries" ) )
							{
								int err = errors;

								ArrayList List = new ArrayList();

								List.Add( Setting.OverrideIndividualEntries );

								try{ List.Add( int.Parse( GetInnerText( setting["SpawnRange"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["WalkRange"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["Amount"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MinDelay"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MaxDelay"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( bool.Parse( GetInnerText( setting["GroupSpawn"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( bool.Parse( GetInnerText( setting["EventAmbush"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( (SpawnType) int.Parse( GetInnerText( setting["SpawnType"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( GetInnerText( setting["EventKeyword"] ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( bool.Parse( GetInnerText( setting["CaseSensitive"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["EventRange"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["BeginTimeBasedHour"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["BeginTimeBasedMinute"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["EndTimeBasedHour"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["EndTimeBasedMinute"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MinDespawn"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MaxDespawn"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( bool.Parse( GetInnerText( setting["Despawn"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( bool.Parse( GetInnerText( setting["DespawnGroup"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( bool.Parse( GetInnerText( setting["DespawnTimeExpire"] ) ) ); }
								catch{ errors++; totalErrors++; }

								if ( err == errors )
									megaSpawner.SettingsList.Add( List );
							}
						}
						catch{}

						try
						{
							foreach ( XmlElement setting in settingsList.GetElementsByTagName( "AddItemSetting" ) )
							{
								int err = errors;

								ArrayList List = new ArrayList();

								List.Add( Setting.AddItem );

								try{ List.Add( GetInnerText( setting["EntryName"] ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["EntryIndex"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( GetInnerText( setting["AddItem"] ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MinStackAmount"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MaxStackAmount"] ) ) ); }
								catch{ errors++; totalErrors++; }

								if ( err == errors )
									megaSpawner.SettingsList.Add( List );
							}
						}
						catch{}

						try
						{
							foreach ( XmlElement setting in settingsList.GetElementsByTagName( "AddContainerSetting" ) )
							{
								int err = errors;

								ArrayList List = new ArrayList();

								List.Add( Setting.AddContainer );

								try{ List.Add( GetInnerText( setting["EntryName"] ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["EntryIndex"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( GetInnerText( setting["AddItem"] ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MinStackAmount"] ) ) ); }
								catch{ errors++; totalErrors++; }

								try{ List.Add( int.Parse( GetInnerText( setting["MaxStackAmount"] ) ) ); }
								catch{ errors++; totalErrors++; }

								XmlElement contList = null;

								try{ contList = setting["ContainerItems"]; }
								catch{ errors++; totalErrors++; }

								try
								{
									foreach ( XmlElement contItem in contList.GetElementsByTagName( "ContainerItem" ) )
									{
										ArrayList ItemsList = new ArrayList();

										try{ ItemsList.Add( GetInnerText( contItem["AddItem"] ) ); }
										catch{ errors++; totalErrors++; }

										try{ ItemsList.Add( int.Parse( GetInnerText( contItem["MinStackAmount"] ) ) ); }
										catch{ errors++; totalErrors++; }

										try{ ItemsList.Add( int.Parse( GetInnerText( contItem["MaxStackAmount"] ) ) ); }
										catch{ errors++; totalErrors++; }

										List.Add( ItemsList );
									}
								}
								catch{}

								if ( err == errors )
									megaSpawner.SettingsList.Add( List );
							}
						}
						catch{}
					}
					else
					{
						foreach ( XmlElement setting in settingsList.GetElementsByTagName( "Setting" ) )
						{
							try{ megaSpawner.SettingsList.Add( setting.InnerText ); }
							catch{ errors++; totalErrors++; }
						}

						megaSpawner.ConvertOldSettings();
					}
				}

				if ( errors > 0 )
				{
					megaSpawner.Delete();

					amountOfSpawners--;
				}
				else
				{
					megaSpawner.MoveToWorld( location, map );

					if ( location.Z == -999 )
						megaSpawner.Location = GetLocation( megaSpawner, location );

					if ( megaSpawner.Location.Z == -999 )
					{
						megaSpawner.Delete();

						amountOfSpawners--;
						autoFailures++;

						return;
					}

					megaSpawner.CheckDupedSettings();
					megaSpawner.CompileSettings();

					megaSpawner.SettingsList.Sort( new MC.SettingsSorter() );

					if ( megaSpawner.OverrideIndividualEntries )
					{
						for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
						{
							megaSpawner.OverrideRespawnEntryList.Add( "" );
							megaSpawner.OverrideRespawnTimeList.Add( 0 );
							megaSpawner.OverrideSpawnCounterList.Add( DateTime.Now );
							megaSpawner.OverrideSpawnTimeList.Add( 0 );
							megaSpawner.OverrideDespawnTimeList.Add( 0 );
						}
					}
					else
					{
						for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
						{
							ArrayList respawnEntryList = new ArrayList();	
							ArrayList respawnTimeList = new ArrayList();
							ArrayList spawnCounterList = new ArrayList();
							ArrayList spawnTimeList = new ArrayList();
							ArrayList respawnOnSaveList = new ArrayList();
							ArrayList despawnTimeList = new ArrayList();

							for ( int j = 0; j < (int) megaSpawner.AmountList[i]; j++ )
							{
								respawnEntryList.Add( (string) megaSpawner.EntryList[i] );
								respawnTimeList.Add( 0 );
								spawnCounterList.Add( DateTime.Now );
								spawnTimeList.Add( 0 );
								respawnOnSaveList.Add( (bool) false );
								despawnTimeList.Add( 0 );
							}

							megaSpawner.RespawnEntryList.Add( respawnEntryList );
							megaSpawner.RespawnTimeList.Add( respawnTimeList );
							megaSpawner.SpawnCounterList.Add( spawnCounterList );
							megaSpawner.SpawnTimeList.Add( spawnTimeList );	
							megaSpawner.RespawnOnSaveList.Add( respawnOnSaveList );
							megaSpawner.DespawnTimeList.Add( despawnTimeList );
							megaSpawner.SpawnedEntries.Add( new ArrayList() );
							megaSpawner.LastMovedList.Add( new ArrayList() );
						}
					}

					SpawnerList.Add( megaSpawner );

					if ( megaSpawner.Active )
					{
						megaSpawner.Respawn();
					}

					HideSpawnerList.Add( (bool) false );
					MSGCheckBoxesList.Add( (bool) false );

					MC.FileImportAdd( megaSpawner.Imported, megaSpawner.ImportVersion );
				}
			}

			private Point3D GetLocation( MegaSpawner megaSpawner, Point3D loc )
			{
				return new Point3D( megaSpawner.X, megaSpawner.Y, megaSpawner.Map.GetAverageZ( megaSpawner.X, megaSpawner.Y ) );
			}

			private void DupeSpawnerCheck()
			{
				int numDuped = 0;

				foreach ( MegaSpawner fromCompare in SpawnerList )
				{
					ArrayList CompareList = MC.CompileSameLocationList( fromCompare );

					foreach ( MegaSpawner toCompare in CompareList )
						numDuped += MC.SpawnerCompare( fromCompare, toCompare, true );
				}

				if ( numDuped > 0 )
					Messages = String.Format( "{0} There {1} {2} duped spawner{3} detected and removed.", Messages, numDuped == 1 ? "was" : "were", numDuped, numDuped == 1 ? "" : "s" );
				else
					Messages = String.Format( "{0} There were no duped spawners detected.", Messages );
			}

			private void SetArgsList()
			{
				ArgsList[2] = MessagesTitle;
				ArgsList[4] = Messages;
				ArgsList[6] = HideSpawnerList;
				ArgsList[13] = MSGCheckBoxesList;
			}

			private void GetArgsList()
			{
				MessagesTitle = (string)						ArgsList[2];
				Messages = (string)								ArgsList[4];
				HideSpawnerList = (ArrayList) 					ArgsList[6];
				MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			}

			private string GetInnerText( XmlElement node )
			{
				if ( node == null )
					return "Error";

				return node.InnerText;
			}
		}
	}
}