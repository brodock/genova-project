using System;
using Server.Network;
using Server.Spells;

namespace Server.Items
{
    public class FullSpellBook : Spellbook
    {
        public override SpellbookType SpellbookType { get { return SpellbookType.Regular; } }
        public override int BookOffset { get { return 0; } }
        public override int BookCount { get { return 64; } }        

        [Constructable]
        public FullSpellBook()
            : this((ulong.MaxValue))
        {
        }

        [Constructable]
        public FullSpellBook(ulong content)
            : base(content, 0xEFA)
        {
        }

        public FullSpellBook(Serial serial)
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
