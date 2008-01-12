using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Items;
using Server.Gumps;
using MC = Server.MegaSpawnerSystem.MasterControl;
using GeNova.Server.Engines;

namespace Server.MegaSpawnerSystem
{
	public class MegaSpawnerOverride
	{
		public static void CheckEntries( MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < megaSpawner.OverrideSpawnedEntries.Count; i++ )
			{
				object o = megaSpawner.OverrideSpawnedEntries[i];

				if ( o is Item )
				{
					Item item = (Item) o;

					if ( megaSpawner.OverrideLastMovedList[i] is int )
						megaSpawner.OverrideLastMovedList[i] = item.LastMoved;

					DateTime lastMoved = (DateTime) megaSpawner.OverrideLastMovedList[i];

					if ( item.RootParent != null && item.RootParent == (object) megaSpawner.ContainerSpawn && lastMoved != item.LastMoved )
					{
						lastMoved = item.LastMoved;
						megaSpawner.OverrideLastMovedList[i] = item.LastMoved;
					}

					if ( item.Deleted || ( item.RootParent != null && megaSpawner.ContainerSpawn != null && item.RootParent != (object) megaSpawner.ContainerSpawn ) || ( item.RootParent != null && megaSpawner.RootParent != null && item.RootParent != megaSpawner.RootParent ) || lastMoved != item.LastMoved )
					{
						megaSpawner.OverrideSpawnedEntries.RemoveAt( i );
						megaSpawner.OverrideLastMovedList.RemoveAt( i );

						SpawnCounter( megaSpawner, o );

						i--;
					}
				}
				else if ( o is Mobile )
				{
					Mobile mob = (Mobile) o;

					if ( mob.Deleted || ( mob is BaseCreature && ( ( (BaseCreature) mob ).Controlled || ( (BaseCreature) mob ).IsStabled ) ) )
					{
						megaSpawner.OverrideSpawnedEntries.RemoveAt( i );
						megaSpawner.OverrideLastMovedList.RemoveAt( i );

						SpawnCounter( megaSpawner, o );

						i--;
					}
				}
				else
				{
					megaSpawner.OverrideSpawnedEntries.RemoveAt( i );
					megaSpawner.OverrideLastMovedList.RemoveAt( i );

					SpawnCounter( megaSpawner, o );

					i--;
				}
			}
		}

		public static int CountEntries( MegaSpawner megaSpawner )
		{
			return megaSpawner.OverrideSpawnedEntries.Count;
		}

		public static int CountEntries( MegaSpawner megaSpawner, int index )
		{
			int entryCount = 0;

			for ( int i = 0; i < megaSpawner.OverrideSpawnedEntries.Count; i++ )
			{
				object o = megaSpawner.OverrideSpawnedEntries[i];
				string entryType = (string) megaSpawner.EntryList[index];

				if ( o.GetType().Name.ToLower() == entryType.ToLower() )
					entryCount++;
			}

			return entryCount;
		}

		public static void DeleteEntries( MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < megaSpawner.OverrideSpawnedEntries.Count; i++ )
			{
				object o = megaSpawner.OverrideSpawnedEntries[i];

				MC.DeleteObject( o );

				if ( megaSpawner.OverrideRespawnEntryList.Count == megaSpawner.OverrideSpawnedEntries.Count )
					megaSpawner.OverrideRespawnEntryList[i] = "";
			}

			megaSpawner.OverrideSpawnedEntries.Clear();
			megaSpawner.OverrideLastMovedList.Clear();
		}

		public static void DeleteEntries( MegaSpawner megaSpawner, string entryType )
		{
			for ( int i = 0; i < megaSpawner.OverrideSpawnedEntries.Count; i++ )
			{
				object o = megaSpawner.OverrideSpawnedEntries[i];
				Type type = ScriptCompiler.FindTypeByName( entryType );

				if ( o is Item && ( (Item) o ).GetType() == type )
				{
					( (Item) o ).Delete();
					megaSpawner.OverrideRespawnEntryList[i] = "";
				}
				else if ( o is Mobile && ( (Mobile) o ).GetType() == type )
				{
					( (Mobile) o ).Delete();
					megaSpawner.OverrideRespawnEntryList[i] = "";
				}
			}
		}

		public static void ClearSpawner( MegaSpawner megaSpawner )
		{
			CheckEntries( megaSpawner );
			DeleteEntries( megaSpawner );

			megaSpawner.WipeArrayLists();
		}

		public static void RemoveRespawnEntries( MegaSpawner megaSpawner )
		{
			megaSpawner.OverrideRespawnEntryList.Clear();
			megaSpawner.OverrideRespawnTimeList.Clear();
			megaSpawner.OverrideSpawnCounterList.Clear();
			megaSpawner.OverrideSpawnTimeList.Clear();
			megaSpawner.OverrideRespawnOnSave = false;
			megaSpawner.OverrideDespawnTimeList.Clear();
		}

		public static void RemoveRespawnEntries( MegaSpawner megaSpawner, int index )
		{
			string entryType = (string) megaSpawner.EntryList[index];

			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				object o = megaSpawner.OverrideRespawnEntryList[i];

				if ( o is Item && ( (Item) o ).GetType().Name.ToLower() == entryType.ToLower() )
				{
					megaSpawner.OverrideRespawnEntryList.RemoveAt( i );
					megaSpawner.OverrideRespawnTimeList.RemoveAt( i );
					megaSpawner.OverrideSpawnCounterList.RemoveAt( i );
					megaSpawner.OverrideSpawnTimeList.RemoveAt( i );
					megaSpawner.OverrideDespawnTimeList.RemoveAt( i );

					i--;
				}
				else if ( o is Mobile && ( (Mobile) o ).GetType().Name.ToLower() == entryType.ToLower() )
				{
					megaSpawner.OverrideRespawnEntryList.RemoveAt( i );
					megaSpawner.OverrideRespawnTimeList.RemoveAt( i );
					megaSpawner.OverrideSpawnCounterList.RemoveAt( i );
					megaSpawner.OverrideSpawnTimeList.RemoveAt( i );
					megaSpawner.OverrideDespawnTimeList.RemoveAt( i );

					i--;
				}
			}
		}

		public static void CheckDupedEntries( MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < megaSpawner.EntryList.Count; i++ )
			{
				string entryType = (string) megaSpawner.EntryList[i];
				int count = 0;

				for ( int j = 0; j < megaSpawner.EntryList.Count; j++ )
				{
					string entryCompare = (string) megaSpawner.EntryList[j];

					if ( entryType == entryCompare )
						count++;

					if ( count > 1 )
					{
						megaSpawner.DeleteEntry( i );
						i--;

						break;
					}
				}
			}
		}

		public static void ResetSpawnTime( MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				megaSpawner.OverrideRespawnTimeList[i] = 0;
				megaSpawner.OverrideSpawnCounterList[i] = DateTime.Now;
				megaSpawner.OverrideSpawnTimeList[i] = 0;
				megaSpawner.OverrideDespawnTimeList[i] = 0;
			}
		}

		public static int FindFirstSpawn( MegaSpawner megaSpawner )
		{
			int lowestDelay = 0;

			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				if ( megaSpawner.OverrideRespawnEntryList[i] is string )
				{
					if ( (string) megaSpawner.OverrideRespawnEntryList[i] == "" )
					{
						if ( megaSpawner.OverrideGroupSpawn )
							lowestDelay = (int) megaSpawner.OverrideRespawnTimeList[i];
						else if ( lowestDelay <= 0 && (int) megaSpawner.OverrideRespawnTimeList[i] > 0 )
							lowestDelay = (int) megaSpawner.OverrideRespawnTimeList[i];
						else if ( lowestDelay > (int) megaSpawner.OverrideRespawnTimeList[i] )
							lowestDelay = (int) megaSpawner.OverrideRespawnTimeList[i];
					}
				}
			}

			return lowestDelay - MegaSpawner.tickOffset;
		}

		public static DateTime FindFirstSpawnCounter( MegaSpawner megaSpawner )
		{
			DateTime lowestDelay = DateTime.Now;

			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				if ( megaSpawner.OverrideRespawnEntryList[i] is string )
				{
					if ( (string) megaSpawner.OverrideRespawnEntryList[i] == "" )
					{
						if ( megaSpawner.OverrideGroupSpawn )
							lowestDelay = (DateTime) megaSpawner.OverrideSpawnCounterList[i];
						else if ( lowestDelay >= DateTime.Now && (DateTime) megaSpawner.OverrideSpawnCounterList[i] < DateTime.Now )
							lowestDelay = (DateTime) megaSpawner.OverrideSpawnCounterList[i];
						else if ( lowestDelay > (DateTime) megaSpawner.OverrideSpawnCounterList[i] )
							lowestDelay = (DateTime) megaSpawner.OverrideSpawnCounterList[i];
					}
				}
			}

			return lowestDelay;
		}

		public static int FindFirstSpawnTime( MegaSpawner megaSpawner )
		{
			int lowestDelay = 0;

			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				if ( megaSpawner.OverrideRespawnEntryList[i] is string )
				{
					if ( (string) megaSpawner.OverrideRespawnEntryList[i] == "" )
					{
						if ( megaSpawner.OverrideGroupSpawn )
							lowestDelay = (int) megaSpawner.OverrideSpawnTimeList[i];
						else if ( lowestDelay <= 0 && (int) megaSpawner.OverrideSpawnTimeList[i] > 0 )
							lowestDelay = (int) megaSpawner.OverrideSpawnTimeList[i];
						else if ( lowestDelay > (int) megaSpawner.OverrideSpawnTimeList[i] )
							lowestDelay = (int) megaSpawner.OverrideSpawnTimeList[i];
					}
				}
			}

			return lowestDelay - MegaSpawner.tickOffset;
		}

		public static void TickDespawn( MegaSpawner megaSpawner, int delay )
		{
			for ( int i = 0; i < megaSpawner.OverrideDespawnTimeList.Count; i++ )
			{
				if ( megaSpawner.OverrideRespawnEntryList[i] is Item || megaSpawner.OverrideRespawnEntryList[i] is Mobile )
				{
					int despawnTime = (int) megaSpawner.OverrideDespawnTimeList[i];

					if ( megaSpawner.OverrideDespawnGroup )
						despawnTime = (int) megaSpawner.OverrideDespawnTimeList[0];

					if ( despawnTime > 0 )
					{
						megaSpawner.OverrideDespawnTimeList[i] = (int) megaSpawner.OverrideDespawnTimeList[i] - delay;
					}
					else
					{
						object o = megaSpawner.OverrideRespawnEntryList[i];

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
		}

		public static void TickDown( MegaSpawner megaSpawner, int delay )
		{
			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				if ( megaSpawner.OverrideRespawnEntryList[i] is string )
				{
					if ( (string) megaSpawner.OverrideRespawnEntryList[i] == "" )
					{
						if ( (int) megaSpawner.OverrideRespawnTimeList[i] > 0 )
							megaSpawner.OverrideRespawnTimeList[i] = (int) megaSpawner.OverrideRespawnTimeList[i] - delay;
					}
				}
			}
		}

		public static void SpawnCounter( MegaSpawner megaSpawner, object o )
		{
			object fromCompare = o;

			if ( o is Item )
				fromCompare = (Item) o;
			else if ( o is Mobile )
				fromCompare = (Mobile) o;

			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				if ( megaSpawner.OverrideRespawnEntryList[i] is Mobile || megaSpawner.OverrideRespawnEntryList[i] is Item )
				{
					object toCompare = megaSpawner.OverrideRespawnEntryList[i];

					if ( megaSpawner.OverrideRespawnEntryList[i] is Item )
						toCompare = (Item) megaSpawner.OverrideRespawnEntryList[i];
					else if ( megaSpawner.OverrideRespawnEntryList[i] is Mobile )
						toCompare = (Mobile) megaSpawner.OverrideRespawnEntryList[i];

					if ( fromCompare == toCompare && (int) megaSpawner.OverrideRespawnTimeList[i] <= MegaSpawner.tickOffset )
					{
						string entryType = Convert.ToString( toCompare.GetType().Name.ToLower() );
						megaSpawner.OverrideRespawnEntryList[i] = "";

						if ( megaSpawner.OverrideGroupSpawn && IsEmpty( megaSpawner ) )
							AddSpawnCount( megaSpawner, i );
						else if ( !megaSpawner.OverrideGroupSpawn )
							AddSpawnCount( megaSpawner, i );

						return;
					}
				}
			}
		}

		public static void AddSpawnCount( MegaSpawner megaSpawner, int index )
		{
			int randomDelay = Utility.Random( megaSpawner.OverrideMinDelay, megaSpawner.OverrideMaxDelay - megaSpawner.OverrideMinDelay );

			if ( megaSpawner.OverrideGroupSpawn )
			{
				for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
				{
					if ( (string) megaSpawner.OverrideRespawnEntryList[i] == "" )
					{
						megaSpawner.OverrideRespawnTimeList[i] = randomDelay + MegaSpawner.tickOffset;
						megaSpawner.OverrideSpawnCounterList[i] = DateTime.Now;
						megaSpawner.OverrideSpawnTimeList[i] = randomDelay;
					}
				}
			}
			else
			{
				megaSpawner.OverrideRespawnTimeList[index] = randomDelay + MegaSpawner.tickOffset;
				megaSpawner.OverrideSpawnCounterList[index] = DateTime.Now;
				megaSpawner.OverrideSpawnTimeList[index] = randomDelay;
			}
		}

		public static void AddToRespawnEntries( MegaSpawner megaSpawner, object o )
		{
			bool success = false;

			for ( int i = 0; i < megaSpawner.OverrideRespawnEntryList.Count; i++ )
			{
				int randomDespawnTime = Utility.Random( megaSpawner.OverrideMinDespawn, megaSpawner.OverrideMaxDespawn - megaSpawner.OverrideMinDespawn );

				if ( megaSpawner.OverrideRespawnEntryList[i] is string )
				{
					object respawnEntry = o;

					if ( o is Item )
						respawnEntry = (Item) o;
					else if ( o is Mobile )
						respawnEntry = (Mobile) o;

					if ( (string) megaSpawner.OverrideRespawnEntryList[i] == "" && ( (int) megaSpawner.OverrideRespawnTimeList[i] <= MegaSpawner.tickOffset || megaSpawner.OverrideGroupSpawn ) )
					{
						success = true;

						megaSpawner.OverrideRespawnEntryList[i] = respawnEntry;
						megaSpawner.OverrideDespawnTimeList[i] = randomDespawnTime;

						break;
					}
				}
			}

			if ( !success )
				MC.DeleteObject( o );
		}

		public static void ForceReconfigRespawn( MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
			{
				megaSpawner.OverrideRespawnEntryList.Add( "" );
				megaSpawner.OverrideRespawnTimeList.Add( 0 );
				megaSpawner.OverrideSpawnCounterList.Add( DateTime.Now );
				megaSpawner.OverrideSpawnTimeList.Add( 0 );
				megaSpawner.OverrideDespawnTimeList.Add( 0 );
			}

			Respawn( megaSpawner );
		}

		public static void Respawn( MegaSpawner megaSpawner )
		{
			megaSpawner.Respawning = true;

			DeleteEntries( megaSpawner );
			ResetSpawnTime( megaSpawner );

			if ( megaSpawner.EntryList.Count != 0 )
			{
				SpawnType st = megaSpawner.OverrideSpawnType;

				switch ( st )
				{
					case SpawnType.Regular:
					{
						for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
							SpawnEntry( megaSpawner, SelectIndex( megaSpawner, megaSpawner.EntryList.Count ) );

						break;
					}
					case SpawnType.Proximity:
					{
						megaSpawner.OverrideTriggerEventNow = true;

						break;
					}
					case SpawnType.GameTimeBased:
					{
						int UOHour, UOMinute, time;

						Server.Items.Clock.GetTime( megaSpawner.Map, megaSpawner.X, megaSpawner.Y, out UOHour, out UOMinute );

						if ( UOHour == 24 )
							UOHour = 0;

						time = ( UOHour * 60 ) + UOMinute;

						for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
							CheckSpawnTime( megaSpawner, time );

						break;
					}
					case SpawnType.RealTimeBased:
					{
						int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

						for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
							CheckSpawnTime( megaSpawner, time );

						break;
					}
					case SpawnType.Speech:
					{
						megaSpawner.OverrideTriggerEventNow = true;

						break;
					}
				}
			}

			megaSpawner.Respawning = false;
		}

		public static void BringToHome( MegaSpawner megaSpawner )
		{
			for ( int i = 0; i < megaSpawner.OverrideSpawnedEntries.Count; i++ )
			{
				object o = megaSpawner.OverrideSpawnedEntries[i];

				if ( o is Mobile )
					( (Mobile) o ).Location = megaSpawner.Location;
			}
		}

		public static void BringToHome( MegaSpawner megaSpawner, int index )
		{
			string entryType = (string) megaSpawner.EntryList[index];

			for ( int i = 0; i < megaSpawner.OverrideSpawnedEntries.Count; i++ )
			{
				object o = megaSpawner.OverrideSpawnedEntries[i];

				if ( o is Mobile )
				{
					if ( o.GetType().Name.ToLower() == entryType.ToLower() )
						( (Mobile) o ).Location = megaSpawner.Location;
				}
			}
		}

		public static void SpawnEntry( MegaSpawner megaSpawner, int index )
		{
			if ( !(bool) megaSpawner.ActivatedList[index] )
				return;

			Type spawnType = ScriptCompiler.FindTypeByName( (string) megaSpawner.EntryList[index] );

			if ( spawnType != null )
			{
				object check = null;

				try
				{
					object o = Activator.CreateInstance( spawnType );
					check = o;

					if ( o is Item )
					{
						Item item = (Item) o;

						if ( item.Stackable )
						{
							int min = (int) megaSpawner.MinStackAmountList[index];
							int max = (int) megaSpawner.MaxStackAmountList[index];
							int stack = Utility.Random( min, ( max - min ) );

							item.Amount = stack;
						}

						if ( megaSpawner.RootParent is Container )
							( (Container) megaSpawner.RootParent ).DropItem( item );
						else if ( megaSpawner.ContainerSpawn != null )
							megaSpawner.ContainerSpawn.DropItem( item );
						else
							item.MoveToWorld( GetSpawnPosition( megaSpawner ), megaSpawner.Map );

						item.Movable = (bool) megaSpawner.MovableList[index];

						megaSpawner.OverrideSpawnedEntries.Add( item );
						megaSpawner.OverrideLastMovedList.Add( item.LastMoved );
						AddToRespawnEntries( megaSpawner, item );
					}
					else if ( o is Mobile )
					{
						Mobile m = (Mobile) o;

						Map map = megaSpawner.Map;
						Point3D loc = ( m is BaseVendor ? megaSpawner.Location : GetSpawnPosition( megaSpawner ) );

						m.Map = map;
						m.Location = loc;

						if ( m is BaseCreature )
						{
							BaseCreature creature = (BaseCreature) m;

							creature.Location = GetSpawnPosition( megaSpawner );
							creature.Home = megaSpawner.Location;
							creature.RangeHome = megaSpawner.OverrideWalkRange;
							creature.CantWalk = !(bool) megaSpawner.MovableList[index];

                            // Genova: customização no sistema para traduzir nome da criatura.
                            TraducaoDeNomesMobiles.AplicarAlteracoesCriatura(creature, spawnType.ToString(), creature.Title);
						}

						megaSpawner.OverrideSpawnedEntries.Add( m );
						megaSpawner.OverrideLastMovedList.Add( DateTime.Now );
						AddToRespawnEntries( megaSpawner, m );
					}

					megaSpawner.CheckAddItem( index, o );
				}
				catch
				{
					if ( check is Item )
						( (Item) check ).Delete();
					else if ( check is Mobile )
						( (Mobile) check ).Delete();
				}
			}
		}

		public static Point3D GetSpawnPosition( MegaSpawner megaSpawner )
		{
			int range = megaSpawner.OverrideSpawnRange;

			for ( int i = 0; i < MegaSpawner.spawnRetries; i++ )
			{
				int x, y;

				if ( megaSpawner.RootParent is Container )
				{
					x = ( (Container) megaSpawner.RootParent ).Location.X;
					y = ( (Container) megaSpawner.RootParent ).Location.Y;
				}
				else
				{
					x = megaSpawner.Location.X;
					y = megaSpawner.Location.Y;
				}

				x += ( Utility.Random( ( (int) range * 2 ) + 1 ) - range );
				y += ( Utility.Random( ( (int) range * 2 ) + 1 ) - range );
				int z = megaSpawner.Map.GetAverageZ( x, y );

				if ( megaSpawner.Map.CanSpawnMobile( new Point2D( x, y ), megaSpawner.Location.Z ) )
					return new Point3D( x, y, megaSpawner.Location.Z );
				else if ( megaSpawner.Map.CanSpawnMobile( new Point2D( x, y ), z ) )
					return new Point3D( x, y, z );
			}

			return megaSpawner.Location;
		}

		public static void TimerTick( MegaSpawner megaSpawner, int delay )
		{
			CheckEntries( megaSpawner );

			SpawnType st = megaSpawner.OverrideSpawnType;

			CheckTimeExpire( megaSpawner, st );

			int countDown = FindFirstSpawn( megaSpawner );
			int entryCount = CountEntries( megaSpawner );

			if ( countDown <= 0 && !IsFull( megaSpawner ) )
			{
				switch ( st )
				{
					case SpawnType.Regular:
					{
						Spawn( megaSpawner );

						break;
					}
					case SpawnType.Proximity:
					{
						megaSpawner.OverrideTriggerEventNow = true;

						break;
					}
					case SpawnType.GameTimeBased:
					{
						int UOHour, UOMinute, time;

						Server.Items.Clock.GetTime( megaSpawner.Map, megaSpawner.X, megaSpawner.Y, out UOHour, out UOMinute );

						if ( UOHour == 24 )
							UOHour = 0;

						time = ( UOHour * 60 ) + UOMinute;

						CheckSpawnTime( megaSpawner, time );

						break;
					}
					case SpawnType.RealTimeBased:
					{
						int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

						CheckSpawnTime( megaSpawner, time );

						break;
					}
					case SpawnType.Speech:
					{
						megaSpawner.OverrideTriggerEventNow = true;

						break;
					}
				}
			}

			if ( megaSpawner.OverrideDespawn )
				TickDespawn( megaSpawner, delay );

			if ( megaSpawner.OverrideGroupSpawn && entryCount != 0 )
			{}
			else if ( !IsFull( megaSpawner ) )
				TickDown( megaSpawner, delay );

			megaSpawner.Start();
		}

		public static void Spawn( MegaSpawner megaSpawner )
		{
			int entryCount = CountEntries( megaSpawner );

			if ( megaSpawner.Respawning || ( megaSpawner.OverrideGroupSpawn && entryCount == 0 ) )
				FullSpawn( megaSpawner );
			else
				SingleSpawn( megaSpawner );
		}

		public static void SingleSpawn( MegaSpawner megaSpawner )
		{
			if ( megaSpawner.OverrideGroupSpawn )
				return;

			int entryCount = CountEntries( megaSpawner );

			int index = SelectIndex( megaSpawner, megaSpawner.EntryList.Count );

			if ( entryCount < megaSpawner.OverrideAmount )
				SpawnEntry( megaSpawner, index );
		}

		public static void FullSpawn( MegaSpawner megaSpawner )
		{
			int index;

			for ( int i = 0; i < megaSpawner.OverrideAmount; i++ )
			{
				index = SelectIndex( megaSpawner, megaSpawner.EntryList.Count );
				SpawnEntry( megaSpawner, index );
			}
		}

		public static void CheckSpawnTime( MegaSpawner megaSpawner, int time )
		{
			int beginTime = megaSpawner.OverrideBeginTimeBased;
			int endTime = megaSpawner.OverrideEndTimeBased;

			if ( endTime < beginTime )
			{
				if ( beginTime <= time && time <= 1439 )
					Spawn( megaSpawner );

				if ( time <= endTime )
					Spawn( megaSpawner );
			}
			else
			{
				if ( beginTime <= time && time <= endTime )
					Spawn( megaSpawner );
			}
		}

		public static void CheckTimeExpire( MegaSpawner megaSpawner, SpawnType st )
		{
			if ( !megaSpawner.OverrideDespawnTimeExpire )
				return;

			switch ( st )
			{
				case SpawnType.GameTimeBased:
				{
					int UOHour, UOMinute, time;

					Server.Items.Clock.GetTime( megaSpawner.Map, megaSpawner.X, megaSpawner.Y, out UOHour, out UOMinute );

					if ( UOHour == 24 )
						UOHour = 0;

					time = ( UOHour * 60 ) + UOMinute;

					CheckDespawnTime( megaSpawner, time );

					break;
				}
				case SpawnType.RealTimeBased:
				{
					int time = ( DateTime.Now.Hour * 60 ) + DateTime.Now.Minute;

					CheckDespawnTime( megaSpawner, time );

					break;
				}
			}
		}

		public static void CheckDespawnTime( MegaSpawner megaSpawner, int time )
		{
			int beginTime = megaSpawner.OverrideBeginTimeBased;
			int endTime = megaSpawner.OverrideEndTimeBased;

			if ( endTime < beginTime )
			{
				if ( endTime < time && time < beginTime )
					DeleteEntries( megaSpawner );
			}
			else
			{
				if ( endTime < time && time <= 1439 )
					DeleteEntries( megaSpawner );
				else if ( time < beginTime )
					DeleteEntries( megaSpawner );
			}
		}

		public static bool EventSpawnAttempt( MegaSpawner megaSpawner, SpawnType st )
		{
			return ( megaSpawner.Active && megaSpawner.OverrideSpawnType == st && megaSpawner.OverrideTriggerEventNow );
		}

		public static bool IsFull( MegaSpawner megaSpawner )
		{
			return megaSpawner.OverrideSpawnedEntries.Count >= megaSpawner.OverrideAmount;
		}

		public static bool IsEmpty( MegaSpawner megaSpawner )
		{
			return CountEntries( megaSpawner ) <= 0;
		}

		public static bool IsTriggerBased( MegaSpawner megaSpawner )
		{
			return ( megaSpawner.OverrideSpawnType == SpawnType.Proximity || megaSpawner.OverrideSpawnType == SpawnType.Speech );
		}

		public static bool IsGroupSpawn( MegaSpawner megaSpawner )
		{
			return megaSpawner.OverrideGroupSpawn;
		}

		public static bool CheckEventAmbush( MegaSpawner megaSpawner )
		{
			int entryCount = CountEntries( megaSpawner );

			if ( entryCount != 0 )
				return false;
			else
				return true;
		}

		public static int SelectIndex( MegaSpawner megaSpawner, int max )
		{
			ArrayList randomIndex = new ArrayList();

			for( int i = 0; i < max; i++ )
			{
				int amount = (int) megaSpawner.AmountList[i];

				for( int j = 0; j < amount; j++ )
					randomIndex.Add( i );
			}

			if( randomIndex.Count == 0 )
				randomIndex.Add( 0 );

			int random = Utility.Random( 0, randomIndex.Count );

			return (int) randomIndex[random];
		}

		public static void OnMovement( MegaSpawner megaSpawner, Mobile mobile, Point3D oldLocation )
		{
			if ( EventSpawnAttempt( megaSpawner, SpawnType.Proximity ) )
			{
				if ( Utility.InRange( megaSpawner.Location, mobile.Location, megaSpawner.OverrideEventRange ) )
				{
					int entryCount = CountEntries( megaSpawner );

					if ( megaSpawner.OverrideGroupSpawn && entryCount != 0 )
						return;

					if ( ( megaSpawner.OverrideGroupSpawn && entryCount == 0 ) || CheckEventAmbush( megaSpawner ) )
						FullSpawn( megaSpawner );
					else
						SingleSpawn( megaSpawner );
				}

				megaSpawner.OverrideTriggerEventNow = false;
			}
		}

		public static void OnSpeech( MegaSpawner megaSpawner, SpeechEventArgs e )
		{
			Mobile mobile = e.Mobile;

			if ( EventSpawnAttempt( megaSpawner, SpawnType.Speech ) )
			{
				if ( Utility.InRange( megaSpawner.Location, mobile.Location, megaSpawner.OverrideEventRange ) )
				{
					int entryCount = CountEntries( megaSpawner );

					if ( megaSpawner.OverrideCaseSensitive && megaSpawner.OverrideEventKeyword != e.Speech )
						return;
					else if ( megaSpawner.OverrideEventKeyword.ToLower() != e.Speech.ToLower() )
						return;

					if ( megaSpawner.OverrideGroupSpawn && entryCount != 0 )
						return;

					if ( ( megaSpawner.OverrideGroupSpawn && entryCount == 0 ) || CheckEventAmbush( megaSpawner ) )
						FullSpawn( megaSpawner );
					else
						SingleSpawn( megaSpawner );
				}

				megaSpawner.OverrideTriggerEventNow = false;
			}
		}
	}
}
