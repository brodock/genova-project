using System;
using Server;
using Server.Targeting;
using Server.Targets;
using Server.Mobiles;
using Server.Commands;

namespace Server.Scripts.Commands
{
    public class SayCommand
    {
        public static void Initialize()
        {
            CommandSystem.Register("Say", AccessLevel.GameMaster, new CommandEventHandler(SayThis_OnCommand));
        }

        [Usage("Say <text>")]
        [Description("Forces Target to Say <text>.")]
        public static void SayThis_OnCommand(CommandEventArgs e)
        {
            string toSay = e.ArgString.Trim();
            if (toSay.Length > 0)
                e.Mobile.Target = new SayThisTarget(toSay);
            else
                e.Mobile.SendMessage("Format: SayThis \"<text>\"");
        }

        private class SayThisTarget : Target
        {
            private string m_toSay;

            public SayThisTarget(string s)
                : base(-1, false, TargetFlags.None)
            {
                m_toSay = s;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;
                    if (from != targ && from.AccessLevel > targ.AccessLevel)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} forcing speech on {2}", from.AccessLevel, CommandLogging.Format(from), CommandLogging.Format(targ));
                        targ.Say(m_toSay);
                    }
                }
            }
        }
    }
}