using System;
using System.Collections.Generic;
using Server;
using Server.Items;
using Server.Multis;
using Server.ContextMenus;
using Server.Network;

namespace Server.Mobiles
{    
    public enum StatueType
    {
    	Marble,
    	Jade,
    	Bronze
    }
    
    public enum StatuePose
    {
        Ready,
        Casting,
        Salute,
        AllPraiseMe,
        Fighting,
        HandsOnHips
    }
    
    public enum StatueMaterial
    {
    	Antique,
    	Dark,
    	Medium, 
    	Light
    }
    
	public class CharacterStatue : Mobile
	{		
		private StatueType m_Type;
		private StatuePose m_Pose;		
		private StatueMaterial m_Material;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public StatueType StatueType
		{
			get { return m_Type; }
			set { m_Type = value; InvalidateHues(); InvalidatePose(); }			
		}		
		
		[CommandProperty( AccessLevel.GameMaster )]
		public StatuePose Pose
		{
			get { return m_Pose; }
			set { m_Pose = value; InvalidatePose(); }			
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public StatueMaterial Material
		{
			get { return m_Material; }
			set { m_Material = value; InvalidateHues(); InvalidatePose(); }			
		}
		
		private Mobile m_SculptedBy;
		private DateTime m_SculptedOn;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile SculptedBy
		{
			get{ return m_SculptedBy; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public DateTime SculptedOn
		{
			get{ return m_SculptedOn; }
		}
		
		private CharacterStatuePlinth m_Plinth;
		
		public CharacterStatuePlinth Plinth
		{
			get { return m_Plinth; }
			set { m_Plinth = value; }				
		}
	
		public CharacterStatue( Mobile from, StatueType type ) : base()
		{		
			m_Type = type;
			m_Pose = StatuePose.Ready;
			m_Material = StatueMaterial.Antique;
			
			m_SculptedBy = from;
			m_SculptedOn = DateTime.Now;
			
			Direction = Direction.South;
			AccessLevel = AccessLevel.Counselor;
			Hits = HitsMax;
			Blessed = true;
			Frozen = true;
			
			CloneBody( from );
			CloneClothes( from );
			InvalidateHues();
		}

		public CharacterStatue( Serial serial ) : base( serial )
		{
		}    	
		
		public override void OnDoubleClick( Mobile from )
		{
			DisplayPaperdollTo( from );
		}		
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_SculptedBy != null )
			{
				if ( m_SculptedBy.Title != null )
					list.Add( 1076202, m_SculptedBy.Title + " " + m_SculptedBy.Name ); // Sculpted by ~1_Name~
				else
					list.Add( 1076202, m_SculptedBy.Name ); // Sculpted by ~1_Name~
			}
		}   	
		
		public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
		{
			base.GetContextMenuEntries( from, list );
			
			if ( from.Alive )
			{
				BaseHouse house = BaseHouse.FindHouseAt( this );
								
				if ( ( house != null && house.IsCoOwner( from ) ) || (int) from.AccessLevel > (int) AccessLevel.Counselor )
					list.Add( new DemolishEntry( this ) );
			}
		}
		
		public override void OnAfterDelete()
		{
			base.OnAfterDelete();
			
			if ( m_Plinth != null && !m_Plinth.Deleted )
				m_Plinth.Delete();
		}
		
		protected override void OnMapChange( Map oldMap )
		{			
			InvalidatePose();
			
			if ( m_Plinth != null )
				m_Plinth.Map = Map;
		}
		
		protected override void OnLocationChange( Point3D oldLocation )
		{
			InvalidatePose();
			
			if ( m_Plinth != null )
				m_Plinth.Location = new Point3D( X, Y, Z - 5 );
		}
		
		public override bool CanBeRenamedBy( Mobile from )
		{
			return false;
		}
		
		public override bool CanBeDamaged()
		{
			return false;
		}
		
		public override void OnRequestedAnimation( Mobile from )
		{				
			from.Send( new UpdateStatueAnimation( this, 1, m_Animation, m_Frames ) );
		}
		
		public override bool ShowIncomingName( Mobile from )
		{
			return false;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
			
			writer.Write( (int) m_Type );
			writer.Write( (int) m_Pose );
			writer.Write( (int) m_Material );
			
			writer.Write( (Mobile) m_SculptedBy );
			writer.Write( (DateTime) m_SculptedOn );			
			
			writer.Write( (Item) m_Plinth );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
			
			m_Type = (StatueType) reader.ReadInt();
			m_Pose = (StatuePose) reader.ReadInt();
			m_Material = (StatueMaterial) reader.ReadInt();
			
			m_SculptedBy = reader.ReadMobile();
			m_SculptedOn = reader.ReadDateTime();
			
			m_Plinth = reader.ReadItem() as CharacterStatuePlinth;
			
			InvalidatePose();
			
			Frozen = true;
		}
		
		public void Sculpt( Mobile by )
		{
			m_SculptedBy = by;
			m_SculptedOn = DateTime.Now;
			
			InvalidateProperties();
		}
		
		public void Demolish( Mobile by )
		{
			CharacterStatueDeed deed = new CharacterStatueDeed( null );
		
			if ( by.PlaceInBackpack( deed ) )
			{
				Internalize();
				
				deed.Statue = this;
				
				if ( m_Plinth != null )
					m_Plinth.Internalize();
			}
			else
			{
				by.SendLocalizedMessage( 500720 ); // You don't have enough room in your backpack!
				deed.Delete();
			}
		}
		
		public void Restore( CharacterStatue from )
		{		
			m_Material = from.Material;
			m_Pose = from.Pose;
			
			Direction = from.Direction;
			
			CloneBody( from );
			CloneClothes( from );
			
			InvalidateHues();
			InvalidatePose();
		}
		
		public void CloneBody( Mobile from )
		{
			Name = from.Name;
			BodyValue = from.BodyValue;
			HairItemID = from.HairItemID;
			FacialHairItemID = from.FacialHairItemID;			
		}
		
		public void CloneClothes( Mobile from )
		{
			for ( int i = Items.Count - 1; i >= 0; i -- )
				Items[ i ].Delete();
			
			for ( int i = from.Items.Count - 1; i >= 0; i -- )
			{
				Item item = from.Items[ i ];
				
				if ( item.Layer != Layer.Backpack && item.Layer != Layer.Mount && item.Layer != Layer.Bank )
					AddItem( CloneItem( item ) );
			}
		}
		
		public Item CloneItem( Item item )
		{
			Item cloned = new Item( item.ItemID );
			cloned.Layer = item.Layer;
			cloned.Name = item.Name;
			cloned.Hue = item.Hue;
			cloned.Weight = item.Weight;
			cloned.Movable = false;
			
			return cloned;
		}
		
		public void InvalidateHues()
		{
			Hue = 0xB8F + (int) m_Type * 4 + (int) m_Material;
			
			HairHue = Hue;
			
			if ( FacialHairItemID > 0 )
				FacialHairHue = Hue;
			
			for ( int i = Items.Count - 1; i >= 0; i -- )
				Items[ i ].Hue = Hue;
			
			if ( m_Plinth != null )
				m_Plinth.InvalidateHue();
		}
		
		private int m_Animation;
		private int m_Frames;
		
		public void InvalidatePose()
		{		
			switch ( m_Pose )
            {
            	case StatuePose.Ready: 
            			m_Animation = 4;
                        m_Frames = 0;
                        break;
                case StatuePose.Casting:
                        m_Animation = 16;
                        m_Frames = 2;
                        break;
                case StatuePose.Salute:
                        m_Animation = 33;
                        m_Frames = 1;
                        break;
                case StatuePose.AllPraiseMe:
                        m_Animation = 17;
                        m_Frames = 4;
                        break;
                case StatuePose.Fighting:
                        m_Animation = 31;
                        m_Frames = 5;
                        break;
                case StatuePose.HandsOnHips:
                        m_Animation = 6;
                        m_Frames = 1;
                        break;
            }

			if( Map != null )
			{
				ProcessDelta();

				Packet p = null;

				IPooledEnumerable eable = Map.GetClientsInRange( Location );

				foreach( NetState state in eable )
				{
					state.Mobile.ProcessDelta();

					if( p == null )
						p = Packet.Acquire( new UpdateStatueAnimation( this, 1, m_Animation, m_Frames ) );

					state.Send( p );
				}

				Packet.Release( p );

				eable.Free();
			}
	    }
		
		private class DemolishEntry : ContextMenuEntry
		{
			private CharacterStatue m_Statue;

			public DemolishEntry( CharacterStatue statue ) : base( 6275, 2 )
			{
				m_Statue = statue;
			}
	
			public override void OnClick()
			{
				if ( m_Statue.Deleted )
					return;
					
				m_Statue.Demolish( Owner.From );
			}
		}
	}
}