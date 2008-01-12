using System;

namespace Server.Items
{
    public class BonecoDeNeve : Item
    {
        public override int ItemID { get { return 9000; } }

        [Constructable]
        public BonecoDeNeve()
            : base(0xE77)
        {
            Name = "Boneco de Neve";
        }

        public BonecoDeNeve(Serial serial)
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
    }
}

