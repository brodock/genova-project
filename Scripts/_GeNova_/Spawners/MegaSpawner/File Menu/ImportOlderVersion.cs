using System;
using System.IO;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ImportOlderVersion
	{
		public static void ImportOldVersion( Mobile mobile, string fileName, int version, StreamReader r, ArrayList ArgsList )
		{
			switch( version )
			{
				case 1:
				{
					Importv1( mobile, fileName, r, ArgsList );

					break;
				}
				default:
				{
					Importv2( mobile, fileName, r, ArgsList, version );

					break;
				}
			}
		}

		public static void Importv1( Mobile mobile, string fileName, StreamReader r, ArrayList ArgsList )
		{
			ArrayList SpawnerList = new ArrayList();
			int totalErrors = 0;

			string MessagesTitle = (string)					ArgsList[2];
			string Messages = (string)					ArgsList[4];
			ArrayList HideSpawnerList = (ArrayList)				ArgsList[6];
			ArrayList MSGCheckBoxesList = (ArrayList)			ArgsList[13];

			mobile.SendMessage( "Importing spawners..." );

			DateTime beginTime = DateTime.Now;

			int amountOfSpawners=0, amountOfEntries=0, locX=0, locY=0, locZ=0;
			string map = null;
			Map spawnerMap = null;

			try{ amountOfSpawners = Convert.ToInt32( r.ReadLine() ); }
			catch{ totalErrors++; }

			for ( int i = 0; i < amountOfSpawners; i++ )
			{
				int errors = 0;

				MegaSpawner megaSpawner = new MegaSpawner();

				megaSpawner.Imported = CropDirectory( fileName.ToLower() );
				megaSpawner.ImportVersion = "1";
				megaSpawner.Editor = null;
				megaSpawner.Workspace = false;

				try{ map =  r.ReadLine().ToLower(); }
				catch{ errors++; totalErrors++; }

				if ( map == "felucca" )
					spawnerMap = Map.Felucca;

				if ( map == "trammel" )
					spawnerMap = Map.Trammel;

				if ( map == "ilshenar" )
					spawnerMap = Map.Ilshenar;

				if ( map == "malas" )
					spawnerMap = Map.Malas;

				try{ locX = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ locY = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ locZ = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ amountOfEntries = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				for ( int cnt = 0; cnt < amountOfEntries; cnt++ )
				{
					try{ megaSpawner.EntryList.Add( r.ReadLine() );	}					// Entry List
					catch{ errors++; totalErrors++; }

					try{ megaSpawner.SpawnRangeList.Add( Convert.ToInt32( r.ReadLine() ) );	}		// Spawn Range List
					catch{ errors++; totalErrors++; }

					try{ megaSpawner.WalkRangeList.Add( Convert.ToInt32( r.ReadLine() ) ); }		// Walk Range List
					catch{ errors++; totalErrors++; }

					try{ megaSpawner.AmountList.Add( Convert.ToInt32( r.ReadLine() ) ); }			// Amount List
					catch{ errors++; totalErrors++; }

					try{ megaSpawner.MinDelayList.Add( Convert.ToInt32( r.ReadLine() ) ); }			// Min Delay List
					catch{ errors++; totalErrors++; }

					try{ megaSpawner.MaxDelayList.Add( Convert.ToInt32( r.ReadLine() ) ); }			// Max Delay List
					catch{ errors++; totalErrors++; }

					megaSpawner.SpawnTypeList.Add( SpawnType.Regular );					// Spawn Type List
					megaSpawner.ActivatedList.Add( (bool) true );						// Activated List
					megaSpawner.EventRangeList.Add( 10 );							// EVent Range List
					megaSpawner.EventKeywordList.Add( "" );							// EVent Keyword List
					megaSpawner.KeywordCaseSensitiveList.Add( (bool) false );
					megaSpawner.TriggerEventNowList.Add( (bool) true );					// Trigger Event Now List
					megaSpawner.EventAmbushList.Add( (bool) true );						// Event Ambush List
					megaSpawner.BeginTimeBasedList.Add( 0 );						// Begin Time Based List
					megaSpawner.EndTimeBasedList.Add( 0 );							// End Time Based List
					megaSpawner.GroupSpawnList.Add( (bool) false );						// Group Spawn List
					megaSpawner.MinStackAmountList.Add( 0 );
					megaSpawner.MaxStackAmountList.Add( 0 );
			 		megaSpawner.MovableList.Add( (bool) true );
					megaSpawner.MinDespawnList.Add( 1800 );
					megaSpawner.MaxDespawnList.Add( 3600 );
					megaSpawner.DespawnList.Add( (bool) false );
					megaSpawner.DespawnGroupList.Add( (bool) false );
					megaSpawner.DespawnTimeExpireList.Add( (bool) true );

					ArrayList respawnEntryList = new ArrayList();
					ArrayList respawnTimeList = new ArrayList();
					ArrayList spawnCounterList = new ArrayList();
					ArrayList spawnTimeList = new ArrayList();
					ArrayList respawnOnSaveList = new ArrayList();
					ArrayList despawnTimeList = new ArrayList();

					for ( int j = 0; j < (int) megaSpawner.AmountList[cnt]; j++ )
					{
						respawnEntryList.Add( (string) megaSpawner.EntryList[cnt] );
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

				if ( errors > 0 )
				{
					megaSpawner.Delete();

					amountOfSpawners--;
				}
				else
				{
					SpawnerList.Add( megaSpawner );

					megaSpawner.MoveToWorld( new Point3D( locX, locY, locZ ), spawnerMap );

					if ( megaSpawner.Active )
					{
						megaSpawner.Start();
						megaSpawner.Respawn();
					}

					HideSpawnerList.Add( (bool) false );
					MSGCheckBoxesList.Add( (bool) false );

					MC.FileImportAdd( megaSpawner.Imported, megaSpawner.ImportVersion );
				}
			}

			r.Close();

			TimeSpan finishTime = DateTime.Now - beginTime;

			MessagesTitle = "Import Spawners";

			if ( amountOfSpawners > 0 )
			{
				Messages = String.Format( "File type identified as a Mega Spawner v1 file. {0} Mega Spawner{1} imported. The process took {2} second{3}.", amountOfSpawners, amountOfSpawners == 1 ? "" : "s", (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );
			}
			else
			{
				Messages = String.Format( "File type identified as a Mega Spawner v1 file. No Mega Spawners were imported due to errors in the file. The process took {0} second{1}.", (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );

				MC.FileImportRemove( CropDirectory( fileName ) );
			}

			DateTime beginDupeTime = DateTime.Now;

			Messages = DupeSpawnerCheck( SpawnerList, Messages );

			TimeSpan finishDupeTime = DateTime.Now - beginDupeTime;

			Messages = String.Format( "{0} The duped spawner check process took {1} second{2}.", Messages, (int) finishDupeTime.TotalSeconds, (int) finishDupeTime.TotalSeconds == 1 ? "" : "s" );

			if ( totalErrors > 0 )
				Messages = String.Format( "{0} {1} error{2} been detected.", Messages, totalErrors, totalErrors == 1 ? " has" : "s have" );

			MC.CheckFileImportList( fileName );

			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[13] = MSGCheckBoxesList;

			mobile.CloseGump( typeof ( LoadFileGump ) );
			mobile.SendGump( new FileMenuGump( mobile, ArgsList ) );
		}

		public static void Importv2( Mobile mobile, string fileName, StreamReader r, ArrayList ArgsList, int version )
		{
			ArrayList SpawnerList = new ArrayList();
			int totalErrors = 0;
			string importVersion = null;

			string MessagesTitle = (string)					ArgsList[2];
			string Messages = (string)					ArgsList[4];
			ArrayList HideSpawnerList = (ArrayList)				ArgsList[6];
			ArrayList MSGCheckBoxesList = (ArrayList)			ArgsList[13];

			switch ( version )
			{
				case 2:{ importVersion = "2"; break; }
				case 3:{ importVersion = "2.06"; break; }
			}

			mobile.SendMessage( "Importing spawners..." );

			DateTime beginTime = DateTime.Now;

			int amountOfSpawners=0, amountOfEntries=0, locX=0, locY=0, locZ=0;
			string map = null;
			Map spawnerMap = null;
			bool DummyBool = false;

			try{ amountOfSpawners = Convert.ToInt32( r.ReadLine() ); }
			catch{ totalErrors++; }

			bool attempt, retry;

			for ( int i = 0; i < amountOfSpawners; i++ )
			{
				int errors = 0;

				MegaSpawner megaSpawner = new MegaSpawner();

				megaSpawner.Imported = CropDirectory( fileName.ToLower() );
				megaSpawner.ImportVersion = importVersion;
				megaSpawner.Editor = null;
				megaSpawner.Workspace = false;

				if ( version >= 3 )
				{
					try{ megaSpawner.Name = r.ReadLine(); }
					catch{ errors++; totalErrors++; }
				}

				try{ megaSpawner.Active = Convert.ToBoolean( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ DummyBool = Convert.ToBoolean( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ map =  r.ReadLine().ToLower(); }
				catch{ errors++; totalErrors++; }

				if ( map == "felucca" )
					spawnerMap = Map.Felucca;

				if ( map == "trammel" )
					spawnerMap = Map.Trammel;

				if ( map == "ilshenar" )
					spawnerMap = Map.Ilshenar;

				if ( map == "malas" )
					spawnerMap = Map.Malas;

				try{ locX = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ locY = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ locZ = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				try{ amountOfEntries = Convert.ToInt32( r.ReadLine() ); }
				catch{ errors++; totalErrors++; }

				attempt = false;
				retry = false;

				while ( !attempt )
				{
					for ( int cnt = 0; cnt < amountOfEntries; cnt++ )
					{
						try{ megaSpawner.EntryList.Add( r.ReadLine() );	}					// Entry List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.SpawnRangeList.Add( Convert.ToInt32( r.ReadLine() ) ); }		// Spawn Range List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.WalkRangeList.Add( Convert.ToInt32( r.ReadLine() ) ); }		// Walk Range List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.AmountList.Add( Convert.ToInt32( r.ReadLine() ) ); }			// Amount List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.MinDelayList.Add( Convert.ToInt32( r.ReadLine() ) ); }			// Min Delay List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.MaxDelayList.Add( Convert.ToInt32( r.ReadLine() ) ); }			// Max Delay List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.SpawnTypeList.Add( Convert.ToInt32( r.ReadLine() ) ); }		// Spawn Type List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.ActivatedList.Add( Convert.ToBoolean( r.ReadLine() ) ); }		// Activated List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.EventRangeList.Add( Convert.ToInt32( r.ReadLine() ) );	}		// Event Range List
						catch{ errors++; totalErrors++; }

						if ( !retry )
						{
							try{ megaSpawner.EventKeywordList.Add( Convert.ToString( r.ReadLine() ) ); }	// Event Keyword List
							catch{ errors++; totalErrors++; }
						}
						else
						{
							megaSpawner.EventKeywordList.Add( "" );
						}

						megaSpawner.KeywordCaseSensitiveList.Add( (bool) false );

						try{ megaSpawner.EventAmbushList.Add( Convert.ToBoolean( r.ReadLine() ) ); }		// Event Ambush List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.BeginTimeBasedList.Add( Convert.ToInt32( r.ReadLine() ) ); }		// Begin Time Based List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.EndTimeBasedList.Add( Convert.ToInt32( r.ReadLine() ) ); }		// End Time Based List
						catch{ errors++; totalErrors++; }

						try{ megaSpawner.GroupSpawnList.Add( Convert.ToBoolean( r.ReadLine() ) ); }		// Group Spawn List
						catch{ errors++; totalErrors++; }

						megaSpawner.TriggerEventNowList.Add( (bool) true );					// Trigger Event Now List
						megaSpawner.MinStackAmountList.Add( 0 );
						megaSpawner.MaxStackAmountList.Add( 0 );
				 		megaSpawner.MovableList.Add( (bool) true );
						megaSpawner.MinDespawnList.Add( 1800 );
						megaSpawner.MaxDespawnList.Add( 3600 );
						megaSpawner.DespawnList.Add( (bool) false );
						megaSpawner.DespawnGroupList.Add( (bool) false );
						megaSpawner.DespawnTimeExpireList.Add( (bool) true );

						ArrayList respawnEntryList = new ArrayList();
						ArrayList respawnTimeList = new ArrayList();
						ArrayList spawnCounterList = new ArrayList();
						ArrayList spawnTimeList = new ArrayList();
						ArrayList respawnOnSaveList = new ArrayList();
						ArrayList despawnTimeList = new ArrayList();

						for ( int j = 0; j < (int) megaSpawner.AmountList[cnt]; j++ )
						{
							respawnEntryList.Add( (string) megaSpawner.EntryList[cnt] );
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

					if ( retry )
					{
						attempt = true;

						break;
					}

					if ( errors > 0 && !retry )
					{
						errors = 0;

						retry = true;

						MegaSpawner ms = new MegaSpawner();

						ms.Active = megaSpawner.Active;
						ms.Imported = megaSpawner.Imported;
						ms.ImportVersion = megaSpawner.ImportVersion;
						ms.Editor = null;
						ms.Workspace = false;

						megaSpawner.Delete();
						megaSpawner = ms;

						r.Close();
						r = File.OpenText( fileName );

						string DummyRead;

						for ( int cnt = 0; cnt < 9; cnt++ )
							DummyRead = r.ReadLine();

						DummyRead = null;
					}
					else
					{
						attempt = true;
					}
				}

				if ( errors > 0 )
				{
					megaSpawner.Delete();

					amountOfSpawners--;
				}
				else
				{
					SpawnerList.Add( megaSpawner );

					megaSpawner.MoveToWorld( new Point3D( locX, locY, locZ ), spawnerMap );

					if ( megaSpawner.Active )
					{
						megaSpawner.Start();
						megaSpawner.Respawn();
					}

					HideSpawnerList.Add( (bool) false );
					MSGCheckBoxesList.Add( (bool) false );

					MC.FileImportAdd( megaSpawner.Imported, megaSpawner.ImportVersion );
				}
			}

			r.Close();

			TimeSpan finishTime = DateTime.Now - beginTime;

			mobile.SendMessage( "Spawner import complete." );
			mobile.SendMessage( "Checking for duplicate spawners..." );

			MessagesTitle = "Import Spawners";

			if ( amountOfSpawners > 0 )
			{
				Messages = String.Format( "File type identified as a Mega Spawner v{0} file. {1} Mega Spawner{2} imported. The process took {3} second{4}.", importVersion, amountOfSpawners, amountOfSpawners == 1 ? "" : "s", (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );
			}
			else
			{
				Messages = String.Format( "File type identified as a Mega Spawner v{0} file. No Mega Spawners were imported due to errors in the file. The process took {1} second{2}.", importVersion, (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );

				MC.FileImportRemove( CropDirectory( fileName ) );
			}

			DateTime beginDupeTime = DateTime.Now;

			Messages = DupeSpawnerCheck( SpawnerList, Messages );

			TimeSpan finishDupeTime = DateTime.Now - beginDupeTime;

			Messages = String.Format( "{0} The duped spawner check process took {1} second{2}.", Messages, (int) finishDupeTime.TotalSeconds, (int) finishDupeTime.TotalSeconds == 1 ? "" : "s" );

			if ( totalErrors > 0 )
				Messages = String.Format( "{0} {1} error{2} been detected.", Messages, totalErrors, totalErrors == 1 ? " has" : "s have" );

			MC.CheckFileImportList( fileName );

			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[13] = MSGCheckBoxesList;

			mobile.CloseGump( typeof ( LoadFileGump ) );
			mobile.SendGump( new FileMenuGump( mobile, ArgsList ) );
		}

		public static string CropDirectory( string entry )
		{
			string[] CropDir = entry.Split('\\');
			string[] CropDirTwo = entry.Split('/');

			if ( CropDir.Length > CropDirTwo.Length )
				entry = CropDir[CropDir.Length-1];
			else
				entry = CropDirTwo[CropDirTwo.Length-1];

			return entry;
		}

		public static string DupeSpawnerCheck( ArrayList SpawnerList, string Messages )
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

			return Messages;
		}
	}
}