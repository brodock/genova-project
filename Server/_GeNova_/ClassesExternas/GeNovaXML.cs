/***************************************************************************
 *   copyright            : (C) GeNova Project
 *   webSite              : http://code.google.com/p/genovaproject/
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GeNova.Core.Utilitarios.XML;
using System.Xml;

namespace GeNova.Core.ClassesExternas
{
    public abstract class GeNovaXML
    {
        private static UtilitariosXML XmlUtils = new UtilitariosXML(CaminhosXML.FilePath_Configs_GeNova);
        private static XmlNode GeNovaNode = XmlUtils.GetRootNode(XMLNames.GeNova);

        #region Misc
        public static string UOPath
        {
            get
            {
                XmlNode miscNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Misc);
                XmlNode uoPathNode = XmlUtils.GetSingleNode(miscNode, XMLNames.UOPath);
                return XmlUtils.GetAttributeValue(uoPathNode);
            }
        }
        #endregion

        #region Systems
        public static bool CustomRace_Active
        {
            get
            {
                XmlNode systemsNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Systems);
                XmlNode customRaceNode = XmlUtils.GetSingleNode(systemsNode, XMLNames.CustomRace);
                return Convert.ToBoolean(XmlUtils.GetAttributeValue(customRaceNode, XMLNames.Active));
            }
        }

        public static bool CustomRace_Property_Active(string customRaceName, string propertyName)
        {
            XmlNode systemsNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Systems);
            XmlNode customRaceNode = XmlUtils.GetSingleNode(systemsNode, XMLNames.CustomRace);
            XmlNode raceNode = XmlUtils.GetSingleNode(customRaceNode, customRaceName);
            XmlNode propertyNode = XmlUtils.GetSingleNode(raceNode, propertyName);
            return Convert.ToBoolean(XmlUtils.GetAttributeValue(propertyNode, XMLNames.Active));
        }
        #endregion

        #region Flags
        public static bool Flags_Active(string flagName)
        {
            XmlNode flagsNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Flags);
            XmlNode flagSelectedNode = XmlUtils.GetSingleNode(flagsNode, flagName);
            return Convert.ToBoolean(XmlUtils.GetAttributeValue(flagSelectedNode, XMLNames.Active));
        }

        public static bool Flags_RewardSystem_SkillcapRewards_Active
        {
            get
            {
                XmlNode flagsNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Flags);
                XmlNode rewardSystemNode = XmlUtils.GetSingleNode(flagsNode, XMLNames.RewardSystem);
                XmlNode skillCapRewardsNode = XmlUtils.GetSingleNode(rewardSystemNode, XMLNames.SkillCapRewards);
                return Convert.ToBoolean(XmlUtils.GetAttributeValue(skillCapRewardsNode, XMLNames.Active));
            }
        }
        public static TimeSpan Flags_RewardSystem_RewardInterval
        {
            get
            {
                XmlNode flagsNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Flags);
                XmlNode rewardSystemNode = XmlUtils.GetSingleNode(flagsNode, XMLNames.RewardSystem);
                XmlNode rewardIntervalNode = XmlUtils.GetSingleNode(rewardSystemNode, XMLNames.RewardInterval);
                double days = Convert.ToDouble(XmlUtils.GetAttributeValue(rewardIntervalNode));
                return TimeSpan.FromDays(days);
            }
        }

        public static bool Flags_Vendors_Invulnerable
        {
            get
            {
                XmlNode flagsNode = XmlUtils.GetSingleNode(GeNovaNode, XMLNames.Flags);
                XmlNode vendorsNode = XmlUtils.GetSingleNode(flagsNode, XMLNames.Vendors);
                return Convert.ToBoolean(XmlUtils.GetAttributeValue(vendorsNode, XMLNames.Invulnerable));
            }
        }
        #endregion
    }
}
