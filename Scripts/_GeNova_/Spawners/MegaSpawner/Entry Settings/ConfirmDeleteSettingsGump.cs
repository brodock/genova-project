using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteSettingsGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList SettingsCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private Mobile gumpMobile;

		public ConfirmDeleteSettingsGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
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
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete All Settings" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete Selected Settings" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete all settings?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete selected settings?" ), false, false ); break; }
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
						case 1: { MessagesTitle = "Delete All Settings"; Messages = "You have chosen not to delete all settings on the Mega Spawner."; break; }
						case 2: { MessagesTitle = "Delete Selected Settings"; Messages = "You have chosen not to delete selected settings on the Mega Spawner."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Settings
				{
					if ( CheckProcess() )
						break;

					switch ( Select )
					{
						case 1: // Delete All Settings
						{
							if ( megaSpawner.OverrideIndividualEntries )
							{
								MegaSpawnerOverride.DeleteEntries( megaSpawner );
								MegaSpawnerOverride.RemoveRespawnEntries( megaSpawner );
								megaSpawner.RemoveRespawnEntries();
								megaSpawner.SpawnedEntries.Clear();
								megaSpawner.LastMovedList.Clear();

								for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
								{
									int amount = (int) megaSpawner.AmountList[i];

									ArrayList respawnEntryList = new ArrayList();
									ArrayList respawnTimeList = new ArrayList();
									ArrayList spawnCounterList = new ArrayList();
									ArrayList spawnTimeList = new ArrayList();
									ArrayList respawnOnSaveList = new ArrayList();
									ArrayList despawnTimeList = new ArrayList();

									for ( int j = 0; j < amount; j++ )
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

							megaSpawner.ResetSettingValues();
							megaSpawner.SettingsList.Clear();
							SettingsCheckBoxesList.Clear();
							megaSpawner.Respawn();

							MessagesTitle = "Delete All Settings";
							Messages = "All of the settings on the Mega Spawner have been deleted.";

							break;
						}
						case 2: // Delete Selected Settings
						{
							for ( int i = 0; i < megaSpawner.SettingsList.Count; i++ )
							{
								if ( (bool) SettingsCheckBoxesList[i] )
								{
									ArrayList settingList = (ArrayList) megaSpawner.SettingsList[i];

									if ( (Setting) settingList[0] == Setting.OverrideIndividualEntries )
									{
										MegaSpawnerOverride.DeleteEntries( megaSpawner );
										MegaSpawnerOverride.RemoveRespawnEntries( megaSpawner );
										megaSpawner.RemoveRespawnEntries();
										megaSpawner.SpawnedEntries.Clear();
										megaSpawner.LastMovedList.Clear();

										for ( int j = 0; j < megaSpawner.EntryList.Count; j++ )
										{
											int amount = (int) megaSpawner.AmountList[j];

											ArrayList respawnEntryList = new ArrayList();
											ArrayList respawnTimeList = new ArrayList();
											ArrayList spawnCounterList = new ArrayList();
											ArrayList spawnTimeList = new ArrayList();
											ArrayList respawnOnSaveList = new ArrayList();
											ArrayList despawnTimeList = new ArrayList();

											for ( int k = 0; k < amount; k++ )
											{
												respawnEntryList.Add( (string) megaSpawner.EntryList[j] );
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

									megaSpawner.ResetSetting( i );
									megaSpawner.SettingsList.RemoveAt( i );
									SettingsCheckBoxesList.RemoveAt( i );
									megaSpawner.Respawn();

									i--;
								}
							}

							megaSpawner.Respawn();

							MessagesTitle = "Delete Selected Settings";
							Messages = "All selected settings on the Mega Spawner have been deleted.";

							break;
						}
					}

					SetArgsList();

					gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[17] = SettingsCheckBoxesList;
			ArgsList[19] = megaSpawner;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			SettingsCheckBoxesList = (ArrayList)				ArgsList[17];
			megaSpawner = (MegaSpawner) 					ArgsList[19];
			PersonalConfigList = (ArrayList)				ArgsList[28];

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

				gumpMobile.SendGump( new SettingsGump( gumpMobile, ArgsList ) );
			}

			return MC.CheckProcess();
		}
	}
}