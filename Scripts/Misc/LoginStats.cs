using System;
using Server.Network;
using GeNova.Server.Engines;
using System.Collections.Generic;

namespace Server.Misc
{
    public class LoginStats
    {
        public static void Initialize()
        {
            // Genova: check whether the status custom is alive, if so, do not show default status.
            if (!StatusLogin.Ativo)
                EventSink.Login += new LoginEventHandler(EventSink_Login); // Register our event handler
        }

        private static void EventSink_Login(LoginEventArgs args)
        {
            int userCount = NetState.Instances.Count;
            int itemCount = World.Items.Count;
            int mobileCount = World.Mobiles.Count;

            Mobile m = args.Mobile;

            m.SendMessage("Welcome, {0}! There {1} currently {2} user{3} online, with {4} item{5} and {6} mobile{7} in the world.",
                args.Mobile.Name,
                userCount == 1 ? "is" : "are",
                userCount, userCount == 1 ? "" : "s",
                itemCount, itemCount == 1 ? "" : "s",
                mobileCount, mobileCount == 1 ? "" : "s");

            #region GeNova: KR Support
            List<Server.Engines.Quests.MondainQuester> listMobilesQuester = new List<Server.Engines.Quests.MondainQuester>();
            foreach (Mobile m_mobile in World.Mobiles.Values)
            {
                Server.Engines.Quests.MondainQuester mQuester = m_mobile as Server.Engines.Quests.MondainQuester;
                if (mQuester != null)
                    listMobilesQuester.Add(mQuester);
            }

            foreach (Server.Engines.Quests.MondainQuester quester in listMobilesQuester)
            {
                if (args.Mobile.NetState != null)
                {
                    string name = string.Empty;
                    if (quester.Name != null)
                        name += quester.Name;
                    if (quester.Title != null)
                        name += " " + quester.Title;
                    args.Mobile.NetState.Send(new DisplayWaypoint(quester.Serial, quester.X, quester.Y, quester.Z, quester.Map.MapID, 4, name));
                }
            }
            #endregion
        }
    }
}