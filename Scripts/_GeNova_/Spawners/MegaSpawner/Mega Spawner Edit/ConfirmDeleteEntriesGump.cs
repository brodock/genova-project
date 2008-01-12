using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;

namespace Server.MegaSpawnerSystem
{
	public class ConfirmDeleteEntriesGump : Gump
	{
		private ArrayList ArgsList = new ArrayList();

		private ArrayList MSEGCheckBoxesList = new ArrayList();
		private MegaSpawner megaSpawner;
		private ArrayList PersonalConfigList = new ArrayList();

		private StyleType StyleTypeConfig;
		private BackgroundType BackgroundTypeConfig;
		private TextColor DefaultTextColor, TitleTextColor;

		private string MessagesTitle, Messages;
		private string defaultTextColor, titleTextColor;

		private int Select;
		private Mobile gumpMobile;

		public ConfirmDeleteEntriesGump( Mobile mobile, ArrayList argsList, int select ) : base( 0,0 )
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
				case 1: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete All Entries" ), false, false ); break; }
				case 2: { AddHtml( 221, 181, 320, 20, MC.ColorText( titleTextColor, "Confirmation Of Delete Selected Entries" ), false, false ); break; }
			}

			switch ( Select )
			{
				case 1: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete all entries?" ), false, false ); break; }
				case 2: { AddHtml( 201, 201, 340, 20, MC.ColorText( defaultTextColor, "Are you sure you want to delete selected entries?" ), false, false ); break; }
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
						case 1: { MessagesTitle = "Delete All Entries"; Messages = "You have chosen not to delete all entries on the Mega Spawner."; break; }
						case 2: { MessagesTitle = "Delete Selected Entries"; Messages = "You have chosen not to delete selected entries on the Mega Spawner."; break; }
					}

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
				case 1: // Delete Entries
				{
					if ( CheckProcess() )
						break;

					switch ( Select )
					{
						case 1: // Delete All Entries
						{
							megaSpawner.ClearSpawner();

							MSEGCheckBoxesList.Clear();

							MessagesTitle = "Delete All Entries";
							Messages = "All of the entries on the Mega Spawner have been deleted.";

							break;
						}
						case 2: // Delete Selected Entries
						{
							for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
							{
								if ( (bool) MSEGCheckBoxesList[i] )
								{
									MSEGCheckBoxesList.RemoveAt( i );
									megaSpawner.DeleteEntry( i );

									i--;
								}
							}

							MessagesTitle = "Delete Selected Entries";
							Messages = "All selected entries on the Mega Spawner have been deleted.";

							break;
						}
					}

					SetArgsList();

					gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );

					break;
				}
			}
		}

		private void SetArgsList()
		{
			ArgsList[2] = MessagesTitle;
			ArgsList[4] = Messages;
			ArgsList[14] = MSEGCheckBoxesList;
			ArgsList[19] = megaSpawner;
		}

		private void GetArgsList()
		{
			MessagesTitle = (string)					ArgsList[2];
			Messages = (string)						ArgsList[4];
			MSEGCheckBoxesList = (ArrayList)				ArgsList[14];
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

				gumpMobile.SendGump( new MegaSpawnerEditGump( gumpMobile, ArgsList ) );
			}

			return MC.CheckProcess();
		}
	}
}