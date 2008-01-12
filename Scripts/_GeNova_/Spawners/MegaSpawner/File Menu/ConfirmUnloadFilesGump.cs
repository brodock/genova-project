using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmUnloadFilesGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList FMCheckBoxesList = new ArrayList();
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private Mobile gumpMobile;

		public ConfirmUnloadFilesGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
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
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Unload All Files" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Unload Selected Files" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to unload all files?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to unload selected files?" ), false, false ); break; }
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
						case 1: { MessagesTitle = "Unload All Files"; Messages = "You have chosen not to unload all files loaded in the Mega Spawner System."; break; }
						case 2: { MessagesTitle = "Unload Selected Files"; Messages = "You have chosen not to unload selected files loaded in the Mega Spawner System."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Unload Files
				{
					switch ( Select )
					{
						case 1: // Unload All Files
						{
							for ( int i = 0; i < MC.FileImportList.Count; i++ )
							{
								string fileName = (string) MC.FileImportList[i];

								for ( int j = 0; j < MC.SpawnerList.Count; j++ )
								{
									MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[j];

									if ( megaSpawner.Imported == fileName )
										megaSpawner.Delete();
								}

								FMCheckBoxesList.RemoveAt( i );

								i--;
							}

							MessagesTitle = "Unload All Files";
							Messages = "All files have been unloaded.";

							break;
						}
						case 2: // Unload Selected Files
						{
							for ( int i = 0; i < MC.FileImportList.Count; i++ )
							{
								string fileName = (string) MC.FileImportList[i];

								if ( (bool) FMCheckBoxesList[i] )
								{
									for ( int j = 0; j < MC.SpawnerList.Count; j++ )
									{
										MegaSpawner megaSpawner = (MegaSpawner) MC.SpawnerList[j];

										if ( megaSpawner.Imported == fileName )
											megaSpawner.Delete();
									}

									FMCheckBoxesList.RemoveAt( i );

									i--;
								}
							}

							MessagesTitle = "Unload Selected Files";
							Messages = "All selected files have been unloaded.";

							break;
						}
					}

					SetArgsList();

					gumpMobile.SendGump( new FileMenuGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[15] = FMCheckBoxesList;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			FMCheckBoxesList = (ArrayList)					ArgsList[15];
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
	}
}