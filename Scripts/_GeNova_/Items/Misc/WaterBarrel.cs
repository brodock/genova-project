using System;

namespace Server.Items
{
    public class WaterBarrelAddon : BaseContainer, IWaterSource
    {
        public override int ItemID { get { return 5453; } }

        public override Rectangle2D Bounds
        {
            get { return new Rectangle2D(33, 36, 109, 112); }
        }

        [Constructable]
        public WaterBarrelAddon()
            : base(0xE77)
        {
            Name = "Barril de Agua";
            Movable = false;
        }

        public WaterBarrelAddon(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        public int Quantity
        {
            get { return 500; }
            set { }
        }
    }
}