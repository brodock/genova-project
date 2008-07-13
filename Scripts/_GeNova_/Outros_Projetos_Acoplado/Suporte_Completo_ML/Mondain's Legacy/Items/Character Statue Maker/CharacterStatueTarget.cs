using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Spells;
using Server.Mobiles;
using Server.Multis;
using Server.Gumps;
using Server.Targeting;

namespace Server.Targets
{	
	public class CharacterStatueTarget : Target
	{
		private Item m_Maker;
		private StatueType m_Type;
	
		public CharacterStatueTarget( Item maker, StatueType type ) : base( -1, true, TargetFlags.None )
		{
			m_Maker = maker;
			m_Type = type;
		}
		
		protected override void OnTarget( Mobile from, object targeted )
		{
			IPoint3D p = targeted as IPoint3D;
			Map map = from.Map;

			if ( p == null || map == null || m_Maker == null || m_Maker.Deleted )
				return;
				
			if ( m_Maker.IsChildOf( from.Backpack ) )
			{
				SpellHelper.GetSurfaceTop( ref p );			
				BaseHouse house = null;
				Point3D loc = new Point3D( p );
				
				AddonFitResult result = CouldFit( loc, map, from, ref house );
								
				if ( result == AddonFitResult.Valid )
				{				
					CharacterStatue statue = new CharacterStatue( from, m_Type );
					CharacterStatuePlinth plinth = new CharacterStatuePlinth( statue );
										
					house.Addons.Add( plinth );
					
					statue.Plinth = plinth;
					plinth.MoveToWorld( loc, map );
					statue.InvalidatePose();
					
					from.SendGump( new CharacterStatueGump( m_Maker, statue ) );
				}
				else if ( result == AddonFitResult.Blocked )
					from.SendLocalizedMessage( 500269 ); // You cannot build that there.
				else if ( result == AddonFitResult.NotInHouse )
					from.SendLocalizedMessage( 1076192 ); // Statues can only be placed in houses where you are the owner or co-owner.
				else if ( result == AddonFitResult.DoorsNotClosed )
					from.SendMessage( "You must close all house doors before placing this." );
				else if ( result == AddonFitResult.DoorTooClose )
					from.SendLocalizedMessage( 500271 ); // You cannot build near the door.				
			}
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}
		
		public static AddonFitResult CouldFit( Point3D p, Map map, Mobile from, ref BaseHouse house )
		{
			if ( !map.CanFit( p.X, p.Y, p.Z, 20, false, true, true ) )
				return AddonFitResult.Blocked;
			else if ( !BaseAddon.CheckHouse( from, p, map, 20, ref house ) )
				return AddonFitResult.NotInHouse;
			else
				return CheckDoors( p, 20, house );
		}
		
		public static AddonFitResult CheckDoors( Point3D p, int height, BaseHouse house )
		{
			ArrayList doors = house.Doors;

			for ( int i = 0; i < doors.Count; i ++ )
			{
				BaseDoor door = doors[ i ] as BaseDoor;

				if ( door != null && door.Open )
					return AddonFitResult.DoorsNotClosed;

				Point3D doorLoc = door.GetWorldLocation();
				int doorHeight = door.ItemData.CalcHeight;
				
				if ( Utility.InRange( doorLoc, p, 1 ) && (p.Z == doorLoc.Z || ((p.Z + height) > doorLoc.Z && (doorLoc.Z + doorHeight) > p.Z)) )
					return AddonFitResult.DoorTooClose;
			}
			
			return AddonFitResult.Valid;
		}
	}
}