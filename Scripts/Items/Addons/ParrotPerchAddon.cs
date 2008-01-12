// Genova: suporte ao UO:ML.
using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class ParrotPerchAddon : BaseAddon
	{
		public override BaseAddonDeed Deed
		{ 
			get
			{ 
				if ( m_Parrot != null )
					return new ParrotPerchAddonDeed( m_Parrot.Birth, m_Parrot.Name, m_Parrot.Hue );
				else
					return new ParrotPerchAddonDeed();
			} 
		}
		
		private PetParrot m_Parrot;
		
		[CommandProperty( AccessLevel.GameMaster)]
		public PetParrot Parrot
		{
			get { return m_Parrot; }
			set { m_Parrot = value; }
		} 

		[Constructable]
		public ParrotPerchAddon() : this( null )
		{			
		}
		
		public ParrotPerchAddon( PetParrot parrot )
		{
			AddComponent( new AddonComponent( 0x2FB6 ), 0, 0, 0 );
			
			m_Parrot = parrot;
		}
		
		public override void OnLocationChange( Point3D oldLocation )
		{
			base.OnLocationChange( oldLocation );
			
			if ( m_Parrot != null )
			{
				m_Parrot.MoveToWorld( Location, Map );
				m_Parrot.Z += 12;				
			}
		}		
		
		public override void OnMapChange()
		{
			base.OnMapChange();
			
			if ( m_Parrot != null )
			{
				m_Parrot.MoveToWorld( Location, Map );
				m_Parrot.Z += 12;				
			}
		}

		public ParrotPerchAddon( Serial serial ) : base( serial )
		{
		}

		public override void OnAfterDelete()
		{
			base.OnAfterDelete();
			
			if ( m_Parrot != null )
				m_Parrot.Delete();
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); //  version
			
			writer.Write( (Mobile) m_Parrot );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_Parrot = reader.ReadMobile() as PetParrot;
		}
	}
	
	public class ParrotPerchAddonDeed : BaseAddonDeed
	{
		public override int LabelNumber{ get{ return 1072619; } } // A deed for a Parrot Perch
		
		public override BaseAddon Addon
		{ 
			get
			{ 
				if ( m_Birth != DateTime.MinValue )
					return new ParrotPerchAddon( new PetParrot( m_Birth, m_PetName, m_PetHue ) ); 
				else
					return new ParrotPerchAddon();
			} 
		}
		
		
		private DateTime m_Birth;
		private string m_PetName;
		private int m_PetHue;
		
		[CommandProperty( AccessLevel.GameMaster)]
		public DateTime Birth
		{
			get { return m_Birth; }
			set { m_Birth = value; InvalidateProperties(); }
		} 
		
		[CommandProperty( AccessLevel.GameMaster)]
		public string PetName
		{
			get { return m_PetName; }
			set { m_PetName = value; InvalidateProperties(); }
		} 
		
		[CommandProperty( AccessLevel.GameMaster)]
		public int PetHue
		{
			get { return m_PetHue; }
			set { m_PetHue = value; InvalidateProperties(); }
		} 

		[Constructable]
		public ParrotPerchAddonDeed() : this( DateTime.MinValue, null, 0 )
		{
		}
		
		public ParrotPerchAddonDeed( DateTime birth, string name, int hue )
		{
			LootType = LootType.Blessed;
		
			m_Birth = birth;
			m_PetName = name;
			m_PetHue = hue;
		}

		public ParrotPerchAddonDeed( Serial serial ) : base( serial )
		{
		}			
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			if ( m_Birth != DateTime.MinValue )
			{
				if ( m_PetName != null )
					list.Add( 1072624, m_PetName ); // Includes a pet Parrot named ~1_NAME~
				else
					list.Add( 1072620 ); // Includes a pet Parrot
					
				int weeks = PetParrot.GetWeeks( m_Birth );
				
				if ( weeks == 1 )
					list.Add( 1072626 ); // 1 week old
				else if ( weeks > 1 )
					list.Add( 1072627, weeks.ToString() ); // ~1_AGE~ weeks old
			}
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); //  version
			
			writer.Write( (DateTime) m_Birth );
			writer.Write( (string) m_PetName );
			writer.Write( (int) m_PetHue );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
			
			m_Birth = reader.ReadDateTime();
			m_PetName = reader.ReadString();
			m_PetHue = reader.ReadInt();
		}
	}
}