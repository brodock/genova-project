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

namespace GeNova.Core.Utilitarios.XML
{
    public abstract class XMLNames
    {
        #region Defaults Nodes
        public static readonly string Object = "object";
        #endregion

        #region Attributes
        public static readonly string Name = "name";
        public static readonly string Value = "value";
        public static readonly string Active = "active";
        public static readonly string Invulnerable = "invulnerable";
        #endregion

        #region genova.xml
        public static readonly string GeNova = "genova";
        public static readonly string Misc = "misc";
        public static readonly string Systems = "systems";
        public static readonly string Flags = "flags";

        public static readonly string UOPath = "uopath";

        public static readonly string CustomRace = "customrace";
        public static readonly string Elf = "elf";
        public static readonly string Human = "human";
        public static readonly string ActiveStatusCapFilter = "statuscapfilter";
        public static readonly string ActiveStatusGainFilter = "statusgainfilter";
        public static readonly string ActiveSkillsCapFilter = "skillsCapFilter";

        public static readonly string AntiMacro = "antimacro";
        public static readonly string Artifacts = "artifacts";
        public static readonly string MinorArtifacts = "minorartifacts";
        public static readonly string PowerScrolls = "powerscrolls";
        public static readonly string StatusCapScrolls = "statuscapscrolls";
        public static readonly string RewardSystem = "rewardsystem";
        public static readonly string SkillCapRewards = "skillcaprewards";
        public static readonly string RewardInterval = "rewardinterval";
        public static readonly string TreasureOfTokuno = "treasureoftokuno";
        public static readonly string YoungSystem = "youngsystem";
        public static readonly string Vendors = "vendors";
        #endregion

        #region mysql.xml
        public static readonly string MySql = "mysql";
        public static readonly string Server = "server";
        public static readonly string DataBase = "database";
        public static readonly string UserID = "userid";
        public static readonly string Password = "password";
        #endregion

        #region gumps.xml
        public static readonly string Gumps = "gumps";
        #endregion
    }
}
