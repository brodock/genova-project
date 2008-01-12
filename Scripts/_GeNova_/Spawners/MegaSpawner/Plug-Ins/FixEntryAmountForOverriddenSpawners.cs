using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class FixEntryAmountGump : Gump
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
			string helpDesc = "That plug-in will fix the entry amount for overridden spawners whose entry amount is 0.";
			MC.RegisterPlugIn( new MC.OnCommand( FixEntryAmount ), "FixEntryAmount", "Fix Entry Amount For Overridden Spawners", helpDesc );
		}

		public static void FixEntryAmount( Mobile mobile, ArrayList argsList )
		{
			mobile.SendGump( new FixEntryAmountGump( mobile, argsList ) );
		}

		public FixEntryAmountGump( Mobile mobile, ArrayList argsList ) : base( 0,0 )
		{
			gumpMobile = mobile;

			ArgsList = argsList;
			GetArgsList();

			GetTextColors();

			AddPage(0); // Page 0

			MC.DisplayStyle( this, StyleTypeConfig, 180, 180, 380, 110 );

			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 200, 340, 40 );
			MC.DisplayBackground( this, BackgroundTypeConfig, 200, 250, 340, 20 );

			AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Fixing Entry Amount" ), false, false );

			AddHtml( 201, 201, 340, 40, MC.ColorText( defaultTextColor, "Are you sure you want to fix the entry amount for overridden spawners?" ), false, false );

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
					MessagesTitle = "Fix Entry Amount";
					Messages = "You have chosen not to fix the entry amount for overridden spawners.";

					SetArgsList();

					gumpMobile.SendGump( new PlugInsGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Fix Entry Amount
				{
					int count = 0;

					for ( int i = 0; i < MC.SpawnerList.Count; i++ )
					{
						MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[i];

						bool success = false;

						for ( int j = 0; j < megaSpawner.SettingsList.Count; j++ )
						{
							ArrayList settingList = (ArrayList) megaSpawner.SettingsList[j];

							if ( (Setting) settingList[0] == Setting.OverrideIndividualEntries )
							{
								Console.WriteLine( "FOUND!" );
								for ( int k = 0; k < megaSpawner.AmountList.Count; k++ )
								{
									Console.WriteLine( "AMOUNT!" );
									int amount = (int) megaSpawner.AmountList[k];

									if ( amount == 0 )
									{
										Console.WriteLine( "TRUE!" );
										success = true;

										megaSpawner.AmountList[k] = 1;
									}
								}
							}

							if ( success )
								count++;
						}
					}

					MessagesTitle = "Fix Entry Amount";

					if ( count > 0 )
						Messages = String.Format( "{0} Overridden Mega Spawner{1} have had their entry amounts fixed.", count, count == 1 ? "" : "s" );
					else
						Messages = "Either no overridden spawners exist, or none of the overridden spawners contain invalid entry amounts.";

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