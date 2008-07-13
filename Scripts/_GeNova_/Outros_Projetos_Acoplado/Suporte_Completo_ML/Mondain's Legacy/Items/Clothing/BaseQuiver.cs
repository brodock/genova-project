using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Targeting;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
	public class BaseQuiver : Container, ICraftable, ISetItem
	{				
		public override Rectangle2D Bounds{ get{ return new Rectangle2D( 25, 30, 120, 75 ); } }
		public override int DefaultGumpID{ get{ return 0x108; } }
		public override int DefaultMaxItems{ get{ return 1; } }
		public override int DefaultMaxWeight{ get{ return 50; } }
		
		private AosAttributes m_Attributes;
		private AosElementAttributes m_DamageModifier;
		private int m_Capacity;
		private int m_LowerAmmoCost;
		private int m_WeightReduction;
		private int m_DamageIncrease;
		
		[CommandProperty( AccessLevel.GameMaster)]
		public AosAttributes Attributes
		{
			get{ return m_Attributes; }
			set{}
		}			
		
		[CommandProperty( AccessLevel.GameMaster)]
		public AosElementAttributes DamageModifier
		{
			get{ return m_DamageModifier; }
			set{}
		}		
		
		[CommandProperty( AccessLevel.GameMaster)]
		public int Capacity
		{
			get{ return m_Capacity; }
			set{ m_Capacity = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster)]
		public int LowerAmmoCost
		{
			get{ return m_LowerAmmoCost; }
			set{ m_LowerAmmoCost = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster)]
		public int WeightReduction
		{
			get{ return m_WeightReduction; }
			set{ m_WeightReduction = value; InvalidateProperties(); }
		}
		
		// damage increased after resists applied
		[CommandProperty( AccessLevel.GameMaster)]
		public int DamageIncrease
		{
			get{ return m_DamageIncrease; }
			set{ m_DamageIncrease = value; InvalidateProperties(); }
		}		
		
		#region Craftable
		private Mobile m_Crafter;
		private ClothingQuality m_Quality;
		private bool m_PlayerConstructed;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); }
		}		
		
		[CommandProperty( AccessLevel.GameMaster )]
		public ClothingQuality Quality
		{
			get{ return m_Quality; }
			set{ m_Quality = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}
		
		public Item Ammo
		{
			get{ return Items.Count == 1 ? Items[ 0 ] : null; }
		}
		#endregion
		
		#region Set Armor
		public virtual SetItem SetID{ get{ return SetItem.None; } }
		public virtual int Pieces{ get{ return 0; } }
		public virtual bool MixedSet{ get{ return false; } }
		
		public bool IsSetItem{ get{ return SetID == SetItem.None ? false : true; } }
		
		private int m_SetHue;
		private bool m_SetEquipped;
		private bool m_LastEquipped;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SetHue
		{
			get{ return m_SetHue; }
			set{ m_SetHue = value; InvalidateProperties(); }
		}
		
		public bool SetEquipped
		{
			get{ return m_SetEquipped; }
			set{ m_SetEquipped = value; }
		}
		
		public bool LastEquipped
		{
			get{ return m_LastEquipped; }
			set{ m_LastEquipped = value; }
		}		
		
		private AosAttributes m_SetAttributes;
		private AosSkillBonuses m_SetSkillBonuses;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes SetAttributes
		{
			get{ return m_SetAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SetSkillBonuses
		{
			get{ return m_SetSkillBonuses; }
			set{}
		}	
		#endregion
		
		public BaseQuiver() : this( 0x2FB7 )
		{
		}
		
		public BaseQuiver( int itemID ) : base( itemID )
		{
			Weight = 2;
			Capacity = 500;
			Layer = Layer.Cloak;
			
			m_Attributes = new AosAttributes( this );
			m_SetAttributes = new AosAttributes( this );
			m_SetSkillBonuses = new AosSkillBonuses( this );
			m_DamageModifier = new AosElementAttributes( this );
		}
		
		public BaseQuiver( Serial serial ) : base( serial )
		{
		}		
		
		public override int GetTotal( TotalType type )
		{
			if ( type != TotalType.Weight )
				return base.GetTotal( type );
				
			double weight = 0;
				
			if ( Items.Count == 1 )
				weight += Items[ 0 ].PileWeight;
				
			if ( weight > 0 && m_WeightReduction != 0 )
				weight -= weight * m_WeightReduction / (double) 100;  
				
			return (int) ( weight + Weight );
		}
		
		public override bool CheckHold( Mobile m, Item item, bool message, bool checkItems, int plusItems, int plusWeight )
		{
			if ( !CheckType( item ) )
			{
				m.SendLocalizedMessage( 1074836 ); // The container can not hold that type of object.
				return false;
			}
		
			if ( Items.Count < DefaultMaxItems )
			{
				if ( item.Amount <= m_Capacity )
					return base.CheckHold( m, item, message, checkItems, plusItems, plusWeight );
			}
			else
			{				
				Item ammo = Ammo;
				
				if ( ammo == null || ammo.Deleted )
					return false;
				
				if ( ammo.Amount + item.Amount <= m_Capacity )
					return true;
			}
			
			return false;
		}
		
		public override void AddItem( Item dropped )
		{
			base.AddItem( dropped );
			
			InvalidateWeight();
		}
		
		public override void RemoveItem( Item dropped )
		{
			base.RemoveItem( dropped );
						
			InvalidateWeight();
		}
		
		public override void OnAdded( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile mob = (Mobile) parent;
				
				BaseRanged ranged = mob.Weapon as BaseRanged;
				
				if ( ranged != null )
					ranged.InvalidateProperties();	
								
				#region Set Armor
				if ( IsSetItem )
					m_SetEquipped = SetHelper.FullSetPresent( mob, SetID, Pieces );
				
				if ( m_SetEquipped )
				{
					m_LastEquipped = true;				
					
					SetHelper.AddSetBonus( mob, SetID );
				}
				#endregion
			}
		}
		
		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile mob = (Mobile) parent;
				
				BaseRanged ranged = mob.Weapon as BaseRanged;
				
				if ( ranged != null )
					ranged.InvalidateProperties();							
				
				string modName = this.Serial.ToString();

				mob.RemoveStatMod( modName + "Str" );
				mob.RemoveStatMod( modName + "Dex" );
				mob.RemoveStatMod( modName + "Int" );

				mob.CheckStatTimers();					
				
				if ( IsSetItem ? m_SetEquipped : false )
					SetHelper.RemoveSetBonus( mob, SetID, this );
			}
		}	
		
		public override bool OnDragLift( Mobile from )
		{
			#region Set Armor
			if ( Parent is Mobile && from == Parent )
			{
				Mobile m = (Mobile) Parent;
			
				if ( IsSetItem ? m_SetEquipped : false )
					SetHelper.RemoveSetBonus( from, SetID, this );
			}			
			#endregion
			
			return base.OnDragLift( from );
		}

		public override bool OnEquip( Mobile from )
		{
			from.CheckStatTimers();

			int strBonus = m_Attributes.BonusStr;
			int dexBonus = m_Attributes.BonusDex;
			int intBonus = m_Attributes.BonusInt;

			if ( strBonus != 0 || dexBonus != 0 || intBonus != 0 )
			{
				string modName = this.Serial.ToString();

				if ( strBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			return base.OnEquip( from );
		}
		
		public override bool CanEquip( Mobile m )
		{
			if ( m.NetState != null && !m.NetState.SupportsExpansion( Expansion.ML ) )
			{
				m.SendLocalizedMessage( 1072791 ); // You must upgrade to Mondain's Legacy in order to use that item.				
				return false;
			}
			
			return true;
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
				
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~
							
			if ( m_Quality == ClothingQuality.Exceptional )
				list.Add( 1063341 ); // exceptional
			
			if ( Ammo != null )
			{				
				if ( Ammo.GetType() == typeof( Arrow ) )
					list.Add( 1075265, "{0}\t{1}", Ammo.Amount, Capacity ); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ arrows
				else if ( Ammo.GetType() == typeof( Bolt ) )
					list.Add( 1075266, "{0}\t{1}", Ammo.Amount, Capacity ); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ bolts
			}			
			else
				list.Add( 1075265, "{0}\t{1}", 0, Capacity ); // Ammo: ~1_QUANTITY~/~2_CAPACITY~ arrows			
			
			int prop;		
				
			if ( (prop = m_DamageIncrease) != 0 )
				list.Add( 1074762, prop.ToString() ); // Damage modifier: ~1_PERCENT~%
			
			if ( (prop = m_DamageModifier.Direct) != 0 )
				list.Add( 1079978, prop.ToString() ); // Direct Damage: ~1_PERCENT~%				
				
			if ( (prop = m_DamageModifier.Chaos) != 0 )
				list.Add( 1072846, prop.ToString() ); // chaos damage ~1_val~%
			
			if ( (prop = m_DamageModifier.Physical) != 0 )
				list.Add( 1060403, prop.ToString() ); // physical damage ~1_val~%
				
			if ( (prop = m_DamageModifier.Fire) != 0 )
				list.Add( 1060405, prop.ToString() ); // fire damage ~1_val~%
			
			if ( (prop = m_DamageModifier.Cold) != 0 )
				list.Add( 1060404, prop.ToString() ); // cold damage ~1_val~%
				
			if ( (prop = m_DamageModifier.Poison) != 0 )
				list.Add( 1060406, prop.ToString() ); // poison damage ~1_val~%
				
			if ( (prop = m_DamageModifier.Energy) != 0 )
				list.Add( 1060407, prop.ToString() ); // energy damage ~1_val~%
				
			list.Add( 1075085 ); // Requirement: Mondain's Legacy		

			if ( (prop = m_Attributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_Attributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_Attributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_Attributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_Attributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = m_Attributes.AttackChance) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_Attributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_Attributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_Attributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = m_Attributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%	
			
			if ( (prop = m_Attributes.Luck) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~

			if ( (prop = m_Attributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_Attributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_Attributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_Attributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_Attributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_Attributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~

			if ( (prop = m_Attributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_Attributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_Attributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_Attributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%
					
			if ( (prop = m_LowerAmmoCost) > 0 )
				list.Add( 1075208, prop.ToString() ); // Lower Ammo Cost ~1_Percentage~%
				
			#region Set Armor
			if ( IsSetItem )
			{
				list.Add( 1073491, Pieces.ToString() ); // Part of a Weapon/Armor Set (~1_val~ pieces)
					
				if ( m_SetEquipped )
				{
					list.Add( 1073492 ); // Full Weapon/Armor Set Present				
					
					SetHelper.GetSetProperties( list, m_Attributes, m_SetAttributes, m_SetEquipped );
				}
			}
			#endregion	
				
			double weight = 0;
			
			if ( Items.Count > 0 )
				weight = Items[ 0 ].Weight;
			
			list.Add( 1072241, "{0}\t{1}\t{2}\t{3}", Items.Count, DefaultMaxItems, TotalWeight - Weight, DefaultMaxWeight ); // Contents: ~1_COUNT~/~2_MAXCOUNT items, ~3_WEIGHT~/~4_MAXWEIGHT~ stones
			
			if ( (prop = m_WeightReduction) != 0 )
				list.Add( 1072210, prop.ToString() ); // Weight reduction: ~1_PERCENTAGE~%	
				
			
			#region Set Armor
			if ( IsSetItem && !m_SetEquipped )
			{
				list.Add( 1072378 ); // <br>Only when full set is present:
				
				SetHelper.GetSetProperties( list, m_Attributes, m_SetAttributes, m_SetEquipped );
			}
			#endregion		
		}
		
		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}	

		[Flags]
		private enum SaveFlag
		{
			None				= 0x00000000,
			Attributes			= 0x00000001,
			DamageModifier		= 0x00000002,
			LowerAmmoCost		= 0x00000004,		
			WeightReduction		= 0x00000008,	
			Crafter				= 0x00000010,
			Quality				= 0x00000020,
			PlayerConstructed	= 0x00000040,
			Capacity			= 0x00000080,
			
			#region Set Armor
			SetAttributes		= 0x00000100,
			SetHue				= 0x00000200,
			LastEquipped		= 0x00000400,
			SetEquipped			= 0x00000800,
			#endregion
			
			DamageIncrease		= 0x00001000
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( 0 ); // version
			
			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Attributes,		!m_Attributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.DamageModifier,	!m_DamageModifier.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.LowerAmmoCost,		m_LowerAmmoCost != 0 );
			SetSaveFlag( ref flags, SaveFlag.WeightReduction,	m_WeightReduction != 0 );
			SetSaveFlag( ref flags, SaveFlag.DamageIncrease,	m_DamageIncrease != 0 );
			SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
			SetSaveFlag( ref flags, SaveFlag.Quality,			true );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed );
			SetSaveFlag( ref flags, SaveFlag.Capacity,			m_Capacity > 0 );
			
			#region Set Armor
			SetSaveFlag( ref flags, SaveFlag.SetAttributes,		!m_SetAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.SetHue,			m_SetHue != 0 );
			SetSaveFlag( ref flags, SaveFlag.LastEquipped,		true );			
			SetSaveFlag( ref flags, SaveFlag.SetEquipped,		true );
			#endregion
			
			writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_Attributes.Serialize( writer );
				
			if ( GetSaveFlag( flags, SaveFlag.DamageModifier ) )
				m_DamageModifier.Serialize( writer );
				
			if ( GetSaveFlag( flags, SaveFlag.LowerAmmoCost ) )
				writer.Write( (int) m_LowerAmmoCost );
				
			if ( GetSaveFlag( flags, SaveFlag.WeightReduction ) )
				writer.Write( (int) m_WeightReduction );
				
			if ( GetSaveFlag( flags, SaveFlag.DamageIncrease ) )
				writer.Write( (int) m_DamageIncrease );
				
			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( (Mobile) m_Crafter );
				
			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.Write( (int) m_Quality );
				
			if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
				writer.Write( (bool) m_PlayerConstructed );
				
			if ( GetSaveFlag( flags, SaveFlag.Capacity ) )
				writer.Write( (int) m_Capacity );
				
			#region Set Armor			
			if ( GetSaveFlag( flags, SaveFlag.SetAttributes ) )
				m_SetAttributes.Serialize( writer );
				
			if ( GetSaveFlag( flags, SaveFlag.SetHue ) )
				writer.Write( (int) m_SetHue );
				
			if ( GetSaveFlag( flags, SaveFlag.LastEquipped ) )
				writer.Write( (bool) m_LastEquipped );
				
			if ( GetSaveFlag( flags, SaveFlag.SetEquipped ) )
				writer.Write( (bool) m_SetEquipped );
			#endregion
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			SaveFlag flags = (SaveFlag) reader.ReadEncodedInt();
			
			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_Attributes = new AosAttributes( this, reader );
			else
				m_Attributes = new AosAttributes( this );
				
			if ( GetSaveFlag( flags, SaveFlag.DamageModifier ) )
				m_DamageModifier = new AosElementAttributes( this, reader );
			else
				m_DamageModifier = new AosElementAttributes( this );
				
			if ( GetSaveFlag( flags, SaveFlag.LowerAmmoCost ) )
				m_LowerAmmoCost = reader.ReadInt();
				
			if ( GetSaveFlag( flags, SaveFlag.WeightReduction ) )
				m_WeightReduction = reader.ReadInt();
				
			if ( GetSaveFlag( flags, SaveFlag.DamageIncrease ) )
				m_DamageIncrease = reader.ReadInt();
				
			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				m_Crafter = reader.ReadMobile();
				
			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				m_Quality = (ClothingQuality) reader.ReadInt();
				
			if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
				m_PlayerConstructed = reader.ReadBool();
				
			if ( GetSaveFlag( flags, SaveFlag.Capacity ) )
				m_Capacity = reader.ReadInt();
							
			#region Set Armor		
			if ( m_SetSkillBonuses == null )
				m_SetSkillBonuses = new AosSkillBonuses( this );
				
			if ( GetSaveFlag( flags, SaveFlag.SetAttributes ) )
				m_SetAttributes = new AosAttributes( this, reader );
			else
				m_SetAttributes = new AosAttributes( this );
				
			if ( GetSaveFlag( flags, SaveFlag.SetHue ) )
				m_SetHue = reader.ReadInt();
				
			if ( GetSaveFlag( flags, SaveFlag.LastEquipped ) )
				m_LastEquipped = reader.ReadBool();
									
			if ( GetSaveFlag( flags, SaveFlag.SetEquipped ) )
				m_SetEquipped = reader.ReadBool();
			#endregion
			
			if ( version == 1 )
				Layer = Layer.Cloak;
		}
		
		#region Virtual members		
		public virtual void GetDamageTypes( out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			phys = m_DamageModifier.Physical;
			fire = m_DamageModifier.Fire;
			cold = m_DamageModifier.Cold;
			pois = m_DamageModifier.Poison;
			nrgy = m_DamageModifier.Energy;
			chaos = m_DamageModifier.Chaos;
			direct = m_DamageModifier.Direct;
		}
		
		public virtual bool ConsumeAmmo( Type ammo )
		{
			if ( m_LowerAmmoCost / 100 > Utility.RandomDouble() ) 
				return true;
		
			if ( Items.Count > 0 )
			{
				Item item = Items[ 0 ];
				
				if ( ammo.IsAssignableFrom( item.GetType() ) )
				{						
					if ( item.Amount > 1 )
						item.Amount -= 1;
					else
						item.Delete();
						
					InvalidateProperties();	
					
					return true;
				}					
			}
			
			return false;
		}		
		#endregion
		
		#region Static members
		private static Type[] m_Ammo = new Type[]
		{
			typeof( Arrow ), typeof( Bolt )
		};
		#endregion
		
		#region Members		
		public bool CheckType( Item item )
		{		
			Type type = item.GetType();
			Item ammo = Ammo;
			
			if ( ammo != null )
			{
				if ( ammo.GetType() == type )
					return true;
				
				return false;
			}
		
			for ( int i = 0; i < m_Ammo.Length; i ++ )
			{
				if ( m_Ammo[ i ].IsAssignableFrom( type ) )
					return true;
			} 
			
			return false;
		}
		
		public void InvalidateWeight()
		{						
			if ( RootParent is Mobile )
			{
				Mobile m = (Mobile) RootParent;				
				
				m.UpdateTotals();
				
				if ( m.NetState != null )						
					m.NetState.Send( new MobileStatusExtended( m ) );
			}
		}
		#endregion
		
		#region ICraftable
		public virtual int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ClothingQuality) quality;
			
			if ( makersMark )
				Crafter = from;
				
			PlayerConstructed = true;
			
			return quality;
		}
		#endregion
	}
}