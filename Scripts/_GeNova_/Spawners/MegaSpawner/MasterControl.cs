using System;
using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using Server.Accounting;
using Server.Commands;


namespace Server.MegaSpawnerSystem
{
	public class MasterControl
	{
		// ****** Variables Declaration ************************
		
		public const string Version = "3.68";
		public const AccessLevel AccessLevelReq = AccessLevel.GameMaster;
		
		private static ArrayList m_AccountsList = new ArrayList();
		private static ArrayList m_AccessList = new ArrayList();
		private static ArrayList m_AdminMenuAccessList = new ArrayList();
		
		private static ArrayList m_MSUsers = new ArrayList();
		private static ArrayList m_MSEdit = new ArrayList();
		private static ArrayList m_MSWorkspace = new ArrayList();
		private static ArrayList m_MSFileEdit = new ArrayList();
		
		private static ArrayList m_SpawnerList = new ArrayList();
		private static ArrayList m_FileImportList = new ArrayList();
		private static ArrayList m_FileImportVersionList = new ArrayList();
		private static ArrayList m_FileImportCountList = new ArrayList();
		
		private static Mobile m_EditSystemConfig;
		private static ArrayList m_AccountEditors = new ArrayList();
		
		private static Hashtable m_PlugIns = new Hashtable();
		private static ArrayList m_PlugInsList = new ArrayList();
		
		private static Process RunningProcess;
		private static MasterSpawnerTimer masterSpawnerTimer;
		private static ArrayList DebugLog = new ArrayList();
		private static int m_Delay;
		
		public static ArrayList AccountsList
		{
			get{ return m_AccountsList; }
			set{ m_AccountsList = value; }
		}
		
		public static ArrayList AccessList
		{
			get{ return m_AccessList; }
			set{ m_AccessList = value; }
		}
		
		public static ArrayList AdminMenuAccessList
		{
			get{ return m_AdminMenuAccessList; }
			set{ m_AdminMenuAccessList = value; }
		}
		
		public static ArrayList MSUsers
		{
			get{ return m_MSUsers; }
			set{ m_MSUsers = value; }
		}
		
		public static ArrayList MSEdit
		{
			get{ return m_MSEdit; }
			set{ m_MSEdit = value; }
		}
		
		
		public static ArrayList MSWorkspace
		{
			get{ return m_MSWorkspace; }
			set{ m_MSWorkspace = value; }
		}
		
		
		public static ArrayList MSFileEdit
		{
			get{ return m_MSFileEdit; }
			set{ m_MSFileEdit = value; }
		}
		
		public static ArrayList SpawnerList
		{
			get{ return m_SpawnerList; }
			set{ m_SpawnerList = value; }
		}
		
		public static ArrayList FileImportList
		{
			get{ return m_FileImportList; }
			set{ m_FileImportList = value; }
		}
		
		public static ArrayList FileImportVersionList
		{
			get{ return m_FileImportVersionList; }
			set{ m_FileImportVersionList = value; }
		}
		
		public static ArrayList FileImportCountList
		{
			get{ return m_FileImportCountList; }
			set{ m_FileImportCountList = value; }
		}
		
		public static Mobile EditSystemConfig
		{
			get{ return m_EditSystemConfig; }
			set{ m_EditSystemConfig = value; }
		}
		
		public static ArrayList AccountEditors
		{
			get{ return m_AccountEditors; }
			set{ m_AccountEditors = value; }
		}
		
		public static Hashtable PlugIns
		{
			get{ return m_PlugIns; }
			set{ m_PlugIns = value; }
		}
		
		public static ArrayList PlugInsList
		{
			get{ return m_PlugInsList; }
			set{ m_PlugInsList = value; }
		}
		
		// ****** Variables Loaded By System Configuration *************
		
		private static TimerType m_TimerTypeConfig;
		private static bool m_Debug, m_StaffTriggerEvent;
		
		public static TimerType TimerTypeConfig
		{
			get{ return m_TimerTypeConfig; }
			set{ m_TimerTypeConfig = value; }
		}
		
		public static int Delay
		{
			get{ return m_Delay; }
			set{ m_Delay = value; }
		}
		
		public static bool Debug
		{
			get{ return m_Debug; }
			set{ m_Debug = value; }
		}
		
		public static bool StaffTriggerEvent
		{
			get{ return m_StaffTriggerEvent; }
			set{ m_StaffTriggerEvent = value; }
		}
		
		// ****** End of Variables Loaded By System Configuration ******
		
		// ****** Variables Controlled By Command Line Args ************
		
		private static bool m_CA_Disabled, m_CA_WipeAll;
		
		public static bool CA_Disabled
		{
			get{ return m_CA_Disabled; }
			set{ m_CA_Disabled = value; }
		}
		
		public static bool CA_WipeAll
		{
			get{ return m_CA_WipeAll; }
			set{ m_CA_WipeAll = value; }
		}
		
		// ****** End of Variables Controlled By Command Line Args *****
		
		private static string[] m_KnownExtensions = new string[]
		{
			"msf", "mbk"
		};
		
		private static string[] m_BadEntryTypes = new string[]
		{
			"megaspawner", "spawner", "xmlspawner"
		};
		
		private static string[] m_ValidVersions = new string[]
		{
			// Version 3
			"3.0", "3.02", "3.03", "3.04", "3.2", "3.21", "3.22", "3.23",
			"3.24", "3.25", "3.26", "3.3", "3.31", "3.32", "3.33", "3.34",
			"3.4", "3.41", "3.42", "3.43", "3.5", "3.51", "3.52", "3.6",
			"3.61", "3.62", "3.63", "3.64", "3.65", "3.66", "3.67", "3.68"
		};
		
		private static string[] m_DeleteMsg = new string[]
		{
			"You cannot delete me!",
			"I shall never be deleted!",
			"Keep on trying buddy!",
			"Ouch! That hurts!",
			"Stop that!",
			"Quit poking me!",
			"You are mean!",
			"Ohhh I like that. Do it again!",
			"How would you feel if I deleted you?",
			"I shall get my revenge!",
			"Never!",
			"You will never win!",
			"Eat me!",
			"dieirlkthnxbye!",
			"I am now taking over your server!",
			"Fear the undeletable Mega Spawner!",
			"Formatting hard drive. Please wait..."
		};
		
		public static string[] KnownExtensions
		{
			get{ return m_KnownExtensions; }
			set{ m_KnownExtensions = value; }
		}
		
		public static string[] BadEntryTypes
		{
			get{ return m_BadEntryTypes; }
			set{ m_BadEntryTypes = value; }
		}
		
		public static string[] ValidVersions
		{
			get{ return m_ValidVersions; }
			set{ m_ValidVersions = value; }
		}
		
		public static string[] DeleteMsg
		{
			get{ return m_DeleteMsg; }
			set{ m_DeleteMsg = value; }
		}
		
		// ****** Data Paths ***********
		
		private static string SavePath = "Data\\Mega Spawner System";
		private static string m_SaveDirectory = Path.Combine( Core.BaseDirectory, SavePath );
		private static string m_PersonalConfigsDirectory = Path.Combine( SaveDirectory, "Personal Configs" );
		private static string m_SpawnerExportsDirectory = Path.Combine( SaveDirectory, "Spawner Exports" );
		private static string m_BackupDirectory = Path.Combine( SavePath, "Backups" );
		private static string m_ConfigFileName = Path.Combine( SaveDirectory, "MasterControl.mcf" );
		private static string m_AccountsFileName = Path.Combine( SaveDirectory, "Accounts.mcf" );
		
		// ****** End of Data Paths ****
		
		public static string SaveDirectory
		{
			get{ return m_SaveDirectory; }
			set{ m_SaveDirectory = value; }
		}
		
		public static string PersonalConfigsDirectory
		{
			get{ return m_PersonalConfigsDirectory; }
			set{ m_PersonalConfigsDirectory = value; }
		}
		
		public static string SpawnerExportsDirectory
		{
			get{ return m_SpawnerExportsDirectory; }
			set{ m_SpawnerExportsDirectory = value; }
		}
		
		public static string BackupDirectory
		{
			get{ return m_BackupDirectory; }
			set{ m_BackupDirectory = value; }
		}
		
		public static string ConfigFileName
		{
			get{ return m_ConfigFileName; }
			set{ m_ConfigFileName = value; }
		}
		
		public static string AccountsFileName
		{
			get{ return m_AccountsFileName; }
			set{ m_AccountsFileName = value; }
		}
		
		// ****** End of Variables Declaration *****************
		
		public static bool IsKnownExtension( string fileName )
		{
			string ext = fileName.Remove( 0, fileName.Length - 3 ).ToLower();
			
			return ( Array.IndexOf( KnownExtensions, ext ) >= 0 );
			
		}
		
		public static bool FileExtensionIs( string fileName, string isExt )
		{
			string ext = fileName.Remove( 0, fileName.Length - 3 ).ToLower();
			
			return ( ext == isExt );
		}
		
		public static LoadType GetLoadType( string fileName )
		{
			string ext = fileName.Remove( 0, fileName.Length - 3 ).ToLower();
			
			switch ( ext )
			{
					case "msf":{ return LoadType.FileBrowserMsf; }
					case "mbk":{ return LoadType.FileBrowserMbk; }
					default:{ return LoadType.Error; }
			}
		}
		
		public static bool IsBadEntryType( string entry )
		{
			return ( Array.IndexOf( BadEntryTypes, entry.ToLower() ) >= 0 );
		}
		
		public static bool IsValidVersion( string version )
		{
			return ( Array.IndexOf( ValidVersions, version.ToLower() ) >= 0 );
		}
		
		// ****** Html Colors *************
		
		public static string TextColorLightPurple{ get{ return "CC66FF"; } }
		public static string TextColorPurple{ get{ return "9900FF"; } }
		public static string TextColorDarkPurple{ get{ return "660099"; } }
		
		public static string TextColorLightBlue{ get{ return "33CCFF"; } }
		public static string TextColorBlue{ get{ return "3366FF"; } }
		public static string TextColorDarkBlue{ get{ return "000099"; } }
		
		public static string TextColorLightRed{ get{ return "FF3366"; } }
		public static string TextColorRed{ get{ return "FF0000"; } }
		public static string TextColorDarkRed{ get{ return "990000"; } }
		
		public static string TextColorLightGreen{ get{ return "99FF66"; } }
		public static string TextColorGreen{ get{ return "009900"; } }
		public static string TextColorDarkGreen{ get{ return "003300"; } }
		
		public static string TextColorLightYellow{ get{ return "FFFFAA"; } }
		public static string TextColorYellow{ get{ return "FFFF00"; } }
		public static string TextColorDarkYellow{ get{ return "CCCC00"; } }
		
		public static string TextColorWhite{ get{ return "FFFFFF"; } }
		public static string TextColorLightGray{ get{ return "CCCCCC"; } }
		public static string TextColorMediumLightGray{ get{ return "999999"; } }
		public static string TextColorMediumGray{ get{ return "666666"; } }
		public static string TextColorDarkGray{ get{ return "333333"; } }
		public static string TextColorBlack{ get{ return "111111"; } }
		
		// ****** End Of Html Colors ******
		
		public delegate void OnCommand( Mobile mobile, ArrayList list );
		
		public static void Initialize()
		{
			SetVersion();
			
			RunningProcess = Process.None;
			
			EventSink.Logout += new LogoutEventHandler( OnLogout );
			EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
			
			LoadAccountsAccess();
			LoadSystemConfig();
			
			StartTimerSystem();
		}
		
		private static void OnLogout( LogoutEventArgs e )
		{
			Logout( e.Mobile );
		}
		
		private static void EventSink_Disconnected( DisconnectedEventArgs e )
		{
			Logout( e.Mobile );
		}
		
		public static void Logout( Mobile mobile )
		{
			if ( EditSystemConfig == mobile )
				EditSystemConfig = null;
			
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				Mobile editor = (Mobile) AccountEditors[i];
				
				if ( mobile == editor )
				{
					AccountEditors[i] = null;
					
					break;
				}
			}
			
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
				{
					MegaSpawner megaSpawner = (MegaSpawner) MSEdit[i];
					
					if ( megaSpawner != null )
						megaSpawner.Editor = null;
					
					ArrayList Workspace = (ArrayList) MSWorkspace[i];
					
					for ( int j = 0; j < Workspace.Count; j++ )
					{
						megaSpawner = (MegaSpawner) Workspace[j];
						
						if ( !megaSpawner.Deleted )
							megaSpawner.Delete();
					}
					
					ArrayList FileEdit = (ArrayList) MSFileEdit[i];
					
					for ( int j = 0; j < FileEdit.Count; j++ )
					{
						megaSpawner = (MegaSpawner) FileEdit[j];
						
						if ( !megaSpawner.Deleted )
							megaSpawner.FileEdit = null;
					}
					
					MSUsers.RemoveAt( i );
					MSEdit.RemoveAt( i );
					MSWorkspace.RemoveAt( i );
					MSFileEdit.RemoveAt( i );
					
					break;
				}
			}
		}
		
		public static void CheckCommandLineArgs()
		{
			string[] args = Environment.GetCommandLineArgs();
			
			foreach( string arg in args )
			{
				switch ( arg.ToLower() )
				{
					case "-mss(disable)":
						{
							if ( !CA_Disabled )
							{
								Console.WriteLine( "\nMega Spawner System: All spawners have been deactivated." );
								CA_Disabled = true;
							}
							
							break;
						}
					case "-mss(wipeall)":
						{
							if ( !CA_WipeAll )
							{
								Console.WriteLine( "\nMega Spawner System: All spawners have been deleted." );
								CA_WipeAll = true;
							}
							
							break;
						}
				}
			}
		}
		
		public static bool IsLoggedIn( Mobile mobile )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
					return true;
			}
			
			return false;
		}
		
		public static void AddMSUser( Mobile mobile )
		{
			MSUsers.Add( mobile );
			MSEdit.Add( null );
			MSWorkspace.Add( new ArrayList() );
			MSFileEdit.Add( new ArrayList() );
		}
		
		public static void AddToWorkspace( Mobile mobile, MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
				{
					ArrayList Workspace = (ArrayList) MSWorkspace[i];
					Workspace.Add( megaSpawner );
					MSWorkspace[i] = Workspace;
				}
			}
		}
		
		public static void RemoveWorkspace( Mobile mobile )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
				{
					ArrayList Workspace = (ArrayList) MSWorkspace[i];
					
					for ( int j = 0; j < Workspace.Count; j++ )
					{
						MegaSpawner workSpawner = (MegaSpawner) Workspace[j];
						
						if ( !workSpawner.Deleted )
						{
							workSpawner.Delete();
						}
					}
					
					MSWorkspace[i] = new ArrayList();
					
					break;
				}
			}
		}
		
		public static void AddToFileEdit( Mobile mobile, MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
				{
					ArrayList FileEdit = (ArrayList) MSFileEdit[i];
					FileEdit.Add( megaSpawner );
					MSFileEdit[i] = FileEdit;
				}
			}
		}
		
		public static void RemoveFileEdit( Mobile mobile )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
				{
					ArrayList FileEdit = (ArrayList) MSFileEdit[i];
					
					for ( int j = 0; j < FileEdit.Count; j++ )
					{
						MegaSpawner fileEditSpawner = (MegaSpawner) FileEdit[j];
						
						if ( !fileEditSpawner.Deleted )
							fileEditSpawner.FileEdit = null;
					}
					
					MSFileEdit[i] = new ArrayList();
					
					break;
				}
			}
		}
		
		public static void AddEditor( Mobile mobile, MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
					MSEdit[i] = megaSpawner;
			}
		}
		
		public static void RemoveEditor( Mobile mobile, MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
					MSEdit[i] = null;
			}
		}
		
		public static MegaSpawner DupeSpawner( MegaSpawner fromDupe, MegaSpawner toDupe )
		{
			toDupe.EntryList 					= 	DupeList( fromDupe.EntryList );
			toDupe.SpawnRangeList 				= 	DupeList( fromDupe.SpawnRangeList );
			toDupe.WalkRangeList				= 	DupeList( fromDupe.WalkRangeList );
			toDupe.AmountList 					= 	DupeList( fromDupe.AmountList );
			toDupe.MinDelayList 				= 	DupeList( fromDupe.MinDelayList );
			toDupe.MaxDelayList 				= 	DupeList( fromDupe.MaxDelayList );
			toDupe.SpawnTypeList 				= 	DupeList( fromDupe.SpawnTypeList );
			toDupe.ActivatedList 				= 	DupeList( fromDupe.ActivatedList );
			toDupe.EventRangeList 				= 	DupeList( fromDupe.EventRangeList );
			toDupe.EventKeywordList 			= 	DupeList( fromDupe.EventKeywordList );
			toDupe.KeywordCaseSensitiveList 	= 	DupeList( fromDupe.KeywordCaseSensitiveList );
			toDupe.TriggerEventNowList 			= 	DupeList( fromDupe.TriggerEventNowList );
			toDupe.EventAmbushList 				=	DupeList( fromDupe.EventAmbushList );
			toDupe.BeginTimeBasedList 			= 	DupeList( fromDupe.BeginTimeBasedList );
			toDupe.EndTimeBasedList 			= 	DupeList( fromDupe.EndTimeBasedList );
			toDupe.GroupSpawnList 				= 	DupeList( fromDupe.GroupSpawnList );
			toDupe.MinStackAmountList 			= 	DupeList( fromDupe.MinStackAmountList );
			toDupe.MaxStackAmountList 			= 	DupeList( fromDupe.MaxStackAmountList );
			toDupe.MovableList 					= 	DupeList( fromDupe.MovableList );
			toDupe.MinDespawnList 				= 	DupeList( fromDupe.MinDespawnList );
			toDupe.MaxDespawnList 				= 	DupeList( fromDupe.MaxDespawnList );
			toDupe.DespawnList 					= 	DupeList( fromDupe.DespawnList );
			toDupe.DespawnGroupList 			= 	DupeList( fromDupe.DespawnGroupList );
			toDupe.DespawnTimeExpireList 		= 	DupeList( fromDupe.DespawnTimeExpireList );
			toDupe.SettingsList 				= 	DupeList( fromDupe.SettingsList );
			
			toDupe.RespawnEntryList 			= 	new ArrayList();
			toDupe.RespawnTimeList 				=	new ArrayList();
			toDupe.SpawnCounterList 			= 	new ArrayList();
			toDupe.SpawnTimeList 				= 	new ArrayList();
			toDupe.RespawnOnSaveList 			= 	new ArrayList();
			toDupe.DespawnTimeList 				=	new ArrayList();
			
			toDupe.OverrideRespawnEntryList 	= 	new ArrayList();
			toDupe.OverrideRespawnTimeList 		=	new ArrayList();
			toDupe.OverrideSpawnCounterList 	= 	new ArrayList();
			toDupe.OverrideSpawnTimeList 		= 	new ArrayList();
			toDupe.OverrideDespawnTimeList 		=	new ArrayList();
			
			toDupe.SpawnedEntries 				= 	new ArrayList();
			toDupe.LastMovedList 				= 	new ArrayList();
			
			toDupe.OverrideSpawnedEntries 		= 	new ArrayList();
			toDupe.OverrideLastMovedList		=	new ArrayList();
			
			toDupe.Name 						= 	fromDupe.Name;
			toDupe.Active 						= 	fromDupe.Active;
			toDupe.NoProximityType 				= 	fromDupe.NoProximityType;
			toDupe.NoSpeechType 				= 	fromDupe.NoSpeechType;
			toDupe.ContainerSpawn 				= 	fromDupe.ContainerSpawn;
			toDupe.OverrideRespawnOnSave 		= 	fromDupe.OverrideRespawnOnSave;
			
			toDupe.CompileSettings();
			
			for( int i = 0; i < toDupe.EntryList.Count; i++ )
			{
				string entryType = (string) toDupe.EntryList[i];
				int amount = (int) toDupe.AmountList[i];
				
				toDupe.SpawnedEntries.Add( new ArrayList() );
				toDupe.LastMovedList.Add( new ArrayList() );
				
				ArrayList respawnEntryList = new ArrayList();
				ArrayList respawnTimeList = new ArrayList();
				ArrayList spawnCounterList = new ArrayList();
				ArrayList spawnTimeList = new ArrayList();
				ArrayList respawnOnSaveList = new ArrayList();
				ArrayList despawnTimeList = new ArrayList();
				
				for( int j = 0; j < amount; j++ )
				{
					respawnEntryList.Add( entryType );
					respawnTimeList.Add( 0 );
					spawnCounterList.Add( DateTime.Now );
					spawnTimeList.Add( 0 );
					respawnOnSaveList.Add( (bool) false );
					despawnTimeList.Add( 0 );
				}
				
				toDupe.RespawnEntryList.Add( respawnEntryList );
				toDupe.RespawnTimeList.Add( respawnTimeList );
				toDupe.SpawnCounterList.Add( spawnCounterList );
				toDupe.SpawnTimeList.Add( spawnTimeList );
				toDupe.RespawnOnSaveList.Add( respawnOnSaveList );
				toDupe.DespawnTimeList.Add( despawnTimeList );
			}
			
			for( int i = 0; i < toDupe.OverrideAmount; i++ )
			{
				toDupe.OverrideRespawnEntryList.Add( "" );
				toDupe.OverrideRespawnTimeList.Add( 0 );
				toDupe.OverrideSpawnCounterList.Add( DateTime.Now );
				toDupe.OverrideSpawnTimeList.Add( 0 );
				toDupe.OverrideDespawnTimeList.Add( 0 );
			}
			
			return toDupe;
		}
		
		public static ArrayList DupeList( ArrayList fromList )
		{
			ArrayList toList = new ArrayList();
			
			for ( int i = 0; i < fromList.Count; i++ )
				toList.Add( fromList[i] );
			
			return toList;
		}
		
		public static void CheckSpawners()
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
		
		public static void AddToDebugLog( string debugLine )
		{
			if ( !Debug )
				return;
			
			DebugLog.Add( debugLine );
		}
		
		public static void CheckFileImportList( string fileName )
		{
			fileName = fileName.ToLower();
			
			if ( !CheckFileExists( fileName ) )
				FileImportRemove( fileName );
			
			for ( int i = 0; i < FileImportList.Count; i++ )
			{
				string file = (string) FileImportList[i];
				
				if ( file == fileName )
				{
					if ( (int) FileImportCountList[i] < 1 )
						FileImportRemove( fileName );
					
					break;
				}
			}
		}
		
		public static void FileImportAdd( string fileName, string fileVersion )
		{
			fileName = fileName.ToLower();
			
			if ( !CheckFileExists( fileName ) && !CheckBadFileName( fileName ) )
			{
				FileImportList.Add( fileName );
				FileImportVersionList.Add( fileVersion );
				FileImportCountList.Add( 1 );
			}
			else
			{
				AdjustImportCount( fileName, 1 );
			}
		}
		
		public static void AdjustImportCount( string fileName, int value )
		{
			fileName = fileName.ToLower();
			
			for ( int i = 0; i < FileImportList.Count; i++ )
			{
				string file = (string) FileImportList[i];
				
				if ( file == fileName )
				{
					FileImportCountList[i] = (int) FileImportCountList[i] + value;
					
					break;
				}
			}
		}
		
		public static void FileImportRemove( string fileName )
		{
			fileName = fileName.ToLower();
			
			for ( int i = 0; i < FileImportList.Count; i++ )
			{
				string file = (string) FileImportList[i];
				
				if ( file == fileName )
				{
					FileImportList.RemoveAt( i );
					FileImportVersionList.RemoveAt( i );
					FileImportCountList.RemoveAt( i );
					
					break;
				}
			}
		}
		
		public static bool CheckFileExists( string fileName )
		{
			fileName = fileName.ToLower();
			
			bool found = false;
			
			foreach ( string file in FileImportList )
			{
				if ( file == fileName )
				{
					found = true;
					
					break;
				}
			}
			
			return found;
		}
		
		public static bool CheckBadFileName( string fileName )
		{
			return ( fileName == "new" || fileName == "" || fileName == null );
		}
		
		public static bool CheckMSUser( Mobile mobile )
		{
			foreach ( Mobile MSUser in MSUsers )
			{
				if ( mobile == MSUser )
					return true;
			}
			
			return false;
		}
		
		public static void RemoveMSUser( Mobile mobile )
		{
			for ( int i = 0; i < MSUsers.Count; i++ )
			{
				Mobile MSUser = (Mobile) MSUsers[i];
				
				if ( mobile == MSUser )
				{
					MSUsers.RemoveAt( i );
					
					return;
				}
			}
		}
		
		public static bool IsContainerOrMobile( string check )
		{
			if ( check == null )
				return false;
			
			bool valid;
			Type type = ScriptCompiler.FindTypeByName( check );
			
			if ( type == null )
				return false;
			
			object o;
			
			try{ o = Activator.CreateInstance( type ); }
			catch{ return false; }
			
			if ( o is Container || o is Mobile )
				valid = true;
			else
				valid = false;
			
			DeleteObject( o );
			
			return valid;
		}
		
		public static bool IsMobile( string check )
		{
			if ( check == null )
				return false;
			
			bool valid;
			Type type = ScriptCompiler.FindTypeByName( check );
			
			if ( type == null )
				return false;
			
			object o;
			try{ o = Activator.CreateInstance( type ); }
			catch{ return false; }
			
			if ( o is Mobile )
				valid = true;
			else
				valid = false;
			
			DeleteObject( o );
			
			return valid;
		}
		
		public static bool IsContainer( string check )
		{
			if ( check == null )
				return false;
			
			bool valid;
			Type type = ScriptCompiler.FindTypeByName( check );
			
			if ( type == null )
				return false;
			
			object o;
			try{ o = Activator.CreateInstance( type ); }
			catch{ return false; }
			
			if ( o is Container )
				valid = true;
			else
				valid = false;
			
			DeleteObject( o );
			
			return valid;
		}
		
		public static bool IsItem( string check )
		{
			bool valid;
			Type type = ScriptCompiler.FindTypeByName( check );
			
			if ( type == null )
				return false;
			
			object o = Activator.CreateInstance( type );
			
			if ( o is Item )
				valid = true;
			else
				valid = false;
			
			DeleteObject( o );
			
			return valid;
		}
		
		public static bool IsStackable( string check )
		{
			bool valid;
			Type type = ScriptCompiler.FindTypeByName( check );
			
			if ( type == null )
				return false;
			
			object o = Activator.CreateInstance( type );
			
			if ( o is Item )
			{
				if ( ( (Item) o ).Stackable )
					valid = true;
				else
					valid = false;
			}
			else
			{
				valid = false;
			}
			
			DeleteObject( o );
			
			return valid;
		}
		
		public static bool IsMobileHasBackpack( string check )
		{
			bool valid;
			Type type = ScriptCompiler.FindTypeByName( check );
			
			if ( type == null )
				return false;
			
			object o = Activator.CreateInstance( type );
			
			if ( o is Mobile )
			{
				if ( ( (Mobile) o ).Backpack != null )
					valid = true;
				else
					valid = false;
			}
			else
			{
				valid = true;
			}
			
			DeleteObject( o );
			
			return valid;
		}
		
		public static void SetProcess( Process process )
		{
			RunningProcess = process;
			
			if ( process == Process.ConvertPre3_2 )
				Console.WriteLine( "\nMega Spawner System: Pre v3.2 spawners detected. Converting...Please wait..." );
		}
		
		public static Process GetProcess{ get { return RunningProcess; } }
		
		public static bool CheckProcess( out string MessagesTitle, out string Messages )
		{
			MessagesTitle = null;
			Messages = null;
			
			switch ( GetProcess )
			{
				case Process.LoadBackup:
					{
						Messages = "You may not use that command during the load backup process.";
						
						break;
					}
				case Process.SaveBackup:
					{
						Messages = "You may not use that command during the save backup process.";
						
						break;
					}
			}
			
			if ( RunningProcess != Process.None )
			{
				MessagesTitle = "Process Running";
			}
			
			return CheckProcess();
		}
		
		public static bool CheckProcess()
		{
			return RunningProcess != Process.None;
		}
		
		public static void DeleteObject( object o )
		{
			if ( o is Mobile )
				( (Mobile) o ).Delete();
			else if ( o is Item )
				( (Item) o ).Delete();
		}
		
		public static bool VerifySetting( ArrayList setting )
		{
			if ( setting.Count == 0 )
				return false;
			
			string entry = null;
			
			if ( setting[1] is string )
				entry = (string) setting[1];
			
			if ( setting[0] is Setting )
			{
				switch( (Setting) setting[0] )
				{
					case Setting.AddItem:
						{
							if ( IsMobile( entry ) && !IsMobileHasBackpack( entry ) )
								return false;
							else if ( IsItem( entry ) && !IsContainer( entry ) )
								return false;
							
							break;
						}
					case Setting.AddContainer:
						{
							if ( IsMobile( entry ) && !IsMobileHasBackpack( entry ) )
								return false;
							else if ( IsItem( entry ) && !IsContainer( entry ) )
								return false;
							
							break;
						}
				}
			}
			
			return true;
		}
		
		public static string GetSettingInfo( MegaSpawner megaSpawner, ArrayList setting )
		{
			bool AddItem=false, ContainerItems=false;
			
			string entryName=null, addItem=null, entryType=null;
			int entryIndex=0, minStackAmount=0, maxStackAmount=0;
			
			try
			{
				switch( (Setting) setting[0] )
				{
						case Setting.OverrideIndividualEntries:{ return "Override Individual Entries"; }
					case Setting.AddItem:
						{
							AddItem = true;
							
							entryName = (string)								setting[1];
							entryIndex = (int)							setting[2];
							addItem = (string)									setting[3];
							minStackAmount = (int)							setting[4];
							maxStackAmount = (int)							setting[5];
							
							if ( megaSpawner != null )
								entryType = (string) megaSpawner.EntryList[entryIndex];
							else
								entryType = entryName;
							
							break;
						}
					case Setting.AddContainer:
						{
							AddItem = true;
							ContainerItems = true;
							
							entryName = (string)								setting[1];
							entryIndex = (int)							setting[2];
							addItem = (string)									setting[3];
							minStackAmount = (int)							setting[4];
							maxStackAmount = (int)							setting[5];
							
							if ( megaSpawner != null )
								entryType = (string) megaSpawner.EntryList[entryIndex];
							else
								entryType = entryName;
							
							break;
						}
						default:{ return "Unknown Error"; }
				}
				
				if ( AddItem )
				{
					if ( megaSpawner != null && entryName != entryType )
						return String.Format( "Entry #{0}: Add Item To Spawn Entry [Discrepancy Error]", entryIndex );
					else if ( IsStackable( addItem ) )
						return String.Format( "Entry #{0}: Add Random {1} To {2} \"{3}\" To \"{4}\"", entryIndex, minStackAmount, maxStackAmount, addItem, entryType );
					else
						return String.Format( "Entry #{0}: Add \"{1}\"{2} To \"{3}\"", entryIndex, addItem, ContainerItems ? " With Items" : "", entryType );
				}
			}
			catch{}
			
			return "Error reading setting info. Possible missing or corrupt data.";
		}
		
		public static void DisplayStyle( Gump gump, StyleType style, int x, int y, int width, int height )
		{
			switch ( style )
			{
					case StyleType.OriginalBlack:{ gump.AddBackground( x, y, width, height, 0 ); gump.AddImageTiled( x, y, width, height, 2624 ); break; }
					case StyleType.DistroGray:{ gump.AddBackground( x, y, width, height, 5054 ); break; }
					case StyleType.Marble:{ gump.AddBackground( x, y, width, height, 5100 ); break; }
					case StyleType.BlackBorder:{ gump.AddBackground( x, y, width, height, 2620 ); break; }
					case StyleType.GrayEmbroidered:{ gump.AddBackground( x, y, width, height, 2600 ); break; }
					case StyleType.Offwhite:{ gump.AddBackground( x, y, width, height, 3000 ); break; }
					case StyleType.OffwhiteBorder:{ gump.AddBackground( x, y, width, height, 3500 ); break; }
					case StyleType.GrayMultiBorder:{ gump.AddBackground( x, y, width, height, 3600 ); break; }
					case StyleType.DarkGray:{ gump.AddBackground( x, y, width, height, 5120 ); break; }
					case StyleType.Scroll:{ gump.AddBackground( x - 17, y, width + 34, height, 5170 ); break; }
			}
		}
		
		public static void DisplayBackground( Gump gump, BackgroundType color, int x, int y, int width, int height )
		{
			switch ( color )
			{
					case BackgroundType.Alpha:{ gump.AddAlphaRegion( x, y, width, height ); break; }
					case BackgroundType.BlackAlpha:{ gump.AddImageTiled( x, y, width, height, 2624 ); gump.AddAlphaRegion( x, y, width, height ); break; }
					case BackgroundType.Black:{ gump.AddImageTiled( x, y, width, height, 2624 ); break; }
					case BackgroundType.Gray:{ gump.AddImageTiled( x, y, width, height, 5058 ); break; }
					case BackgroundType.Marble:{ gump.AddImageTiled( x, y, width, height, 5104 ); break; }
					case BackgroundType.Offwhite:{ gump.AddImageTiled( x, y, width, height, 3004 ); break; }
					case BackgroundType.DarkGray:{ gump.AddImageTiled( x, y, width, height, 5124 ); break; }
					case BackgroundType.Scroll:{ gump.AddImageTiled( x, y, width, height, 5174 ); break; }
					case BackgroundType.ActiveTextEntry:{ gump.AddImageTiled( x, y, width, height, 2604 ); break; }
					case BackgroundType.InactiveTextEntry:{ gump.AddImageTiled( x, y, width, height, 1416 ); break; }
			}
		}
		
		public static void DisplayImage( Gump gump, StyleType style )
		{
			switch ( style )
			{
					case StyleType.Scroll:{ break; }
					default:{ gump.AddImage( 30, 50, 10440 ); gump.AddImage( 648, 50, 10441 ); break; }
			}
		}
		
		public static string ColorText( string textColor, string text )
		{
			return String.Format( "<basefont color=#{0}>{1}</font>", textColor, text );
		}
		
		public static string ColorTextDir( string textColor, string text )
		{
			return String.Format( "<basefont color=#{0}>{1}", textColor, text );
		}
		
		public static string GetTextColor( TextColor textColor )
		{
			switch ( textColor )
			{
					case TextColor.LightPurple:{ return TextColorLightPurple; }
					case TextColor.Purple:{ return TextColorPurple; }
					case TextColor.DarkPurple:{ return TextColorDarkPurple; }
					case TextColor.LightBlue:{ return TextColorLightBlue; }
					case TextColor.Blue:{ return TextColorBlue; }
					case TextColor.DarkBlue:{ return TextColorDarkBlue; }
					case TextColor.LightRed:{ return TextColorLightRed; }
					case TextColor.Red:{ return TextColorRed; }
					case TextColor.DarkRed:{ return TextColorDarkRed; }
					case TextColor.LightGreen:{ return TextColorLightGreen; }
					case TextColor.Green:{ return TextColorGreen; }
					case TextColor.DarkGreen:{ return TextColorDarkGreen; }
					case TextColor.LightYellow:{ return TextColorLightYellow; }
					case TextColor.Yellow:{ return TextColorYellow; }
					case TextColor.DarkYellow:{ return TextColorDarkYellow; }
					case TextColor.White:{ return TextColorWhite; }
					case TextColor.LightGray:{ return TextColorLightGray; }
					case TextColor.MediumLightGray:{ return TextColorMediumLightGray; }
					case TextColor.MediumGray:{ return TextColorMediumGray; }
					case TextColor.DarkGray:{ return TextColorDarkGray; }
					case TextColor.Black:{ return TextColorBlack; }
			}
			
			return TextColorPurple;
		}
		
		public static int GetSwitchNum( RelayInfo info, int start, int end )
		{
			int num = 0;
			
			for ( int i = start; i <= end; i++ )
			{
				if ( info.IsSwitched( i ) )
					return num;
				
				num++;
			}
			
			return 0;
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
		
		public static string CropCoreDirectory( string entry )
		{
			string[] CropDir = entry.Split('\\');
			string[] CoreDir = Core.BaseDirectory.Split('\\');
			
			entry = "\\";
			
			for ( int i = CoreDir.Length; i < CropDir.Length; i++ )
				entry = String.Format( "{0}{1}\\", entry, CropDir[i] );
			
			return entry;
		}
		
		public static string GetPreviousDirectory( string entry )
		{
			string[] GetDir = entry.Split('\\');
			
			entry = null;
			
			for ( int i = 0; i < GetDir.Length - 1; i++ )
				entry = String.Format( "{0}{1}\\", entry, GetDir[i] );
			
			entry = entry.TrimEnd('\\');
			
			return entry;
		}
		
		public static void RegisterPlugIn( OnCommand cmd, string name, string desc, string helpDesc )
		{
			PlugIns[name] = cmd;
			
			ArrayList List = new ArrayList();
			
			List.Add( name );
			List.Add( desc );
			List.Add( helpDesc );
			
			PlugInsList.Add( List );
			PlugInsList.Sort( new PlugInSorter() );
		}
		
		public static ArrayList DiscrepancyCheck( ArrayList fromCompare, ArrayList toCompare )
		{
			if ( fromCompare.Count > toCompare.Count )
			{
				int diff = fromCompare.Count - toCompare.Count;
				
				for ( int i = 0; i < diff; i++ )
					toCompare.Add( 0 );
			}
			else if ( fromCompare.Count < toCompare.Count )
			{
				int diff = toCompare.Count - fromCompare.Count;
				
				for ( int i = 0; i < diff; i++ )
					toCompare.RemoveAt( toCompare.Count - 1 );
			}
			
			return toCompare;
		}
		
		public static ArrayList CompileSameLocationList( MegaSpawner fromCompare )
		{
			CheckSpawners();
			
			ArrayList List = new ArrayList();
			
			foreach ( MegaSpawner toCompare in SpawnerList )
				if ( fromCompare != toCompare )
				if ( fromCompare.Location == toCompare.Location && fromCompare.Map == toCompare.Map )
				List.Add( toCompare );
			
			return List;
		}
		
		public static ArrayList CompileSameCriteriaList()
		{
			CheckSpawners();
			
			ArrayList List = new ArrayList();
			
			for ( int i = 0; i < SpawnerList.Count; i++ )
			{
				MegaSpawner fromCompare = (MegaSpawner) SpawnerList[i];
				
				for ( int j = 0; j < SpawnerList.Count; j++ )
				{
					MegaSpawner toCompare = (MegaSpawner) SpawnerList[j];
					
					if ( fromCompare != toCompare )
					{
						if ( SpawnerCompare( fromCompare, toCompare, false ) == 1 )
						{
							List.Add( fromCompare );
							List.Add( toCompare );
						}
					}
				}
			}
			
			return List;
		}
		
		public static int SpawnerCompare( MegaSpawner fromCompare, MegaSpawner toCompare, bool delete )
		{
			if ( !ListCompare( fromCompare.EntryList, toCompare.EntryList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.SpawnRangeList, toCompare.SpawnRangeList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.WalkRangeList, toCompare.WalkRangeList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.AmountList, toCompare.AmountList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MinDelayList, toCompare.MinDelayList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MaxDelayList, toCompare.MaxDelayList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.SpawnTypeList, toCompare.SpawnTypeList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.ActivatedList, toCompare.ActivatedList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.EventRangeList, toCompare.EventRangeList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.EventKeywordList, toCompare.EventKeywordList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.KeywordCaseSensitiveList, toCompare.KeywordCaseSensitiveList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.EventAmbushList, toCompare.EventAmbushList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.BeginTimeBasedList, toCompare.BeginTimeBasedList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.EndTimeBasedList, toCompare.EndTimeBasedList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.GroupSpawnList, toCompare.GroupSpawnList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MinStackAmountList, toCompare.MinStackAmountList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MaxStackAmountList, toCompare.MaxStackAmountList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MovableList, toCompare.MovableList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MinDespawnList, toCompare.MinDespawnList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.MaxDespawnList, toCompare.MaxDespawnList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.DespawnList, toCompare.DespawnList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.DespawnGroupList, toCompare.DespawnGroupList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.DespawnTimeExpireList, toCompare.DespawnTimeExpireList ) )
				return 0;
			
			if ( !ListCompare( fromCompare.SettingsList, toCompare.SettingsList ) )
				return 0;
			
			if ( delete )
				toCompare.Delete();
			
			return 1;
		}
		
		public static bool ListCompare( ArrayList fromList, ArrayList toList )
		{
			if ( fromList.Count != toList.Count )
				return false;
			
			for ( int i = 0; i < fromList.Count; i++ )
			{
				if ( fromList[i] is string )
					if ( (string) fromList[i] != (string) toList[i] )
					return false;
				
				if ( fromList[i] is int )
					if ( (int) fromList[i] != (int) toList[i] )
					return false;
				
				if ( fromList[i] is bool )
					if ( (bool) fromList[i] != (bool) toList[i] )
					return false;
				
				if ( fromList[i] is ArrayList )
					if ( (ArrayList) fromList[i] != (ArrayList) toList[i] )
					return false;
			}
			
			return true;
		}
		
		public static ArrayList CompileDefaultArgsList()
		{
			ArrayList List = CompileDefaultArgsList( "" );
			
			return List;
		}
		
		public static ArrayList CompileDefaultArgsList( object o )
		{
			object megaSpawner = null;
			
			ArrayList ArgsList = new ArrayList();
			ArrayList HideSpawnerList = new ArrayList();
			ArrayList PageInfoList = CreatePageInfoList();
			ArrayList MSGCheckBoxesList = new ArrayList();
			ArrayList MSEGCheckBoxesList = new ArrayList();
			ArrayList ESGArgsList = CreateESGArgsList();
			ArrayList AVSArgsList = CreateAVSArgsList();
			ArrayList AVSSetList = CreateAVSSetList();
			ArrayList SCGArgsList = CreateSCGArgsList();
			ArrayList SCGSetList = CreateSCGSetList();
			ArrayList PCGArgsList = CreatePCGArgsList();
			ArrayList PCGSetList = CreatePCGSetList();
			ArrayList PersonalConfigList = CreatePersonalConfigList();
			ArrayList EAGArgsList = CreateEAGArgsList();
			ArrayList SEGArgsList = CreateSEGArgsList();
			
			if ( o is string )
			{
				megaSpawner = (string) o;
				
				CheckSpawners();
				
				for ( int i = 0; i < SpawnerList.Count; i++ )
				{
					HideSpawnerList.Add( (bool) false );
					MSGCheckBoxesList.Add( (bool) false );
				}
			}
			else if ( o is MegaSpawner )
			{
				megaSpawner = (MegaSpawner) o;
				
				for ( int i = 0; i < ( (MegaSpawner) megaSpawner ).EntryList.Count; i++ )
					MSEGCheckBoxesList.Add( (bool) false );
			}
			
			string MessagesTitle = String.Format( "Mega Spawner System v{0}", Version );
			string Messages = String.Format( "Welcome to the Mega Spawner System v{0}.", Version );
			
			ArgsList.Add( (bool) false );				// Help
			ArgsList.Add( (bool) true );				// DisplayMessages
			ArgsList.Add( MessagesTitle );				// MessagesTitle
			ArgsList.Add( MessagesTitle );				// OldMessagesTitle
			ArgsList.Add( Messages );					// Messages
			ArgsList.Add( Messages );					// OldMessages
			ArgsList.Add( HideSpawnerList );			// HideSpawnerList
			ArgsList.Add( "" );							// FileName
			ArgsList.Add( "" );							// Setting
			ArgsList.Add( "" );							// DirectoryPath
			ArgsList.Add( (bool) false );				// Changed
			ArgsList.Add( new ArrayList() );			// ChangedSpawnerList
			ArgsList.Add( PageInfoList );				// PageInfoList
			ArgsList.Add( MSGCheckBoxesList );			// MSGCheckBoxesList
			ArgsList.Add( MSEGCheckBoxesList );			// MSEGCheckBoxesList
			ArgsList.Add( new ArrayList() );			// FMCheckBoxesList
			ArgsList.Add( new ArrayList() );			// CUGCheckBoxesList
			ArgsList.Add( new ArrayList() );			// SettingsCheckBoxesList
			ArgsList.Add( new ArrayList() );			// AMGCheckBoxesList
			ArgsList.Add( megaSpawner );				// megaSpawner
			ArgsList.Add( (bool) false );				// fromSpawnerList
			ArgsList.Add( ESGArgsList );				// ESGArgsList
			ArgsList.Add( AVSArgsList );				// AVSArgsList
			ArgsList.Add( AVSSetList );					// AVSSetList
			ArgsList.Add( SCGArgsList );				// SCGArgsList
			ArgsList.Add( SCGSetList );					// SCGSetList
			ArgsList.Add( PCGArgsList );				// PCGArgsList
			ArgsList.Add( PCGSetList );					// PCGSetList
			ArgsList.Add( PersonalConfigList );			// PersonalConfigList
			ArgsList.Add( EAGArgsList );				// EAGArgsList
			ArgsList.Add( 0 );							// FromWhere
			ArgsList.Add( new ArrayList() );			// AITCCheckBoxesList
			ArgsList.Add( SEGArgsList );				// SEGArgsList
			ArgsList.Add( new ArrayList() );			// ConvertSpawnersList
			ArgsList.Add( (bool) true );				// RefreshSpawnerLists
			ArgsList.Add( new ArrayList() );			// SpawnerList
			ArgsList.Add( new ArrayList() );			// MasterSpawnerList
			ArgsList.Add( new ArrayList() );			// ModHideSpawnerList
			ArgsList.Add( (bool) false );				// GotoLocation
			
			return ArgsList;
		}
		
		public static ArrayList CreatePageInfoList()
		{
			ArrayList List = new ArrayList();
			
			for ( int i = 0; i <= 37; i++ )
				List.Add( 1 );
			
			return List;
		}
		
		public static ArrayList CreateESGArgsList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( (bool) false );						// AddToSpawner
			List.Add( 0 );									// index (EditSpawnGump)
			List.Add( (bool) false );						// activatedSwitch
			List.Add( (bool) false );						// spawnGroupSwitch
			List.Add( (bool) false );						// eventAmbushSwitch
			List.Add( 0 );									// spawnTypeSwitch
			List.Add( "" );									// entryType
			List.Add( 0 );									// spawnRange
			List.Add( 0 );									// walkRange
			List.Add( 0 );									// amount
			List.Add( 0 );									// minDelayHour
			List.Add( 0 );									// minDelayMin
			List.Add( 0 );									// minDelaySec
			List.Add( 0 );									// maxDelayHour
			List.Add( 0 );									// maxDelayMin
			List.Add( 0 );									// maxDelaySec
			List.Add( 0 );									// eventRange
			List.Add( 0 );									// beginTimeBasedHour
			List.Add( 0 );									// beginTimeBasedMin
			List.Add( 0 );									// endTimeBasedHour
			List.Add( 0 );									// endTimeBasedMin
			List.Add( "" );									// keyword
			List.Add( (bool) false );						// caseSensitiveSwitch
			List.Add( 1 );									// minStackAmount
			List.Add( 1 );									// maxStackAmount
			List.Add( (bool) true );						// movableSwitch
			List.Add( 0 );									// minDespawnHour
			List.Add( 0 );									// minDespawnMin
			List.Add( 0 );									// minDespawnSec
			List.Add( 0 );									// maxDespawnHour
			List.Add( 0 );									// maxDespawnMin
			List.Add( 0 );									// maxDespawnSec
			List.Add( (bool) false );						// despawnSwitch
			List.Add( (bool) false );						// despawnGroupSwitch
			List.Add( (bool) false );						// despawnTimeExpireSwitch
			
			return List;
		}
		
		public static ArrayList CreateAVSArgsList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( (bool) false );						// AddSetting
			List.Add( 0 );									// index
			List.Add( (bool) true );						// activatedSwitch
			List.Add( (bool) false );						// spawnGroupSwitch
			List.Add( (bool) true );						// eventAmbushSwitch
			List.Add( SpawnType.Regular );					// spawnTypeSwitch
			List.Add( 10 );									// spawnRange
			List.Add( 10 );									// walkRange
			List.Add( 1 );									// amount
			List.Add( 0 );									// minDelayHour
			List.Add( 5 );									// minDelayMin
			List.Add( 0 );									// minDelaySec
			List.Add( 0 );									// maxDelayHour
			List.Add( 10 );									// maxDelayMin
			List.Add( 0 );									// maxDelaySec
			List.Add( 10 );									// eventRange
			List.Add( 0 );									// beginTimeBasedHour
			List.Add( 0 );									// beginTimeBasedMinute
			List.Add( 0 );									// endTimeBasedHour
			List.Add( 0 );									// endTimeBasedMinute
			List.Add( "" );									// keyword
			List.Add( (bool) false );						// caseSensitiveSwitch
			List.Add( "" );									// entryName
			List.Add( "" );									// addItem
			List.Add( -1 );									// entryIndex
			List.Add( 1 );									// minStackAmount
			List.Add( 1 );									// maxStackAmount
			List.Add( new ArrayList() );					// InsideItemList
			List.Add( 0 );									// insideIndex
			List.Add( 0 );									// minDespawnHour
			List.Add( 0 );									// minDespawnMin
			List.Add( 0 );									// minDespawnSec
			List.Add( 0 );									// maxDespawnHour
			List.Add( 0 );									// maxDespawnMin
			List.Add( 0 );									// maxDespawnSec
			List.Add( (bool) false );						// despawnSwitch
			List.Add( (bool) false );						// despawnGroupSwitch
			List.Add( (bool) false );						// despawnTimeExpireSwitch
			
			return List;
		}
		
		public static ArrayList CreateAVSSetList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( (bool) false );						// OverrideIndividualEntriesClicked
			List.Add( (bool) false );						// AddItemClicked
			
			return List;
		}
		
		public static ArrayList CreateSCGArgsList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( (bool) false );						// ConfigRead
			List.Add( TimerType.Personal );					// timerTypeSwitch
			List.Add( (bool) false );						// debugSwitch
			List.Add( (bool) false );					// staffTriggerEventSwitch
			
			return List;
		}
		
		public static ArrayList CreateSCGSetList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( (bool) false );						// TimerConfigClicked
			List.Add( (bool) false );						// MiscConfigClicked
			
			return List;
		}
		
		public static ArrayList CreatePCGArgsList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( StyleType.OriginalBlack );			// styleTypeSwitch
			List.Add( BackgroundType.Alpha );				// backgroundTypeSwitch
			List.Add( BackgroundType.ActiveTextEntry );		// activeTEBGTypeSwitch
			List.Add( BackgroundType.InactiveTextEntry );	// inactiveTEBGTypeSwitch
			List.Add( TextColor.LightBlue );				// defaultTextColorSwitch
			List.Add( TextColor.LightYellow );				// titleTextColorSwitch
			List.Add( TextColor.White );					// messagesTextColorSwitch
			List.Add( TextColor.LightBlue );				// commandButtonsTextColorSwitch
			List.Add( TextColor.LightRed );					// flagTextColorSwitch
			
			return List;
		}
		
		public static ArrayList CreatePCGSetList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( (bool) false );						// StyleConfigClicked
			List.Add( (bool) false );						// TextColorConfigClicked
			
			return List;
		}
		
		public static ArrayList CreatePersonalConfigList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( StyleType.OriginalBlack );			// StyleTypeConfig
			List.Add( BackgroundType.Alpha );				// BackgroundTypeConfig
			List.Add( BackgroundType.ActiveTextEntry );		// ActiveTEBGTypeConfig
			List.Add( BackgroundType.InactiveTextEntry );	// InactiveTEBGTypeConfig
			List.Add( TextColor.LightBlue );				// DefaultTextColor
			List.Add( TextColor.LightYellow );				// TitleTextColor
			List.Add( TextColor.White );					// MessagesTextColor
			List.Add( TextColor.LightBlue );				// CommandButtonsTextColor
			List.Add( 60 );									// PageNumberTextColor
			List.Add( 1153 );								// ActiveTextEntryTextColor
			List.Add( 1071 );								// InactiveTextEntryTextColor
			List.Add( 1286 );								// DirectoryTextColor
			List.Add( 1280 );								// FileTextColor
			List.Add( 1284 );								// KnownFileTextColor
			List.Add( TextColor.LightRed );					// FlagTextColor
			List.Add( "" );									// sortSearchFor
			List.Add( SortSearchType.None );				// sortSearchType
			List.Add( SortOrder.Ascending );				// sortOrder
			List.Add( SortSearchType.None );				// sortType
			List.Add( (bool) false );						// sortSearchCaseSensitive
			List.Add( (bool) false );						// sortSearchFlagged
			List.Add( (bool) false );						// sortSearchedBadLocation
			List.Add( (bool) false );						// sortSearchedDupeSpawners
			
			return List;
		}
		
		public static ArrayList CreateEAGArgsList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( 0 );					// index
			List.Add( (bool) false );			// AddAccount
			List.Add( Access.None );			// accessSwitch
			List.Add( (bool) false );			// accountsMgmtSwitch
			List.Add( (bool) false );			// systemConfigSwitch
			List.Add( "" );					// accountName
			
			return List;
		}
		
		public static ArrayList CreateSEGArgsList()
		{
			ArrayList List = new ArrayList();
			
			List.Add( SearchType.Any );			// searchType
			List.Add( "" );					// SearchFor
			
			return List;
		}
		
		public static bool CheckString( string check, out bool failed )
		{
			if ( check == "" || check == null )
				failed = true;
			else
				failed = false;
			
			return failed;
		}
		
		public static string Validize( string valid )
		{
			valid = valid.Replace( "/", " " );
			valid = valid.Replace( "\\", " " );
			valid = valid.Replace( ":", " " );
			valid = valid.Replace( "*", " " );
			valid = valid.Replace( "?", " " );
			valid = valid.Replace( "\"", " " );
			valid = valid.Replace( "<", " " );
			valid = valid.Replace( ">", " " );
			valid = valid.Replace( "|", " " );
			
			return valid;
		}
		
		public static string GetPersonalFileName( Mobile mobile )
		{
			return String.Format( "{0}\\{1} - {2}.mpc", PersonalConfigsDirectory, Convert.ToString( ( (Account) mobile.Account ).Username ), Validize( mobile.Name ) );
		}
		
		public static Access GetAccessLevel( Mobile mobile )
		{
			string fromCompare = ( (Account) mobile.Account ).Username;
			
			if ( (string) AccountsList[0] == "master/admin" )
			{
				AccountsList[0] = fromCompare;
				
				SaveAccountsAccess();
				
				return (Access) AccessList[0];
			}
			
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				string toCompare = (string) AccountsList[i];
				
				if ( fromCompare.ToLower() == toCompare.ToLower() )
					return (Access) AccessList[i];
			}
			
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				string toCompare = (string) AccountsList[i];
				
				if ( toCompare.ToLower() == "default user" )
					return (Access) AccessList[i];
			}
			
			return Access.None;
		}
		
		public static bool IsDefaultUser( Mobile mobile )
		{
			string fromCompare = ( (Account) mobile.Account ).Username;
			
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				string toCompare = (string) AccountsList[i];
				
				if ( fromCompare.ToLower() == toCompare.ToLower() )
					return false;
			}
			
			return true;
		}
		
		public static ArrayList GetMenuAccess( Mobile mobile )
		{
			string fromCompare = ( (Account) mobile.Account ).Username;
			
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				string toCompare = (string) AccountsList[i];
				
				if ( fromCompare.ToLower() == toCompare.ToLower() )
					return (ArrayList) AdminMenuAccessList[i];
			}
			
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				string toCompare = (string) AccountsList[i];
				
				if ( toCompare.ToLower() == "default user" )
					return (ArrayList) AdminMenuAccessList[i];
			}
			
			return new ArrayList();
		}
		
		public static ArrayList GetMenuAccess( int index )
		{
			return (ArrayList) AdminMenuAccessList[index];
		}
		
		public static bool AccountExists( string accountName )
		{
			for ( int i = 0; i < AccountsList.Count; i++ )
			{
				string check = (string) AccountsList[i];
				
				if ( accountName.ToLower() == check.ToLower() )
					return true;
			}
			
			return false;
		}
		
		public static bool AccountNotExist( string accountName )
		{
			bool found = false;
			ICollection<IAccount> accounts = Accounts.GetAccounts();
			
			foreach ( IAccount acct in accounts )
			{
				if ( acct.Username.ToLower() == accountName.ToLower() )
				{
					found = true;
					
					break;
				}
			}
			
			if ( accountName.ToLower() == "default user" )
				found = true;
			
			return !found;
		}
		
		public static bool AccountBanned( string accountName )
		{
			bool banned = false;

			foreach ( Account acct in Accounts.GetAccounts() )
			{
				if ( acct.Username.ToLower() == accountName.ToLower() )
				{
					if ( acct.Banned )
						banned = true;

					break;
				}
			}

			return banned;
		}
		
		public static bool AccountNoCommandAccess( string accountName )
		{
			bool noAccess = false;
			ICollection<IAccount> accounts = Accounts.GetAccounts();
			
			foreach ( IAccount acct in accounts )
			{
				if ( acct.Username.ToLower() == accountName.ToLower() && acct.AccessLevel < AccessLevelReq )
				{
					noAccess = true;
					
					break;
				}
			}
			
			return noAccess;
		}
		
		public static void LoadAccountsAccess()
		{
			if ( !Directory.Exists( SaveDirectory ) )
				Directory.CreateDirectory( SaveDirectory );
			
			bool failed=false, mainLoopRan=false;
			
			try
			{
				XmlDocument xml = new XmlDocument();
				xml.Load( AccountsFileName );
				
				XmlElement accountsList = xml["AccountsList"];
				
				int cnt = 0;
				
				foreach ( XmlElement account in xml.GetElementsByTagName( "Account" ) )
				{
					mainLoopRan = true;
					
					ArrayList adminMenuAccessList = new ArrayList();
					
					string accountName = GetInnerText( account["Name"] );
					
					if ( CheckString( accountName, out failed ) )
						break;
					
					if ( cnt == 1 && accountName != "default user" )
					{
						AccountsList.Add( "default user" );
						AccessList.Add( Access.None );
						adminMenuAccessList.Add( (bool) false );
						adminMenuAccessList.Add( (bool) false );
						AdminMenuAccessList.Add( adminMenuAccessList );
						
						adminMenuAccessList.Clear();
					}
					
					AccountsList.Add( accountName );
					AccessList.Add( (Access) int.Parse( GetInnerText( account["Access"] ) ) );
					
					failed = true;
					
					foreach ( XmlElement access in account.GetElementsByTagName( "AdminMenuAccessList" ) )
					{
						failed = false;
						
						adminMenuAccessList.Add( bool.Parse( GetInnerText( access["AccountsManagement"] ) ) );
						adminMenuAccessList.Add( bool.Parse( GetInnerText( access["SystemConfig"] ) ) );
					}
					
					if ( failed )
						break;
					
					AdminMenuAccessList.Add( adminMenuAccessList );
					
					cnt++;
				}
				
				if ( cnt == 1 )
				{
					ArrayList adminMenuAccessList = new ArrayList();
					
					AccountsList.Add( "default user" );
					AccessList.Add( Access.None );
					adminMenuAccessList.Add( (bool) false );
					adminMenuAccessList.Add( (bool) false );
					AdminMenuAccessList.Add( adminMenuAccessList );
				}
			}
			catch{ DefaultAccountsAccessCreate(); }
			
			if ( !mainLoopRan || failed )
				DefaultAccountsAccessCreate();
			
			if ( !failed )
			{
				AccountEditors.Clear();
				
				for ( int i = 0; i < AccountsList.Count; i++ )
					AccountEditors.Add( null );
			}
		}
		
		public static void SaveAccountsAccess()
		{
			using ( StreamWriter sw = new StreamWriter( AccountsFileName ) )
			{
				XmlTextWriter xml = new XmlTextWriter( sw );
				
				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );
				
				xml.WriteStartElement( "AccountsList" );
				
				for ( int i = 0; i < AccountsList.Count; i++ )
				{
					xml.WriteStartElement( "Account" );
					
					xml.WriteStartElement( "Name" );
					xml.WriteString( (string) AccountsList[i] );
					xml.WriteEndElement();
					
					xml.WriteStartElement( "Access" );
					xml.WriteString( ( (int) AccessList[i] ).ToString() );
					xml.WriteEndElement();
					
					xml.WriteStartElement( "AdminMenuAccessList" );
					
					ArrayList List = (ArrayList) AdminMenuAccessList[i];
					
					xml.WriteStartElement( "AccountsManagement" );
					xml.WriteString( ( (bool) List[0] ).ToString() );
					xml.WriteEndElement();
					
					xml.WriteStartElement( "SystemConfig" );
					xml.WriteString( ( (bool) List[1] ).ToString() );
					xml.WriteEndElement();
					
					xml.WriteEndElement();
					
					xml.WriteEndElement();
				}
				
				xml.WriteEndElement();
				
				xml.Close();
			}
		}
		
		public static void DefaultAccountsAccessCreate()
		{
			AccountsList.Clear();
			AccessList.Clear();
			AdminMenuAccessList.Clear();
			
			using ( StreamWriter sw = new StreamWriter( AccountsFileName ) )
			{
				XmlTextWriter xml = new XmlTextWriter( sw );
				
				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );
				
				xml.WriteStartElement( "AccountsList" );
				
				xml.WriteStartElement( "Account" );
				
				xml.WriteStartElement( "Name" );
				xml.WriteString( "master/admin" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "Access" );
				xml.WriteString( ( (int) Access.MasterAdmin ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "AdminMenuAccessList" );
				
				xml.WriteStartElement( "AccountsManagement" );
				xml.WriteString( "true" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SystemConfig" );
				xml.WriteString( "true" );
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.WriteStartElement( "Account" );
				
				xml.WriteStartElement( "Name" );
				xml.WriteString( "default user" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "Access" );
				xml.WriteString( ( (int) Access.None ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "AdminMenuAccessList" );
				
				xml.WriteStartElement( "AccountsManagement" );
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SystemConfig" );
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.Close();
			}
			
			LoadAccountsAccess();
		}
		
		public static ArrayList LoadPersonalConfig( Mobile mobile, ArrayList ArgsList )
		{
			ArrayList PersonalConfigList = (ArrayList) ArgsList[28];
			
			if ( !Directory.Exists( SaveDirectory ) )
				Directory.CreateDirectory( SaveDirectory );
			
			if ( !Directory.Exists( PersonalConfigsDirectory ) )
				Directory.CreateDirectory( PersonalConfigsDirectory );
			
			try
			{
				XmlDocument xml = new XmlDocument();
				xml.Load( GetPersonalFileName( mobile ) );
				
				XmlElement node = xml["YourPersonalConfig"];
				
				try{ PersonalConfigList[0] = (StyleType) int.Parse( GetInnerText( node["StyleType"] ) ); }
				catch{ PersonalConfigList[0] = StyleType.OriginalBlack; }
				
				try{ PersonalConfigList[1] = (BackgroundType) int.Parse( GetInnerText( node["BackgroundType"] ) ); }
				catch{ PersonalConfigList[1] = BackgroundType.Alpha; }
				
				try{ PersonalConfigList[2] = (BackgroundType) int.Parse( GetInnerText( node["ActiveTextEntryBackgroundType"] ) ); }
				catch{ PersonalConfigList[2] = BackgroundType.ActiveTextEntry; }
				
				try{ PersonalConfigList[3] = (BackgroundType) int.Parse( GetInnerText( node["InactiveTextEntryBackgroundType"] ) ); }
				catch{ PersonalConfigList[3] = BackgroundType.InactiveTextEntry; }
				
				try{ PersonalConfigList[4] = (TextColor) int.Parse( GetInnerText( node["DefaultTextColor"] ) ); }
				catch{ PersonalConfigList[4] = TextColor.LightBlue; }
				
				try{ PersonalConfigList[5] = (TextColor) int.Parse( GetInnerText( node["TitleTextColor"] ) ); }
				catch{ PersonalConfigList[5] = TextColor.LightYellow; }
				
				try{ PersonalConfigList[6] = (TextColor) int.Parse( GetInnerText( node["MessagesTextColor"] ) ); }
				catch{ PersonalConfigList[6] = TextColor.White; }
				
				try{ PersonalConfigList[7] = (TextColor) int.Parse( GetInnerText( node["CommandButtonsTextColor"] ) ); }
				catch{ PersonalConfigList[7] = TextColor.LightBlue; }
				
				try{ PersonalConfigList[8] = int.Parse( GetInnerText( node["PageNumberTextColor"] ) ); }
				catch{ PersonalConfigList[8] = 60; }
				
				try{ PersonalConfigList[9] = int.Parse( GetInnerText( node["ActiveTextEntryTextColor"] ) ); }
				catch{ PersonalConfigList[9] = 1153; }
				
				try{ PersonalConfigList[10] = int.Parse( GetInnerText( node["InactiveTextEntryTextColor"] ) ); }
				catch{ PersonalConfigList[10] = 1071; }
				
				try{ PersonalConfigList[11] = int.Parse( GetInnerText( node["DirectoryTextColor"] ) ); }
				catch{ PersonalConfigList[11] = 1286; }
				
				try{ PersonalConfigList[12] = int.Parse( GetInnerText( node["FileTextColor"] ) ); }
				catch{ PersonalConfigList[12] = 1280; }
				
				try{ PersonalConfigList[13] = int.Parse( GetInnerText( node["KnownFileTextColor"] ) ); }
				catch{ PersonalConfigList[13] = 1284; }
				
				try{ PersonalConfigList[14] = (TextColor) int.Parse( GetInnerText( node["FlagTextColor"] ) ); }
				catch{ PersonalConfigList[14] = TextColor.LightRed; }
				
				XmlElement sort = node["SortSearchOptions"];
				
				if ( sort != null )
				{
					try{ PersonalConfigList[15] = GetInnerText( sort["SortSearchFor"] ); }
					catch{ PersonalConfigList[15] = ""; }
					
					try{ PersonalConfigList[16] = (SortSearchType) int.Parse( GetInnerText( sort["SortSearchType"] ) ); }
					catch{ PersonalConfigList[16] = SortSearchType.None; }
					
					try{ PersonalConfigList[17] = (SortOrder) int.Parse( GetInnerText( sort["SortOrder"] ) ); }
					catch{ PersonalConfigList[17] = SortOrder.Ascending; }
					
					try{ PersonalConfigList[18] = (SortSearchType) int.Parse( GetInnerText( sort["SortType"] ) ); }
					catch{ PersonalConfigList[18] = SortSearchType.None; }
					
					try{ PersonalConfigList[19] = bool.Parse( GetInnerText( sort["SortSearchCaseSensitive"] ) ); }
					catch{ PersonalConfigList[19] = (bool) false; }
					
					try{ PersonalConfigList[20] = bool.Parse( GetInnerText( sort["SortSearchFlagged"] ) ); }
					catch{ PersonalConfigList[20] = (bool) false; }
					
					try{ PersonalConfigList[21] = bool.Parse( GetInnerText( sort["SortSearchBadLocation"] ) ); }
					catch{ PersonalConfigList[21] = (bool) false; }
					
					try{ PersonalConfigList[22] = bool.Parse( GetInnerText( sort["SortSearchDupeSpawners"] ) ); }
					catch{ PersonalConfigList[22] = (bool) false; }
				}
			}
			catch{ DefaultPersonalConfigCreate( mobile, ArgsList ); }
			
			ArgsList[28] = PersonalConfigList;
			
			return ArgsList;
		}
		
		public static void DefaultPersonalConfigCreate( Mobile mobile, ArrayList ArgsList )
		{
			using ( StreamWriter sw = new StreamWriter( GetPersonalFileName( mobile ) ) )
			{
				XmlTextWriter xml = new XmlTextWriter( sw );
				
				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;
				xml.WriteStartDocument( true );
				
				xml.WriteStartElement( "YourPersonalConfig" );
				
				xml.WriteStartElement( "StyleType" );
				xml.WriteString( ( (int) StyleType.OriginalBlack ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "BackgroundType" );
				xml.WriteString( ( (int) BackgroundType.Alpha ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "ActiveTextEntryBackgroundType" );
				xml.WriteString( ( (int) BackgroundType.ActiveTextEntry ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "InactiveTextEntryBackgroundType" );
				xml.WriteString( ( (int) BackgroundType.InactiveTextEntry ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "DefaultTextColor" );
				xml.WriteString( ( (int) TextColor.LightBlue ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "TitleTextColor" );
				xml.WriteString( ( (int) TextColor.LightYellow ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "MessagesTextColor" );
				xml.WriteString( ( (int) TextColor.White ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "CommandButtonsTextColor" );
				xml.WriteString( ( (int) TextColor.LightBlue ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "PageNumberTextColor" );
				xml.WriteString( "60" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "ActiveTextEntryTextColor" );
				xml.WriteString( "1153" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "InactiveTextEntryTextColor" );
				xml.WriteString( "1071" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "DirectoryTextColor" );
				xml.WriteString( "1286" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "FileTextColor" );
				xml.WriteString( "1280" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "KnownFileTextColor" );
				xml.WriteString( "1284" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "FlagTextColor" );
				xml.WriteString( ( (int) TextColor.LightRed ).ToString() );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortSearchOptions" );
				
				xml.WriteStartElement( "SortSearchFor" );
				xml.WriteString( "" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortSearchType" );
				xml.WriteString( ( (int) SortSearchType.None ).ToString()  );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortOrder" );
				xml.WriteString( ( (int) SortOrder.Ascending ).ToString()  );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortType" );
				xml.WriteString( ( (int) SortSearchType.None ).ToString()  );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortSearchCaseSensitive" );
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortSearchFlagged" );
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortSearchBadLocation" );
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteStartElement( "SortSearchDupeSpawners" );
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.Close();
			}
			
			LoadPersonalConfig( mobile, ArgsList );
		}
		
		public static void LoadSystemConfig()
		{
			if ( !Directory.Exists( SaveDirectory ) )
				Directory.CreateDirectory( SaveDirectory );
			
			try
			{
				XmlDocument xml = new XmlDocument();
				xml.Load( ConfigFileName );
				
				XmlElement node = xml["MegaSpawnerSystemConfig"];
				
				try{ TimerTypeConfig = (TimerType) int.Parse( GetInnerText( node["TimerSetting"] ) ); }
				catch{ TimerTypeConfig = TimerType.Personal; }
				
				try{ Delay = int.Parse( GetInnerText( node["TimerDelay"] ) ); }
				catch{ Delay = 28; }
				
				try{ StaffTriggerEvent = bool.Parse( GetInnerText( node["StaffTriggerEvent"] ) ); }
				catch{ StaffTriggerEvent = false; }
			}
			catch{ DefaultSystemConfigCreate(); }
		}
		
		public static void DefaultSystemConfigCreate()
		{
			using ( StreamWriter sw = new StreamWriter( ConfigFileName ) )
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
				xml.WriteString( "false" );
				xml.WriteEndElement();
				
				xml.WriteEndElement();
				
				xml.Close();
			}
			
			LoadSystemConfig();
		}
		
		public static string GetInnerText( XmlElement node )
		{
			if ( node == null )
				return "Error";
			
			return node.InnerText;
		}
		
		public static int GetDelay( MegaSpawner megaSpawner )
		{
			switch ( TimerTypeConfig )
			{
					case TimerType.Master:{ return ( Delay ); }
					case TimerType.Personal:{ return ( Delay + AddRandom ); }
				case TimerType.RandomEntryDelay:
					{
						int entryCount = megaSpawner.EntryList.Count - 1;
						
						if ( entryCount < 0 )
							return Delay + AddRandom;
						
						int randomEntry = Utility.Random( 0, entryCount );
						
						return Utility.Random( (int) megaSpawner.MinDelayList[randomEntry], ( (int) megaSpawner.MaxDelayList[randomEntry] - (int) megaSpawner.MinDelayList[randomEntry] ) );
					}
			}
			
			return 1;
		}
		
		public static int AddRandom{ get{ return Utility.Random( 0, 2 ); } }
		
		public static ArrayList Sort( ArrayList SpawnerList, SortOrder sortOrder, SortSearchType sortType )
		{
			switch ( sortOrder )
			{
				case SortOrder.Ascending:
					{
						switch ( sortType )
						{
								case SortSearchType.Name:{ SpawnerList.Sort( new AscNameSorter() ); break; }
								case SortSearchType.Facet:{ SpawnerList.Sort( new AscFacetSorter() ); break; }
								case SortSearchType.LocationX:{ SpawnerList.Sort( new AscLocationXSorter() ); break; }
								case SortSearchType.LocationY:{ SpawnerList.Sort( new AscLocationYSorter() ); break; }
								case SortSearchType.LocationZ:{ SpawnerList.Sort( new AscLocationZSorter() ); break; }
						}
						
						break;
					}
				case SortOrder.Descending:
					{
						switch ( sortType )
						{
								case SortSearchType.Name:{ SpawnerList.Sort( new DescNameSorter() ); break; }
								case SortSearchType.Facet:{ SpawnerList.Sort( new DescFacetSorter() ); break; }
								case SortSearchType.LocationX:{ SpawnerList.Sort( new DescLocationXSorter() ); break; }
								case SortSearchType.LocationY:{ SpawnerList.Sort( new DescLocationYSorter() ); break; }
								case SortSearchType.LocationZ:{ SpawnerList.Sort( new DescLocationZSorter() ); break; }
						}
						
						break;
					}
			}
			
			return SpawnerList;
		}
		
		// ****** Sorting Comparers ******************
		// ****** Based Off Of Distro Methods ********
		
		private class AscNameSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return a.Name.CompareTo( b.Name );
			}
		}
		
		private class DescNameSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return b.Name.CompareTo( a.Name );
			}
		}
		
		private class AscFacetSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				int compare = a.Map.ToString().CompareTo( b.Map.ToString() );
				
				if ( compare == 0 )
				{
					if ( a.Name.ToLower() == b.Name.ToLower() )
						return LocationSorter( false, SortSearchType.LocationX, a, b );
					else
						return a.Name.CompareTo( b.Name );
				}
				else
				{
					return compare;
				}
			}
		}
		
		private class DescFacetSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				int compare = b.Map.ToString().CompareTo( a.Map.ToString() );
				
				if ( compare == 0 )
				{
					if ( a.Name.ToLower() == b.Name.ToLower() )
						return LocationSorter( false, SortSearchType.LocationX, a, b );
					else
						return a.Name.CompareTo( b.Name );
				}
				else
				{
					return compare;
				}
			}
		}
		
		private class AscLocationXSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return LocationSorter( false, SortSearchType.LocationX, a, b );
			}
		}
		
		private class DescLocationXSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return LocationSorter( true, SortSearchType.LocationX, a, b );
			}
		}
		
		private class AscLocationYSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return LocationSorter( false, SortSearchType.LocationY, a, b );
			}
		}
		
		private class DescLocationYSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return LocationSorter( true, SortSearchType.LocationY, a, b );
			}
		}
		
		private class AscLocationZSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return LocationSorter( false, SortSearchType.LocationZ, a, b );
			}
		}
		
		private class DescLocationZSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				MegaSpawner a = x as MegaSpawner;
				MegaSpawner b = y as MegaSpawner;
				
				return LocationSorter( true, SortSearchType.LocationZ, a, b );
			}
		}
		
		private static int LocationSorter( bool descOrder, SortSearchType sortType, MegaSpawner a, MegaSpawner b )
		{
			int aX, bX, aY, bY, aZ, bZ, sort=0;
			
			if ( a.Parent != null )
			{
				aX = ( (Item) a.Parent ).Location.X;
				aY = ( (Item) a.Parent ).Location.Y;
				aZ = ( (Item) a.Parent ).Location.Z;
			}
			else
			{
				aX = a.X;
				aY = a.Y;
				aZ = a.Z;
			}
			
			if ( b.Parent != null )
			{
				bX = ( (Item) b.Parent ).Location.X;
				bY = ( (Item) b.Parent ).Location.Y;
				bZ = ( (Item) b.Parent ).Location.Z;
			}
			else
			{
				bX = b.X;
				bY = b.Y;
				bZ = b.Z;
			}
			
			switch ( sortType )
			{
				case SortSearchType.LocationX:
					{
						if ( aX < bX )
						{
							sort = -1;
						}
						else if ( aX > bX )
						{
							sort = 1;
						}
						else // if ( aX == bX )
						{
							if ( aY < bY )
							{
								if ( descOrder )
									sort = 1;
								else
									sort = -1;
							}
							else if ( aY > bY )
							{
								if ( descOrder )
									sort = -1;
								else
									sort = 1;
							}
							else // if ( aY == bY )
							{
								if ( aZ < bZ )
									sort = -1;
								else if ( aZ > bZ )
									sort = 1;
								else // if ( aZ == bZ )
									sort = 0;
								
								if ( descOrder )
									sort = -sort;
							}
						}
						
						break;
					}
				case SortSearchType.LocationY:
					{
						if ( aY < bY )
						{
							sort = -1;
						}
						else if ( aY > bY )
						{
							sort = 1;
						}
						else // if ( aY == bY )
						{
							if ( aX < bX )
							{
								if ( descOrder )
									sort = 1;
								else
									sort = -1;
							}
							else if ( aX > bX )
							{
								if ( descOrder )
									sort = -1;
								else
									sort = 1;
							}
							else // if ( aX == bX )
							{
								if ( aZ < bZ )
									sort = -1;
								else if ( aZ > bZ )
									sort = 1;
								else // if ( aZ == bZ )
									sort = 0;
								
								if ( descOrder )
									sort = -sort;
							}
						}
						
						break;
					}
				case SortSearchType.LocationZ:
					{
						if ( aZ < bZ )
						{
							sort = -1;
						}
						else if ( aZ > bZ )
						{
							sort = 1;
						}
						else // if ( aZ == bZ )
						{
							if ( aX < bX )
							{
								if ( descOrder )
									sort = 1;
								else
									sort = -1;
							}
							else if ( aX > bX )
							{
								if ( descOrder )
									sort = -1;
								else
									sort = 1;
							}
							else // if ( aX == bX )
							{
								if ( aY < bY )
									sort = -1;
								else if ( aY > bY )
									sort = 1;
								else // if ( aY == bY )
									sort = 0;
								
								if ( descOrder )
									sort = -sort;
							}
						}
						
						break;
					}
			}
			
			if ( descOrder )
				return -sort;
			else
				return sort;
		}
		
		private class PlugInSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				ArrayList a = x as ArrayList;
				ArrayList b = y as ArrayList;
				
				try{ return ( (string) a[1] ).CompareTo( (string) b[1] ); }
				catch{ return 0; }
			}
		}
		
		public class SettingsSorter : IComparer
		{
			public int Compare( object x, object y )
			{
				ArrayList a = x as ArrayList;
				ArrayList b = y as ArrayList;
				
				try
				{
					if ( a == b )
						return 0;
					else if ( (Setting) a[0] == Setting.OverrideIndividualEntries )
						return -1;
					else if ( (Setting) b[0] == Setting.OverrideIndividualEntries )
						return 1;
					
					return ( GetSettingInfo( null, a ) ).CompareTo( GetSettingInfo( null, b ) );
				}
				catch{ return 0; }
			}
		}
		
		// ****** End of Sorting Comparers ***********
		
		public static void SetVersion()
		{
			CommandSystem.Register( "MSS", AccessLevel.Player, new CommandEventHandler( MSS_OnCommand ) );
		}
		
		[Usage( "MSS" )]
		[Description( "Displays the Mega Spawner System version." )]
		public static void MSS_OnCommand( CommandEventArgs e )
		{
			Mobile mobile = e.Mobile;
			mobile.SendMessage( "Running: Mega Spawner System v{0}", Version );
		}
		
		private static void StartTimerSystem()
		{
			if ( TimerTypeConfig == TimerType.Master )
			{
				StartTimer();
			}
			else
			{
				foreach ( MegaSpawner megaSpawner in SpawnerList )
					megaSpawner.Start();
			}
		}
		
		public static void StartTimer()
		{
			if ( masterSpawnerTimer != null )
				masterSpawnerTimer.Stop();
			
			masterSpawnerTimer = new MasterSpawnerTimer( Delay );
			masterSpawnerTimer.Start();
		}
		
		private class MasterSpawnerTimer : Timer
		{
			private MegaSpawner megaSpawner;
			private int delay;
			
			public MasterSpawnerTimer( int timerdelay ) : base( TimeSpan.FromSeconds( timerdelay ), TimeSpan.FromSeconds( timerdelay ) )
			{
				Priority = TimerPriority.OneSecond;
				
				delay = timerdelay;
			}
			
			protected override void OnTick()
			{
				if ( TimerTypeConfig != TimerType.Master )
				{
					Stop();
					
					return;
				}
				
				for ( int i = 0; i < SpawnerList.Count; i++ )
				{
					megaSpawner = (MegaSpawner) SpawnerList[i];
					
					if ( megaSpawner != null || megaSpawner.Active || !megaSpawner.Deleted || megaSpawner.Map != null || megaSpawner.Map != Map.Internal || !megaSpawner.Workspace )
						megaSpawner.TimerTick( delay );
				}
			}
		}
	}
}
