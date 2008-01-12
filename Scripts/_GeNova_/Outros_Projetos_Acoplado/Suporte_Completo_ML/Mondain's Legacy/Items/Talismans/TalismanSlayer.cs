using System;
using System.Collections;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public enum TalismanSlayerName
	{			
		None		= 0,
		Bear		= 1072504, // Bear Slayer
		Vermin		= 1072505, // Vermin Slayer
		Bat			= 1072506, // Bat Slayer
		Mage		= 1072507, // Mage Slayer
		Beetle		= 1072508, // Beetle Slayer
		Bird		= 1072509, // Bird Slayer
		Ice			= 1072510, // Ice Slayer
		Flame		= 1072511, // Flame Slayer
		Bovine		= 1072512, // Bovine Slayer
		Wolf		= 1075462, // Wolf Slayer
	}	
	
	public class TalismanSlayer
	{			
		private static Hashtable m_Table;
		
		public static void InitSlayer()
		{
			m_Table = new Hashtable();
			
			m_Table[ TalismanSlayerName.Bear ] = new Type[] 
			{ 
				typeof( GrizzlyBear ), typeof( BlackBear ), typeof( BrownBear ), typeof( PolarBear ), typeof( Grobu )
			};
			
			m_Table[ TalismanSlayerName.Vermin ] = new Type[] 
			{ 
				typeof( RatmanMage ), typeof( RatmanMage ), 
				typeof( Ratman ), typeof( Sewerrat ), typeof( Rat ), typeof( GiantRat ), typeof( Chiikkaha )
			};
			
			m_Table[ TalismanSlayerName.Bat ] = new Type[] 
			{ 
				typeof( Mongbat ), typeof( StrongMongbat ), typeof( VampireBat ) 
			};
			
			m_Table[ TalismanSlayerName.Mage ] = new Type[] 
			{ 
				typeof( EvilMage ), typeof( EvilMageLord ), typeof( AncientLich ), typeof( Lich ), typeof( LichLord ),
				typeof( SkeletalMage ), typeof( BoneMagi ), typeof( OrcishMage ), typeof( KhaldunZealot ), typeof( JukaMage )
			};
			
			m_Table[ TalismanSlayerName.Beetle ] = new Type[] 
			{ 
				typeof( Beetle ), typeof( RuneBeetle ), typeof( FireBeetle ), typeof( DeathwatchBeetle ), 
				typeof( DeathwatchBeetleHatchling ) 
			};
			
			m_Table[ TalismanSlayerName.Bird ] = new Type[] 
			{ 
				typeof( Bird ), typeof( TropicalBird ), typeof( Chicken ), typeof( Crane ), 
				typeof( DesertOstard ), typeof( Eagle ), typeof( ForestOstard ), typeof( FrenziedOstard ), 
				typeof( Phoenix ), typeof( Pyre ), typeof( Swoop )
			};
			
			m_Table[ TalismanSlayerName.Ice ] = new Type[] 
			{ 
				typeof( IceElemental ), typeof( IceFiend ), typeof( IceSnake ), typeof( IceFiend ),
				typeof( FrostOoze ), typeof( SnowLeopard ), typeof( LadyOfTheSnow ), typeof( SnowElemental ),
				typeof( PolarBear ), typeof( FrostTroll ), typeof( IceSerpent ), typeof( GiantIceWorm ),
				typeof( FrostSpider )
			};
			
			m_Table[ TalismanSlayerName.Flame ] = new Type[] 
			{ 
				typeof( FireElemental ), typeof( HellHound ), typeof( HellCat ), typeof( FireSteed ),
				typeof( PredatorHellCat ), typeof( LavaSerpent ), typeof( LavaSnake ), typeof( LavaLizard ) 
			};
			
			m_Table[ TalismanSlayerName.Bovine ] = new Type[] 
			{ 
				typeof( Cow ), typeof( Bull ), typeof( Gaman ), typeof( MinotaurCaptain ), typeof( MinotaurScout ), typeof( Minotaur ) // typeof( TormentedMinotaur )
			};
			
			m_Table[ TalismanSlayerName.Wolf ] = new Type[]
			{
				typeof( BakeKitsune ), typeof( DireWolf ), typeof( GreyWolf ), typeof( TimberWolf ),
				typeof( WhiteWolf ), typeof( TsukiWolf ), typeof( Gnaw )
			};
		}
		
		public static Type[] GetSlayer( TalismanSlayerName name )
		{
			if ( m_Table == null )
				InitSlayer();
				
			return (Type[]) m_Table[ name ];
		}
		
		public static bool Check( TalismanSlayerName name, BaseCreature creature )
		{
			Type[] types = GetSlayer( name );
			
			if ( types == null || creature == null )
				return false;
				
			for ( int i = 0; i < types.Length; i ++ )
			{
				Type type = types[ i ];
				
				if ( type == creature.GetType() )
					return true;
			}
			
			return false;
		}
	}
}