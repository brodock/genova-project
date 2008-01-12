using System;
using Server;

namespace Server.Items
{
	[PropertyObject]
	public class TalismanAttribute
	{
		private Type m_Type;
		private int m_Amount;
		private object m_Name;
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Type Type
		{
			get{ return m_Type; }
			set{ m_Type = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public int Amount
		{
			get{ return m_Amount; }
			set{ m_Amount = value; }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public object Name
		{
			get{ return m_Name; }
			set{ m_Name = value; }
		}		
		
		public TalismanAttribute()
		{
		}
		
		public TalismanAttribute( Type type, int amount, object name )
		{
			m_Type = type;
			m_Amount = amount;
			m_Name = name;
		}
		
		public override string ToString()
		{
			return "...";
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
			Type				= 0x00000001,
			Name				= 0x00000002,
			Amount				= 0x00000004,
		}
		
		public virtual void Serialize( GenericWriter writer )
		{
			writer.Write( (int) 0 ); // version
			
			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Type,		m_Type != null );
			SetSaveFlag( ref flags, SaveFlag.Name,		m_Name != null );
			SetSaveFlag( ref flags, SaveFlag.Amount,	m_Amount != 0 );
			
			writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Type ) )
				writer.Write( m_Type.FullName );

			if ( GetSaveFlag( flags, SaveFlag.Name ) )
			{
				if ( m_Name is int )
				{
					writer.WriteEncodedInt( 0x1 );
					writer.WriteEncodedInt( (int) m_Name );
				}
				else if ( m_Name is String )
				{
					writer.WriteEncodedInt( 0x2 );
					writer.Write( (String) m_Name );
				}
				else
					writer.WriteEncodedInt( 0x0 );
			}
				
			if ( GetSaveFlag( flags, SaveFlag.Amount ) )				
				writer.WriteEncodedInt( m_Amount );			
		}	
		
		public virtual void Deserialize( GenericReader reader )
		{
			int version = reader.ReadInt();
			
			SaveFlag flags = (SaveFlag) reader.ReadEncodedInt();
			
			if ( GetSaveFlag( flags, SaveFlag.Type ) )
				m_Type = ScriptCompiler.FindTypeByFullName( reader.ReadString(), false );
				
			if ( GetSaveFlag( flags, SaveFlag.Name ) )
			{
				int nameType = reader.ReadEncodedInt();
			
				switch ( nameType )
				{
					case 0x0: break;
					case 0x1: m_Name = reader.ReadEncodedInt(); break;
					case 0x2: m_Name = reader.ReadString(); break;
				}
			}
				
			if ( GetSaveFlag( flags, SaveFlag.Amount ) )
				m_Amount = reader.ReadEncodedInt();
		}
	}
}