using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class RemoveOverrideSettingGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private string FileName;
		private ArrayList MSGCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private Mobile gumpMobile;

		public static void Initialize()
		{
			string helpDesc = "That plug-in will remove the \"Override Individual Entries\" setting from all selected spawners.";
			MC.RegisterPlugIn( new MC.OnCommand( RemoveOverrideSetting ), "RemoveOverrideSetting", "Remove Override Setting From Selected Spawners", helpDesc );
		}

		public static void RemoveOverrideSetting( Mobile mobile, ArrayList argsList )
		{
			mobile.SendGump( new RemoveOverrideSettingGump( mobile, argsList ) );
		}

		public RemoveOverrideSettingGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 380, 110 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 340, 40 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 250, 340, 20 );

			AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Deleting Override Setting" ), false, false );

			AddHtml( 201, 201, 340, 40, MC.ColorText( defaultTextColor, "Are you sure you want to delete the override setting on selected spawners?" ), false, false );

			AddHtml( 491, 251, 60, 20, MC.ColorText( defaultTextColor, "Cancel" ), false, false );
			AddButton( 450, 250, 4017, 4019, 0, GumpButtonType.Reply, 0 );

			AddHtml( 241, 251, 60, 20, MC.ColorText( defaultTextColor, "Ok" ), false, false );
			AddButton( 200, 250, 4023, 4025, 1, GumpButtonType.Reply, 0 );
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			switch ( info.ButtonID )
			{
				case 0: // Close Gump
				{
					MessagesTitle = "Delete Override Settings";
					Messages = "You have chosen not to delete the override setting for selected spawners.";

					SetArgsList();

					gumpMobile.SendGump( new PlugInsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Override Settings
				{
					int count = 0;

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
					{
						MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

						if ( (bool) MSGCheckBoxesList[i] )
						{
							bool success = false;

							for ( int j = 0; j < megaSpawner.SettingsList.Count; j++ )
							{
								ArrayList settingList = (ArrayList) megaSpawner.SettingsList[j];

								if ( (Setting) settingList[0] == Setting.OverrideIndividualEntries )
								{
									success = true;

									MegaSpawnerOverride.DeleteEntries( megaSpawner );
									MegaSpawnerOverride.RemoveRespawnEntries( megaSpawner );
									megaSpawner.RemoveRespawnEntries();
									megaSpawner.SpawnedEntries.Clear();
									megaSpawner.LastMovedList.Clear();

									for ( int k = 0; k < megaSpawner.EntryList.Count; k++ )
									{
										int amount = (int) megaSpawner.AmountList[k];

										ArrayList respawnEntryList = new ArrayList();
										ArrayList respawnTimeList = new ArrayList();
										ArrayList spawnCounterList = new ArrayList();
										ArrayList spawnTimeList = new ArrayList();
										ArrayList respawnOnSaveList = new ArrayList();
										ArrayList despawnTimeList = new ArrayList();

										for ( int l = 0; l < amount; l++ )
										{
											respawnEntryList.Add( (string) megaSpawner.EntryList[k] );
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

									megaSpawner.ResetSetting( j );
									megaSpawner.SettingsList.RemoveAt( j );
									megaSpawner.Respawn();
									j--;
								}
							}

							if ( success )
								count++;
						}
					}

					MessagesTitle = "Delete Override Settings";

					if ( count > 0 )
						Messages = String.Format( "\"Override Individual Entries\" setting has been removed on {0} Mega Spawner{1}.", count, count == 1 ? "" : "s" );
					else
						Messages = "Either no Mega Spawners have been selected or none of them had the \"Override Individual Entries\" setting.";

					SetArgsList();

					gumpMobile.SendGump( new PlugInsGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[13] = MSGCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)						ArgsList[2];
			Messages = (string)								ArgsList[4];
			MSGCheckBoxesList = (ArrayList)					ArgsList[13];
			PersonalConfigList = (ArrayList)				ArgsList[28];

			StyleTypeConfig = (StyleType)					PersonalConfigList[0];
			BackgroundTypeConfig = (BackgroundType)			PersonalConfigList[1];
			DefaultTextColor = (TextColor)					PersonalConfigList[4];
			TitleTextColor = (TextColor)					PersonalConfigList[5];
		}

		private void GetTextColors()
		{
			defaultTextColor = MC.GetTextColor( DefaultTextColor );
			titleTextColor = MC.GetTextColor( TitleTextColor );
		}
	}
}