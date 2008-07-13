using System;
using Server.Items;
using Server.Network;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items
{
	public abstract class BaseRanged : BaseMeleeWeapon
	{
		public abstract int EffectID{ get; }
		public abstract Type AmmoType{ get; }
		public abstract Item Ammo{ get; }

		public override int DefHitSound{ get{ return 0x234; } }
		public override int DefMissSound{ get{ return 0x238; } }

		public override SkillName DefSkill{ get{ return SkillName.Archery; } }
		public override WeaponType DefType{ get{ return WeaponType.Ranged; } }
		public override WeaponAnimation DefAnimation{ get{ return WeaponAnimation.ShootXBow; } }

		public override SkillName AccuracySkill{ get{ return SkillName.Archery; } }

		#region GeNova: Mondain's Legacy
		private bool m_Balanced;
		private int m_Velocity;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Balanced
		{
			get{ return m_Balanced; }
			set{ m_Balanced = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Velocity
		{
			get{ return m_Velocity; }
			set{ m_Velocity = value; InvalidateProperties(); }
		}
		#endregion

		public BaseRanged( int itemID ) : base( itemID )
		{
		}

		public BaseRanged( Serial serial ) : base( serial )
		{
		}

		public override TimeSpan OnSwing( Mobile attacker, Mobile defender )
		{
			WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

			// Make sure we've been standing still for .25/.5/1 second depending on Era
			if ( DateTime.Now > (attacker.LastMoveTime + TimeSpan.FromSeconds( Core.SE ? 0.25 : (Core.AOS ? 0.5 : 1.0) )) || (Core.AOS && WeaponAbility.GetCurrentAbility( attacker ) is MovingShot) )
			{
				bool canSwing = true;

				if ( Core.AOS )
				{
					canSwing = ( !attacker.Paralyzed && !attacker.Frozen );

					if ( canSwing )
					{
						Spell sp = attacker.Spell as Spell;

						canSwing = ( sp == null || !sp.IsCasting || !sp.BlocksMovement );
					}
				}

				if ( canSwing && attacker.HarmfulCheck( defender ) )
				{
					attacker.DisruptiveAction();
					attacker.Send( new Swing( 0, attacker, defender ) );

					if ( OnFired( attacker, defender ) )
					{
						if ( CheckHit( attacker, defender ) )
							OnHit( attacker, defender );
						else
							OnMiss( attacker, defender );
					}
				}

				attacker.RevealingAction();

				return GetDelay( attacker );
			}
			else
			{
				attacker.RevealingAction();

				return TimeSpan.FromSeconds( 0.25 );
			}
		}

		public override void OnHit( Mobile attacker, Mobile defender, double damageBonus )
		{
			if ( attacker.Player && !defender.Player && (defender.Body.IsAnimal || defender.Body.IsMonster) && 0.4 >= Utility.RandomDouble() )
				defender.AddToBackpack( Ammo );

			base.OnHit( attacker, defender, damageBonus );
		}

		public override void OnMiss( Mobile attacker, Mobile defender )
		{
			if ( attacker.Player && 0.4 >= Utility.RandomDouble() )
				Ammo.MoveToWorld( new Point3D( defender.X + Utility.RandomMinMax( -1, 1 ), defender.Y + Utility.RandomMinMax( -1, 1 ), defender.Z ), defender.Map );

			base.OnMiss( attacker, defender );
		}

		public virtual bool OnFired( Mobile attacker, Mobile defender )
		{
			Container pack = attacker.Backpack;

			#region GeNova: Mondain's Legacy			
			BaseQuiver quiver = attacker.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;
			
			if ( attacker.Player && (quiver == null || !quiver.ConsumeAmmo( AmmoType )) )
			{				
				if ( pack == null || !pack.ConsumeTotal( AmmoType, 1 ) )
					return false;
			}
			#endregion

			attacker.MovingEffect( defender, EffectID, 18, 1, false, false );

			return true;
		}

		#region GeNova: Mondain's Legacy
		public override int ComputeDamage( Mobile attacker, Mobile defender )
		{
			int damage = base.ComputeDamage( attacker, defender );
			
			// add velocity bonus
			if ( m_Velocity > 0 )
			{
				int range = (int) Math.Round( Math.Sqrt( Math.Pow( attacker.X - defender.X, 2 ) + Math.Pow( attacker.Y - defender.Y, 2 ) ) ); 	
				damage += (int) Math.Round( Math.Min( range * 3, 30 ) * ( m_Velocity / (double) 100 ) );
			}	
			
			return damage;	
		}
		
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy, out int chaos, out int direct )
		{
			if ( Parent is Mobile )
			{
				Mobile parent = (Mobile) Parent;
			
				BaseQuiver quiver = parent.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;
				
				if ( quiver != null && !quiver.DamageModifier.IsEmpty )
				{
					quiver.GetDamageTypes( out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );
					
					return;
				}
			}
			
			base.GetDamageTypes( wielder, out phys, out fire, out cold, out pois, out nrgy, out chaos, out direct );
		}
		
		public override void AddNameProperties( ObjectPropertyList list )
		{
			base.AddNameProperties( list );
			
			if ( m_Balanced )
				list.Add( 1072792 ); // Balanced
			
			if ( m_Velocity > 0 )
				list.Add( 1072793, m_Velocity.ToString() ); // Velocity ~1_val~%
		}
		#endregion

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 4 ); // version
			
			#region GeNova: Mondain's Legacy ver 4
			writer.Write( (int) m_Velocity );
			#endregion

			#region GeNova: Mondain's Legacy ver 3
			writer.Write( (bool) m_Balanced );
			#endregion
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 4:
					#region GeNova: Mondain's Legacy
					m_Velocity = reader.ReadInt();
					#endregion
					goto case 3;
				case 3:
					#region GeNova: Mondain's Legacy
					m_Balanced = reader.ReadBool();
					#endregion
					goto case 2;
				case 2:
				case 1:
				{
					break;
				}
				case 0:
				{
					/*m_EffectID =*/ reader.ReadInt();
					break;
				}
			}

			if ( version < 2 )
			{
				WeaponAttributes.MageWeapon = 0;
				WeaponAttributes.UseBestSkill = 0;
			}
		}
	}
}