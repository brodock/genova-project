using Server.Mobiles;

namespace Server.Items
{
    public class KarmaBong : Item
    {
        [Constructable]
        public KarmaBong()
            : base(0xED4)
        {
            Movable = false;
            Name = "Pedra de Status";
        }

        public KarmaBong(Serial serial)
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

        public override void OnDoubleClick(Mobile from)
        {
            PlayerMobile pm = from as PlayerMobile;
            Effects.PlaySound(from.Location, from.Map, 33);
            from.SendMessage(0, "{0}, esta com {1} pontos de Karma e {2} pontos de Fama. Voce possui {3} mortes em seu nome.", from.Name, from.Karma, from.Fame, from.Kills);
        }
    }
}