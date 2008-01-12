using System;
using System.Reflection;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmConvertSpawnersGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList HideSpawnerList = new ArrayList();
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList CUGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();
		private ArrayList ConvertSpawnersList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private Mobile gumpMobile;

		public ConfirmConvertSpawnersGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
		{
			gumpMobile = mobile;

			Select = select;
			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 380, 90 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 340, 20 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 230, 340, 20 );

			switch ( Select )
			{
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Converting All Spawners" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Converting Selected Spawners" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to convert all spawners?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to convert selected spawners?" ), false, false ); break; }
			}

			AddHtml( 491, 231, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 450, 230, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 231, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 230, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					switch ( Select )
					{
						case 1:	{ MessagesTitle = "Convert All Spawners"; Messages = "You have chosen not to convert all Mega Spawners."; break; }
						case 2:	{ MessagesTitle = "Convert Selected Spawners"; Messages = "You have chosen not to convert all selected Mega Spawners."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new ConversionUtilityGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Convert Spawners
				{
					if ( CheckProcess() )
						break;

					if ( Select == 2 )
					{
						for ( int i = 0; i < ConvertSpawnersList.Count; i++ )
						{
							if ( !(bool) CUGCheckBoxesList[i] )
							{
								ConvertSpawnersList.RemoveAt( i );
								CUGCheckBoxesList.RemoveAt( i );
								i--;
							}
						}
					}

					int delay = 30;

					new ConvertSpawnersTimer( ArgsList, gumpMobile, Select, false, delay ).Start();

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[6] = HideSpawnerList;
			ArgsList[13] = MSGCheckBoxesList;
			ArgsList[16] = CUGCheckBoxesList;
			ArgsList[33] = ConvertSpawnersList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			HideSpawnerList = (ArrayList) 					ArgsList[6];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			CUGCheckBoxesList = (ArrayList)					ArgsList[16];
			PersonalConfigList = (ArrayList)				ArgsList[28];
			ConvertSpawnersList = (ArrayList)				ArgsList[33];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)				PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
		}

		private bool CheckProcess()
		{
			string msgsTitle, msgs;

			if ( MC.CheckProcess( out msgsTitle, out msgs ) )
			{
				if ( msgsTitle != null )
					MessagesTitle = msgsTitle;

				if ( msgs != null )
					Messages = msgs;

				SetArgsList();

				gumpMobile.SendGump( new ConversionUtilityGump( gumpMobile, ArgsList ) );
			}

			return MC.CheckProcess();
		}

		private class ConvertSpawnersTimer : Timer
		{ 
			private ArrayList ArgsList = new ArrayList();

			private ArrayList HideSpawnerList = new ArrayList();
			private ArrayList MSGCheckBoxesList = new ArrayList();
			private ArrayList CUGCheckBoxesList = new ArrayList();
			private ArrayList ConvertSpawnersList = new ArrayList();

			private string MessagesTitle, Messages;

			private int count = 0;
			private DateTime beginTime;
			private TimeSpan finishTime;

			private int Select;
			private Mobile gumpMobile;
			private bool fullForce;

			public ConvertSpawnersTimer( ArrayList argsList, Mobile mobile, int select, bool fullforce, int delay ) : base( TimeSpan.Zero, TimeSpan.FromMilliseconds( delay ) )
			{
				gumpMobile = mobile;
				Select = select;
				fullForce = fullforce;

				ArgsList = argsList;
				GetArgsList();
			}

			protected override void OnTick()
			{
				if ( count == 0 )
				{
					beginTime = (DateTime) DateTime.Now;

					gumpMobile.SendMessage( "Converting {0} Spawners... Please wait...", Select == 1 ? "All" : "Selected" );
				}

				Item item = null;

				if ( !fullForce )
				{
					item = (Item) ConvertSpawnersList[count];

					if ( item is Spawner && !item.Deleted )
						ConvertDistroSpawner( item );
				}
				else if ( count == 0 )
				{
					for ( int i = 0; i < ConvertSpawnersList.Count; i++ )
					{
						item = (Item) ConvertSpawnersList[i];

						if ( item is Spawner && !item.Deleted )
							ConvertDistroSpawner( item );
					}

					count = ConvertSpawnersList.Count;
				}

				count++;

				if ( count >= ConvertSpawnersList.Count )
				{
					Stop();

					finishTime = DateTime.Now - beginTime;

					MessagesTitle = String.Format( "Convert {0} Spawners", Select == 1 ? "All" : "Selected" );
					Messages = String.Format( "{0} spawner{1} been converted to Mega Spawner. The process took {2} second{3}.", count, count == 1 ? " has" : "s have", (int) finishTime.TotalSeconds, (int) finishTime.TotalSeconds == 1 ? "" : "s" );

					SetArgsList();

					ArgsList[34] = (bool) true;				// RefreshSpawnerLists

					gumpMobile.SendGump( new ConversionUtilityGump( gumpMobile, ArgsList ) );
				}
			}

			private void ConvertDistroSpawner( Item item )
			{
				MegaSpawner megaSpawner = new MegaSpawner();
				Spawner spawner = (Spawner) item;
				Type type = spawner.GetType();
				PropertyInfo[] props = type.GetProperties( BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public );

				int count = spawner.CreaturesName.Count;
				int walkRange = spawner.HomeRange;

				foreach( PropertyInfo prop in props )
				{
					if ( prop.Name == "WalkingRange" )
					{
						walkRange = Convert.ToInt32( prop.GetValue( spawner, null ) );

						break;
					}
				}

				if ( spawner.Name != "Spawner" )
					megaSpawner.Name = spawner.Name;

				int calcMinDelay=0, calcMaxDelay=0;

				for( int i = 0; i < count; i++ )
				{
					int entryCount = 0;

					for ( int j = 0; j < megaSpawner.EntryList.Count; j++ )
					{
						string entryType = (string) megaSpawner.EntryList[j];

						if ( entryType.ToLower() == ( (string) spawner.CreaturesName[i] ).ToLower() )
							entryCount++;
					}

					if ( entryCount == 0 )
					{
						bool movable = true;

						megaSpawner.EntryList.Add( (string) spawner.CreaturesName[i] );

						megaSpawner.SpawnRangeList.Add( spawner.HomeRange );
						megaSpawner.WalkRangeList.Add( walkRange );

						if ( count == 1 )
							megaSpawner.AmountList.Add( (int) ( spawner.Count ) );
						else if ( ( (int) spawner.Count / count ) == (int) ( (double) spawner.Count / (double) count ) )
							megaSpawner.AmountList.Add( (int) ( spawner.Count / count ) );
						else
							megaSpawner.AmountList.Add( (int) ( spawner.Count / count ) + 1 );

						calcMinDelay = ( spawner.MinDelay.Hours * 3600 ) + ( spawner.MinDelay.Minutes * 60 ) + spawner.MinDelay.Seconds;
						calcMaxDelay = ( spawner.MaxDelay.Hours * 3600 ) + ( spawner.MaxDelay.Minutes * 60 ) + spawner.MaxDelay.Seconds;

						Type entryType = ScriptCompiler.FindTypeByName( (string) spawner.CreaturesName[i] );

						if ( entryType != null )
						{
							Item toAdd = null;

							try
							{
								toAdd = (Item) Activator.CreateInstance( entryType );
								movable = toAdd.Movable;
							}
							catch{}

							if ( toAdd != null )
								toAdd.Delete();
						}

						megaSpawner.MinDelayList.Add( calcMinDelay );
						megaSpawner.MaxDelayList.Add( calcMaxDelay );
						megaSpawner.SpawnTypeList.Add( SpawnType.Regular );
						megaSpawner.ActivatedList.Add( (bool) true );
						megaSpawner.EventRangeList.Add( 10 );
						megaSpawner.EventKeywordList.Add( "" );
						megaSpawner.KeywordCaseSensitiveList.Add( (bool) false );
						megaSpawner.TriggerEventNowList.Add( (bool) true );
						megaSpawner.EventAmbushList.Add( (bool) true );
						megaSpawner.BeginTimeBasedList.Add( 0 );
						megaSpawner.EndTimeBasedList.Add( 0 );
						megaSpawner.GroupSpawnList.Add( spawner.Group );
						megaSpawner.MinStackAmountList.Add( 1 );
						megaSpawner.MaxStackAmountList.Add( 1 );
						megaSpawner.MovableList.Add( movable );
						megaSpawner.MinDespawnList.Add( 1800 );
						megaSpawner.MaxDespawnList.Add( 3600 );
						megaSpawner.DespawnList.Add( (bool) false );
						megaSpawner.DespawnGroupList.Add( (bool) false );
						megaSpawner.DespawnTimeExpireList.Add( (bool) true );
					}
				}

				ArrayList settingList = new ArrayList();

				settingList.Add( Setting.OverrideIndividualEntries );
				settingList.Add( spawner.HomeRange );
				settingList.Add( walkRange );
				settingList.Add( spawner.Count );
				settingList.Add( calcMinDelay );
				settingList.Add( calcMaxDelay );
				settingList.Add( spawner.Group );
				settingList.Add( (bool) true );
				settingList.Add( SpawnType.Regular );
				settingList.Add( "" );
				settingList.Add( (bool) false );
				settingList.Add( 10 );
				settingList.Add( 0 );
				settingList.Add( 0 );
				settingList.Add( 0 );
				settingList.Add( 0 );
				settingList.Add( 1800 );
				settingList.Add( 3600 );
				settingList.Add( (bool) false );
				settingList.Add( (bool) false );
				settingList.Add( (bool) true );

				megaSpawner.SettingsList.Add( settingList );

				megaSpawner.CompileSettings();

				megaSpawner.SettingsList.Sort( new MC.SettingsSorter() );

				for ( int j = 0; j < megaSpawner.OverrideAmount; j++ )
				{
					megaSpawner.OverrideRespawnEntryList.Add( "" );
					megaSpawner.OverrideRespawnTimeList.Add( 0 );
					megaSpawner.OverrideSpawnCounterList.Add( DateTime.Now );
					megaSpawner.OverrideSpawnTimeList.Add( 0 );
					megaSpawner.OverrideDespawnTimeList.Add( 0 );
				}

				megaSpawner.MoveToWorld( spawner.Location, spawner.Map );

				if ( megaSpawner.Active )
				{
					MegaSpawnerOverride.CheckDupedEntries( megaSpawner );

					megaSpawner.Start();
					megaSpawner.Respawn();
				}

				HideSpawnerList.Add( (bool) false );
				MSGCheckBoxesList.Add( (bool) false );

				spawner.Delete();
			}

			private void SetArgsList()
			{
				ArgsList[2] = MessagesTitle;
				ArgsList[4] = Messages;
				ArgsList[6] = HideSpawnerList;
				ArgsList[13] = MSGCheckBoxesList;
				ArgsList[16] = CUGCheckBoxesList;
				ArgsList[33] = ConvertSpawnersList;
			}

			private void GetArgsList()
			{
				MessagesTitle = (string)						ArgsList[2];
				Messages = (string)								ArgsList[4];
				HideSpawnerList = (ArrayList) 					ArgsList[6];
				MSGCheckBoxesList = (ArrayList)					ArgsList[13];
				CUGCheckBoxesList = (ArrayList)					ArgsList[16];
				ConvertSpawnersList = (ArrayList)				ArgsList[33];
			}
		}
	}
}