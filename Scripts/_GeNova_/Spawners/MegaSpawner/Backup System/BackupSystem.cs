using System;
using System.IO;
using System.Collections;
using Server;
using Server.Items;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class BackupSystem
	{
		public static void LoadBackup( Mobile mobile, ArrayList ArgsList, string filePath )
		{
			ArrayList HideSpawnerList = (ArrayList)			ArgsList[6];
			ArrayList MSGCheckBoxesList = (ArrayList)		ArgsList[13];

			MC.SetProcess( Process.LoadBackup );

			FileStream fs;
			BinaryFileReader reader;

			mobile.SendMessage( "Loading backup file..." );

			try
			{
				fs = new FileStream( filePath, (FileMode) 3, (FileAccess) 1, (FileShare) 1 );
				reader = new BinaryFileReader( new BinaryReader( fs ) );
			}
			catch(Exception ex)
			{
				MC.SetProcess( Process.None );

				ArgsList[2] = "Load Backup File";
				ArgsList[4] = String.Format( "Exception caught:\n{0}", ex );

				mobile.SendGump( new FileBrowserGump( mobile, ArgsList ) );

				return;
			}

			int amountOfSpawners = reader.ReadInt();
			int cnt = 0;

			for ( int i = 0; i < amountOfSpawners; i++ )
			{
				if ( Deserialize( (GenericReader) reader ) )
				{
					HideSpawnerList.Add( (bool) false );
					MSGCheckBoxesList.Add( (bool) false );

					cnt++;
				}
			}

			if ( fs != null )
				fs.Close();

			MC.SetProcess( Process.None );

			ArgsList[2] = "Load Backup File";
			ArgsList[4] = String.Format( "Loading of backup file complete. {0} Mega Spawner{1} been installed.", cnt, cnt == 1 ? " has" : "s have" );
			ArgsList[6] = HideSpawnerList;
			ArgsList[13] = MSGCheckBoxesList;

			mobile.SendGump( new FileMenuGump( mobile, ArgsList ) );
		}

		public static bool Deserialize( GenericReader reader )
		{
			bool success = false;

			Map map = null;
			Point3D location = new Point3D();

			MegaSpawner megaSpawner = new MegaSpawner( true );

			try
			{
				int version = reader.ReadInt();

				if ( version >= 1 )
				{
					location = reader.ReadPoint3D();
					map = reader.ReadMap();
					megaSpawner.Active = reader.ReadBool();

					megaSpawner.Imported = reader.ReadString();
					megaSpawner.ImportVersion = reader.ReadString();
					megaSpawner.ContainerSpawn = (Container) reader.ReadItem();
					megaSpawner.Workspace = reader.ReadBool();

					int count = reader.ReadInt();

					for ( int i = 0; i < count; i++ )
						megaSpawner.EntryList.Add( reader.ReadString() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.SpawnRangeList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.WalkRangeList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
					{
						int amount = reader.ReadInt();

						if ( amount == 0 )
							amount = 1;

						megaSpawner.AmountList.Add( amount );
					}

					for ( int i = 0; i < count; i++ )
						megaSpawner.MinDelayList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.MaxDelayList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.SpawnTypeList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.ActivatedList.Add( reader.ReadBool() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.EventRangeList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.EventKeywordList.Add( reader.ReadString() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.KeywordCaseSensitiveList.Add( reader.ReadBool() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.TriggerEventNowList.Add( reader.ReadBool() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.EventAmbushList.Add( reader.ReadBool() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.BeginTimeBasedList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.EndTimeBasedList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.GroupSpawnList.Add( reader.ReadBool() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.MinStackAmountList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.MaxStackAmountList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						megaSpawner.MovableList.Add( reader.ReadBool() );

					if ( version >= 2 )
					{
						for ( int i = 0; i < count; i++ )
							megaSpawner.MinDespawnList.Add( reader.ReadInt() );

						for ( int i = 0; i < count; i++ )
							megaSpawner.MaxDespawnList.Add( reader.ReadInt() );

						for ( int i = 0; i < count; i++ )
							megaSpawner.DespawnList.Add( reader.ReadBool() );

						for ( int i = 0; i < count; i++ )
							megaSpawner.DespawnGroupList.Add( reader.ReadBool() );

						for ( int i = 0; i < count; i++ )
							megaSpawner.DespawnTimeExpireList.Add( reader.ReadBool() );
					}
					else
					{
						for ( int i = 0; i < count; i++ )
							megaSpawner.MinDespawnList.Add( 1800 );

						for ( int i = 0; i < count; i++ )
							megaSpawner.MaxDespawnList.Add( 3600 );

						for ( int i = 0; i < count; i++ )
							megaSpawner.DespawnList.Add( (bool) false );

						for ( int i = 0; i < count; i++ )
							megaSpawner.DespawnGroupList.Add( (bool) false );

						for ( int i = 0; i < count; i++ )
							megaSpawner.DespawnTimeExpireList.Add( (bool) true );
					}

					int settingsCount = reader.ReadInt();

					if ( version >= 3 )
					{
						for ( int i = 0; i < settingsCount; i++ )
						{
							ArrayList List = new ArrayList();

							Setting setting = (Setting) reader.ReadInt();

							List.Add( setting );

							switch ( setting )
							{
								case Setting.OverrideIndividualEntries:
								{
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadBool() );
									List.Add( reader.ReadBool() );
									List.Add( (SpawnType) reader.ReadInt() );
									List.Add( reader.ReadString() );
									List.Add( reader.ReadBool() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadBool() );
									List.Add( reader.ReadBool() );
									List.Add( reader.ReadBool() );

									break;
								}
								case Setting.AddItem:
								{
									List.Add( reader.ReadString() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadString() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );

									break;
								}
								case Setting.AddContainer:
								{
									List.Add( reader.ReadString() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadString() );
									List.Add( reader.ReadInt() );
									List.Add( reader.ReadInt() );

									int settingCount = reader.ReadInt();

									for ( int j = 6; j < settingCount; j++ )
									{
										ArrayList ItemsList = new ArrayList();

										ItemsList.Add( reader.ReadString() );
										ItemsList.Add( reader.ReadInt() );
										ItemsList.Add( reader.ReadInt() );

										List.Add( ItemsList );
									}

									break;
								}
							}

							megaSpawner.SettingsList.Add( List );
						}
					}
					else
					{
						for ( int i = 0; i < settingsCount; i++ )
							megaSpawner.SettingsList.Add( reader.ReadString() );

						megaSpawner.ConvertOldSettings();
					}
				}

				if ( megaSpawner.Workspace )
				{
					megaSpawner.Delete();
				}
				else
				{
					megaSpawner.MoveToWorld( location, map );
					megaSpawner.Start();

					if ( megaSpawner.Imported != "" )
						MC.FileImportAdd( megaSpawner.Imported, megaSpawner.ImportVersion );

					megaSpawner.CompileSettings();
					megaSpawner.CheckEntryErrors();

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

					megaSpawner.Respawn();
				}

				success = true;
			}
			catch
			{
				if ( megaSpawner != null )
					megaSpawner.Delete();
			}

			return success;
		}

		public static void SaveBackup( Mobile mobile, ArrayList ArgsList )
		{
			MC.SetProcess( Process.SaveBackup );

			if ( !Directory.Exists( MC.BackupDirectory ) )
				Directory.CreateDirectory( MC.BackupDirectory );

			string path = Path.Combine( MC.BackupDirectory, "Backup.mbk" );

			mobile.SendMessage( "Creating backup file..." );

			MC.CheckSpawners();

			ArrayList SpawnerList = CompileSpawnerList();

			GenericWriter writer;

			try
			{
				writer = new BinaryFileWriter( path, true );
			}
			catch(Exception ex)
			{
				MC.SetProcess( Process.None );

				ArgsList[2] = "Create Backup File";
				ArgsList[4] = String.Format( "Exception caught:\n{0}", ex );

				mobile.SendGump( new FileMenuGump( mobile, ArgsList ) );

				return;
			}

			writer.Write( SpawnerList.Count );

			try
			{
				foreach ( MegaSpawner megaSpawner in SpawnerList )
					Serialize( megaSpawner, writer );
			}
			catch{}

			writer.Close();

			MC.SetProcess( Process.None );

			ArgsList[2] = "Create Backup File";
			ArgsList[4] = "All Mega Spawners have now been backed up to the backup file.";

			mobile.SendGump( new FileMenuGump( mobile, ArgsList ) );
		}

		public static void Serialize( MegaSpawner megaSpawner, GenericWriter writer )
		{
			writer.Write( (int) 3 ); // version

			writer.Write( megaSpawner.Location );
			writer.Write( megaSpawner.Map );
			writer.Write( megaSpawner.Active );

			writer.Write( megaSpawner.Imported );
			writer.Write( megaSpawner.ImportVersion );
			writer.Write( megaSpawner.ContainerSpawn );
			writer.Write( megaSpawner.Workspace );

			writer.Write( megaSpawner.EntryList.Count );

			for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
				writer.Write( (string) megaSpawner.EntryList[i] );

			for ( int i = 0; i < megaSpawner.SpawnRangeList.Count; i++ )
				writer.Write( (int) megaSpawner.SpawnRangeList[i] );

			for ( int i = 0; i < megaSpawner.WalkRangeList.Count; i++ )
				writer.Write( (int) megaSpawner.WalkRangeList[i] );

			for ( int i = 0; i < megaSpawner.AmountList.Count; i++ )
				writer.Write( (int) megaSpawner.AmountList[i] );

			for ( int i = 0; i < megaSpawner.MinDelayList.Count; i++ )
				writer.Write( (int) megaSpawner.MinDelayList[i] );

			for ( int i = 0; i < megaSpawner.MaxDelayList.Count; i++ )
				writer.Write( (int) megaSpawner.MaxDelayList[i] );

			for ( int i = 0; i < megaSpawner.SpawnTypeList.Count; i++ )
				writer.Write( (int) megaSpawner.SpawnTypeList[i] );

			for ( int i = 0; i < megaSpawner.ActivatedList.Count; i++ )
				writer.Write( (bool) megaSpawner.ActivatedList[i] );

			for ( int i = 0; i < megaSpawner.EventRangeList.Count; i++ )
				writer.Write( (int) megaSpawner.EventRangeList[i] );

			for ( int i = 0; i < megaSpawner.EventKeywordList.Count; i++ )
				writer.Write( (string) megaSpawner.EventKeywordList[i] );

			for ( int i = 0; i < megaSpawner.KeywordCaseSensitiveList.Count; i++ )
				writer.Write( (bool) megaSpawner.KeywordCaseSensitiveList[i] );

			for ( int i = 0; i < megaSpawner.TriggerEventNowList.Count; i++ )
				writer.Write( (bool) megaSpawner.TriggerEventNowList[i] );

			for ( int i = 0; i < megaSpawner.EventAmbushList.Count; i++ )
				writer.Write( (bool) megaSpawner.EventAmbushList[i] );

			for ( int i = 0; i < megaSpawner.BeginTimeBasedList.Count; i++ )
				writer.Write( (int) megaSpawner.BeginTimeBasedList[i] );

			for ( int i = 0; i < megaSpawner.EndTimeBasedList.Count; i++ )
				writer.Write( (int) megaSpawner.EndTimeBasedList[i] );

			for ( int i = 0; i < megaSpawner.GroupSpawnList.Count; i++ )
				writer.Write( (bool) megaSpawner.GroupSpawnList[i] );

			for ( int i = 0; i < megaSpawner.MinStackAmountList.Count; i++ )
				writer.Write( (int) megaSpawner.MinStackAmountList[i] );

			for ( int i = 0; i < megaSpawner.MaxStackAmountList.Count; i++ )
				writer.Write( (int) megaSpawner.MaxStackAmountList[i] );

			for ( int i = 0; i < megaSpawner.MovableList.Count; i++ )
				writer.Write( (bool) megaSpawner.MovableList[i] );

			for ( int i = 0; i < megaSpawner.MinDespawnList.Count; i++ )
				writer.Write( (int) megaSpawner.MinDespawnList[i] );

			for ( int i = 0; i < megaSpawner.MaxDespawnList.Count; i++ )
				writer.Write( (int) megaSpawner.MaxDespawnList[i] );

			for ( int i = 0; i < megaSpawner.DespawnList.Count; i++ )
				writer.Write( (bool) megaSpawner.DespawnList[i] );

			for ( int i = 0; i < megaSpawner.DespawnGroupList.Count; i++ )
				writer.Write( (bool) megaSpawner.DespawnGroupList[i] );

			for ( int i = 0; i < megaSpawner.DespawnTimeExpireList.Count; i++ )
				writer.Write( (bool) megaSpawner.DespawnTimeExpireList[i] );

			writer.Write( megaSpawner.SettingsList.Count );

			for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
			{
				ArrayList setting = (ArrayList) megaSpawner.SettingsList[i];

				writer.Write( (int) setting[0] );

				switch( (Setting) setting[0] )
				{
					case Setting.OverrideIndividualEntries:
					{
						writer.Write( (int) setting[1] );
						writer.Write( (int) setting[2] );
						writer.Write( (int) setting[3] );
						writer.Write( (int) setting[4] );
						writer.Write( (int) setting[5] );
						writer.Write( (bool) setting[6] );
						writer.Write( (bool) setting[7] );
						writer.Write( (int) setting[8] );
						writer.Write( (string) setting[9] );
						writer.Write( (bool) setting[10] );
						writer.Write( (int) setting[11] );
						writer.Write( (int) setting[12] );
						writer.Write( (int) setting[13] );
						writer.Write( (int) setting[14] );
						writer.Write( (int) setting[15] );
						writer.Write( (int) setting[16] );
						writer.Write( (int) setting[17] );
						writer.Write( (bool) setting[18] );
						writer.Write( (bool) setting[19] );
						writer.Write( (bool) setting[20] );

						break;
					}
					case Setting.AddItem:
					{
						writer.Write( (string) setting[1] );
						writer.Write( (int) setting[2] );
						writer.Write( (string) setting[3] );
						writer.Write( (int) setting[4] );
						writer.Write( (int) setting[5] );

						break;
					}
					case Setting.AddContainer:
					{
						writer.Write( (string) setting[1] );
						writer.Write( (int) setting[2] );
						writer.Write( (string) setting[3] );
						writer.Write( (int) setting[4] );
						writer.Write( (int) setting[5] );

						writer.Write( setting.Count );

						for ( int j = 6; j < setting.Count; j++ )
						{
							ArrayList ItemsList = (ArrayList) setting[j];

							writer.Write( (string) ItemsList[0] );
							writer.Write( (int) ItemsList[1] );
							writer.Write( (int) ItemsList[2] );
						}

						break;
					}
				}
			}
		}

		public static ArrayList CompileSpawnerList()
		{
			ArrayList List = new ArrayList();

			for ( int i = 0; i < MC.SpawnerList.Count; i++ )
				List.Add( MC.SpawnerList[i] );

			return List;
		}
	}
}