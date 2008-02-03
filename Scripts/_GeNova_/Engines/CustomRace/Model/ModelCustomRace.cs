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
using System.Text;
using Server.Mobiles;
using Server;
using GeNova.Core.Controladores;

namespace GeNova.Server.Engines.CustomRace
{
    public abstract class ModelCustomRace
    {
        #region Attributes readonly
        // View Table GeNova_Raca in database.
        private readonly int FirstColumnSkillsInTable = 2;
        private readonly int LastColumnSkillsInTable = 57;
        #endregion

        #region Attributes
        private string _name;
        private string _description;

        private PlayerMobile _player;
        private List<double> _listRaceSkillCapValues = new List<double>();
        #endregion

        #region Properties
        public string Name
        {
            get { return this._name; }
        }
        public string Description
        {
            get { return this._description; }
        }
        #endregion

        public ModelCustomRace(PlayerMobile player, string name, string description)
        {
            this._name = name;
            this._player = player;

            if (ActiveSkillsCapFilter())
                this.ModifyPlayerSkillsCap();
        }

        #region Methods : ActiveSkillsCapFilter
        private void ModifyPlayerSkillsCap()
        {
            this.SetDefaultValueInList();
            this.SetModifiedValuesInListRaceSkillCap();

            for (int i = 0; i < this._player.Skills.Length; i++)
            {
                this._player.Skills[i].Cap = this._listRaceSkillCapValues[i];

                if (this._player.Skills[i].Value > this._listRaceSkillCapValues[i])
                    this._player.Skills[i].Base = this._listRaceSkillCapValues[i];
            }
        }
        private void SetDefaultValueInList()
        {
            while (this._listRaceSkillCapValues.Count < SkillInfo.Table.Length)
                this._listRaceSkillCapValues.Add(0.00);
        }
        private void SetModifiedValuesInListRaceSkillCap()
        {
            List<double> listaSkills = ControladorODBC.ODBCRecuperarSkillsCustomRace(this._name, this.FirstColumnSkillsInTable, this.LastColumnSkillsInTable);
            for (int i = 0; i < this._listRaceSkillCapValues.Count; i++)
                this._listRaceSkillCapValues[i] = listaSkills[i];
        }
        #endregion

        #region Abstract Methods : Filters
        protected abstract bool ActiveStatusCapFilter();
        protected abstract bool ActiveStatusGainFilter();
        protected abstract bool ActiveSkillsCapFilter();
        #endregion
    }
}
