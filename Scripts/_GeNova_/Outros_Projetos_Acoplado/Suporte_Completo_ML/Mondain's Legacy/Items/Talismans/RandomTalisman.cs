using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Items
{
	public class RandomTalisman : BaseTalisman
	{
		[Constructable]
		public RandomTalisman() : base( GetRandomID() )
		{
			MaxCharges = GetRandomCharges();		
			Summoner = GetRandomSummoner();
			
			if ( Summoner == null )
			{
				Removal = GetRandomRemoval();
				
				if ( Removal != TalismanRemoval.None )
					MaxChargeTime = 1200;
			}
			else if ( MaxCharges > 0 )
			{
				if (Summoner.Type == typeof( IronIngot ) ||
					Summoner.Type == typeof( Board ) ||
					Summoner.Type == typeof( Bandage ) 
				)
					MaxChargeTime = 60;
				else
					MaxChargeTime = 1800;
			}	
			else
				MaxChargeTime = 1800;
				
			Blessed = GetRandomBlessed();
			Slayer = GetRandomSlayer();	
			Protection = GetRandomProtection();
			Killer = GetRandomKiller();
			Skill = GetRandomSkill(); 
			ExceptionalBonus = GetRandomExceptional();	
			SuccessBonus = GetRandomSuccessful();	
			
			if ( Summoner == null && Removal == TalismanRemoval.None )
			{
				MaxCharges = 0;
				MaxChargeTime = 0;
			}
			else
				Charges = MaxCharges;
				
		}
		
		public RandomTalisman( Serial serial ) :  base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
		
		#region randomize
		private static int[] m_ItemID = new int[] { 0x2F58, 0x2F59, 0x2F5A, 0x2F5B };
		
		public static int GetRandomID()
		{
			return m_ItemID[ Utility.Random( m_ItemID.Length ) ];
		} 
		
		private static Type[] m_Summons = new Type[]
		{
			typeof( SummonedAntLion ),
			typeof( SummonedCow ),
			typeof( SummonedLavaSerpent ),
			typeof( SummonedOrcBrute ),
			typeof( SummonedFrostSpider ), 
			typeof( SummonedPanther ),
			typeof( SummonedDoppleganger ), 
			typeof( SummonedGreatHart ),
			typeof( SummonedBullFrog ),
			typeof( SummonedArcticOgreLord ),
			typeof( SummonedBogling ),
			typeof( SummonedBakeKitsune ),
			typeof( SummonedSheep ),
			typeof( SummonedSkeletalKnight ),
			typeof( SummonedWailingBanshee ),
			typeof( SummonedChicken ),
			typeof( SummonedVorpalBunny ),
			
			typeof( Board ),
			typeof( IronIngot ),
			typeof( Bandage ),
		};
		
		private static int[] m_SummonLabels = new int[]
		{
			1075211, // Ant Lion
			1072494, // Cow
			1072434, // Lava Serpent
			1072414, // Orc Brute
			1072476, // Frost Spider
			1029653, // Panther 
			1029741, // Doppleganger
			1018292, // great hart
			1028496, // bullfrog
			1018227, // arctic ogre lord
			1029735, // Bogling
			1030083, // bake-kitsune
			1018285, // sheep
			1018239, // skeletal knight
			1072399, // Wailing Banshee
			1072459, // Chicken
			1072401, // Vorpal Bunny
			
			1015101, // Boards
			1044036, // Ingots
			1023817, // clean bandage
		};
		
		public static Type GetRandomSummonType()
		{
			return m_Summons[ Utility.Random( m_Summons.Length ) ]; 
		}
		
		public static TalismanAttribute GetRandomSummoner()
		{
			if ( 0.025 > Utility.RandomDouble() )
			{
				int num = Utility.Random( m_Summons.Length );
				
				if ( num > 14 )
					return new TalismanAttribute( m_Summons[ num ], 10, m_SummonLabels[ num ] );
				else
					return new TalismanAttribute( m_Summons[ num ], 0, m_SummonLabels[ num ] );
			}
			
			return null;
		}
		
		public static TalismanRemoval GetRandomRemoval()
		{
			if ( 0.65 > Utility.RandomDouble() )
				return (TalismanRemoval) Utility.RandomList( 390, 404, 407 ); 
			
			return TalismanRemoval.None;
		}
		
		private static Type[] m_Killers = new Type[]
		{
			typeof( OrcBomber ), 	typeof( OrcBrute ), 				typeof( Sewerrat ), 		typeof( Rat ), 				typeof( GiantRat ), 
			typeof( Ratman ), 		typeof( RatmanArcher ), 			typeof( GiantSpider ), 		typeof( FrostSpider ), 		typeof( GiantBlackWidow ), 
			typeof( DreadSpider ), 	typeof( SilverSerpent ), 			typeof( DeepSeaSerpent ), 	typeof( GiantSerpent ), 	typeof( Snake ), 
			typeof( IceSnake ), 	typeof( IceSerpent ), 				typeof( LavaSerpent ), 		typeof( LavaSnake ),		typeof( Yamandon ),		
			typeof( StrongMongbat ),typeof( Mongbat ), 					typeof( VampireBat ), 		typeof( Lich ),				typeof( EvilMage ), 	
			typeof( LichLord ),		typeof( EvilMageLord ), 			typeof( SkeletalMage ), 	typeof( KhaldunZealot ), 	typeof( AncientLich ), 	
			typeof( JukaMage ), 	typeof( MeerMage ), 				typeof( Beetle ), 			typeof( DeathwatchBeetle ), typeof( RuneBeetle ),	
			typeof( FireBeetle ),	typeof( DeathwatchBeetleHatchling), typeof( Bird ), 			typeof( Chicken ), 			typeof( Eagle ), 		
			typeof( TropicalBird ), typeof( Phoenix ), 					typeof( DesertOstard ), 	typeof( FrenziedOstard ), 	typeof( ForestOstard ), 	
			typeof( Crane ),		typeof( SnowLeopard ), 				typeof( IceFiend ), 		typeof( FrostOoze ), 		typeof( FrostTroll ), 		
			typeof( IceElemental ),	typeof( SnowElemental ), 			typeof( GiantIceWorm ), 	typeof( LadyOfTheSnow ), 	typeof( FireElemental ), 	
			typeof( FireSteed ), 	typeof( HellHound ), 				typeof( HellCat ), 			typeof( PredatorHellCat ), 	typeof( LavaLizard ), 		
			typeof( FireBeetle ), 	typeof( Cow ), 						typeof( Bull ), 			typeof( Gaman ) // TODO Meraktus, Tormented Minotaur, Minotaur
		};
		
		private static int[] m_KillerLabels = new int[]
		{
			1072413, 1072414, 1072418, 1072419, 1072420, 
			1072421, 1072423, 1072424, 1072425, 1072426, 
			1072427, 1072428, 1072429, 1072430, 1072431, 
			1072432, 1072433, 1072434, 1072435, 1072438, 
			1072440, 1072441, 1072443, 1072444, 1072445, 
			1072446, 1072447, 1072448, 1072449, 1072450, 
			1072451, 1072452, 1072453, 1072454, 1072455, 
			1072456, 1072457, 1072458, 1072459, 1072461, 
			1072462, 1072465, 1072468, 1072469, 1072470, 
			1072473, 1072474, 1072477, 1072478, 1072479, 
			1072480, 1072481, 1072483, 1072485, 1072486, 
			1072487, 1072489, 1072490, 1072491, 1072492, 
			1072493, 1072494, 1072495, 1072498, 
		};
		
		public static TalismanAttribute GetRandomKiller()
		{
			if ( 0.5 > Utility.RandomDouble() )
			{	
				int num = Utility.Random( m_Killers.Length );
				
				return new TalismanAttribute( m_Killers[ num ], Utility.RandomMinMax( 10, 100 ), m_KillerLabels[ num ] );
			}
			
			return null;
		}
		
		public static TalismanAttribute GetRandomProtection()
		{
			if ( 0.5 > Utility.RandomDouble() )
			{	
				int num = Utility.Random( m_Killers.Length );
				
				return new TalismanAttribute( m_Killers[ num ], Utility.RandomMinMax( 5, 60 ), m_KillerLabels[ num ] );
			}
			
			return null;
		}
		
		private static SkillName[] m_Skills = new SkillName[]
		{
			SkillName.Alchemy, 
			SkillName.Blacksmith, 
			SkillName.Carpentry,
			SkillName.Cartography, 
			SkillName.Cooking, 
			SkillName.Fletching, 
			SkillName.Inscribe, 
			SkillName.Tailoring, 
			SkillName.Tinkering,
		};
		
		public static SkillName GetRandomSkill()
		{
			return m_Skills[ Utility.Random( m_Skills.Length ) ];
		}
		
		public static int GetRandomExceptional()
		{
			if ( 0.3 > Utility.RandomDouble() )
			{
				double num = 40 - Math.Log( Utility.RandomMinMax( 5, 149 ) ) * 6;
				
				return (int) Math.Round( num );
			}			
			
			return 0;
		}
		
		public static int GetRandomSuccessful()
		{
			if ( 0.75 > Utility.RandomDouble() )
			{
				double num = 40 - Math.Log( Utility.RandomMinMax( 5, 149 ) ) * 6;
				
				return (int) Math.Round( num );
			}			
			
			return 0;
		}
		
		public static bool GetRandomBlessed()
		{
			if ( 0.02 > Utility.RandomDouble() )
				return true;
				
			return false;
		}		
		
		public static TalismanSlayerName GetRandomSlayer()
		{
			if ( 0.01 > Utility.RandomDouble() )
			{
				if ( 0.9 > Utility.RandomDouble() ) 
					return (TalismanSlayerName) Utility.Random( 1072504, 9 );				
				else
					return TalismanSlayerName.Wolf;
			}
			
			return TalismanSlayerName.None;
		}
		
		public static int GetRandomCharges()
		{
			if ( 0.5 > Utility.RandomDouble() )
				return Utility.RandomMinMax( 10, 50 );
				
			return -1;
		}
		#endregion
	}
}