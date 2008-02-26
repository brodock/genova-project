using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.First;
using Server.Spells.Second;
using Server.Spells.Fourth;
using Server.Spells.Fifth;
using Server.Network;
using Server.ContextMenus;

namespace Server.Items
{
	public enum TalismanRemoval
	{
		None	= 0,
		Ward	= 390,
		Damage	= 404,
		Curse	= 407,
		// TODO Wildfire
	}

	public class BaseTalisman : Item
	{				
		public override int LabelNumber{ get{ return 1071023; } } // Talisman
		
		public virtual bool ForceShowName{ get{ return false; } } // used to override default summoner/removal name
	
		private int m_KarmaLoss;
		private int m_MaxCharges;
		private int m_Charges;		
		private int m_MaxChargeTime;
		private int m_ChargeTime;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int KarmaLoss
		{
			get{ return m_KarmaLoss; }
			set{ m_KarmaLoss = value; InvalidateProperties(); }
		}		
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxCharges
		{
			get{ return m_MaxCharges; }
			set{ m_MaxCharges = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Charges
		{
			get{ return m_Charges; }
			set{ m_Charges = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxChargeTime
		{
			get{ return m_MaxChargeTime; }
			set{ m_MaxChargeTime = value; InvalidateProperties(); }
		}
		
		public int ChargeTime
		{
			get{ return m_ChargeTime; }
			set{ m_ChargeTime = value; InvalidateProperties(); }
		}	
		
		#region slayer
		private TalismanSlayerName m_Slayer;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public TalismanSlayerName Slayer
		{
			get{ return m_Slayer; }
			set{ m_Slayer = value; InvalidateProperties(); }
		}	
		#endregion
		
		#region blessed		
		private Mobile m_Owner;
		private bool m_Blessed; 
				
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Owner
		{
			get{ return m_Owner; }
			set{ m_Owner = value; InvalidateProperties(); }
		}	
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Blessed
		{
			get{ return m_Blessed; }
			set{ m_Blessed = value; InvalidateProperties(); }
		}
		#endregion
		
		#region summoner/removal
		private TalismanAttribute m_Summoner;
		private TalismanRemoval m_Removal;
		private Mobile m_Creature;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public TalismanAttribute Summoner
		{
			get{ return m_Summoner; }
			set{ m_Summoner = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public TalismanRemoval Removal
		{
			get{ return m_Removal; }
			set{ m_Removal = value; InvalidateProperties(); }
		}
		#endregion
		
		#region protection/killer
		private TalismanAttribute m_Protection;		
		private TalismanAttribute m_Killer;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public TalismanAttribute Protection
		{
			get{ return m_Protection; }
			set{ m_Protection = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public TalismanAttribute Killer
		{
			get{ return m_Killer; }
			set{ m_Killer = value; InvalidateProperties(); }
		}
		#endregion
		
		#region craft bonuses
		private SkillName m_Skill;
		private int m_SuccessBonus;
		private int m_ExceptionalBonus;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public SkillName Skill
		{
			get{ return m_Skill; }
			set{ m_Skill = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int SuccessBonus
		{
			get{ return m_SuccessBonus; }
			set{ m_SuccessBonus = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int ExceptionalBonus
		{
			get{ return m_ExceptionalBonus; }
			set{ m_ExceptionalBonus = value; InvalidateProperties(); }
		}
		#endregion
		
		#region aos bonuses
		private AosAttributes m_AosAttributes;
		private AosSkillBonuses m_AosSkillBonuses;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}
		#endregion
	
		public BaseTalisman( int itemID ) : base( itemID )
		{
			Layer = Layer.Talisman;
			Weight = 1.0;
			
			m_AosAttributes = new AosAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
		}
		
		public BaseTalisman( Serial serial ) :  base( serial )
		{
		}
		
		public override bool CanEquip( Mobile m )
		{
			if ( m_Owner == null || m_Owner == m )
				return true;
				
			return false;
		}
		
		public override void OnAdded( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( from );
					
				if ( m_Blessed && m_Owner == null )
				{
					m_Owner = (Mobile) parent;
					
					LootType = LootType.Blessed;
				}
				
				if ( m_ChargeTime > 0 )
				{
					m_ChargeTime = m_MaxChargeTime;
				
					StartTimer();
				}
				
				int strBonus = m_AosAttributes.BonusStr;
				int dexBonus = m_AosAttributes.BonusDex;
				int intBonus = m_AosAttributes.BonusInt;

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
				
				from.CheckStatTimers();
			}
				
			InvalidateProperties();
			
			base.OnAdded( parent );
		}
		
		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;
				
				if ( Core.AOS )
					m_AosSkillBonuses.Remove();
					
				if ( m_Creature != null )
				{
					Effects.SendLocationParticles( EffectItem.Create( m_Creature.Location, m_Creature.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
					Effects.PlaySound( m_Creature, m_Creature.Map, 0x201 );

					m_Creature.Delete();
				}
				
				string modName = this.Serial.ToString();

				from.RemoveStatMod( modName + "Str" );
				from.RemoveStatMod( modName + "Dex" );
				from.RemoveStatMod( modName + "Int" );
				
				from.CheckStatTimers();
					
				StopTimer();
			}			

			base.OnRemoved( parent );
			
			InvalidateProperties();
		}
		
		public override void OnDoubleClick( Mobile from )
		{				
			if ( from.Talisman != this )
			{
				from.SendLocalizedMessage( 502641 ); // You must equip this item to use it.
				return;
			}
			
			if ( m_ChargeTime > 0 )
			{
				from.SendLocalizedMessage( 1074882, m_ChargeTime.ToString() ); // You must wait ~1_val~ seconds for this to recharge.
				return;
			}
			
			if ( m_Charges == 0 && m_MaxCharges > 0 )
			{
				from.SendLocalizedMessage( 1042544 ); // This item is out of charges.
				return;
			}
			
			Type type = GetSummonType();
			
			if ( m_Summoner != null || type != null )
			{					
				if ( m_Summoner != null && m_Summoner.Type != null )
					type = m_Summoner.Type;
				
				object obj;
				
				try{ obj = Activator.CreateInstance( type ); }
				catch{ obj = null; }
				
				if ( obj is Item )
				{
					Item item = (Item) obj;
					
					if ( item.Stackable )
					{
						if ( m_Summoner != null && m_Summoner.Amount > 1 )
							item.Amount = m_Summoner.Amount;		
						else
							item.Amount = 10; // default value
					}								
					
					if ( !from.AddToBackpack( item ) )
					{
						from.SendLocalizedMessage( 502660 ); // You do not have enough space for this in your backpack!
						
						item.Delete();
						
						return;
					}
					
					if ( item is Board )
						from.SendLocalizedMessage( 1075000 ); // You have been given some wooden boards.
					else if ( item is IronIngot )
						from.SendLocalizedMessage( 1075001 ); // You have been given some ingots.
					else if ( item is Bandage )
						from.SendLocalizedMessage( 1075002 ); // You have been given some clean bandages.
					else if ( m_Summoner.Name is int )
						from.SendLocalizedMessage( 1074853, "#" + (int) m_Summoner.Name ); // You have been given ~1_name~
					else if ( m_Summoner.Name is String )
						from.SendLocalizedMessage( 1074853, (String) m_Summoner.Name ); // You have been given ~1_name~
						
					m_ChargeTime = m_MaxChargeTime;
					
					if ( m_Charges > 0 )
						m_Charges -= 1;
				}
				else if ( obj is BaseCreature )
				{
					BaseCreature mob = (BaseCreature) obj;
					
					if ( from.Followers + mob.ControlSlots > from.FollowersMax )
					{
						from.SendLocalizedMessage( 1074270 ); // You have too many followers to summon another one.
						
						mob.Delete();
						
						return;
					}
					
					BaseCreature.Summon( mob, from, from.Location, mob.BaseSoundID, TimeSpan.FromMinutes( 30 ) );
					Effects.SendLocationParticles( EffectItem.Create( mob.Location, mob.Map, EffectItem.DefaultDuration ), 0x3728, 1, 10, 0x26B6 );
					mob.Summoned = false;
					mob.ControlOrder = OrderType.Friend;
					
					m_ChargeTime = m_MaxChargeTime;
					m_Creature = mob;
					
					if ( m_Charges > 0 )
						m_Charges -= 1;
				}
					
				InvalidateProperties();
					
				if ( m_Charges > 0 || m_MaxChargeTime == 0 )
					StartTimer();
				
				return;
			}
			else if ( m_Removal != TalismanRemoval.None )
			{
				from.Target = new TalismanTarget( this );
			}
		}
		
		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( ForceShowName )
				base.AddNameProperty( list );
			else if ( m_Summoner != null )
				list.Add( 1072400, m_Summoner.Name is int ? "#" + (int) m_Summoner.Name : m_Summoner.Name is String ? (String) m_Summoner.Name : "Random" ); // Talisman of ~1_name~ Summoning
			else if ( m_Removal != TalismanRemoval.None )
				list.Add( 1072389, "#" + ( 1072000 + (int) m_Removal ) ); // Talisman of ~1_name~
			else
				base.AddNameProperty( list );
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
				
			if ( Weight != 1 )	
				list.Add( 1072789, Weight.ToString() ); // Weight: ~1_WEIGHT~ stones
			else
				list.Add( 1072788, Weight.ToString() ); // Weight: ~1_WEIGHT~ stone
				
			if ( m_Blessed && m_Owner != null )
				list.Add( 1072304, m_Owner.Name ); // Owned by ~1_name~
			else if ( m_Blessed )
				list.Add( 1072304, "Nobody" ); // Owned by ~1_name~
				
			if ( Parent is Mobile && ((Mobile) Parent).Talisman == this && m_MaxChargeTime > 0 )
			{
				if ( m_ChargeTime > 0 )
					list.Add( 1074884, m_ChargeTime.ToString() ); // Charge time left: ~1_val~
				else
					list.Add( 1074883 ); // Fully Charged
			}
			
			list.Add( 1075085 ); // Requirement: Mondain's Legacy		
			
			if ( m_Killer != null && m_Killer.Amount > 0 )
				list.Add( 1072388, "{0}\t{1}", m_Killer.Name is int ? "#" + (int) m_Killer.Name : m_Killer.Name is String ? (String) m_Killer.Name : null, m_Killer.Amount ); // ~1_NAME~ Killer: +~2_val~%
				
			if ( m_Protection != null && m_Protection.Amount > 0 )
				list.Add( 1072387, "{0}\t{1}", m_Protection.Name is int ? "#" + (int) m_Protection.Name : m_Protection.Name is String ? (String) m_Protection.Name : null, m_Protection.Amount ); // ~1_NAME~ Protection: +~2_val~%
			
			if ( m_ExceptionalBonus != 0 )
				list.Add( 1072395, "#{0}\t{1}", 1044060 + (int) m_Skill, m_ExceptionalBonus ); // ~1_NAME~ Exceptional Bonus: ~2_val~%
			
			if ( m_SuccessBonus != 0 )
				list.Add( 1072394, "#{0}\t{1}", 1044060 + (int) m_Skill, m_SuccessBonus ); // ~1_NAME~ Bonus: ~2_val~%
				
			m_AosSkillBonuses.GetProperties( list );
			
			int prop;
				
			if ( (prop = m_AosAttributes.WeaponDamage) != 0 )
				list.Add( 1060401, prop.ToString() ); // damage increase ~1_val~%

			if ( (prop = m_AosAttributes.DefendChance) != 0 )
				list.Add( 1060408, prop.ToString() ); // defense chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusDex) != 0 )
				list.Add( 1060409, prop.ToString() ); // dexterity bonus ~1_val~

			if ( (prop = m_AosAttributes.EnhancePotions) != 0 )
				list.Add( 1060411, prop.ToString() ); // enhance potions ~1_val~%

			if ( (prop = m_AosAttributes.CastRecovery) != 0 )
				list.Add( 1060412, prop.ToString() ); // faster cast recovery ~1_val~

			if ( (prop = m_AosAttributes.CastSpeed) != 0 )
				list.Add( 1060413, prop.ToString() ); // faster casting ~1_val~

			if ( (prop = m_AosAttributes.AttackChance) != 0 )
				list.Add( 1060415, prop.ToString() ); // hit chance increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusHits) != 0 )
				list.Add( 1060431, prop.ToString() ); // hit point increase ~1_val~

			if ( (prop = m_AosAttributes.BonusInt) != 0 )
				list.Add( 1060432, prop.ToString() ); // intelligence bonus ~1_val~

			if ( (prop = m_AosAttributes.LowerManaCost) != 0 )
				list.Add( 1060433, prop.ToString() ); // lower mana cost ~1_val~%

			if ( (prop = m_AosAttributes.LowerRegCost) != 0 )
				list.Add( 1060434, prop.ToString() ); // lower reagent cost ~1_val~%
				
			if ( (prop = m_AosAttributes.Luck) != 0 )
				list.Add( 1060436, prop.ToString() ); // luck ~1_val~
				
			if ( (prop = m_AosAttributes.BonusMana) != 0 )
				list.Add( 1060439, prop.ToString() ); // mana increase ~1_val~

			if ( (prop = m_AosAttributes.RegenMana) != 0 )
				list.Add( 1060440, prop.ToString() ); // mana regeneration ~1_val~

			if ( (prop = m_AosAttributes.NightSight) != 0 )
				list.Add( 1060441 ); // night sight

			if ( (prop = m_AosAttributes.ReflectPhysical) != 0 )
				list.Add( 1060442, prop.ToString() ); // reflect physical damage ~1_val~%

			if ( (prop = m_AosAttributes.RegenStam) != 0 )
				list.Add( 1060443, prop.ToString() ); // stamina regeneration ~1_val~

			if ( (prop = m_AosAttributes.RegenHits) != 0 )
				list.Add( 1060444, prop.ToString() ); // hit point regeneration ~1_val~
				
			if ( (prop = m_AosAttributes.SpellChanneling) != 0 )
				list.Add( 1060482 ); // spell channeling

			if ( (prop = m_AosAttributes.SpellDamage) != 0 )
				list.Add( 1060483, prop.ToString() ); // spell damage increase ~1_val~%

			if ( (prop = m_AosAttributes.BonusStam) != 0 )
				list.Add( 1060484, prop.ToString() ); // stamina increase ~1_val~

			if ( (prop = m_AosAttributes.BonusStr) != 0 )
				list.Add( 1060485, prop.ToString() ); // strength bonus ~1_val~

			if ( (prop = m_AosAttributes.WeaponSpeed) != 0 )
				list.Add( 1060486, prop.ToString() ); // swing speed increase ~1_val~%			
				
			if ( m_KarmaLoss != 0 )
				list.Add( 1075210, m_KarmaLoss.ToString() ); // Increased Karma Loss ~1val~%			
			
			if ( m_Charges >= 0 && m_MaxCharges > 0 )
				list.Add( 1060741, m_Charges.ToString() ); // charges: ~1_val~
				
			if ( m_Slayer != TalismanSlayerName.None )
				list.Add( (int) m_Slayer );
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
			SkillBonuses		= 0x00000002,
			Owner				= 0x00000004,
			Protection			= 0x00000008,
			Killer				= 0x00000010,
			Summoner			= 0x00000020,
			Removal				= 0x00000040,
			KarmaLoss			= 0x00000080,
			Skill				= 0x00000100,
			SuccessBonus		= 0x00000200,
			ExceptionalBonus	= 0x00000400,
			MaxCharges			= 0x00000800,
			Charges				= 0x00001000,
			MaxChargeTime		= 0x00002000,
			ChargeTime			= 0x00004000,
			Blessed				= 0x00008000,
			Slayer				= 0x00010000,
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Attributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.Owner,				m_Owner != null );
			SetSaveFlag( ref flags, SaveFlag.Protection,		m_Protection != null );
			SetSaveFlag( ref flags, SaveFlag.Killer,			m_Killer != null );
			SetSaveFlag( ref flags, SaveFlag.Summoner,			m_Summoner != null );
			SetSaveFlag( ref flags, SaveFlag.Removal,			m_Removal != TalismanRemoval.None );
			SetSaveFlag( ref flags, SaveFlag.KarmaLoss,			m_KarmaLoss != 0 );
			SetSaveFlag( ref flags, SaveFlag.Skill,				(int) m_Skill != 0 );
			SetSaveFlag( ref flags, SaveFlag.SuccessBonus,		m_SuccessBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.ExceptionalBonus,	m_ExceptionalBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.MaxCharges,		m_MaxCharges != 0 );		
			SetSaveFlag( ref flags, SaveFlag.Charges,			m_Charges != 0 );		
			SetSaveFlag( ref flags, SaveFlag.MaxChargeTime,		m_MaxChargeTime != 0 );	
			SetSaveFlag( ref flags, SaveFlag.ChargeTime,		m_ChargeTime != 0 );
			SetSaveFlag( ref flags, SaveFlag.Blessed,			m_Blessed );
			SetSaveFlag( ref flags, SaveFlag.Slayer,			m_Slayer != TalismanSlayerName.None );

			writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.Owner ) )
				writer.Write( (Mobile) m_Owner );
				
			if ( GetSaveFlag( flags, SaveFlag.Protection ) )
				m_Protection.Serialize( writer );
				
			if ( GetSaveFlag( flags, SaveFlag.Killer ) )
				m_Killer.Serialize( writer );
				
			if ( GetSaveFlag( flags, SaveFlag.Summoner ) )
				m_Summoner.Serialize( writer );
				
			if ( GetSaveFlag( flags, SaveFlag.Removal ) )
				writer.WriteEncodedInt( (int) m_Removal );
				
			if ( GetSaveFlag( flags, SaveFlag.KarmaLoss ) )
				writer.WriteEncodedInt( m_KarmaLoss );
				
			if ( GetSaveFlag( flags, SaveFlag.Skill ) )
				writer.WriteEncodedInt( (int) m_Skill );
				
			if ( GetSaveFlag( flags, SaveFlag.SuccessBonus ) )
				writer.WriteEncodedInt( (int) m_SuccessBonus );
				
			if ( GetSaveFlag( flags, SaveFlag.ExceptionalBonus ) )
				writer.WriteEncodedInt( (int) m_ExceptionalBonus );
				
			if ( GetSaveFlag( flags, SaveFlag.MaxCharges ) )
				writer.WriteEncodedInt( (int) m_MaxCharges );
				
			if ( GetSaveFlag( flags, SaveFlag.Charges ) )
				writer.WriteEncodedInt( (int) m_Charges );
				
			if ( GetSaveFlag( flags, SaveFlag.MaxChargeTime ) )
				writer.WriteEncodedInt( (int) m_MaxChargeTime );
				
			if ( GetSaveFlag( flags, SaveFlag.ChargeTime ) )
				writer.WriteEncodedInt( (int) m_ChargeTime );
				
			if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
				writer.WriteEncodedInt( (int) m_Slayer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					SaveFlag flags = (SaveFlag) reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );
						
					if ( GetSaveFlag( flags, SaveFlag.Owner ) )
						m_Owner = reader.ReadMobile();
					
					
					m_Protection = new TalismanAttribute();	
						
					if ( GetSaveFlag( flags, SaveFlag.Protection ) )					
						m_Protection.Deserialize( reader );
					else
						m_Protection = null;
						
					m_Killer = new TalismanAttribute();	
						
					if ( GetSaveFlag( flags, SaveFlag.Killer ) )
						m_Killer.Deserialize( reader );
					else
						m_Killer = null;
						
					m_Summoner = new TalismanAttribute();		
						
					if ( GetSaveFlag( flags, SaveFlag.Summoner ) )
						m_Summoner.Deserialize( reader );
					else
						m_Summoner = null;
						
					if ( GetSaveFlag( flags, SaveFlag.Removal ) )
						m_Removal = (TalismanRemoval) reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.KarmaLoss ) )
						m_KarmaLoss = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.Skill ) )
						m_Skill = (SkillName) reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.SuccessBonus ) )
						m_SuccessBonus = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.ExceptionalBonus ) )
						m_ExceptionalBonus = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.MaxCharges ) )
						m_MaxCharges = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.Charges ) )
						m_Charges = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.MaxChargeTime ) )
						m_MaxChargeTime = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.ChargeTime ) )
						m_ChargeTime = reader.ReadEncodedInt();
						
					if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
						m_Slayer = (TalismanSlayerName) reader.ReadEncodedInt();
						
					m_Blessed = GetSaveFlag( flags, SaveFlag.Blessed );
					
					break;
				}
			}
			
			if ( Parent is Mobile && ChargeTime > 0 )
				StartTimer();
				
			if ( m_AosSkillBonuses == null )
				m_AosSkillBonuses = new AosSkillBonuses( this );
				
			if ( Core.AOS && Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile)Parent );
				
			int strBonus = m_AosAttributes.BonusStr;
			int dexBonus = m_AosAttributes.BonusDex;
			int intBonus = m_AosAttributes.BonusInt;

			if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)Parent;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();
		}
		
		public virtual Type GetSummonType()
		{
			return null;
		}
		
		public virtual void SetSummoner( Type type, object name )
		{
			SetSummoner( type, name, 0 );
		}
		
		public virtual void SetSummoner( Type type, object name, int amount )
		{
			m_Summoner = new TalismanAttribute( type, amount, name );
		}
		
		public virtual void SetProtection( Type type, object name, int amount )
		{
			m_Protection = new TalismanAttribute( type, amount, name );
		}
		
		public virtual void SetKiller( Type type, object name, int amount )
		{
			m_Killer = new TalismanAttribute( type, amount, name );
		}
		
		private Timer m_Timer;
				
		public virtual void StartTimer()
		{
			if ( m_Timer != null )
				return;

			m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 10 ), TimeSpan.FromSeconds( 10 ), new TimerCallback( Slice ) );
		}

		public virtual void StopTimer()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;
		}
		
		public virtual void Slice()
		{
			if ( m_ChargeTime - 10 > 0 )
				m_ChargeTime -= 10;
			else
			{
				m_ChargeTime = 0;
				
				StopTimer();
			}
			
			InvalidateProperties();
		}
		
		private class TalismanTarget : Target
		{
			private BaseTalisman m_Talisman;
	
			public TalismanTarget( BaseTalisman talisman ) : base( 12, false, TargetFlags.Beneficial )
			{
				m_Talisman = talisman;
			}
	
			protected override void OnTarget( Mobile from, object o )
			{
				if ( !( o is Mobile ) )
				{
					from.SendLocalizedMessage( 1046439 ); // That is not a valid target.
					return;
				}
					
				Mobile target = (Mobile) o;
					
				switch ( m_Talisman.Removal )
				{
					case TalismanRemoval.Curse:
						target.PlaySound( 0xF6 );
						target.PlaySound( 0x1F7 );
						target.FixedParticles( 0x3709, 1, 30, 9963, 13, 3, EffectLayer.Head );
	
						IEntity mfrom = new Entity( Serial.Zero, new Point3D( target.X, target.Y, target.Z - 10 ), from.Map );
						IEntity mto = new Entity( Serial.Zero, new Point3D( target.X, target.Y, target.Z + 50 ), from.Map );
						Effects.SendMovingParticles( mfrom, mto, 0x2255, 1, 0, false, false, 13, 3, 9501, 1, 0, EffectLayer.Head, 0x100 );
	
						StatMod mod;
	
						mod = target.GetStatMod( "[Magic] Str Offset" );
						if ( mod != null && mod.Offset < 0 )
							target.RemoveStatMod( "[Magic] Str Offset" );
	
						mod = target.GetStatMod( "[Magic] Dex Offset" );
						if ( mod != null && mod.Offset < 0 )
							target.RemoveStatMod( "[Magic] Dex Offset" );
	
						mod = target.GetStatMod( "[Magic] Int Offset" );
						if ( mod != null && mod.Offset < 0 )
							target.RemoveStatMod( "[Magic] Int Offset" );
	
						target.Paralyzed = false;
	
						EvilOmenSpell.CheckEffect( target );
						StrangleSpell.RemoveCurse( target );
						CorpseSkinSpell.RemoveCurse( target );
						CurseSpell.RemoveEffect( target );
	
						BuffInfo.RemoveBuff( target, BuffIcon.Clumsy );
						BuffInfo.RemoveBuff( target, BuffIcon.FeebleMind );
						BuffInfo.RemoveBuff( target, BuffIcon.Weaken );
						BuffInfo.RemoveBuff( target, BuffIcon.MassCurse );					
						
						if ( target == from )
							from.SendLocalizedMessage( 1072408 ); // Any curses on you have been lifted
						else
						{
							from.SendLocalizedMessage( 1072409 ); // Your targets curses have been lifted
							target.SendLocalizedMessage( 1072408 ); // Any curses on you have been lifted
						}
						
						break;
					case TalismanRemoval.Damage:
						target.PlaySound( 0x201 );
						Effects.SendLocationParticles( EffectItem.Create( target.Location, target.Map, EffectItem.DefaultDuration ), 0x3728, 1, 13, 0x834, 0, 0x13B2, 0 );
						
						BleedAttack.EndBleed( target, false );
						MortalStrike.EndWound( target );
						
						BuffInfo.RemoveBuff( target, BuffIcon.Bleed );
						BuffInfo.RemoveBuff( target, BuffIcon.MortalStrike );
						
						if ( target == from )
							from.SendLocalizedMessage( 1072405 ); // Your lasting damage effects have been removed!
						else
						{
							from.SendLocalizedMessage( 1072406 ); // Your Targets lasting damage effects have been removed!
							target.SendLocalizedMessage( 1072405 ); // Your lasting damage effects have been removed!
						}
						
						break;					
					case TalismanRemoval.Ward:
						target.PlaySound( 0x201 );
						Effects.SendLocationParticles( EffectItem.Create( target.Location, target.Map, EffectItem.DefaultDuration ), 0x3728, 1, 13, 0x834, 0, 0x13B2, 0 );
						
						// Magic reflect					
						Hashtable m_Table = MagicReflectSpell.m_Table;	
						
						if ( m_Table == null )
							return;
											
						ResistanceMod[] mods = (ResistanceMod[]) m_Table[ target ];
						
						m_Table.Remove( target );
						
						if ( mods != null )
						{	
							for ( int i = 0; i < mods.Length; ++i )
								target.RemoveResistanceMod( mods[ i ] );
						}
	
						BuffInfo.RemoveBuff( target, BuffIcon.MagicReflection );
						
						
						// Reactive armor
						m_Table = ReactiveArmorSpell.m_Table;
						
						if ( m_Table == null )
							return;
							
						mods = (ResistanceMod[])m_Table[ target ];
						
						if ( mods != null )
						{						
							m_Table.Remove( target );
		
							for ( int i = 0; i < mods.Length; ++i )
								target.RemoveResistanceMod( mods[ i ] );
						}
							
						BuffInfo.RemoveBuff( target, BuffIcon.ReactiveArmor );
						
						
						// Protection
						m_Table = ProtectionSpell.m_Table;
						
						if ( m_Table == null )
							return;
							
						object[] pmods = (object[])m_Table[target];
						
						if ( pmods != null )
						{						
							m_Table.Remove( target );
							ProtectionSpell.Registry.Remove( target );
			
							target.RemoveResistanceMod( (ResistanceMod)pmods[0] );
							target.RemoveSkillMod( (SkillMod)pmods[1] );
						}
						
						BuffInfo.RemoveBuff( target, BuffIcon.Protection );
						
						if ( target == from )
							from.SendLocalizedMessage( 1072402 ); // Your wards have been removed!
						else
						{
							from.SendLocalizedMessage( 1072403 ); // Your target's wards have been removed!
							target.SendLocalizedMessage( 1072402 ); // Your wards have been removed!
						}
						
						break;									
				}
				
				m_Talisman.ChargeTime = m_Talisman.MaxChargeTime;
						
				if ( m_Talisman.Charges > 0 )
					m_Talisman.Charges -= 1;
				
				m_Talisman.StartTimer();	
				m_Talisman.InvalidateProperties();
			}
		}
	}
	
	public class TalismanReleaseEntry : ContextMenuEntry
	{
		private Mobile m_Mobile;

		public TalismanReleaseEntry( Mobile m ) : base( 6118, 3 )
		{
			m_Mobile = m;
		}

		public override void OnClick()
		{
			Effects.SendLocationParticles( EffectItem.Create( m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
			Effects.PlaySound( m_Mobile, m_Mobile.Map, 0x201 );
			
			m_Mobile.Delete();
		}
	}
}