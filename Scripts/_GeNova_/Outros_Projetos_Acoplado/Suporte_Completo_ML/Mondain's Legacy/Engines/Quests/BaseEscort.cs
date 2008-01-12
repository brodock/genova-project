using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.ContextMenus;
using EDI = Server.Mobiles.EscortDestinationInfo;

namespace Server.Engines.Quests
{
	public class BaseEscort : MondainQuester
	{
		public override bool Commandable{ get{ return false; } }
		public override Type[] Quests{ get{ return null; } }	

		private static TimeSpan m_EscortDelay = TimeSpan.FromMinutes( 5.0 ); // delay
		
		private DateTime m_LastSeenEscorter;
		private DateTime m_DeleteTime;
		private Timer m_DeleteTimer;
		
		/// <summary>
		/// Date when escort last seen player.
		/// </summary>
		public DateTime LastSeenEscorter
		{
			get{ return m_LastSeenEscorter; }
			set{ m_LastSeenEscorter = value; }
		}
		
		private BaseQuest m_Quest;	
		
		/// <summary>
		/// Reference to quest.
		/// </summary>
		public BaseQuest Quest
		{
			get{ return m_Quest; }
			set{ m_Quest = value; }
		}
		
		public BaseEscort() : base()
		{
			AI = AIType.AI_Melee;
			FightMode = FightMode.Aggressor;
			RangePerception = 22;
			RangeFight = 1;
			ActiveSpeed = 0.2;
			PassiveSpeed = 1.0;
		
			InitBody();
			InitOutfit();
			
			ControlSlots = 3;
		}
		
		public BaseEscort( Serial serial ) : base( serial )
		{
		}		

		public override void InitBody()
		{
			SetStr( 90, 100 );
			SetDex( 90, 100 );
			SetInt( 15, 25 );

			Hue = Utility.RandomSkinHue();

			if ( Female = Utility.RandomBool() )
			{
				Body = 401;
				Name = NameList.RandomName( "female" );
			}
			else
			{
				Body = 400;
				Name = NameList.RandomName( "male" );
			}
		}

		public override void InitOutfit()
		{
			AddItem( new FancyShirt( Utility.RandomNeutralHue() ) );
			AddItem( new ShortPants( Utility.RandomNeutralHue() ) );
			AddItem( new Boots( Utility.RandomNeutralHue() ) );

			Utility.AssignRandomHair( this );

			PackGold( 200, 250 );
		}

		private static Hashtable m_EscortTable = new Hashtable();
		
		/// <summary>
		/// Static. Escort table, where all player with escorts are.
		/// </summary>
		public static Hashtable EscortTable
		{
			get{ return m_EscortTable; }
		}
		
		/// <summary>
		/// Static. Adds a player to escort table, where all player with escorts are.
		/// </summary>
		public void AddHash( PlayerMobile player )
		{
			m_EscortTable[ player ] = this;
		}
				
		public override bool HandlesOnSpeech( Mobile from )
		{
			return false;
		}
		
		public override void OnTalk( PlayerMobile player )
		{
			if ( AcceptEscorter( player ) )
				base.OnTalk( player );
		}
		
		/// <summary>
		/// Overridable. Player can escort this guy?.
		/// </summary>
		public virtual bool AcceptEscorter( Mobile m )
		{
			Mobile escorter = GetEscorter();

			if ( escorter != null || !m.Alive || !m.InRange( Location, 5 ) )
				return false;

			BaseEscort escortable = (BaseEscort) m_EscortTable[ m ];

			if ( escortable != null && !escortable.Deleted && escortable.GetEscorter() == m )
			{
				Say( 500896 ); // I see you already have an escort.
				
				return false;
			}
			else if ( m is PlayerMobile && (((PlayerMobile)m).LastEscortTime + m_EscortDelay) >= DateTime.Now )
			{
				int minutes = (int)Math.Ceiling( ((((PlayerMobile)m).LastEscortTime + m_EscortDelay) - DateTime.Now).TotalMinutes );

				Say( "You must rest {0} minute{1} before we set out on this journey.", minutes, minutes == 1 ? "" : "s" );
				
				return false;
			}

			return true;
		}

		public override void OnAfterDelete()
		{
			if ( m_DeleteTimer != null )
				m_DeleteTimer.Stop();				
				
			if ( m_Quest != null )
				m_Quest.RemoveQuest();	
			
			m_DeleteTimer = null;

			base.OnAfterDelete();
		}

		public override void OnThink()
		{
			base.OnThink();
			
			CheckAtDestination();
		}
		
		/// <summary>
		/// Overridable. Starts to follow control master.
		/// </summary>
		public virtual void StartFollow()
		{
			StartFollow( GetEscorter() );
		}
		
		/// <summary>
		/// Overridable. Starts to follow escorter.
		/// </summary>
		public virtual void StartFollow( Mobile escorter )
		{		
			if ( escorter == null )
				return;

			ActiveSpeed = 0.1;
			PassiveSpeed = 0.2;

			ControlOrder = OrderType.Follow;
			ControlTarget = escorter;

			CurrentSpeed = 0.1;
		}
		
		/// <summary>
		/// Overridable. Stops following escorter.
		/// </summary>
		public virtual void StopFollow()
		{
			ActiveSpeed = 0.2;
			PassiveSpeed = 1.0;

			ControlOrder = OrderType.None;
			ControlTarget = null;

			CurrentSpeed = 1.0;
		}
		
		/// <summary>
		/// Overridable. Returns control master and makes escort stop following him if not in range or dead.
		/// </summary>
		public virtual Mobile GetEscorter()
		{
			if ( !Controlled )
				return null;

			Mobile master = ControlMaster;

			if ( master == null )
				return null;

			if ( master.Deleted || master.Map != this.Map || !master.InRange( Location, 30 ) || !master.Alive )
			{
				StopFollow();

				TimeSpan lastSeenDelay = DateTime.Now - m_LastSeenEscorter;

				if ( lastSeenDelay >= TimeSpan.FromMinutes( 2.0 ) )
				{
					master.SendLocalizedMessage( 1042473 ); // You have lost the person you were escorting.
					Say( 1005653 ); // Hmmm.  I seem to have lost my master.

					SetControlMaster( null );
					m_EscortTable.Remove( master );

					Timer.DelayCall( TimeSpan.FromSeconds( 5.0 ), new TimerCallback( Delete ) );
					return null;
				}
				else
				{
					ControlOrder = OrderType.Stay;
					return master;
				}
			}

			if ( ControlOrder != OrderType.Follow )
				StartFollow( master );

			m_LastSeenEscorter = DateTime.Now;
			
			return master;
		}
		
		/// <summary>
		/// Overridable. Starts 30 second removal delay.
		/// </summary>
		public virtual void BeginDelete()
		{
			if ( m_DeleteTimer != null )
				m_DeleteTimer.Stop();

			m_DeleteTime = DateTime.Now + TimeSpan.FromSeconds( 30.0 );

			m_DeleteTimer = new DeleteTimer( this, m_DeleteTime - DateTime.Now );
			m_DeleteTimer.Start();
		}
		
		/// <summary>
		/// Overridable. Checks if at destination.
		/// </summary>
		public virtual bool CheckAtDestination()
		{
			EscortObjective escort = GetObjective(); 
				
			if ( escort == null )
				return false;			
				
			if ( escort.Region == null )
			{
				escort.Fail();
				return CheckAtDestination();
			}
			
			Mobile escorter = GetEscorter();

			if ( escorter == null )
				return false;
			
			if ( escort.Region.Contains( Location ) )
			{
				Say( 1042809, escorter.Name ); // We have arrived! I thank thee, ~1_PLAYER_NAME~! I have no further need of thy services. Here is thy pay.
					
				escort.Complete();	
						
				if ( m_Quest != null )
				{					
					if ( m_Quest.Completed )
					{
						escorter.SendLocalizedMessage( 1046258, null, 0x23 ); // Your quest is complete.		
						
						if ( !QuestHelper.AnyRewards( m_Quest ) )
							m_Quest.GiveRewards();
						else
							escorter.SendGump( new MondainQuestGump( m_Quest, MondainQuestGump.Section.Rewards, false, true ) );							
						
						escorter.PlaySound( m_Quest.CompleteSound );
					}
					else
						escorter.PlaySound( m_Quest.UpdateSound );						
				}
				
				StopFollow();
				SetControlMaster( null );				
				m_EscortTable.Remove( escorter );
				BeginDelete();
				
				// fame, compassion
				if ( escort.Fame > 0 )
				{
					Misc.Titles.AwardFame( escorter, escort.Fame, true );
	
					bool gainedPath = false;
	
					PlayerMobile pm = escorter as PlayerMobile;
	
					if ( pm != null )
					{
						if ( pm.CompassionGains > 0 && DateTime.Now > pm.NextCompassionDay )
						{
							pm.NextCompassionDay = DateTime.MinValue;
							pm.CompassionGains = 0;
						}
	
						if ( pm.CompassionGains >= 5 ) // have already gained 5 times in one day, can gain no more
						{
							pm.SendLocalizedMessage( 1053004 ); // You must wait about a day before you can gain in compassion again.
						}
						else if ( VirtueHelper.Award( pm, VirtueName.Compassion, 200, ref gainedPath ) )
						{
							if ( gainedPath )
								pm.SendLocalizedMessage( 1053005 ); // You have achieved a path in compassion!
							else
								pm.SendLocalizedMessage( 1053002 ); // You have gained in compassion.
	
							pm.NextCompassionDay = DateTime.Now + TimeSpan.FromDays( 1.0 ); // in one day CompassionGains gets reset to 0
							++pm.CompassionGains;
						}
						else
						{
							pm.SendLocalizedMessage( 1053003 ); // You have achieved the highest path of compassion and can no longer gain any further.
						}
					}
				}

				return true;
			}

			return false;
		}
		
		/// <summary>
		/// Overridable. Returns first uncompleted and not failed escort objective.
		/// </summary>
		public virtual EscortObjective GetObjective()
		{
			if ( m_Quest == null )
				return null;
								
			for ( int i = 0; i < m_Quest.Objectives.Count; i ++ )
			{
				if ( m_Quest.Objectives[ i ] is EscortObjective )
				{
					EscortObjective escort = (EscortObjective) m_Quest.Objectives[ i ];
					
					if ( !escort.Completed && !escort.Failed )
						return escort;
				}
			}
			
			return null;
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1072269 ); // Quest Giver
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( m_DeleteTimer != null );

			if ( m_DeleteTimer != null )
				writer.WriteDeltaTime( m_DeleteTime );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( reader.ReadBool() )
			{
				m_DeleteTime = reader.ReadDeltaTime();
				m_DeleteTimer = new DeleteTimer( this, m_DeleteTime - DateTime.Now );
				m_DeleteTimer.Start();
			}
		}

		public override bool CanBeRenamedBy( Mobile from )
		{
			return ( from.AccessLevel >= AccessLevel.GameMaster );
		}
		
		public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
		{
			if ( from.Alive && from == ControlMaster )
				list.Add( new AbandonEscortEntry( this ) );
				
			base.AddCustomContextEntries( from, list );
		}
		
		private class AbandonEscortEntry : ContextMenuEntry
		{
			private BaseEscort m_Mobile;
	
			public AbandonEscortEntry( BaseEscort m ) : base( 6102, 3 )
			{
				m_Mobile = m;
			}
	
			public override void OnClick()
			{								
				m_Mobile.Delete();
			}
		}
		
		private class DeleteTimer : Timer
		{
			private Mobile m_Mobile;

			public DeleteTimer( Mobile m, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;

				Priority = TimerPriority.OneSecond;
			}

			protected override void OnTick()
			{
				m_Mobile.Delete();
			}
		}
	}
}