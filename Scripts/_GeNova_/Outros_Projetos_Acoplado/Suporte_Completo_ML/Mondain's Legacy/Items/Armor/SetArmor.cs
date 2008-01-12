using System;
using Server;
using Server.Items;
using Server.Mobiles;

namespace Server
{	
	public interface ISetItem
	{
		SetItem SetID{ get; }	
		int Pieces{ get; }	
		int SetHue{ get; set; }		
		
		bool IsSetItem{ get; }
		bool SetEquipped{ get; set; }
		bool LastEquipped{ get; set; }		
		
		AosAttributes SetAttributes{ get; }
		AosSkillBonuses SetSkillBonuses{ get; }
	}

	public enum SetItem
	{
		None,
		Acolyte,
		Assassin,
		Darkwood,
		Grizzle,
		Hunter,
		Juggernaut,
		Mage,
		Marksman,
		Myrmidon,
		Necromancer,
		Paladin,
		Virtue
	}
	
	public static class SetHelper
	{	
		public static void GetSetProperties( ObjectPropertyList list, AosAttributes attributes, AosAttributes setAttributes, bool equipped )
		{
			int prop;
	            		
			if ( (prop = setAttributes.DefendChance) != 0 )
				list.Add( 1073493, prop.ToString() ); // defense chance increase ~1_val~% (total)
			
			if ( (prop = setAttributes.AttackChance) != 0 )
				list.Add( 1073490, prop.ToString() ); // hit chance increase ~1_val~% (total)
			
			if ( (prop = setAttributes.BonusStr) != 0 )
				list.Add( 1072514, prop.ToString() ); // strength bonus ~1_val~ (total)
			
			if ( (prop = setAttributes.BonusDex) != 0 )
				list.Add( 1072503, prop.ToString() ); // dexterity bonus ~1_val~ (total)
			
			if ( (prop = setAttributes.BonusInt) != 0 )
				list.Add( 1072381, prop.ToString() ); // intelligence bonus ~1_val~ (total)
			
			if ( (prop = setAttributes.LowerManaCost) != 0 )
				list.Add( 1073488, prop.ToString() ); // lower mana cost ~1_val~% (total)
			
			if ( (prop = setAttributes.Luck) != 0 )
				list.Add( 1073489, prop.ToString() ); // luck ~1_val~% (total)
			
			if ( (prop = setAttributes.ReflectPhysical) != 0 )
				list.Add( 1072513, prop.ToString() ); // reflect physical damage ~1_val~% (total)
			
			if ( (prop = setAttributes.SpellDamage) != 0 )
				list.Add( 1072380, prop.ToString() ); // spell damage increase ~1_val~% (total)
			
			if ( (prop = setAttributes.WeaponSpeed) != 0 )
				list.Add( 1074323, prop.ToString() ); // swing speed increase ~1_val~% (total)	
				
			if ( !equipped )
			{			
				if ( (prop = setAttributes.WeaponDamage) != 0 )
					list.Add( 1074325, prop.ToString() ); // increased weapon damage ~1_val~%
	
				if ( (prop = setAttributes.EnhancePotions) != 0 && attributes.EnhancePotions == 0 )
					list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%
	
				if ( (prop = setAttributes.CastRecovery) != 0 && attributes.CastRecovery == 0 )
					list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~
	
				if ( (prop = setAttributes.CastSpeed) != 0 && attributes.CastSpeed == 0 )
					list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~
	
				if ( (prop = setAttributes.LowerRegCost) != 0 && attributes.LowerRegCost == 0 )
					list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%
	
				if ( (prop = setAttributes.RegenHits) != 0 && attributes.RegenHits == 0 )
					list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~
	
				if ( (prop = setAttributes.RegenStam) != 0 && attributes.RegenStam == 0 )
					list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~
	
				if ( (prop = setAttributes.RegenMana) != 0 && attributes.RegenMana == 0 )
					list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~
	
				if ( (prop = setAttributes.BonusHits) != 0 && attributes.BonusHits == 0 )
					list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~
	
				if ( (prop = setAttributes.BonusStam) != 0 && attributes.BonusStam == 0 )
					list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~
	
				if ( (prop = setAttributes.BonusMana) != 0 && attributes.BonusMana == 0 )
					list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~
	
				if ( (prop = setAttributes.NightSight) != 0 && attributes.NightSight == 0 )
					list.Add( 1060441 ); // night sight
			}
		}
	
		public static void RemoveSetBonus( Mobile from, SetItem setID, Item item )
		{				
			bool self = false;
			
			for ( int i = 0; i < from.Items.Count; i ++ )
			{
				if ( from.Items[ i ] == item )
					self = true;
					
				Remove( from, setID, from.Items[ i ] );
			}	
			
			if ( !self )
				Remove( from, setID, item );
		}
		
		public static void Remove( Mobile from, SetItem setID, Item item )
		{						
			if ( item is ISetItem )
			{
				ISetItem setItem = (ISetItem) item;
				
				if ( setItem.IsSetItem && setItem.SetID == setID )
				{				
					if ( setItem.LastEquipped )
					{
						if ( from != null )
							RemoveStatBonuses( from, item );
						
						setItem.SetSkillBonuses.Remove();					
					}
										
					int temp = item.Hue;
					item.Hue = setItem.SetHue;
					setItem.SetHue = temp;
					
					setItem.SetEquipped = false;
					setItem.LastEquipped = false;		
					item.InvalidateProperties();
				}
			}
		}
		
		public static void AddSetBonus( Mobile to, SetItem setID )
		{
			int temp;
			
			for ( int i = 0; i < to.Items.Count; i ++ )
			{
				if ( to.Items[ i ] is ISetItem )
				{
					ISetItem setItem = (ISetItem) to.Items[ i ];
					
					if ( setItem.IsSetItem && setItem.SetID == setID )
					{				
						if ( setItem.LastEquipped )
						{
							AddStatBonuses( to, to.Items[ i ], setItem.SetAttributes.BonusStr, setItem.SetAttributes.BonusDex, setItem.SetAttributes.BonusInt );
							
							setItem.SetSkillBonuses.AddTo( to );	
						}
						
						temp = to.Items[ i ].Hue;
						to.Items[ i ].Hue = setItem.SetHue;
						setItem.SetHue = temp;
						
						setItem.SetEquipped = true;
						to.Items[ i ].InvalidateProperties();
					}
				}
			}				
			
			Effects.PlaySound( to.Location, to.Map, 0x1F7 );
			to.FixedParticles( 0x376A, 9, 32, 5030, EffectLayer.Waist );
			to.SendLocalizedMessage( 1072391 ); // The magic of your armor combines to assist you!
		}
		
		public static bool FullSetPresent( Mobile from, SetItem setID, int pieces )
		{
			int equipped = 0;
				
			for ( int i = 0; i < from.Items.Count && equipped < pieces; i ++ )
			{
				if ( from.Items[ i ] is ISetItem )
				{
					ISetItem setItem = (ISetItem) from.Items[ i ];
					
					if ( setItem.IsSetItem && setItem.SetID == setID )
						equipped += 1;
				}
			}
			
			if ( equipped == pieces )
				return true;
				
			return false;
		}
		
		public static void RemoveStatBonuses( Mobile from, Item item )
		{			
			string modName = item.Serial.ToString();
			
			from.RemoveStatMod( modName + "Str" );
			from.RemoveStatMod( modName + "Dex" );
			from.RemoveStatMod( modName + "Int" );
			
			from.Delta( MobileDelta.Armor );
			from.CheckStatTimers();
		}
		
		public static void AddStatBonuses( Mobile to, Item item, int str, int dex, int intel )
		{			
			if ( (str != 0 || dex != 0 || intel != 0) )
			{		
				string modName = item.Serial.ToString();

				if ( str != 0 )
					to.AddStatMod( new StatMod( StatType.Str, modName + "Str", str, TimeSpan.Zero ) );

				if ( dex != 0 )
					to.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dex, TimeSpan.Zero ) );

				if ( intel != 0 )
					to.AddStatMod( new StatMod( StatType.Int, modName + "Int", intel, TimeSpan.Zero ) );
			}
			
			to.CheckStatTimers();
		}
		
		public static bool SetCraftedWith( Mobile from, SetItem setID, int pieces, CraftResource resource )
		{
			int count = 0;
		
			for ( int i = 0; i < from.Items.Count && count < pieces; i ++ )
			{
				if ( from.Items[ i ] is BaseArmor )
				{
					BaseArmor armor = (BaseArmor) from.Items[ i ];
					
					if ( armor.IsSetItem && armor.SetID == setID && armor.Resource == resource )
						count += 1;
				}				
				else if ( from.Items[ i ] is BaseWeapon )
				{
					BaseWeapon weapon = (BaseWeapon) from.Items[ i ];
					
					if ( weapon.IsSetItem && weapon.SetID == setID && weapon.Resource == resource )
						count += 1;
				}
			}
			
			if ( count == pieces )
				return true;
			
			return false;
		}
	}
}