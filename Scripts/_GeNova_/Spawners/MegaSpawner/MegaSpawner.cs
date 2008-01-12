using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.MegaSpawnerSystem;
using Server.Gumps;
using MSS = Server.MegaSpawnerSystem;
using MC = Server.MegaSpawnerSystem.MasterControl;
using GeNova.Server.Engines;

namespace Server.Items
{
	public class MegaSpawner : Item
	{
// ****** Variables Declaration ************************

		public const int spawnRetries = 15;
		public const int tickOffset = 1;		// Do not change!

		public static int ListExceptions = 0;

		private bool m_Active;

		private ArrayList m_EntryList = new ArrayList();
		private ArrayList m_SpawnRangeList = new ArrayList();
		private ArrayList m_WalkRangeList = new ArrayList();
		private ArrayList m_AmountList = new ArrayList();
		private ArrayList m_MinDelayList = new ArrayList();
		private ArrayList m_MaxDelayList = new ArrayList();
		private ArrayList m_SpawnTypeList = new ArrayList();
		private ArrayList m_ActivatedList = new ArrayList();
		private ArrayList m_EventRangeList = new ArrayList();
		private ArrayList m_EventKeywordList = new ArrayList();
		private ArrayList m_KeywordCaseSensitiveList = new ArrayList();
		private ArrayList m_TriggerEventNowList = new ArrayList();
		private ArrayList m_EventAmbushList = new ArrayList();
		private ArrayList m_BeginTimeBasedList = new ArrayList();
		private ArrayList m_EndTimeBasedList = new ArrayList();
		private ArrayList m_GroupSpawnList = new ArrayList();
		private ArrayList m_MinStackAmountList = new ArrayList();
		private ArrayList m_MaxStackAmountList = new ArrayList();
		private ArrayList m_MovableList = new ArrayList();
		private ArrayList m_MinDespawnList = new ArrayList();
		private ArrayList m_MaxDespawnList = new ArrayList();
		private ArrayList m_DespawnList = new ArrayList();
		private ArrayList m_DespawnGroupList = new ArrayList();
		private ArrayList m_DespawnTimeExpireList = new ArrayList();

		private ArrayList m_SettingsList = new ArrayList();

		private ArrayList m_RespawnEntryList = new ArrayList();
		private ArrayList m_RespawnTimeList = new ArrayList();
		private ArrayList m_SpawnCounterList = new ArrayList();
		private ArrayList m_SpawnTimeList = new ArrayList();
		private ArrayList m_RespawnOnSaveList = new ArrayList();
		private ArrayList m_DespawnTimeList = new ArrayList();

		private ArrayList m_SpawnedEntries = new ArrayList();
		private ArrayList m_LastMovedList = new ArrayList();

		private Mobile m_Editor, m_WorkspaceEditor, m_FileEdit;
		private bool m_Workspace;
		private bool m_NoProximityType, m_NoSpeechType, m_Respawning;
		private string m_Imported, m_ImportVersion;
		private Container m_ContainerSpawn;
		private MegaSpawnerTimer megaSpawnerTimer;

		private DateTime SaveTime, LoadTime;

		#region Spawn Groups
//  <Spawn Groups>-------------------------------------------------------<Start>
/*		public static string MegaSpawnerDirectoryPath = @"Mega Spawner";
		private static Hashtable MobileSpawnGroups = new Hashtable();
		private static Hashtable ItemSpawnGroups = new Hashtable();

		public static void Initialize()
		{
			// To add more groups just use the following format:
			// SpawnGroups.Add( "nameofgroup", "typename1;typename2;typename3...etc" )
			// NOTE* nameofgroup must be all lower case and with no spaces
			// To spawn them just type the groupname in the spawnentry line
			MobileSpawnGroups.Add( "townanimals", "dog;cat;rat;bird" );
			MobileSpawnGroups.Add( "farmanimals", "cow;bull;chicken;boar;pig;rabbit;sheep;goat" );
			MobileSpawnGroups.Add( "forestanimals", "brownbear;blackbear;cougar;greathart;greywolf;grizzlybear;hind;horse;jackrabbit;llama;mountaingoat;panther;eagle;bird;snake" );
			MobileSpawnGroups.Add( "swampanimals", "alligator;snake;giantserpent;TropicalBird");
			MobileSpawnGroups.Add( "undead1", "Bogle;Shade;Ghoul;Skeleton;Zombie;headlessone" );
			MobileSpawnGroups.Add( "undead2", "BoneMagi;Spectre;Wraith;BoneKnight;SkeletalKnight" );
			MobileSpawnGroups.Add( "undead3", "Lich;Mummy" );
			MobileSpawnGroups.Add( "undead4", "LichLord;ShadowKnight;RottingCorpse" );
			MobileSpawnGroups.Add( "undead5", "AncientLich;DarknightCreeper;SkeletalDragon" );
			MobileSpawnGroups.Add( "undeadall", "AncientLich;Bogle;BoneMagi;Lich;LichLord;Shade;Spectre;Wraith;BoneKnight;Ghoul;Mummy;SkeletalKnight;Skeleton;Zombie;ShadowKnight;DarknightCreeper;RottingCorpse;SkeletalDragon" );
			MobileSpawnGroups.Add( "orcs1", "Orc;OrcCaptain" );
			MobileSpawnGroups.Add( "orcs2", "OrcishMage;OrcBomber" );
			MobileSpawnGroups.Add( "orcs3", "OrcishLord;OrcBrute" );
			MobileSpawnGroups.Add( "orcsall", "Orc;OrcishMage;OrcishLord;OrcBrute;OrcBomber;OrcCaptain" );
			MobileSpawnGroups.Add( "ogres", "Ogre;OgreLord");
			MobileSpawnGroups.Add( "giants", "Troll;Cyclops;Titan" );
			MobileSpawnGroups.Add( "articanimals", "icesnake;walrus;polarbear;snowleopard;whitewolf");
			MobileSpawnGroups.Add( "artic1", "icesnake;frostooze;frostspider");
			MobileSpawnGroups.Add( "artic2", "iceserpent;frosttroll;snowelemental;iceelemental");
			MobileSpawnGroups.Add( "artic3", "ArcticOgreLord;iceelemental");
			MobileSpawnGroups.Add( "articall", "ArcticOgreLord;iceelemental;icefiend;iceserpent;icesnake;snowelemental;frostooze;frostspider;frosttroll");
			MobileSpawnGroups.Add( "ophidians", "ophidianarchmage;ophidianknight;ophidianmage;ophidianmatriarch;ophidianwarrior");
			MobileSpawnGroups.Add( "terathan", "terathanavenger;terathandrone;terathanmatriarch;terathanwarrior");
			MobileSpawnGroups.Add( "blacksolen", "blacksoleninfiltratorqueen;blacksoleninfiltratorwarrior;blacksolenqueen;blacksolenwarrior;blacksolenworker" );
			MobileSpawnGroups.Add( "redsolen", "redsoleninfiltratorqueen;redsoleninfiltratorwarrior;redsolenqueen;redsolenwarrior;redsolenworker" );
			MobileSpawnGroups.Add( "watermob", "waterelemental;seaserpent;dolphin" );
			MobileSpawnGroups.Add( "healers", "EvilHealer;EvilWanderingHealer;Healer;WanderingHealer" );
			MobileSpawnGroups.Add( "ratmen", "ratman;ratmanarcher;ratmanmage" );
			MobileSpawnGroups.Add( "savages", "savageshaman;savage;savagerider" );
			MobileSpawnGroups.Add( "meer", "meereternal;meercaptain;meerwarrior;meermage" );
			MobileSpawnGroups.Add( "juka", "jukalord;jukawarrior;jukamage" );
			MobileSpawnGroups.Add( "lavamobs", "lavalizard;lavasnake;lavaserpent" );
			
			#region Nerun Groups
			MobileSpawnGroups.Add( "bears", "brownbear;grizzlybear;blackbear" );
			MobileSpawnGroups.Add( "wolves", "direwolf;timberwolf;greywolf" );
			MobileSpawnGroups.Add( "cows", "bull;cow" );
			MobileSpawnGroups.Add( "roden", "rabbit;jackrabbit" );
			MobileSpawnGroups.Add( "felin", "cougar;panther" );
			MobileSpawnGroups.Add( "misc", "goat;mountaingoat;greathart;hind;sheep;boar;llama;snake;pig" );
			MobileSpawnGroups.Add( "birds", "bird;eagle" );
			MobileSpawnGroups.Add( "mount", "horse;ridablellama" );
			#endregion
			
			ItemSpawnGroups.Add( "regs", "BlackPearl;Bloodmoss;Garlic;Ginseng;MandrakeRoot;Nightshade;SulfurousAsh;SpidersSilk" );
			ItemSpawnGroups.Add( "regs100", "BlackPearl:100;Bloodmoss:100;Garlic:100;Ginseng:100;MandrakeRoot:100;Nightshade:100;SulfurousAsh:100;SpidersSilk:100" );
			ItemSpawnGroups.Add( "necroregs", "BatWing;GraveDust;DaemonBlood;NoxCrystal;PigIron" );
			ItemSpawnGroups.Add( "allregs", "BlackPearl;Bloodmoss;Garlic;Ginseng;MandrakeRoot;Nightshade;SulfurousAsh;SpidersSilk;BatWing;GraveDust;DaemonBlood;NoxCrystal;PigIron" );
			ItemSpawnGroups.Add( "gems", "Amber;Amethyst;Citrine;Diamond;Emerald;Ruby;Sapphire;StarSapphire;Tourmaline" );
			ItemSpawnGroups.Add( "fruits", "banana;bananas;SplitCoconut;Lemon;Lime;Coconut;Dates;Grapes;Peach;Pear;Apple;Watermelon;Squash;Cantaloupe" );
			ItemSpawnGroups.Add( "vegetables", "Carrot;Cabbage;Onion;Lettuce;Pumpkin" );
			ItemSpawnGroups.Add( "platearmor", "platearms;platechest;plategloves;plategorget;platelegs;platehelm;heatershield" );
			ItemSpawnGroups.Add( "chainarmor", "chainchest;chainlegs" );
			ItemSpawnGroups.Add( "ringarmor", "ringmailarms;ringmailchest;ringmail;gloves;ringmaillegs" );
			ItemSpawnGroups.Add( "rangerarmor", "rangerarms;rangerchest;rangergloves;rangergorget;rangerlegs" );
		}*/
//  <Spawn Groups>---------------------------------------------------------<End>
		#endregion
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Active
		{
			get { return m_Active; }
			set
			{
				if ( !MC.CheckProcess() )
				{
					m_Active = value;

					if ( !value )
						Stop();
					else
						Start();
				}
			}
		}

		public ArrayList EntryList
		{
			get{ return m_EntryList; }
			set{ m_EntryList = value; }
		}

		public ArrayList SpawnRangeList
		{
			get{ return m_SpawnRangeList; }
			set{ m_SpawnRangeList = value; }
		}

		public ArrayList WalkRangeList
		{
			get{ return m_WalkRangeList; }
			set{ m_WalkRangeList = value; }
		}

		public ArrayList AmountList
		{
			get{ return m_AmountList; }
			set{ m_AmountList = value; }
		}

		public ArrayList MinDelayList
		{
			get{ return m_MinDelayList; }
			set{ m_MinDelayList = value; }
		}

		public ArrayList MaxDelayList
		{
			get{ return m_MaxDelayList; }
			set{ m_MaxDelayList = value; }
		}

		public ArrayList SpawnTypeList
		{
			get{ return m_SpawnTypeList; }
			set{ m_SpawnTypeList = value; }
		}

		public ArrayList ActivatedList
		{
			get{ return m_ActivatedList; }
			set{ m_ActivatedList = value; }
		}

		public ArrayList EventRangeList
		{
			get{ return m_EventRangeList; }
			set{ m_EventRangeList = value; }
		}

		public ArrayList EventKeywordList
		{
			get{ return m_EventKeywordList; }
			set{ m_EventKeywordList = value; }
		}

		public ArrayList KeywordCaseSensitiveList
		{
			get{ return m_KeywordCaseSensitiveList; }
			set{ m_KeywordCaseSensitiveList = value; }
		}

		public ArrayList TriggerEventNowList
		{
			get{ return m_TriggerEventNowList; }
			set{ m_TriggerEventNowList = value; }
		}

		public ArrayList EventAmbushList
		{
			get{ return m_EventAmbushList; }
			set{ m_EventAmbushList = value; }
		}

		public ArrayList BeginTimeBasedList
		{
			get{ return m_BeginTimeBasedList; }
			set{ m_BeginTimeBasedList = value; }
		}

		public ArrayList EndTimeBasedList
		{
			get{ return m_EndTimeBasedList; }
			set{ m_EndTimeBasedList = value; }
		}

		public ArrayList GroupSpawnList
		{
			get{ return m_GroupSpawnList; }
			set{ m_GroupSpawnList = value; }
		}

		public ArrayList MinStackAmountList
		{
			get{ return m_MinStackAmountList; }
			set{ m_MinStackAmountList = value; }
		}

		public ArrayList MaxStackAmountList
		{
			get{ return m_MaxStackAmountList; }
			set{ m_MaxStackAmountList = value; }
		}

		public ArrayList MovableList
		{
			get{ return m_MovableList; }
			set{ m_MovableList = value; }
		}

		public ArrayList MinDespawnList
		{
			get{ return m_MinDespawnList; }
			set{ m_MinDespawnList = value; }
		}

		public ArrayList MaxDespawnList
		{
			get{ return m_MaxDespawnList; }
			set{ m_MaxDespawnList = value; }
		}

		public ArrayList DespawnList
		{
			get{ return m_DespawnList; }
			set{ m_DespawnList = value; }
		}

		public ArrayList DespawnGroupList
		{
			get{ return m_DespawnGroupList; }
			set{ m_DespawnGroupList = value; }
		}

		public ArrayList DespawnTimeExpireList
		{
			get{ return m_DespawnTimeExpireList; }
			set{ m_DespawnTimeExpireList = value; }
		}

		public ArrayList SettingsList
		{
			get{ return m_SettingsList; }
			set{ m_SettingsList = value; }
		}

		public ArrayList RespawnEntryList
		{
			get{ return m_RespawnEntryList; }
			set{ m_RespawnEntryList = value; }
		}

		public ArrayList RespawnTimeList
		{
			get{ return m_RespawnTimeList; }
			set{ m_RespawnTimeList = value; }
		}

		public ArrayList SpawnCounterList
		{
			get{ return m_SpawnCounterList; }
			set{ m_SpawnCounterList = value; }
		}

		public ArrayList SpawnTimeList
		{
			get{ return m_SpawnTimeList; }
			set{ m_SpawnTimeList = value; }
		}

		public ArrayList RespawnOnSaveList
		{
			get{ return m_RespawnOnSaveList; }
			set{ m_RespawnOnSaveList = value; }
		}

		public ArrayList DespawnTimeList
		{
			get{ return m_DespawnTimeList; }
			set{ m_DespawnTimeList = value; }
		}

		public ArrayList SpawnedEntries
		{
			get{ return m_SpawnedEntries; }
			set{ m_SpawnedEntries = value; }
		}

		public ArrayList LastMovedList
		{
			get{ return m_LastMovedList; }
			set{ m_LastMovedList = value; }
		}

		public Mobile Editor
		{
			get{ return m_Editor; }
			set{ m_Editor = value; }
		}

		public Mobile WorkspaceEditor
		{
			get{ return m_WorkspaceEditor; }
			set{ m_WorkspaceEditor = value; }
		}

		public Mobile FileEdit
		{
			get{ return m_FileEdit; }
			set{ m_FileEdit = value; }
		}

		public bool Workspace
		{
			get{ return m_Workspace; }
			set{ m_Workspace = value; }
		}

		public bool NoProximityType
		{
			get{ return m_NoProximityType; }
			set{ m_NoProximityType = value; }
		}

		public bool NoSpeechType
		{
			get{ return m_NoSpeechType; }
			set{ m_NoSpeechType = value; }
		}

		public bool Respawning
		{
			get{ return m_Respawning; }
			set{ m_Respawning = value; }
		}

		public string Imported
		{
			get{ return m_Imported; }
			set{ m_Imported = value; }
		}

		public string ImportVersion
		{
			get{ return m_ImportVersion; }
			set{ m_ImportVersion = value; }
		}

		public Container ContainerSpawn
		{
			get{ return m_ContainerSpawn; }
			set{ m_ContainerSpawn = value; }
		}

// ****** End of Variables Declaration *****************

		public static void Initialize()
		{
			if ( ListExceptions > 0 )
				Console.WriteLine( "\nMega Spawner System: {0} ArrayList exceptions have been found. Affected spawners respawned.", ListExceptions );

			if ( MC.CA_Disabled )
				new DelayWipeTimer().Start();
		}

		public override bool HandlesOnMovement{ get{ return CheckOnMovement(); } }
		public override bool HandlesOnSpeech{ get{ return CheckOnSpeech(); } }

		[Constructable]
		public MegaSpawner() : base( 0x1f13 )
		{
			if ( MC.CheckProcess() )
			{
				Delete();
			}
			else
			{
				Active = true;
				Imported = "";
				Visible = false;
				Movable = false;
				Name = "A Mega Spawner";

				MC.SpawnerList.Add( this );
			}
		}

		public MegaSpawner( bool useless ) : base( 0x1f13 )
		{
			Active = true;
			Imported = "";
			Visible = false;
			Movable = false;
			Name = "A Mega Spawner";

			MC.SpawnerList.Add( this );
		}

		public void Dupe( Mobile from, int amount )
		{
			for( int i = 0; i < amount; i++ )
			{
				MegaSpawner ms = MC.DupeSpawner( this, new MegaSpawner() );
				ms.Movable = true;

				from.AddToBackpack( ms );
			}
		}

		public MegaSpawner( Serial serial ) : base( serial )
		{
		}

// ****** Settings Section ***********************

		private bool m_OverrideIndividualEntries;

		public int OverrideSpawnRange, OverrideWalkRange, OverrideAmount, OverrideMinDelay, OverrideMaxDelay, OverrideEventRange, OverrideBeginTimeBased, OverrideEndTimeBased, OverrideBeginTimeBasedHour, OverrideBeginTimeBasedMinute, OverrideEndTimeBasedHour, OverrideEndTimeBasedMinute, OverrideMinDespawn, OverrideMaxDespawn;
		public bool OverrideGroupSpawn, OverrideEventAmbush, OverrideTriggerEventNow, OverrideCaseSensitive, OverrideDespawn, OverrideDespawnGroup, OverrideDespawnTimeExpire;
		public string OverrideEventKeyword;
		public SpawnType OverrideSpawnType;

		public ArrayList OverrideSpawnedEntries = new ArrayList();
		public ArrayList OverrideLastMovedList = new ArrayList();

		public ArrayList OverrideRespawnEntryList = new ArrayList();
		public ArrayList OverrideRespawnTimeList = new ArrayList();
		public ArrayList OverrideSpawnCounterList = new ArrayList();
		public ArrayList OverrideSpawnTimeList = new ArrayList();
		public ArrayList OverrideDespawnTimeList = new ArrayList();
		public bool OverrideRespawnOnSave;

		public ArrayList AddItemRefList = new ArrayList();
		public ArrayList AddItemList = new ArrayList();

		public bool OverrideIndividualEntries
		{
			get{ return m_OverrideIndividualEntries; }
			set{ m_OverrideIndividualEntries = value; }
		}

		public void CompileSettings()
		{
			ResetSettingValues();

			try
			{
				for ( int i = 0; i < SettingsList.Count; i++ )
				{
					ArrayList setting = (ArrayList) SettingsList[i];
					ArrayList List = new ArrayList();
					int entryIndex = -1;

					switch ( (Setting) setting[0] )
					{
						case Setting.OverrideIndividualEntries:
						{
							OverrideSpawnRange = (int)				setting[1];
							OverrideWalkRange = (int)				setting[2];
							OverrideAmount = (int)					setting[3];
							OverrideMinDelay = (int)				setting[4];
							OverrideMaxDelay = (int)				setting[5];
							OverrideGroupSpawn = (bool)				setting[6];
							OverrideEventAmbush = (bool)			setting[7];
							OverrideSpawnType = (SpawnType)			setting[8];
							OverrideEventKeyword = (string)			setting[9];
							OverrideCaseSensitive = (bool)			setting[10];
							OverrideEventRange = (int)				setting[11];
							OverrideBeginTimeBasedHour = (int)		setting[12];
							OverrideBeginTimeBasedMinute = (int)	setting[13];
							OverrideEndTimeBasedHour = (int)		setting[14];
							OverrideEndTimeBasedMinute = (int)		setting[15];
							OverrideMinDespawn = (int)				setting[16];
							OverrideMaxDespawn = (int)				setting[17];
							OverrideDespawn = (bool)				setting[18];
							OverrideDespawnGroup = (bool)			setting[19];
							OverrideDespawnTimeExpire = (bool)		setting[20];

							OverrideIndividualEntries = true; 

							OverrideBeginTimeBased = OverrideBeginTimeBasedHour * OverrideBeginTimeBasedMinute;
							OverrideEndTimeBased = OverrideEndTimeBasedHour * OverrideEndTimeBasedMinute;

							break;
						}
						case Setting.AddItem:{ entryIndex = (int) setting[2]; List = SetItemInfo( setting ); break; }
						case Setting.AddContainer:
						{
							List = SetItemInfo( setting );

							entryIndex = (int)				setting[2];

							for ( int j = 6; j < setting.Count; j++ )
							{
								ArrayList ItemsList = (ArrayList) setting[j];

								List.Add( ItemsList );
							}

							break;
						}
					}

					if ( entryIndex != -1 )
					{
						AddItemRefList.Add( entryIndex );
						AddItemList.Add( List );
					}
				}
			}
			catch{}
		}

		public ArrayList SetItemInfo( ArrayList setting )
		{
			ArrayList List = new ArrayList();

			List.Add( (string)					setting[3] );
			List.Add( (int)						setting[4] );
			List.Add( (int)						setting[5] );

			return List;
		}

		public void ConvertOldSettings()
		{
			ArrayList newSettingsList = new ArrayList();

			try
			{
				for ( int i = 0; i < SettingsList.Count; i++ )
				{
					string setting = (string) SettingsList[i];
					string[] readSetting = setting.Split(':');
					string settingType = readSetting[0];
					string[] settings = readSetting[1].Split('/');
					string settingProp, settingValue;
					string[] settingInfo;
					string minSetValue=null, maxSetValue=null;

					int spawnRange=10, walkRange=10, amount=1, minDelay=300, maxDelay=600, eventRange=10, beginTimeBasedHour=0, beginTimeBasedMinute=0, endTimeBasedHour=0, endTimeBasedMinute=0, minDespawn=1800, maxDespawn=3600;
					bool groupSpawn=false, eventAmbush=true, caseSensitive=false, despawn=false, despawnGroup=false, despawnTimeExpire=true;
					SpawnType spawnType = SpawnType.Regular;
					string keyword = null;

					ArrayList newSetting = new ArrayList();
					newSetting.Add( 0 );

					for ( int j = 0; j < settings.Length; j++ )
					{
						settingInfo = settings[j].Split('=');
						settingProp = settingInfo[0];
						settingValue = settingInfo[1];
						string[] splitValues = settingValue.Split( '<' );

						if ( splitValues.Length > 1 )
						{
							minSetValue = splitValues[0];
							maxSetValue = splitValues[1];
						}

						switch ( settingType )
						{
							case "OVERRIDEINDIVIDUALENTRIES":
							{
								newSetting[0] = Setting.OverrideIndividualEntries;

								switch ( settingProp )
								{
									case "SpawnRange":				{ spawnRange = Convert.ToInt32( settingValue ); break; }
									case "WalkRange":				{ walkRange = Convert.ToInt32( settingValue ); break; }
									case "Amount":					{ amount = Convert.ToInt32( settingValue ); break; }
									case "MinDelay":				{ minDelay = Convert.ToInt32( settingValue ); break; }
									case "MaxDelay":				{ maxDelay = Convert.ToInt32( settingValue ); break; }
									case "GroupSpawn":				{ groupSpawn = Convert.ToBoolean( settingValue ); break; }
									case "EventAmbush":				{ eventAmbush = Convert.ToBoolean( settingValue ); break; }
									case "SpawnType":				{ spawnType = (SpawnType) Convert.ToInt32( settingValue ); break; }
									case "Keyword":					{ keyword = Convert.ToString( settingValue ); break; }
									case "CaseSensitive":			{ caseSensitive = Convert.ToBoolean( settingValue ); break; }
									case "EventRange":				{ eventRange = Convert.ToInt32( settingValue ); break; }
									case "BeginTimeBasedHour":		{ beginTimeBasedHour = Convert.ToInt32( settingValue ); break; }
									case "BeginTimeBasedMinute":	{ beginTimeBasedMinute = Convert.ToInt32( settingValue ); break; }
									case "EndTimeBasedHour":		{ endTimeBasedHour = Convert.ToInt32( settingValue ); break; }
									case "EndTimeBasedMinute":		{ endTimeBasedMinute = Convert.ToInt32( settingValue ); break; }
									case "MinDespawn":				{ minDespawn = Convert.ToInt32( settingValue ); break; }
									case "MaxDespawn":				{ maxDespawn = Convert.ToInt32( settingValue ); break; }
									case "Despawn":					{ despawn = Convert.ToBoolean( settingValue ); break; }
									case "DespawnGroup":			{ despawnGroup = Convert.ToBoolean( settingValue ); break; }
									case "DespawnTimeExpire":		{ despawnTimeExpire = Convert.ToBoolean( settingValue ); break; }
								}

								break;
							}
							case "ADDITEM":
							{
								newSetting[0] = Setting.AddItem;

								switch ( settingProp )
								{
									case "EntryName":				{ newSetting.Add( settingValue ); break; }
									case "EntryIndexNum":			{ newSetting.Add( Convert.ToInt32( settingValue ) ); break; }
									case "ItemAdded":				{ newSetting.Add( settingValue ); break; }
									case "StackAmount":				{ newSetting.Add( Convert.ToInt32( minSetValue ) ); newSetting.Add( Convert.ToInt32( maxSetValue ) ); break; }
									case "ContItem":				{ newSetting[0] = Setting.AddContainer; newSetting.Add( settingValue ); break; }
									case "ContItemStackAmount":		{ newSetting.Add( Convert.ToInt32( minSetValue ) ); newSetting.Add( Convert.ToInt32( maxSetValue ) ); break; }
								}

								break;
							}
						}
					}

					if ( newSetting[0] is Setting )
					{
						switch ( (Setting) newSetting[0] )
						{
							case Setting.OverrideIndividualEntries:
							{
								newSetting.Add( spawnRange );
								newSetting.Add( walkRange );
								newSetting.Add( amount );
								newSetting.Add( minDelay );
								newSetting.Add( maxDelay );
								newSetting.Add( groupSpawn );
								newSetting.Add( eventAmbush );
								newSetting.Add( spawnType );
								newSetting.Add( keyword );
								newSetting.Add( caseSensitive );
								newSetting.Add( eventRange );
								newSetting.Add( beginTimeBasedHour );
								newSetting.Add( beginTimeBasedMinute );
								newSetting.Add( endTimeBasedHour );
								newSetting.Add( endTimeBasedMinute );
								newSetting.Add( minDespawn );
								newSetting.Add( maxDespawn );
								newSetting.Add( despawn );
								newSetting.Add( despawnGroup );
								newSetting.Add( despawnTimeExpire );

								newSettingsList.Add( newSetting );

								break;
							}
							case Setting.AddItem:
							{
								bool failed = false;

								if ( !failed && newSetting[1] is string )
								{}
								else
									failed = true;

								if ( !failed && newSetting[2] is int )
								{}
								else
									failed = true;

								if ( !failed && newSetting[3] is string )
								{}
								else
									failed = true;

								if ( !failed && newSetting[4] is int )
								{}
								else
									failed = true;

								if ( !failed && newSetting[5] is int )
								{}
								else
									failed = true;

								if ( !failed )
									newSettingsList.Add( newSetting );

								break;
							}
							case Setting.AddContainer:
							{
								bool failed = false;

								if ( !failed && newSetting[1] is string )
								{}
								else
									failed = true;

								if ( !failed && newSetting[2] is int )
								{}
								else
									failed = true;

								if ( !failed && newSetting[3] is string )
								{}
								else
									failed = true;

								if ( !failed && newSetting[4] is int )
								{}
								else
									failed = true;

								if ( !failed && newSetting[5] is int )
								{}
								else
									failed = true;

								if ( !failed && newSetting.Count == 6 )
									newSettingsList.Add( newSetting );

								for ( int j = 6; j < newSetting.Count; j++ )
								{
									if ( newSetting[j] is ArrayList )
									{
										ArrayList ItemsList = (ArrayList) newSetting[j];

										if ( ItemsList[0] is string )
										{}
										else
											failed = true;

										if ( ItemsList[1] is int )
										{}
										else
											failed = true;

										if ( ItemsList[2] is int )
										{}
										else
											failed = true;
									}
									else
										failed = true;

									if ( !failed )
										newSettingsList.Add( newSetting );
								}

								break;
							}
						}
					}
				}

				SettingsList = newSettingsList;
			}
			catch{}
		}

		public void RecompileSettings( int index )
		{
			try
			{
				for ( int i = 0; i < SettingsList.Count; i++ )
				{
					ArrayList setting = (ArrayList) SettingsList[i];

					switch ( (Setting) setting[0] )
					{
						case Setting.AddItem:
						{
							if ( index == (int) setting[2] )
								setting[1] = (string) EntryList[index];

							break;
						}
						case Setting.AddContainer:
						{
							if ( index == (int) setting[2] )
								setting[1] = (string) EntryList[index];

							break;
						}
					}
				}
			}
			catch{}
		}

		public void ResetSettingValues()
		{
			OverrideIndividualEntries = false;

			AddItemRefList.Clear();
			AddItemList.Clear();
		}

		public void ResetSetting( int index )
		{
			try
			{
				for ( int i = 0; i < SettingsList.Count; i++ )
				{
					ArrayList setting = (ArrayList) SettingsList[i];

					switch ( (Setting) setting[0] )
					{
						case Setting.OverrideIndividualEntries:{ OverrideIndividualEntries = false; break; }
					}
				}
			}
			catch{}
		}

		public void RemoveSettings( int index )
		{
			string entryType = (string) EntryList[index];

			try
			{
				for ( int i = 0; i < SettingsList.Count; i++ )
				{
					ArrayList setting = (ArrayList) SettingsList[i];

					switch ( (Setting) setting[0] )
					{
						case Setting.AddItem:{ if ( RemoveSetting( setting, i ) ) i--; break; }
						case Setting.AddContainer:{ if ( RemoveSetting( setting, i ) ) i--; break; }
					}
				}
			}
			catch{}
		}

		private bool RemoveSetting( ArrayList setting, int index )
		{
			string entryType = (string) EntryList[index];

			if ( entryType == (string) setting[1] && index == (int) setting[2] )
			{
				SettingsList.RemoveAt( index );

				return true;
			}

			return false;
		}

		public void CheckDupedSettings()
		{
			bool bOverrideIndividualEntries = false;

			try
			{
				for ( int i = 0; i < SettingsList.Count; i++ )
				{
					ArrayList setting = (ArrayList) SettingsList[i];

					switch ( (Setting) setting[0] )
					{
						case Setting.OverrideIndividualEntries:
						{
							if ( !bOverrideIndividualEntries )
							{
								bOverrideIndividualEntries = true;
							}
							else
							{
								SettingsList.RemoveAt( i );
								i--;
							}

							break;
						}
					}
				}
			}
			catch{}
		}

		public void CheckAddItem( int index, object o )
		{
			ArrayList List = new ArrayList();
			ArrayList ContList = new ArrayList();

			for ( int i = 0; i < AddItemRefList.Count; i++ )
			{
				int entryIndex = (int) AddItemRefList[i];

				if ( entryIndex == index )
				{
					List = (ArrayList) AddItemList[i];

					string addItem = (string) List[0];
					int minStackAmount = (int) List[1];
					int maxStackAmount = (int) List[2];
					string contAddItem;
					int contMinStackAmount, contMaxStackAmount;

					Type type = ScriptCompiler.FindTypeByName( addItem );
					Item toAdd = (Item) Activator.CreateInstance( type );

					if ( o is Item )
					{
						Item item = (Item) o;

						if ( item is Container )
						{
							if ( toAdd.Stackable )
								toAdd.Amount = Utility.Random( minStackAmount, maxStackAmount - minStackAmount );

							( (Container) item ).DropItem( toAdd );

							if ( toAdd is Container && List.Count > 3 )
							{
								for ( int j = 3; j < List.Count; j++ )
								{
									ContList = (ArrayList) List[j];

									contAddItem = (string) ContList[0];
									contMinStackAmount = (int) ContList[1];
									contMaxStackAmount = (int) ContList[2];

									Type contType = ScriptCompiler.FindTypeByName( contAddItem );
									Item contToAdd = (Item) Activator.CreateInstance( contType );

									if ( contToAdd.Stackable )
										contToAdd.Amount = Utility.Random( contMinStackAmount, contMaxStackAmount - contMinStackAmount );

									( (Container) toAdd ).DropItem( contToAdd );
								}
							}
						}
						else
						{
							toAdd.Delete();
						}
					}
					else if ( o is Mobile )
					{
						Mobile mobile = (Mobile) o;

						if ( mobile.Backpack != null )
						{
							if ( toAdd.Stackable )
								toAdd.Amount = Utility.Random( minStackAmount, maxStackAmount - minStackAmount );

							mobile.AddToBackpack( toAdd );

							if ( toAdd is Container && List.Count > 3 )
							{
								for ( int j = 3; j < List.Count; j++ )
								{
									ContList = (ArrayList) List[j];

									contAddItem = (string) ContList[0];
									contMinStackAmount = (int) ContList[1];
									contMaxStackAmount = (int) ContList[2];

									Type contType = ScriptCompiler.FindTypeByName( contAddItem );
									Item contToAdd = (Item) Activator.CreateInstance( contType );

									if ( contToAdd.Stackable )
										contToAdd.Amount = Utility.Random( contMinStackAmount, contMaxStackAmount - contMinStackAmount );

									( (Container) toAdd ).DropItem( contToAdd );
								}
							}
						}
						else
						{
							toAdd.Delete();
						}
					}
				}
			}
		}

// ****** End of Settings Section ****************

		public override void OnDoubleClick( Mobile from )
		{
			if ( from.AccessLevel < MC.AccessLevelReq || MC.GetAccessLevel( from ) == MSS.Access.None )
			{
				from.SendMessage( "You do not have authorization to edit that Mega Spawner." );

				return;
			}
			else if ( Workspace && WorkspaceEditor != from )
			{
				from.SendMessage( "You cannot edit that Mega Spawner. It is a part of a Spawner Workspace creation being worked on by {0}.", WorkspaceEditor.Name );

				return;
			}
			else if ( FileEdit != null && FileEdit != from )
			{
				from.SendMessage( "That Mega Spawner is a part of a file which is currently being editted by {0}. You must wait for them to finish before you may edit that Mega Spawner.", FileEdit.Name );

				return;
			}
			else if ( Editor != null )
			{
				from.SendMessage( "That Mega Spawner is currently being editted by {0}. You must wait for them to finish before you may edit that Mega Spawner.", Editor.Name );

				return;
			}

			Editor = from;

			MC.AddEditor( from, this );

			ArrayList ArgsList = MC.CompileDefaultArgsList( this );
			ArgsList = MC.LoadPersonalConfig( from, ArgsList );

			from.SendGump( new MegaSpawnerEditGump( from, ArgsList ) );
		}

		public override void OnParentDeleted( object parent )
		{
			if ( parent is Container )
			{
				if ( RootParent is Item )
					MoveToWorld( ( (Item) RootParent ).Location, Map );
				else if ( RootParent is Mobile )
					MoveToWorld( ( (Mobile) RootParent ).Location, Map );
			}
			else
			{
				base.OnParentDeleted( parent );
			}
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			if ( oldLocation.X != X || oldLocation.Y != Y )
				Respawn();
		}

		public override void Delete()
		{
			if ( MC.GetProcess != Process.SaveBackup && !Workspace && FileEdit == null && Editor == null )
			{
				base.Delete();

				CheckEntries();
				DeleteEntries();
				WipeArrayLists();

				if ( Imported != "" )
				{
					MC.AdjustImportCount( Imported, -1 );
					MC.CheckFileImportList( Imported );
				}
			}
			else
			{
				PublicOverheadMessage( 0, 0x35, true, MC.DeleteMsg[Utility.Random( 0, MC.DeleteMsg.Length )] );
			}
		}

		public void WipeArrayLists()
		{
			EntryList.Clear();
			SpawnRangeList.Clear();
			WalkRangeList.Clear();
			AmountList.Clear();
			MinDelayList.Clear();
			MaxDelayList.Clear();
			SpawnTypeList.Clear();
			ActivatedList.Clear();
			EventRangeList.Clear();
			EventKeywordList.Clear();
			KeywordCaseSensitiveList.Clear();
			TriggerEventNowList.Clear();
			EventAmbushList.Clear();
			BeginTimeBasedList.Clear();
			EndTimeBasedList.Clear();
			GroupSpawnList.Clear();
			MinStackAmountList.Clear();
			MaxStackAmountList.Clear();
			MovableList.Clear();
			MinDespawnList.Clear();
			MaxDespawnList.Clear();
			DespawnList.Clear();
			DespawnGroupList.Clear();
			DespawnTimeExpireList.Clear();

			RespawnEntryList.Clear();
			RespawnTimeList.Clear();
			SpawnCounterList.Clear();
			SpawnTimeList.Clear();
			RespawnOnSaveList.Clear();
			DespawnTimeList.Clear();

			SpawnedEntries.Clear();
			LastMovedList.Clear();

			OverrideRespawnEntryList.Clear();
			OverrideRespawnTimeList.Clear();
			OverrideSpawnCounterList.Clear();
			OverrideSpawnTimeList.Clear();
			OverrideDespawnTimeList.Clear();

			OverrideSpawnedEntries.Clear();
			OverrideLastMovedList.Clear();
		}

		public void DeleteEntry( int index )
		{
			RemoveSettings( index );
			DeleteEntries( index );
			DeleteRespawnEntries( index );

			EntryList.RemoveAt( index );
			SpawnRangeList.RemoveAt( index );
			WalkRangeList.RemoveAt( index );
			AmountList.RemoveAt( index );
			MinDelayList.RemoveAt( index );
			MaxDelayList.RemoveAt( index );
			SpawnTypeList.RemoveAt( index );
			ActivatedList.RemoveAt( index );
			EventRangeList.RemoveAt( index );
			EventKeywordList.RemoveAt( index );
			KeywordCaseSensitiveList.RemoveAt( index );
			TriggerEventNowList.RemoveAt( index );
			EventAmbushList.RemoveAt( index );
			BeginTimeBasedList.RemoveAt( index );
			EndTimeBasedList.RemoveAt( index );
			GroupSpawnList.RemoveAt( index );
			MinStackAmountList.RemoveAt( index );
			MaxStackAmountList.RemoveAt( index );
			MovableList.RemoveAt( index );
			MinDespawnList.RemoveAt( index );
			MaxDespawnList.RemoveAt( index );
			DespawnList.RemoveAt( index );
			DespawnGroupList.RemoveAt( index );
			DespawnTimeExpireList.RemoveAt( index );

			if ( OverrideIndividualEntries )
				MegaSpawnerOverride.ForceReconfigRespawn( this );
		}

		public void CheckEntryErrors()
		{
			for ( int i = 0; i < EntryList.Count; i++ )
			{
				string checkEntryType = Convert.ToString( ScriptCompiler.FindTypeByName( EntryList[i].ToString() ) );

				if ( checkEntryType == "" )
					ActivatedList[i] = (bool) false;
			}
		}

		public void CheckListDiscrepancies()
		{
			bool error = false;

			if ( !OverrideIndividualEntries && ( SpawnedEntries.Count != RespawnEntryList.Count || RespawnEntryList.Count != RespawnTimeList.Count || RespawnEntryList.Count != SpawnCounterList.Count || RespawnEntryList.Count != SpawnTimeList.Count || RespawnEntryList.Count != RespawnOnSaveList.Count ) )
			{
				ListExceptions++;
				error = true;

				RespawnEntryList.Clear();
				RespawnTimeList.Clear();
				SpawnCounterList.Clear();
				SpawnTimeList.Clear();
				RespawnOnSaveList.Clear();
				DespawnTimeList.Clear();

				for ( int i = 0; i < EntryList.Count; i++ )
				{
					int amount = (int) AmountList[i];

					ArrayList respawnEntryList = new ArrayList();
					ArrayList respawnTimeList = new ArrayList();
					ArrayList spawnCounterList = new ArrayList();
					ArrayList spawnTimeList = new ArrayList();
					ArrayList respawnOnSaveList = new ArrayList();
					ArrayList despawnTimeList = new ArrayList();

					for ( int j = 0; j < amount; j++ )
					{
						respawnEntryList.Add( (string) EntryList[i] );
						respawnTimeList.Add( 0 );
						spawnCounterList.Add( DateTime.Now );
						spawnTimeList.Add( 0 );
						respawnOnSaveList.Add( (bool) false );
						despawnTimeList.Add( 0 );
					}

					RespawnEntryList.Add( respawnEntryList );
					RespawnTimeList.Add( respawnTimeList );
					SpawnCounterList.Add( spawnCounterList );
					SpawnTimeList.Add( spawnTimeList );
					RespawnOnSaveList.Add( respawnOnSaveList );
					DespawnTimeList.Add( despawnTimeList );
				}
			}

			if ( OverrideIndividualEntries && ( OverrideRespawnEntryList.Count != OverrideRespawnTimeList.Count || OverrideRespawnEntryList.Count != OverrideSpawnCounterList.Count || OverrideRespawnEntryList.Count != OverrideSpawnTimeList.Count || OverrideRespawnEntryList.Count != OverrideDespawnTimeList.Count ) )
			{
				ListExceptions++;
				error = true;

				for ( int i = 0; i < OverrideAmount; i++ )
				{
					OverrideRespawnEntryList.Add( "" );
					OverrideRespawnTimeList.Add( 0 );
					OverrideSpawnCounterList.Add( DateTime.Now );
					OverrideSpawnTimeList.Add( 0 );
					OverrideDespawnTimeList.Add( 0 );
				}
			}

			if ( error )
				Respawn();
		}

		public void SpawnerFailure()
		{
			RemoveRespawnEntries();
			Respawn();
		}

		public void AddToDebugLog( string debugLine )
		{
			MC.AddToDebugLog( String.Format( "{0} - {1} - {2}", Serial, DateTime.Now, debugLine ) );
		}

		public void CheckProximityType()
		{
			NoProximityType = true;

			for ( int i = 0; i < EntryList.Count; i++ )
			{
				SpawnType st = (SpawnType) SpawnTypeList[i];

				if ( st == SpawnType.Proximity )
					NoProximityType = false;
			}
		}

		public void CheckSpeechType()
		{
			NoSpeechType = true;

			for ( int i = 0; i < EntryList.Count; i++ )
			{
				SpawnType st = (SpawnType) SpawnTypeList[i];

				if ( st == SpawnType.Speech )
					NoSpeechType = false;
			}
		}

		public bool CheckOnMovement()
		{
			if ( Active )
			{
				if ( OverrideIndividualEntries && OverrideSpawnType == SpawnType.Proximity && OverrideTriggerEventNow )
					return true;
				else if ( !NoProximityType && CheckTriggerEventNow() )
					return true;
			}

			return false;
		}

		public bool CheckOnSpeech()
		{
			if ( Active )
			{
				if ( OverrideIndividualEntries && OverrideSpawnType == SpawnType.Speech && OverrideTriggerEventNow )
					return true;
				else if ( !NoSpeechType && CheckTriggerEventNow() )
					return true;
			}

			return false;
		}

		public bool CheckTriggerEventNow()
		{
			for ( int i = 0; i < TriggerEventNowList.Count; i++ )
			{
				if ( (bool) TriggerEventNowList[i] )
					return true;
			}

			return false;
		}

		public void CheckEntries()
		{
			if ( MC.Debug )
			{
				AddToDebugLog( String.Format( "[CheckEntries] (SpawnedEntries) Count: {0}", SpawnedEntries.Count ) );
				AddToDebugLog( String.Format( "[CheckEntries] (LastMovedList) Count: {0}", LastMovedList.Count ) );
			}

			for ( int i = 0; i < SpawnedEntries.Count; i++ )
			{
				ArrayList spawnedEntries, lastMovedList, despawnTimeList;

				try
				{
					spawnedEntries = (ArrayList) SpawnedEntries[i];
					lastMovedList = (ArrayList) LastMovedList[i];
					despawnTimeList = (ArrayList) DespawnTimeList[i];
				}
				catch(Exception ex)
				{
					SpawnerFailure();

					AddToDebugLog( String.Format( "[CheckEntries] Exception: {0}", ex ) );
					AddToDebugLog( "[CheckEntries] Spawner failure, respawned." );

					return;
				}

				if ( MC.Debug )
				{
					if ( spawnedEntries.Count != lastMovedList.Count || spawnedEntries.Count != despawnTimeList.Count )
					{
						AddToDebugLog( String.Format( "[CheckEntries] Parallel ArrayList Mismatch: (spawnedEntries) Count: {0}", spawnedEntries.Count ) );
						AddToDebugLog( String.Format( "[CheckEntries] Parallel ArrayList Mismatch: (lastMovedList) Count: {0}", lastMovedList.Count ) );
						AddToDebugLog( String.Format( "[CheckEntries] Parallel ArrayList Mismatch: (despawnTimeList) Count: {0}", despawnTimeList.Count ) );

						SpawnerFailure();

						return;
					}
					else
					{
						AddToDebugLog( String.Format( "[CheckEntries] (spawnedEntries:#{0}) Count: {1}", i, spawnedEntries.Count ) );
						AddToDebugLog( String.Format( "[CheckEntries] (lastMovedList:#{0}) Count: {1}", i, lastMovedList.Count ) );
						AddToDebugLog( String.Format( "[CheckEntries] (despawnTimeList:#{0}) Count: {1}", i, despawnTimeList.Count ) );
					}
				}

				for ( int j = 0; j < spawnedEntries.Count; j++ )
				{
					if ( MC.Debug )
					{
						AddToDebugLog( String.Format( "[CheckEntries] (spawnedEntries:#{0}:{1}) Content: {2}", i, j, spawnedEntries[j] ) );
						AddToDebugLog( String.Format( "[CheckEntries] (lastMovedList:#{0}:{1}) Content: {2}", i, j, lastMovedList[j] ) );
						AddToDebugLog( String.Format( "[CheckEntries] (despawnTimeList:#{0}:{1}) Content: {2}", i, j, despawnTimeList[j] ) );
					}

					object o = spawnedEntries[j];

					if ( o is Item )
					{
						Item item = (Item) o;

						if ( lastMovedList[j] is int )
							lastMovedList[j] = item.LastMoved;

						DateTime lastMoved = (DateTime) lastMovedList[j];

						if ( item.RootParent != null && item.RootParent == (object) ContainerSpawn && lastMoved != item.LastMoved )
						{
							lastMoved = item.LastMoved;
							lastMovedList[j] = item.LastMoved;
						}

						if ( item.Deleted || ( item.RootParent != null && ContainerSpawn != null && item.RootParent != (object) ContainerSpawn ) || ( item.RootParent != null && RootParent != null && item.RootParent != RootParent ) || lastMoved != item.LastMoved )
						{
							spawnedEntries.RemoveAt( j );
							lastMovedList.RemoveAt( j );

							SpawnCounter( o, i );

							j--;
						}
					}
					else if ( o is Mobile )
					{
						Mobile mob = (Mobile) o;

						if ( mob.Deleted || ( mob is BaseCreature && ( ( (BaseCreature) mob ).Controlled || ( (BaseCreature) mob ).IsStabled ) ) )
						{
							spawnedEntries.RemoveAt( j );
							lastMovedList.RemoveAt( j );

							SpawnCounter( o, i );

							j--;
						}
					}
					else
					{
						spawnedEntries.RemoveAt( j );
						lastMovedList.RemoveAt( j );

						SpawnCounter( o, i );

						j--;
					}
				}

				SpawnedEntries[i] = spawnedEntries;
				LastMovedList[i] = lastMovedList;
			}
		}

		public int CountEntries()
		{
			if ( OverrideIndividualEntries )
				return MegaSpawnerOverride.CountEntries( this );

			int entryCount = 0;

			for ( int i = 0; i < SpawnedEntries.Count; i++ )
			{
				ArrayList spawnedEntries = (ArrayList) SpawnedEntries[i];

				entryCount += spawnedEntries.Count;
			}

			return entryCount;
		}

		public int CountEntries( int index )
		{
			if ( OverrideIndividualEntries )
				return MegaSpawnerOverride.CountEntries( this, index );

			ArrayList spawnedEntries = (ArrayList) SpawnedEntries[index];

			return spawnedEntries.Count;
		}

		public void DeleteEntries()
		{
			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.DeleteEntries( this );

				return;
			}

			for ( int i = 0; i < SpawnedEntries.Count; i++ )
			{
				ArrayList spawnedEntries = (ArrayList) SpawnedEntries[i];

				for ( int j = 0; j < spawnedEntries.Count; j++ )
				{
					object o = spawnedEntries[j];

					MC.DeleteObject( o );
				}
			}

			for ( int i = 0; i < SpawnedEntries.Count; i++ )
			{
				SpawnedEntries[i] = new ArrayList();
				LastMovedList[i] = new ArrayList();
			}

			for ( int i = 0; i < RespawnEntryList.Count; i++ )
			{
				ArrayList respawnEntry = (ArrayList) RespawnEntryList[i];

				for ( int j = 0; j < respawnEntry.Count; j++ )
				{
					if ( respawnEntry[j] is Item || respawnEntry[j] is Mobile )
						respawnEntry[j] = Convert.ToString( respawnEntry[j].GetType().Name.ToLower() );
				}

				RespawnEntryList[i] = respawnEntry;
			}
		}

		public void DeleteEntries( int index )
		{
			string entryName = (string) EntryList[index];

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.DeleteEntries( this, entryName );

				return;
			}

			ArrayList spawnedEntries = (ArrayList) SpawnedEntries[index];
			ArrayList lastMovedList = (ArrayList) LastMovedList[index];

			for ( int j = 0; j < spawnedEntries.Count; j++ )
			{
				object o = spawnedEntries[j];

				if ( Convert.ToString( o.GetType().Name.ToLower() ) == entryName.ToLower() )
				{
					MC.DeleteObject( o );

					spawnedEntries.RemoveAt( j );
					lastMovedList.RemoveAt( j );

					j--;
				}
			}

			SpawnedEntries[index] = new ArrayList();
			LastMovedList[index] = new ArrayList();

			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				if ( respawnEntryList[i] is Item || respawnEntryList[i] is Mobile )
				{
					if ( Convert.ToString( respawnEntryList[i].GetType().Name.ToLower() ) == entryName )
						respawnEntryList[i] = entryName;
				}
			}

			RespawnEntryList[index] = respawnEntryList;
		}

		public void ClearSpawner()
		{
			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.ClearSpawner( this );

				return;
			}

			CheckEntries();
			DeleteEntries();

			WipeArrayLists();
		}

		public void RemoveRespawnEntries()
		{
			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.RemoveRespawnEntries( this );

				return;
			}

			RespawnEntryList.Clear();
			RespawnTimeList.Clear();
			SpawnCounterList.Clear();
			SpawnTimeList.Clear();
			RespawnOnSaveList.Clear();
			DespawnTimeList.Clear();
		}

		public void RemoveRespawnEntries( int index )
		{
			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.RemoveRespawnEntries( this, index );

				return;
			}

			RespawnEntryList[index] = new ArrayList();
			RespawnTimeList[index] = new ArrayList();
			SpawnCounterList[index] = new ArrayList();
			SpawnTimeList[index] = new ArrayList();
			RespawnOnSaveList[index] = new ArrayList();
			DespawnTimeList[index] = new ArrayList();
		}

		public void DeleteRespawnEntries( int index )
		{
			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.RemoveRespawnEntries( this );

				return;
			}

			RespawnEntryList.RemoveAt( index );
			RespawnTimeList.RemoveAt( index );
			SpawnCounterList.RemoveAt( index );
			SpawnTimeList.RemoveAt( index );
			RespawnOnSaveList.RemoveAt( index );
			DespawnTimeList.RemoveAt( index );
			SpawnedEntries.RemoveAt( index );
			LastMovedList.RemoveAt( index );
		}

		public void ResetSpawnTime( int index )
		{
			ArrayList respawnTimeList = (ArrayList) RespawnTimeList[index];
			ArrayList spawnCounterList = (ArrayList) SpawnCounterList[index];
			ArrayList spawnTimeList = (ArrayList) SpawnTimeList[index];
			ArrayList despawnTimeList = (ArrayList) DespawnTimeList[index];

			for ( int i = 0; i < respawnTimeList.Count; i++ )
			{
				respawnTimeList[i] = 0;
				spawnCounterList[i] = DateTime.Now;
				spawnTimeList[i] = 0;
				despawnTimeList[i] = 0;
			}

			RespawnTimeList[index] = respawnTimeList;
			SpawnCounterList[index] = spawnCounterList;
			SpawnTimeList[index] = spawnTimeList;
			DespawnTimeList[index] = despawnTimeList;
		}

		public int FindFirstSpawn( int index )
		{
			string entryType = (string) EntryList[index];
			int lowestDelay = 0;

			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList respawnTimeList = (ArrayList) RespawnTimeList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				if ( respawnEntryList[i] is string )
				{
					if ( IsGroupSpawn( entryType ) )
						lowestDelay = (int) respawnTimeList[i];
					else if ( lowestDelay <= 0 && (int) respawnTimeList[i] > 0 )
						lowestDelay = (int) respawnTimeList[i];
					else if ( lowestDelay > (int) respawnTimeList[i] )
						lowestDelay = (int) respawnTimeList[i];
				}
			}

			return lowestDelay - tickOffset;
		}

		public DateTime FindFirstSpawnCounter( int index )
		{
			string entryType = (string) EntryList[index];
			DateTime lowestDelay = DateTime.Now;

			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList spawnCounterList = (ArrayList) SpawnCounterList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				if ( respawnEntryList[i] is string )
				{
					if ( IsGroupSpawn( entryType ) )
						lowestDelay = (DateTime) spawnCounterList[i];
					else if ( lowestDelay >= DateTime.Now && (DateTime) spawnCounterList[i] < DateTime.Now )
						lowestDelay = (DateTime) spawnCounterList[i];
					else if ( lowestDelay > (DateTime) spawnCounterList[i] )
						lowestDelay = (DateTime) spawnCounterList[i];
				}
			}

			return lowestDelay;
		}

		public int FindFirstSpawnTime( int index )
		{
			string entryType = (string) EntryList[index];
			int lowestDelay = 0;

			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList spawnTimeList = (ArrayList) SpawnTimeList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				if ( respawnEntryList[i] is string )
				{
					if ( IsGroupSpawn( entryType ) )
						lowestDelay = (int) spawnTimeList[i];
					else if ( lowestDelay <= 0 && (int) spawnTimeList[i] > 0 )
						lowestDelay = (int) spawnTimeList[i];
					else if ( lowestDelay > (int) spawnTimeList[i] )
						lowestDelay = (int) spawnTimeList[i];
				}
			}

			return lowestDelay - tickOffset;
		}

		public void TickDespawn( int index, int delay )
		{
			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList despawnTimeList = (ArrayList) DespawnTimeList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				if ( respawnEntryList[i] is Item || respawnEntryList[i] is Mobile )
				{
					int despawnTime = (int) despawnTimeList[i];

					if ( (bool) DespawnGroupList[index] )
						despawnTime = (int) despawnTimeList[0];

					if ( despawnTime > 0 )
					{
						despawnTimeList[i] = (int) despawnTimeList[i] - delay;
					}
					else
					{
						object o = respawnEntryList[i];

						if( o is BaseCreature )
						{
							BaseCreature bc = (BaseCreature) o;

							if( bc.FocusMob != null )
								return;
						}

						MC.DeleteObject( o );
					}
				}
			}

			DespawnTimeList[index] = despawnTimeList;
		}

		public void TickDown( int index, int delay )
		{
			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList respawnTimeList = (ArrayList) RespawnTimeList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				if ( respawnEntryList[i] is string )
				{
					if ( (int) respawnTimeList[i] > 0 )
						respawnTimeList[i] = (int) respawnTimeList[i] - delay;
				}
			}

			RespawnTimeList[index] = respawnTimeList;
		}

		public void SpawnCounter( object o, int index )
		{
			object fromCompare = o;

			if ( o is Item )
				fromCompare = (Item) o;
			else if ( o is Mobile )
				fromCompare = (Mobile) o;

			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList respawnTimeList = (ArrayList) RespawnTimeList[index];

			for ( int j = 0; j < respawnEntryList.Count; j++ )
			{
				if ( respawnEntryList[j] is Mobile || respawnEntryList[j] is Item )
				{
					object toCompare = respawnEntryList[j];

					if ( respawnEntryList[j] is Item )
						toCompare = (Item) respawnEntryList[j];
					else if ( respawnEntryList[j] is Mobile )
						toCompare = (Mobile) respawnEntryList[j];

					if ( fromCompare == toCompare && (int) respawnTimeList[j] <= tickOffset )
					{
						string entryType = Convert.ToString( toCompare.GetType().Name.ToLower() );
						respawnEntryList[j] = entryType;

						if ( IsGroupSpawn( entryType ) && IsEmpty( entryType ) )
							AddSpawnCount( index, j, entryType );
						else if ( !IsGroupSpawn( entryType ) )
							AddSpawnCount( index, j, entryType );

						return;
					}
				}
			}
		}

		public void AddSpawnCount( int index, int j, string entryType )
		{
			int randomDelay = Utility.Random( (int) MinDelayList[index], (int) MaxDelayList[index] - (int) MinDelayList[index] );

			ArrayList spawnCounterList = new ArrayList();
			ArrayList spawnTimeList = new ArrayList();

			try
			{
				spawnCounterList = (ArrayList) SpawnCounterList[index];
				spawnTimeList = (ArrayList) SpawnTimeList[index];
			}
			catch(Exception ex)
			{
				SpawnerFailure();

				AddToDebugLog( String.Format( "[AddSpawnCount] Exception: {0}", ex ) );
				AddToDebugLog( "[AddSpawnCount] Spawner failure, respawned." );

				return;
			}

			ArrayList respawnTimeList = (ArrayList) RespawnTimeList[index];

			if ( IsGroupSpawn( entryType ) )
			{
				for ( int i = 0; i < respawnTimeList.Count; i++ )
				{
					respawnTimeList[i] = randomDelay + tickOffset;
					spawnCounterList[i] = DateTime.Now;
					spawnTimeList[i] = randomDelay;
				}
			}
			else
			{
				respawnTimeList[j] = randomDelay + tickOffset;
				spawnCounterList[j] = DateTime.Now;
				spawnTimeList[j] = randomDelay;
			}

			RespawnTimeList[index] = respawnTimeList;
			SpawnCounterList[index] = spawnCounterList;
			SpawnTimeList[index] = spawnTimeList;
		}

		public void AddToRespawnEntries( object o, int index )
		{
			bool success = false;

			ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
			ArrayList respawnTimeList = (ArrayList) RespawnTimeList[index];
			ArrayList spawnCounterList = (ArrayList) SpawnCounterList[index];
			ArrayList spawnTimeList = (ArrayList) SpawnTimeList[index];
			ArrayList despawnTimeList = (ArrayList) DespawnTimeList[index];

			for ( int i = 0; i < respawnEntryList.Count; i++ )
			{
				int randomDespawnTime = Utility.Random( (int) MinDespawnList[index], (int) MaxDespawnList[index] - (int) MinDespawnList[index] );

				if ( respawnEntryList[i] is string )
				{
					object respawnEntry = o;

					if ( o is Item )
						respawnEntry = (Item) o;
					else if ( o is Mobile )
						respawnEntry = (Mobile) o;

					string entryType = Convert.ToString( ScriptCompiler.FindTypeByName( (string) respawnEntryList[i] ) );
					string entryCompare = Convert.ToString( respawnEntry.GetType() );

					if ( entryType == entryCompare && ( (int) respawnTimeList[i] <= tickOffset || IsGroupSpawn( entryType ) ) )
					{
						success = true;

						respawnEntryList[i] = respawnEntry;
						respawnTimeList[i] = 0;
						spawnCounterList[i] = DateTime.Now;
						spawnTimeList[i] = 0;
						despawnTimeList[i] = randomDespawnTime;

						break;
					}
				}
			}

			if ( !success )
			{
				MC.DeleteObject( o );
			}
			else
			{
				RespawnEntryList[index] = respawnEntryList;
				RespawnTimeList[index] = respawnTimeList;
				SpawnCounterList[index] = spawnCounterList;
				SpawnTimeList[index] = spawnTimeList;
				DespawnTimeList[index] = despawnTimeList;
			}
		}

		public void Respawn()
		{
			if ( !Active || Workspace || Map == null || Map == Map.Internal )
				return;

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.Respawn( this );

				return;
			}

			Respawning = true;

			DeleteEntries();

			for ( int i = 0; i < EntryList.Count; i++ )
			{
				bool activated = (bool) ActivatedList[i];
				SpawnType st = (SpawnType) SpawnTypeList[i];

				if ( activated )
				{
					ResetSpawnTime( i );

					switch ( st )
					{
						case SpawnType.Regular:
						{
							Spawn( i );

							break;
						}
						case SpawnType.Proximity:
						{
							TriggerEventNowList[i] = (bool) true;

							break;
						}
						case SpawnType.GameTimeBased:
						{
							int UOHour, UOMinute, time;

							Server.Items.Clock.GetTime( Map, X, Y, out UOHour, out UOMinute );

							if ( UOHour == 24 )
								UOHour = 0;

							time = ( UOHour * 60 ) + UOMinute;

							CheckSpawnTime( i, time );

							break;
						}
						case SpawnType.RealTimeBased:
						{
							int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

							CheckSpawnTime( i, time );

							break;
						}
						case SpawnType.Speech:
						{
							TriggerEventNowList[i] = (bool) true;

							break;
						}
					}
				}
			}

			Respawning = false;
		}

		public void Respawn( int index )
		{
			if ( !Active || Workspace )
				return;

			if ( OverrideIndividualEntries )
				return;

			Respawning = true;

			DeleteEntries( index );

			bool activated = (bool) ActivatedList[index];
			SpawnType st = (SpawnType) SpawnTypeList[index];

			if ( activated )
			{
				ResetSpawnTime( index );

				switch ( st )
				{
					case SpawnType.Regular:
					{
						Spawn( index );

						break;
					}
					case SpawnType.Proximity:
					{
						TriggerEventNowList[index] = (bool) true;

						break;
					}
					case SpawnType.GameTimeBased:
					{
						int UOHour, UOMinute, time;

						Server.Items.Clock.GetTime( Map, X, Y, out UOHour, out UOMinute );

						if ( UOHour == 24 )
							UOHour = 0;

						time = ( UOHour * 60 ) + UOMinute;

						CheckSpawnTime( index, time );

						break;
					}
					case SpawnType.RealTimeBased:
					{
						int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

						CheckSpawnTime( index, time );

						break;
					}
					case SpawnType.Speech:
					{
						TriggerEventNowList[index] = (bool) true;

						break;
					}
				}
			}

			Respawning = false;
		}

		public void BringToHome()
		{
			if ( !Active || Workspace )
				return;

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.BringToHome( this );

				return;
			}

			for ( int i = 0; i < SpawnedEntries.Count; i++ )
			{
				ArrayList spawnedEntries = (ArrayList) SpawnedEntries[i];

				for ( int j = 0; j < spawnedEntries.Count; j++ )
				{
					object o = spawnedEntries[j];

					if ( o is Mobile )
						( (Mobile) o ).Location = Location;
				}
			}
		}

		public void BringToHome( int index )
		{
			if ( !Active || Workspace )
				return;

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.BringToHome( this, index );

				return;
			}

			ArrayList spawnedEntries = (ArrayList) SpawnedEntries[index];

			foreach( object o in spawnedEntries )
			{
				if ( o is Mobile )
					( (Mobile) o ).Location = Location;
			}
		}

		public void SpawnEntry( int index )
		{
			if ( !(bool) ActivatedList[index] )
				return;

			Type spawnType = ScriptCompiler.FindTypeByName( (string) EntryList[index] );

			if ( spawnType != null )
			{
				object check = null;

				try
				{
					object o = Activator.CreateInstance( spawnType );
					check = o;

					ArrayList spawnedEntries = (ArrayList) SpawnedEntries[index];
					ArrayList lastMovedList = (ArrayList) LastMovedList[index];

					if ( o is Item )
					{
						Item item = (Item) o;

						if ( item.Stackable )
						{
							int min = (int) MinStackAmountList[index];
							int max = (int) MaxStackAmountList[index];
							int stack = Utility.Random( min, ( max - min ) );

							item.Amount = stack;
						}

						Point3D loc = Location;

						if ( Parent is Container )
							loc = GetWorldLocation();
						else if ( ContainerSpawn != null )
							loc = ContainerSpawn.GetWorldLocation();
						else
							loc = GetSpawnPosition( index );

						item.OnBeforeSpawn( loc, Map );

						if ( Parent is Container )
							( (Container) Parent ).DropItem( item );
						else if ( ContainerSpawn != null )
							ContainerSpawn.DropItem( item );
						else
							item.MoveToWorld( loc, Map );

						item.OnAfterSpawn();

						item.Movable = (bool) MovableList[index];

						spawnedEntries.Add( item );
						lastMovedList.Add( item.LastMoved );
						AddToRespawnEntries( item, index );

						if ( item.Map == null || item.Map == Map.Internal )
							item.Delete();
					}
					else if ( o is Mobile )
					{
						Mobile m = (Mobile) o;

						Map map = Map;
						Point3D loc = ( m is BaseVendor ? Location : GetSpawnPosition( index ) );

						m.OnBeforeSpawn( loc, map );
						m.Map = map;
						m.Location = loc;
						m.OnAfterSpawn();
						
						if ( m is BaseCreature )
						{
							BaseCreature creature = (BaseCreature) m;

							creature.Location = GetSpawnPosition( index );
							creature.Home = Location;
							creature.RangeHome = (int) WalkRangeList[index];
							creature.CantWalk = !(bool) MovableList[index];

                            // Genova: customização no sistema para traduzir nome da criatura.
                            TraducaoDeNomesMobiles.AplicarAlteracoesCriatura(creature, spawnType.ToString(), creature.Title);
						}

						spawnedEntries.Add( m );
						lastMovedList.Add( DateTime.Now );
						AddToRespawnEntries( m, index );

						if( m.Map == null || m.Map == Map.Internal )
							m.Delete();
					}

					SpawnedEntries[index] = spawnedEntries;
					LastMovedList[index] = lastMovedList;

					CheckAddItem( index, o );
				}
				catch( Exception ex )
				{
					Console.WriteLine( "{0}", ex );
					MC.DeleteObject( check );
				}
			}
		}

		public Point3D GetSpawnPosition( int index )
		{
			int range = (int) SpawnRangeList[index];

			for ( int i = 0; i < spawnRetries; i++ )
			{
				int x, y;

				if ( RootParent is Container )
				{
					x = ( (Container) RootParent ).Location.X;
					y = ( (Container) RootParent ).Location.Y;
				}
				else
				{
					x = Location.X;
					y = Location.Y;
				}

				x += ( Utility.Random( ( (int) range * 2 ) + 1 ) - range );
				y += ( Utility.Random( ( (int) range * 2 ) + 1 ) - range );
				int z = Map.GetAverageZ( x, y );

				if ( Map.CanSpawnMobile( new Point2D( x, y ), Location.Z ) )
					return new Point3D( x, y, Location.Z );
				else if ( Map.CanSpawnMobile( new Point2D( x, y ), z ) )
					return new Point3D( x, y, z );
			}

			return Location;
		}

		public void TimerTick( int delay )
		{
			if ( ContainerSpawn != null && ContainerSpawn.Deleted )
			{
				Location = ContainerSpawn.Location;

				ContainerSpawn = null;
			}

			if ( Workspace )
				return;

			if ( EntryList.Count == 0 || IsNothingActive )
			{
				Start();

				return;
			}

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.TimerTick( this, delay );

				return;
			}

			CheckEntries();

			for ( int i = 0; i < EntryList.Count; i++ )
			{
				bool activated = (bool) ActivatedList[i];
				SpawnType st = (SpawnType) SpawnTypeList[i];

				if ( activated )
				{
					CheckTimeExpire( i, st );

					int countDown = FindFirstSpawn( i );
					int entryCount = CountEntries( i );

					if ( countDown <= 0 && !IsFull( i ) )
					{
						switch ( st )
						{
							case SpawnType.Regular:
							{
								Spawn( i );

								break;
							}
							case SpawnType.Proximity:
							{
								TriggerEventNowList[i] = (bool) true;

								break;
							}
							case SpawnType.GameTimeBased:
							{
								int UOHour, UOMinute, time;

								Server.Items.Clock.GetTime( Map, X, Y, out UOHour, out UOMinute );

								if ( UOHour == 24 )
									UOHour = 0;

								time = ( UOHour * 60 ) + UOMinute;

								CheckSpawnTime( i, time );

								break;
							}
							case SpawnType.RealTimeBased:
							{
								int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

								CheckSpawnTime( i, time );

								break;
							}
							case SpawnType.Speech:
							{
								TriggerEventNowList[i] = (bool) true;

								break;
							}
						}
					}

					if ( (bool) DespawnList[i] )
						TickDespawn( i, delay );

					if ( (bool) GroupSpawnList[i] && entryCount != 0 )
						break;
					else if ( !IsFull( i ) )
						TickDown( i, delay );
				}
			}

			Start();
		}

		public void Spawn( int index )
		{
			int entryCount = CountEntries( index );

			if ( Respawning || ( (bool) GroupSpawnList[index] && entryCount == 0 ) )
				FullSpawn( index );
			else
				SingleSpawn( index );
		}

		public void SingleSpawn( int index )
		{
			if ( (bool) GroupSpawnList[index] )
				return;

			int entryCount = CountEntries( index );

			if ( entryCount < (int) AmountList[index] )
				SpawnEntry( index );
		}

		public void FullSpawn( int index )
		{
			for ( int i = 0; i < (int) AmountList[index]; i++ )
				SpawnEntry( index );
		}

		public void CheckSpawnTime( int index, int time )
		{
			int beginTime = (int) BeginTimeBasedList[index];
			int endTime = (int) EndTimeBasedList[index];

			if ( endTime < beginTime )
			{
				if ( beginTime <= time && time <= 1439 )
					Spawn( index );
				else if ( time <= endTime )
					Spawn( index );
			}
			else
			{
				if ( beginTime <= time && time <= endTime )
					Spawn( index );
			}
		}

		public void CheckTimeExpire( int index, SpawnType st )
		{
			if ( !(bool) DespawnTimeExpireList[index] )
				return;

			switch ( st )
			{
				case SpawnType.GameTimeBased:
				{
					int UOHour, UOMinute, time;

					Server.Items.Clock.GetTime( Map, X, Y, out UOHour, out UOMinute );

					if ( UOHour == 24 )
						UOHour = 0;

					time = ( UOHour * 60 ) + UOMinute;

					CheckDespawnTime( index, time );

					break;
				}
				case SpawnType.RealTimeBased:
				{
					int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

					CheckDespawnTime( index, time );

					break;
				}
			}
		}

		public void CheckDespawnTime( int index, int time )
		{
			int beginTime = (int) BeginTimeBasedList[index];
			int endTime = (int) EndTimeBasedList[index];

			if ( endTime < beginTime )
			{
				if ( endTime < time && time < beginTime )
					DeleteEntries( index );
			}
			else
			{
				if ( endTime < time && time <= 1439 )
					DeleteEntries( index );
				else if ( time < beginTime )
					DeleteEntries( index );
			}
		}

		public bool EventSpawnAttempt( int index, SpawnType st )
		{
			return ( Active && (SpawnType) SpawnTypeList[index] == st && (bool) TriggerEventNowList[index] );
		}

		public bool IsFull( int index )
		{
			return CountEntries( index ) >= (int) AmountList[index];
		}

		public bool IsEmpty( string entryType )
		{
			for ( int i = 0; i < EntryList.Count; i++ )
			{
				if ( (string) EntryList[i] == entryType )
					return CountEntries( i ) == 0;
			}

			return false;
		}

		public bool IsTriggerBased( int index )
		{
			return ( (SpawnType) SpawnTypeList[index] == SpawnType.Proximity || (SpawnType) SpawnTypeList[index] == SpawnType.Speech );
		}

		public bool IsGroupSpawn( string entryType )
		{
			for ( int i = 0; i < EntryList.Count; i++ )
			{
				if ( (string) EntryList[i] == entryType )
					return (bool) GroupSpawnList[i];
			}

			return false;
		}

		public bool IsPersonalTimerOn{ get{ return MC.TimerTypeConfig != MSS.TimerType.Master; } }

		public bool IsNothingActive
		{
			get
			{
				bool active = false;

				for ( int i = 0; i < EntryList.Count; i++ )
				{
					if ( (bool) ActivatedList[i] )
					{
						active = true;

						break;
					}
				}

				return !active;
			}
		}

		public bool CheckEventAmbush( int index )
		{
			if ( CountEntries( index ) == 0 )
				return true;
			else
				return false;
		}

		public void Start()
		{
			if ( Active && !Workspace )
			{
				if ( IsPersonalTimerOn )
				{
					if ( megaSpawnerTimer != null )
						megaSpawnerTimer.Stop();

					megaSpawnerTimer = new MegaSpawnerTimer( this, MC.GetDelay( this ) );
					megaSpawnerTimer.Start();
				}
			}
		}

		public void Stop()
		{
			DeleteEntries();

			if ( megaSpawnerTimer != null && megaSpawnerTimer.Running )
				megaSpawnerTimer.Stop();
		}

		public int SaveAdjust( int index )
		{
			string entryType = (string) EntryList[index];

			if ( OverrideIndividualEntries )
			{
				if ( OverrideRespawnOnSave )
				{
					int loadHour = LoadTime.Hour * 3600;
					int loadMin = LoadTime.Minute * 60;
					int loadTotal = loadHour + loadMin + LoadTime.Second;

					int saveHour = SaveTime.Hour * 3600;
					int saveMin = SaveTime.Minute * 60;
					int saveTotal = saveHour + saveMin + SaveTime.Second;

					return loadTotal - saveTotal;
				}
			}
			else
			{
				ArrayList respawnEntryList = (ArrayList) RespawnEntryList[index];
				ArrayList respawnOnSaveList = (ArrayList) RespawnOnSaveList[index];

				for ( int i = 0; i < respawnEntryList.Count; i++ )
				{
					if ( respawnEntryList[i] is string )
					{
						if ( (string) respawnEntryList[i] == entryType )
						{
							if ( (bool) respawnOnSaveList[i] )
							{
								int loadHour = LoadTime.Hour * 3600;
								int loadMin = LoadTime.Minute * 60;
								int loadTotal = loadHour + loadMin + LoadTime.Second;

								int saveHour = SaveTime.Hour * 3600;
								int saveMin = SaveTime.Minute * 60;
								int saveTotal = saveHour + saveMin + SaveTime.Second;

								return loadTotal - saveTotal;
							}
						}
					}
				}
			}

			return 0;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 8 ); // version

			writer.Write( Active );
			writer.Write( Imported );
			writer.Write( ImportVersion );
			writer.Write( ContainerSpawn );
			writer.Write( Workspace );
			writer.Write( DateTime.Now );

			writer.Write( EntryList.Count );

			for ( int i = 0; i < EntryList.Count; i++ )
				writer.Write( (string) EntryList[i] );

			for ( int i = 0; i < SpawnRangeList.Count; i++ )
				writer.Write( (int) SpawnRangeList[i] );

			for ( int i = 0; i < WalkRangeList.Count; i++ )
				writer.Write( (int) WalkRangeList[i] );

			for ( int i = 0; i < AmountList.Count; i++ )
				writer.Write( (int) AmountList[i] );

			for ( int i = 0; i < MinDelayList.Count; i++ )
				writer.Write( (int) MinDelayList[i] );

			for ( int i = 0; i < MaxDelayList.Count; i++ )
				writer.Write( (int) MaxDelayList[i] );

			for ( int i = 0; i < SpawnTypeList.Count; i++ )
				writer.Write( (int) SpawnTypeList[i] );

			for ( int i = 0; i < ActivatedList.Count; i++ )
				writer.Write( (bool) ActivatedList[i] );

			for ( int i = 0; i < EventRangeList.Count; i++ )
				writer.Write( (int) EventRangeList[i] );

			for ( int i = 0; i < EventKeywordList.Count; i++ )
				writer.Write( (string) EventKeywordList[i] );

			for ( int i = 0; i < KeywordCaseSensitiveList.Count; i++ )
				writer.Write( (bool) KeywordCaseSensitiveList[i] );

			for ( int i = 0; i < TriggerEventNowList.Count; i++ )
				writer.Write( (bool) TriggerEventNowList[i] );

			for ( int i = 0; i < EventAmbushList.Count; i++ )
				writer.Write( (bool) EventAmbushList[i] );

			for ( int i = 0; i < BeginTimeBasedList.Count; i++ )
				writer.Write( (int) BeginTimeBasedList[i] );

			for ( int i = 0; i < EndTimeBasedList.Count; i++ )
				writer.Write( (int) EndTimeBasedList[i] );

			for ( int i = 0; i < GroupSpawnList.Count; i++ )
				writer.Write( (bool) GroupSpawnList[i] );

			for ( int i = 0; i < MinStackAmountList.Count; i++ )
				writer.Write( (int) MinStackAmountList[i] );

			for ( int i = 0; i < MaxStackAmountList.Count; i++ )
				writer.Write( (int) MaxStackAmountList[i] );

			for ( int i = 0; i < MovableList.Count; i++ )
				writer.Write( (bool) MovableList[i] );

			for ( int i = 0; i < MinDespawnList.Count; i++ )
				writer.Write( (int) MinDespawnList[i] );

			for ( int i = 0; i < MaxDespawnList.Count; i++ )
				writer.Write( (int) MaxDespawnList[i] );

			for ( int i = 0; i < DespawnList.Count; i++ )
				writer.Write( (bool) DespawnList[i] );

			for ( int i = 0; i < DespawnGroupList.Count; i++ )
				writer.Write( (bool) DespawnGroupList[i] );

			for ( int i = 0; i < DespawnTimeExpireList.Count; i++ )
				writer.Write( (bool) DespawnTimeExpireList[i] );

			writer.Write( SpawnedEntries.Count );

			for ( int i = 0; i < SpawnedEntries.Count; i++ )
			{
				ArrayList spawnedEntries = (ArrayList) SpawnedEntries[i];

				writer.Write( spawnedEntries.Count );

				for ( int j = 0; j < spawnedEntries.Count; j++ )
				{
					object o = spawnedEntries[j];

					if ( o is Item )
						writer.Write( (Item) o );
					else if ( o is Mobile )
						writer.Write( (Mobile) o );
					else
						writer.Write( Serial.MinusOne );
				}
			}

			writer.Write( OverrideSpawnedEntries.Count );

			for ( int i = 0; i < OverrideSpawnedEntries.Count; i++ )
			{
				object o = OverrideSpawnedEntries[i];

				if ( o is Item )
					writer.Write( (Item) o );
				else if ( o is Mobile )
					writer.Write( (Mobile) o );
				else
					writer.Write( Serial.MinusOne );
			}

			writer.Write( RespawnEntryList.Count );

			for ( int i = 0; i < RespawnEntryList.Count; i++ )
			{
				ArrayList respawnEntryList = (ArrayList) RespawnEntryList[i];

				writer.Write( respawnEntryList.Count );

				for ( int j = 0; j < respawnEntryList.Count; j++ )
				{
					if ( respawnEntryList[j] is string )
						writer.Write( 1 ); // Is string
					else
						writer.Write( 2 ); // Is Entry
				}
			}

			for ( int i = 0; i < RespawnEntryList.Count; i++ )
			{
				ArrayList respawnEntryList = (ArrayList) RespawnEntryList[i];

				writer.Write( respawnEntryList.Count );

				for ( int j = 0; j < respawnEntryList.Count; j++ )
				{
					if ( respawnEntryList[j] is string )
					{
						writer.Write( (string) respawnEntryList[j] );
					}
					else
					{
						object o = respawnEntryList[j];

						if ( o is Item )
							writer.Write( (Item) o );
						else if ( o is Mobile )
							writer.Write( (Mobile) o );
						else
							writer.Write( Serial.MinusOne );
					}
				}
			}

			for ( int i = 0; i < RespawnTimeList.Count; i++ )
			{
				ArrayList respawnTimeList = (ArrayList) RespawnTimeList[i];

				writer.Write( respawnTimeList.Count );

				for ( int j = 0; j < respawnTimeList.Count; j++ )
					writer.Write( (int) respawnTimeList[j] );
			}

			for ( int i = 0; i < SpawnCounterList.Count; i++ )
			{
				ArrayList spawnCounterList = (ArrayList) SpawnCounterList[i];

				writer.Write( spawnCounterList.Count );

				for ( int j = 0; j < spawnCounterList.Count; j++ )
					writer.Write( (DateTime) spawnCounterList[j] );
			}

			for ( int i = 0; i < SpawnTimeList.Count; i++ )
			{
				ArrayList spawnTimeList = (ArrayList) SpawnTimeList[i];

				writer.Write( spawnTimeList.Count );

				for ( int j = 0; j < spawnTimeList.Count; j++ )
					writer.Write( (int) spawnTimeList[j] );
			}

			for ( int i = 0; i < RespawnOnSaveList.Count; i++ )
			{
				ArrayList respawnTimeList = (ArrayList) RespawnTimeList[i];
				ArrayList respawnOnSaveList = (ArrayList) RespawnOnSaveList[i];

				writer.Write( respawnOnSaveList.Count );

				for ( int j = 0; j < respawnOnSaveList.Count; j++ )
				{
					if ( (int) respawnTimeList[j] > tickOffset )
						writer.Write( (bool) true );
					else
						writer.Write( (bool) false );
				}
			}

			for ( int i = 0; i < DespawnTimeList.Count; i++ )
			{
				ArrayList despawnTimeList = (ArrayList) DespawnTimeList[i];

				writer.Write( despawnTimeList.Count );

				for ( int j = 0; j < despawnTimeList.Count; j++ )
					writer.Write( (int) despawnTimeList[j] );
			}

			writer.Write( OverrideRespawnEntryList.Count );

			for ( int i = 0; i < OverrideRespawnEntryList.Count; i++ )
			{
				if ( OverrideRespawnEntryList[i] is string )
					writer.Write( 1 ); // Is string
				else if ( OverrideRespawnEntryList[i] is Item || OverrideRespawnEntryList[i] is Mobile )
					writer.Write( 2 ); // Is Entry
			}

			for ( int i = 0; i < OverrideRespawnEntryList.Count; i++ )
			{
				if ( OverrideRespawnEntryList[i] is string )
				{
					writer.Write( (string) OverrideRespawnEntryList[i] );
				}
				else
				{
					object o = OverrideRespawnEntryList[i];

					if ( o is Item )
						writer.Write( (Item) o );
					else if ( o is Mobile )
						writer.Write( (Mobile) o );
					else
						writer.Write( Serial.MinusOne );
				}
			}

			for ( int i = 0; i < OverrideRespawnTimeList.Count; i++ )
				writer.Write( (int) OverrideRespawnTimeList[i] );

			for ( int i = 0; i < OverrideSpawnCounterList.Count; i++ )
				writer.Write( (DateTime) OverrideSpawnCounterList[i] );

			bool respawnOnSave = false;

			for ( int i = 0; i < OverrideSpawnTimeList.Count; i++ )
			{
				writer.Write( (int) OverrideSpawnTimeList[i] );

				if ( (int) OverrideSpawnTimeList[i] > tickOffset )
					respawnOnSave = true;
			}

			writer.Write( respawnOnSave );

			for ( int i = 0; i < OverrideDespawnTimeList.Count; i++ )
				writer.Write( (int) OverrideDespawnTimeList[i] );

			writer.Write( SettingsList.Count );

			for ( int i = 0; i < SettingsList.Count; i++ )
			{
				if ( SettingsList[i] is ArrayList )
				{
					ArrayList setting = (ArrayList) SettingsList[i];

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
				else
				{
					SettingsList.RemoveAt( i );
					i--;
				}
			}
		}

		public override void Deserialize( GenericReader reader )
		{
			MC.CheckCommandLineArgs();

			base.Deserialize( reader );

			bool DummyBool;
            int DummyInt;
            DateTime DummyDateTime;
            string DummyString;

			int version = reader.ReadInt();

			if ( version < 4 && MC.GetProcess != Process.ConvertPre3_2 )
				MC.SetProcess( Process.ConvertPre3_2 );

			Active = reader.ReadBool();

			if ( version >= 3 )
			{
				Imported = reader.ReadString();
				ImportVersion = reader.ReadString();
				ContainerSpawn = (Container) reader.ReadItem();
				Workspace = reader.ReadBool();
				SaveTime = reader.ReadDateTime();
			}
			else
			{
				Imported = "";
				ImportVersion = "";
				ContainerSpawn = null;
				Workspace = false;
				SaveTime = DateTime.Now;
			}

			if ( version <= 2 )
			{
				DummyBool = reader.ReadBool();

				if ( DummyBool )
				{
					Imported = "pre v3 file import.msf";
					ImportVersion = "pre v3";
				}
			}

			if ( version == 2 )
				DummyBool = reader.ReadBool();

			int count = reader.ReadInt();

			for ( int i = 0; i < count; i++ )
				EntryList.Add( reader.ReadString() );

			for ( int i = 0; i < count; i++ )
				SpawnRangeList.Add( reader.ReadInt() );

			for ( int i = 0; i < count; i++ )
				WalkRangeList.Add( reader.ReadInt() );

			for ( int i = 0; i < count; i++ )
				AmountList.Add( reader.ReadInt() );

			for ( int i = 0; i < count; i++ )
				MinDelayList.Add( reader.ReadInt() );

			for ( int i = 0; i < count; i++ )
				MaxDelayList.Add( reader.ReadInt() );

			if ( version <= 2 )
			{
				for ( int i = 0; i < count; i++ )
					DummyInt = reader.ReadInt();
			}

			if ( version >= 2 )
			{
				if ( version == 2 )
				{
					for ( int i = 0; i < count; i++ )
						DummyInt = reader.ReadInt();
				}

				for ( int i = 0; i < count; i++ )
					SpawnTypeList.Add( reader.ReadInt() );

				for ( int i = 0; i < count; i++ )
					ActivatedList.Add( reader.ReadBool() );

				for ( int i = 0; i < count; i++ )
					EventRangeList.Add( reader.ReadInt() );

				for ( int i = 0; i < count; i++ )
					EventKeywordList.Add( reader.ReadString() );

				if ( version >= 3 )
				{
					for ( int i = 0; i < count; i++ )
						KeywordCaseSensitiveList.Add( reader.ReadBool() );
				}
				else
				{
					for ( int i = 0; i < count; i++ )
						KeywordCaseSensitiveList.Add( (bool) false );
				}

				for ( int i = 0; i < count; i++ )
					TriggerEventNowList.Add( reader.ReadBool() );

				for ( int i = 0; i < count; i++ )
					EventAmbushList.Add( reader.ReadBool() );

				for ( int i = 0; i < count; i++ )
					BeginTimeBasedList.Add( reader.ReadInt() );

				for ( int i = 0; i < count; i++ )
					EndTimeBasedList.Add( reader.ReadInt() );

				for ( int i = 0; i < count; i++ )
					GroupSpawnList.Add( reader.ReadBool() );

				if ( version >= 3 )
				{
					for ( int i = 0; i < count; i++ )
						MinStackAmountList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						MaxStackAmountList.Add( reader.ReadInt() );

					for ( int i = 0; i < count; i++ )
						MovableList.Add( reader.ReadBool() );

					if ( version >= 7 )
					{
						for ( int i = 0; i < count; i++ )
							MinDespawnList.Add( reader.ReadInt() );

						for ( int i = 0; i < count; i++ )
							MaxDespawnList.Add( reader.ReadInt() );

						for ( int i = 0; i < count; i++ )
							DespawnList.Add( reader.ReadBool() );

						for ( int i = 0; i < count; i++ )
							DespawnGroupList.Add( reader.ReadBool() );

						for ( int i = 0; i < count; i++ )
							DespawnTimeExpireList.Add( reader.ReadBool() );
					}
					else
					{
						for ( int i = 0; i < count; i++ )
							MinDespawnList.Add( 1800 );

						for ( int i = 0; i < count; i++ )
							MaxDespawnList.Add( 3600 );

						for ( int i = 0; i < count; i++ )
							DespawnList.Add( (bool) false );

						for ( int i = 0; i < count; i++ )
							DespawnGroupList.Add( (bool) false );

						for ( int i = 0; i < count; i++ )
							DespawnTimeExpireList.Add( (bool) true );
					}
				}
				else
				{
					for ( int i = 0; i < count; i++ )
						MinStackAmountList.Add( 1 );

					for ( int i = 0; i < count; i++ )
						MaxStackAmountList.Add( 1 );

					for ( int i = 0; i < count; i++ )
						MovableList.Add( (bool) true );

					for ( int i = 0; i < count; i++ )
						MinDespawnList.Add( 1800 );

					for ( int i = 0; i < count; i++ )
						MaxDespawnList.Add( 3600 );

					for ( int i = 0; i < count; i++ )
						DespawnList.Add( (bool) false );

					for ( int i = 0; i < count; i++ )
						DespawnGroupList.Add( (bool) false );

					for ( int i = 0; i < count; i++ )
						DespawnTimeExpireList.Add( (bool) true );
				}
			}
			else
			{
				for ( int i = 0; i < count; i++ )
					SpawnTypeList.Add( SpawnType.Regular );

				for ( int i = 0; i < count; i++ )
					ActivatedList.Add( (bool) true );

				for ( int i = 0; i < count; i++ )
					EventRangeList.Add( 10 );

				for ( int i = 0; i < count; i++ )
					EventKeywordList.Add( "" );

				for ( int i = 0; i < count; i++ )
					KeywordCaseSensitiveList.Add( (bool) false );

				for ( int i = 0; i < count; i++ )
					TriggerEventNowList.Add( (bool) true );

				for ( int i = 0; i < count; i++ )
					EventAmbushList.Add( (bool) true );

				for ( int i = 0; i < count; i++ )
					BeginTimeBasedList.Add( 0 );

				for ( int i = 0; i < count; i++ )
					EndTimeBasedList.Add( 0 );

				for ( int i = 0; i < count; i++ )
					GroupSpawnList.Add( (bool) false );

				for ( int i = 0; i < count; i++ )
					MinStackAmountList.Add( 1 );

				for ( int i = 0; i < count; i++ )
					MaxStackAmountList.Add( 1 );

				for ( int i = 0; i < count; i++ )
					MovableList.Add( (bool) true );

				for ( int i = 0; i < count; i++ )
					MinDespawnList.Add( 1800 );

				for ( int i = 0; i < count; i++ )
					MaxDespawnList.Add( 3600 );

				for ( int i = 0; i < count; i++ )
					DespawnList.Add( (bool) false );

				for ( int i = 0; i < count; i++ )
					DespawnGroupList.Add( (bool) false );

				for ( int i = 0; i < count; i++ )
					DespawnTimeExpireList.Add( (bool) true );
			}

			int entryCount = reader.ReadInt();
			int overrideEntryCount = 0;

			if ( version <= 3 )
			{
				for ( int i = 0; i < entryCount; i++ )
				{
					IEntity e = World.FindEntity( reader.ReadInt() );

					if ( e != null )
					{
						if ( e is Item )
							( (Item) e ).Delete();
						else if ( e is Mobile )
							( (Mobile) e ).Delete();
					}
	
					SpawnedEntries.Add( new ArrayList() );
					LastMovedList.Add( new ArrayList() );
				}

				entryCount = 0;
			}
			else
			{
				for ( int i = 0; i < entryCount; i++ )
				{
					ArrayList spawnedEntries = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
					{
						IEntity e = World.FindEntity( reader.ReadInt() );

						if ( e != null )
							spawnedEntries.Add( e );
					}

					SpawnedEntries.Add( spawnedEntries );
				}

				overrideEntryCount = reader.ReadInt();

				for ( int i = 0; i < overrideEntryCount; i++ )
				{
					IEntity e = World.FindEntity( reader.ReadInt() );

					if ( e != null )
						OverrideSpawnedEntries.Add( e );
				}

				overrideEntryCount = OverrideSpawnedEntries.Count;
			}

			if ( version == 3 )
			{
				int respawnCount = reader.ReadInt();

				ArrayList BullshitList = new ArrayList();

				for ( int i = 0; i < respawnCount; i++ )
					BullshitList.Add( reader.ReadInt() );

				for ( int i = 0; i < respawnCount; i++ )
				{
					switch ( (int) BullshitList[i] )
					{
						case 1: // Is string
						{
							DummyString = reader.ReadString();

							break;
						}
						case 2: // Is Entry
						{
							DummyInt = reader.ReadInt();

							break;
						}
					}
				}

				for ( int i = 0; i < respawnCount; i++ )
					DummyInt = reader.ReadInt();

				for ( int i = 0; i < respawnCount; i++ )
					DummyDateTime = reader.ReadDateTime();

				for ( int i = 0; i < respawnCount; i++ )
					DummyInt = reader.ReadInt();

				for ( int i = 0; i < respawnCount; i++ )
					DummyBool = reader.ReadBool();
			}
			else if ( version >= 4 )
			{
				int respawnCount = reader.ReadInt();

				ArrayList RespawnEntryReadHelp = new ArrayList();

				for ( int i = 0; i < respawnCount; i++ )
				{
					ArrayList respawnEntryReadHelp = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
						respawnEntryReadHelp.Add( reader.ReadInt() );

					RespawnEntryReadHelp.Add( respawnEntryReadHelp );
				}

				for ( int i = 0; i < respawnCount; i++ )
				{
					ArrayList respawnEntryReadHelp = (ArrayList) RespawnEntryReadHelp[i];
					ArrayList respawnEntryList = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
					{
						switch ( (int) respawnEntryReadHelp[j] )
						{
							case 1: // Is string
							{
								respawnEntryList.Add( reader.ReadString() );

								break;
							}
							case 2: // Is Entry
							{
								IEntity e = World.FindEntity( reader.ReadInt() );

								if ( e != null )
									respawnEntryList.Add( e );
								else
									respawnEntryList.Add( "" );

								break;
							}
						}
					}

					RespawnEntryList.Add( respawnEntryList );
				}

				for ( int i = 0; i < respawnCount; i++ )
				{
					ArrayList respawnTimeList = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
						respawnTimeList.Add( reader.ReadInt() );

					RespawnTimeList.Add( respawnTimeList );
				}

				for ( int i = 0; i < respawnCount; i++ )
				{
					ArrayList spawnCounterList = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
						spawnCounterList.Add( reader.ReadDateTime() );

					SpawnCounterList.Add( spawnCounterList );
				}

				for ( int i = 0; i < respawnCount; i++ )
				{
					ArrayList spawnTimeList = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
						spawnTimeList.Add( reader.ReadInt() );

					SpawnTimeList.Add( spawnTimeList );
				}

				for ( int i = 0; i < respawnCount; i++ )
				{
					ArrayList respawnOnSaveList = new ArrayList();

					int icount = reader.ReadInt();

					for ( int j = 0; j < icount; j++ )
						respawnOnSaveList.Add( reader.ReadBool() );

					RespawnOnSaveList.Add( respawnOnSaveList );
				}

				if ( version >= 7 )
				{
					for ( int i = 0; i < respawnCount; i++ )
					{
						ArrayList despawnTimeList = new ArrayList();

						int icount = reader.ReadInt();

						for ( int j = 0; j < icount; j++ )
							despawnTimeList.Add( reader.ReadInt() );

						DespawnTimeList.Add( despawnTimeList );
					}
				}
				else
				{
					for ( int i = 0; i < respawnCount; i++ )
					{
						ArrayList respawnTimeList = (ArrayList) RespawnTimeList[i];
						ArrayList despawnTimeList = new ArrayList();

						int icount = respawnTimeList.Count;

						for ( int j = 0; j < icount; j++ )
							despawnTimeList.Add( 0 );

						DespawnTimeList.Add( despawnTimeList );
					}
				}
			}

			if ( version >= 3 )
			{
				int overrideRespawnCount = reader.ReadInt();

				ArrayList OverrideRespawnEntryReadHelp = new ArrayList();

				for ( int i = 0; i < overrideRespawnCount; i++ )
					OverrideRespawnEntryReadHelp.Add( reader.ReadInt() );

				for ( int i = 0; i < overrideRespawnCount; i++ )
				{
					switch ( (int) OverrideRespawnEntryReadHelp[i] )
					{
						case 1: // Is string
						{
							OverrideRespawnEntryList.Add( reader.ReadString() );

							break;
						}
						case 2: // Is Entry
						{
							IEntity e = World.FindEntity( reader.ReadInt() );

							if ( e != null )
								OverrideRespawnEntryList.Add( e );
							else
								OverrideRespawnEntryList.Add( "" );

							break;
						}
					}
				}

				if ( version <= 5 )
					MegaSpawnerOverride.CheckDupedEntries( this );

				for ( int i = 0; i < overrideRespawnCount; i++ )
					OverrideRespawnTimeList.Add( reader.ReadInt() );

				for ( int i = 0; i < overrideRespawnCount; i++ )
					OverrideSpawnCounterList.Add( reader.ReadDateTime() );

				for ( int i = 0; i < overrideRespawnCount; i++ )
					OverrideSpawnTimeList.Add( reader.ReadInt() );

				if ( version >= 4 )
					OverrideRespawnOnSave = reader.ReadBool();

				if ( version >= 7 )
				{
					for ( int i = 0; i < overrideRespawnCount; i++ )
						OverrideDespawnTimeList.Add( reader.ReadInt() );
				}
				else
				{
					for ( int i = 0; i < overrideRespawnCount; i++ )
						OverrideDespawnTimeList.Add( 0 );
				}

				int settingsCount = reader.ReadInt();

				if ( version >= 8 )
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

						SettingsList.Add( List );
					}
				}
				else
				{
					for ( int i = 0; i < settingsCount; i++ )
						SettingsList.Add( reader.ReadString() );

					ConvertOldSettings();
				}
			}

// ****** Non-Serialized *****************

			if ( version <= 3 )
			{
				RespawnEntryList.Clear();
				RespawnTimeList.Clear();
				SpawnCounterList.Clear();
				SpawnTimeList.Clear();
				RespawnOnSaveList.Clear();
				OverrideRespawnEntryList.Clear();
				OverrideRespawnTimeList.Clear();
				OverrideSpawnCounterList.Clear();
				OverrideSpawnTimeList.Clear();
				OverrideDespawnTimeList.Clear();

				for ( int i = 0; i < EntryList.Count; i++ )
				{
					if ( OverrideIndividualEntries )
					{
						for ( int j = 0; j < OverrideAmount; j++ )
						{
							OverrideRespawnEntryList.Add( "" );
							OverrideRespawnTimeList.Add( 0 );
							OverrideSpawnCounterList.Add( DateTime.Now );
							OverrideSpawnTimeList.Add( 0 );
							OverrideDespawnTimeList.Add( 0 );
						}
					}
					else
					{
						for ( int j = 0; j < EntryList.Count; j++ )
						{
							string entryType = (string) EntryList[j];
							int amount = (int) AmountList[j];

							ArrayList respawnEntryList = new ArrayList();
							ArrayList respawnTimeList = new ArrayList();
							ArrayList spawnCounterList = new ArrayList();
							ArrayList spawnTimeList = new ArrayList();
							ArrayList respawnOnSaveList = new ArrayList();
							ArrayList despawnTimeList = new ArrayList();

							for ( int k = 0; k < amount; k++ )
							{
								respawnEntryList.Add( entryType );
								respawnTimeList.Add( 0 );
								spawnCounterList.Add( DateTime.Now );
								spawnTimeList.Add( 0 );
								respawnOnSaveList.Add( (bool) false );
								despawnTimeList.Add( 0 );
							}

							SpawnedEntries.Add( new ArrayList() );
							LastMovedList.Add( new ArrayList() );
							RespawnEntryList.Add( respawnEntryList );
							RespawnTimeList.Add( respawnTimeList );
							SpawnCounterList.Add( spawnCounterList );
							SpawnTimeList.Add( spawnTimeList );
							RespawnOnSaveList.Add( respawnOnSaveList );
							DespawnTimeList.Add( despawnTimeList );
						}
					}
				}

				if ( !MC.CA_Disabled )
					Respawn();
			}

			for ( int i = 0; i < entryCount; i++ )
			{
				ArrayList spawnedEntries = (ArrayList) SpawnedEntries[i];
				ArrayList lastMovedList = new ArrayList();

				for ( int j = 0; j < spawnedEntries.Count; j++ )
				{
					if ( spawnedEntries[j] is Item )
						lastMovedList.Add( 0 );
					else if ( spawnedEntries[j] is Mobile )
						lastMovedList.Add( DateTime.Now );
				}

				LastMovedList.Add( lastMovedList );
			}

			for ( int i = 0; i < overrideEntryCount; i++ )
			{
				if ( OverrideSpawnedEntries[i] is Item )
					OverrideLastMovedList.Add( 0 );
				else if ( OverrideSpawnedEntries[i] is Mobile )
					OverrideLastMovedList.Add( DateTime.Now );
			}

// ****** End Of Non-Serialized **********

			if ( Workspace || MC.CA_WipeAll )
			{
				Delete();
			}
			else
			{
				if ( MC.CA_Disabled )
					Active = false;
				else
					Start();

				MC.SpawnerList.Add( this );

				if ( Imported != "" )
					MC.FileImportAdd( Imported, ImportVersion );

				CompileSettings();
				CheckEntryErrors();
				CheckListDiscrepancies();

				SettingsList.Sort( new MC.SettingsSorter() );

				LoadTime = DateTime.Now;
			}
		}

		private class MegaSpawnerTimer : Timer
		{ 
			private MegaSpawner megaSpawner;
			private int delay;

			public MegaSpawnerTimer( MegaSpawner megaspawner, int timerdelay ) : base( TimeSpan.FromSeconds( timerdelay ) )
			{
				Priority = TimerPriority.OneSecond;
				
				megaSpawner = megaspawner;
				delay = timerdelay;
			}

			protected override void OnTick()
			{
				if ( !megaSpawner.Active || megaSpawner.Deleted || megaSpawner.Map == null || megaSpawner.Map == Map.Internal || !megaSpawner.IsPersonalTimerOn || megaSpawner.Workspace )
				{
					Stop();

					return;
				}

				megaSpawner.TimerTick( delay );
			}
		}

		private class DelayWipeTimer : Timer
		{ 
			public DelayWipeTimer() : base( TimeSpan.FromSeconds( 5 ) )
			{
			}

			protected override void OnTick()
			{
				foreach ( MegaSpawner megaSpawner in MC.SpawnerList )
					megaSpawner.DeleteEntries();
			}
		}

		public override void OnMovement( Mobile mobile, Point3D oldLocation )
		{
			if ( !Active || !mobile.Alive || !mobile.Player || ( !OverrideIndividualEntries && NoProximityType ) || ( !MC.StaffTriggerEvent && mobile.AccessLevel > AccessLevel.Player ) )
				return;

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.OnMovement( this, mobile, oldLocation );

				return;
			}

			for ( int i = 0; i < EntryList.Count; i++ )
			{
				if ( EventSpawnAttempt( i, SpawnType.Proximity ) )
				{
//					if ( Utility.InRange( Location, mobile.Location, (int) EventRangeList[i] ) )
					if ( Utility.InRange( Location, mobile.Location, (int) 100) )

					{
						int entryCount = CountEntries( i );

						if ( (bool) GroupSpawnList[i] && entryCount != 0 )
							break;

						if ( ( (bool) GroupSpawnList[i] && entryCount == 0 ) || CheckEventAmbush( i ) )
							FullSpawn( i );
						else
							SingleSpawn( i );
					}

					TriggerEventNowList[i] = (bool) false;
				}
			}
		}

		public override void OnSpeech( SpeechEventArgs e )
		{
			Mobile mobile = e.Mobile;

			if ( !Active || !mobile.Alive || !mobile.Player || ( !OverrideIndividualEntries && NoSpeechType ) || ( !MC.StaffTriggerEvent && mobile.AccessLevel > AccessLevel.Player ) )
				return;

			if ( OverrideIndividualEntries )
			{
				MegaSpawnerOverride.OnSpeech( this, e );

				return;
			}

			for ( int i = 0; i < EntryList.Count; i++ )
			{
				if ( EventSpawnAttempt( i, SpawnType.Speech ) )
				{
					if ( Utility.InRange( Location, mobile.Location, (int) EventRangeList[i] ) )
					{
						int entryCount = CountEntries( i );

						if ( (bool) KeywordCaseSensitiveList[i] && (string) EventKeywordList[i] != e.Speech )
							break;
						else if ( ( (string) EventKeywordList[i] ).ToLower() != e.Speech.ToLower() )
							break;

						if ( (bool) GroupSpawnList[i] && entryCount != 0 )
							break;

						if ( ( (bool) GroupSpawnList[i] && entryCount == 0 ) || CheckEventAmbush( i ) )
							FullSpawn( i );
						else
							SingleSpawn( i );
					}

					TriggerEventNowList[i] = (bool) false;
				}
			}
		}
	}
}
