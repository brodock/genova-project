using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Network;
using Server.Targeting;
using Server.Spells;

namespace Server.Items
{
	public abstract class BaseConflagrationPotion : BasePotion
	{
		public abstract int MinDamage{ get; }
		public abstract int MaxDamage{ get; }

		public override bool RequireFreeHand{ get{ return false; } }

		public BaseConflagrationPotion( PotionEffect effect ) : base( 0xF06, effect )
		{
			Hue = 0x489;
		}

		public BaseConflagrationPotion( Serial serial ) : base( serial )
		{
		}
		
		public override void Drink( Mobile from )
		{
			if ( Core.AOS && (from.Paralyzed || from.Frozen || (from.Spell != null && from.Spell.IsCasting)) )
			{
				from.SendLocalizedMessage( 1062725 ); // You can not use that potion while paralyzed.
				return;
			}
			
			int delay = GetDelay( from );
		
			if ( delay > 0 )
			{
				from.SendLocalizedMessage( 1072529, String.Format( "{0}\t{1}", delay, delay > 1 ? "seconds." : "second." ) ); // You cannot use that for another ~1_NUM~ ~2_TIMEUNITS~
				return;
			}

			ThrowTarget targ = from.Target as ThrowTarget;

			if ( targ != null && targ.Potion == this )
				return;

			from.RevealingAction();

			if ( !m_Users.Contains( from ) )
				m_Users.Add( from );

			from.Target = new ThrowTarget( this );
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
		
		private List<Mobile> m_Users = new List<Mobile>();
		
		public void Explode_Callback( object state )
		{
			object[] states = (object[]) state;
			
			Explode( (Mobile) states[ 0 ], (Point3D) states[ 1 ], (Map) states[ 2 ] );
		}
		
		public virtual void Explode( Mobile from, Point3D loc, Map map )
		{
			if ( Deleted || map == null )
				return;

			Consume();
			
			// Check if any other players are using this potion
			for ( int i = 0; i < m_Users.Count; i ++ )
			{
				ThrowTarget targ = m_Users[ i ].Target as ThrowTarget;

				if ( targ != null && targ.Potion == this )
					Target.Cancel( from );
			}			
			
			// Add delay
			AddDelay( from );
			
			// Effects
			Effects.PlaySound( loc, map, 0x20C );
			
			for ( int i = -2; i <= 2; i ++ )
			{
				for ( int j = -2; j <= 2; j ++ )
				{
					Point3D p = new Point3D( loc.X + i, loc.Y + j, loc.Z );
					
					if ( map.CanFit( p, 12, true, false ) && from.InLOS( p ) )
						new InternalItem( from, p, map, MinDamage, MaxDamage );
				}
			}			
		}
		
		#region Delay
		private static Hashtable m_Delay = new Hashtable();
		
		public static void AddDelay( Mobile m )
		{
			Timer timer = m_Delay[ m ] as Timer;
			
			if ( timer != null )
				timer.Stop();
			
			m_Delay[ m ] = Timer.DelayCall( TimeSpan.FromSeconds( 30 ), new TimerStateCallback( EndDelay_Callback ), m );	
		}
		
		public static int GetDelay( Mobile m )
		{
			Timer timer = m_Delay[ m ] as Timer;
			
			if ( timer != null && timer.Next > DateTime.Now )
				return (int) (timer.Next - DateTime.Now).TotalSeconds;
			
			return 0;
		}
		
		private static void EndDelay_Callback( object obj )
		{
			if ( obj is Mobile )
				EndDelay( (Mobile) obj );			
		}
		
		public static void EndDelay( Mobile m )
		{
			Timer timer = m_Delay[ m ] as Timer;
			
			if ( timer != null )
			{
				timer.Stop();
				m_Delay.Remove( m );
			}
		}
		#endregion

		private class ThrowTarget : Target
		{
			private BaseConflagrationPotion m_Potion;
			
			public BaseConflagrationPotion Potion
			{
				get{ return m_Potion; }
			}

			public ThrowTarget( BaseConflagrationPotion potion ) : base( 12, true, TargetFlags.None )
			{
				m_Potion = potion;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				if ( m_Potion.Deleted || m_Potion.Map == Map.Internal )
					return;
					
				IPoint3D p = targeted as IPoint3D;

				if ( p == null || from.Map == null )
					return;

				SpellHelper.GetSurfaceTop( ref p );

				from.RevealingAction();

				IEntity to;

				if ( p is Mobile )
					to = (Mobile)p;
				else
					to = new Entity( Serial.Zero, new Point3D( p ), from.Map );

				Effects.SendMovingEffect( from, to, 0xF0D, 7, 0, false, false, m_Potion.Hue, 0 );
				Timer.DelayCall( TimeSpan.FromMilliseconds( GetDelay( from.Location, new Point3D( p ) ) ), new TimerStateCallback( m_Potion.Explode_Callback ), new object[] { from, new Point3D( p ), from.Map } ); 				
			}
			
			public int GetDelay( Point3D start, Point3D end )
			{
				double range = Math.Sqrt( Math.Pow( start.X - end.X, 2 ) + Math.Pow( start.Y - end.Y, 2 ) );
				
				return (int) ( 1000 * range / 4 );
			}
		}
		
		public class InternalItem : Item
		{
			private Mobile m_Caster;
			private DateTime m_End;
			private Timer m_Timer;
			private int m_Min;
			private int m_Max;
			
			public override bool BlocksFit{ get{ return true; } }

			public InternalItem( Mobile caster, Point3D loc, Map map, int min, int max ) : base( 0x398C )
			{
				Movable = false;
				Light = LightType.Circle300;

				MoveToWorld( loc, map );

				m_Caster = caster;
				m_End = DateTime.Now + TimeSpan.FromSeconds( 10 );
				m_Min = min;
				m_Max = max;

				m_Timer = new InternalTimer( this, min, max );
				m_Timer.Start();
			}

			public override void OnAfterDelete()
			{
				base.OnAfterDelete();

				if ( m_Timer != null )
					m_Timer.Stop();
			}

			public InternalItem( Serial serial ) : base( serial )
			{
			}

			public override void Serialize( GenericWriter writer )
			{
				base.Serialize( writer );

				writer.Write( (int) 0 ); // version

				writer.Write( (Mobile) m_Caster );
				writer.Write( (DateTime) m_End );
				writer.Write( (int) m_Min );
				writer.Write( (int) m_Max );
			}

			public override void Deserialize( GenericReader reader )
			{
				base.Deserialize( reader );

				int version = reader.ReadInt();
				
				m_Caster = reader.ReadMobile();
				m_End = reader.ReadDateTime();
				m_Min = reader.ReadInt();
				m_Max = reader.ReadInt();
				
				m_Timer = new InternalTimer( this, m_Min, m_Max );
				m_Timer.Start();
			}

			public override bool OnMoveOver( Mobile m )
			{
				if ( Visible && m_Caster != null && (!Core.AOS || m != m_Caster) && SpellHelper.ValidIndirectTarget( m_Caster, m ) && m_Caster.CanBeHarmful( m, false ) )
				{
					m_Caster.DoHarmful( m );

					int damage = Utility.RandomMinMax( m_Min, m_Max );

					if ( !Core.AOS && m.CheckSkill( SkillName.MagicResist, 0.0, 30.0 ) )
					{
						damage = Math.Max( 1, damage / 5 );

						m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
					}

					AOS.Damage( m, m_Caster, damage, 0, 100, 0, 0, 0 );
					m.PlaySound( 0x208 );
				}

				return true;
			}

			private class InternalTimer : Timer
			{
				private InternalItem m_Item;
				private int m_Min;
				private int m_Max;

				private static Queue m_Queue = new Queue();

				public InternalTimer( InternalItem item, int min, int max ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 1.0 ) )
				{
					m_Item = item;
					m_Min = min;
					m_Max = max;

					Priority = TimerPriority.FiftyMS;
				}

				protected override void OnTick()
				{
					if ( m_Item.Deleted )
						return;

					if ( DateTime.Now > m_Item.m_End )
					{
						m_Item.Delete();
						Stop();
					}
					else
					{
						Map map = m_Item.Map;
						Mobile caster = m_Item.m_Caster;

						if ( map != null && caster != null )
						{
							foreach ( Mobile m in m_Item.GetMobilesInRange( 0 ) )
							{
								if ( (m.Z + 16) > m_Item.Z && (m_Item.Z + 12) > m.Z && (!Core.AOS || m != caster) && SpellHelper.ValidIndirectTarget( caster, m ) && caster.CanBeHarmful( m, false ) )
									m_Queue.Enqueue( m );
							}

							while ( m_Queue.Count > 0 )
							{
								Mobile m = (Mobile)m_Queue.Dequeue();

								caster.DoHarmful( m );

								int damage = Utility.RandomMinMax( m_Min, m_Max );

								if ( !Core.AOS && m.CheckSkill( SkillName.MagicResist, 0.0, 30.0 ) )
								{
									damage = Math.Max( 1, damage / 5 );

									m.SendLocalizedMessage( 501783 ); // You feel yourself resisting magical energy.
								}

								AOS.Damage( m, caster, damage, 0, 100, 0, 0, 0 );
								m.PlaySound( 0x208 );
							}
						}
					}
				}
			}
		}
	}
}