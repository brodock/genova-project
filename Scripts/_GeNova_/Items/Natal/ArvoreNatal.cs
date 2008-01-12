using System;

namespace Server.Items
{
    public class ArvoreNatal : Item
    {
        public override int ItemID { get { return 0x1B7E; } }

        [Constructable]
        public ArvoreNatal()
            : base(0xE77)
        {
            Name = "Arvore de Natal";
        }

        public ArvoreNatal(Serial serial)
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

