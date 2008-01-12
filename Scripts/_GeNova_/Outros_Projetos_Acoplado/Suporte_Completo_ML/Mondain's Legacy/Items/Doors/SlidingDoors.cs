using System;
using Server;

namespace Server.Items
{	
	public class SlidingDoor : BaseDoor
	{
		[Constructable]
		public SlidingDoor( DoorFacing facing ) : base( 0x2A05 + (2 * (int)facing), 0x2A06 + (2 * (int)facing), 0, 0, BaseDoor.GetOffset( facing ) )
		{
		}

		public SlidingDoor( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer ) // Default Serialize method
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader ) // Default Deserialize method
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	
	public class SlidingDoorLight : BaseDoor
	{
		[Constructable]
		public SlidingDoorLight( DoorFacing facing ) : base( 0x2A0D + (2 * (int)facing), 0x2A0E + (2 * (int)facing), 0, 0, BaseDoor.GetOffset( facing ) )
		{
		}

		public SlidingDoorLight( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer ) // Default Serialize method
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader ) // Default Deserialize method
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
	
	public class SlidingDoorDark : BaseDoor
	{
		[Constructable]
		public SlidingDoorDark( DoorFacing facing ) : base( 0x2A15 + (2 * (int)facing), 0x2A16 + (2 * (int)facing), 0, 0, BaseDoor.GetOffset( facing ) )
		{
		}

		public SlidingDoorDark( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer ) // Default Serialize method
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader ) // Default Deserialize method
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}