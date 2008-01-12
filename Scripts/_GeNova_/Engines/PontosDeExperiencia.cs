using System;
using System.Collections.Generic;
using System.Text;
using Server;
using Server.Mobiles;
using GeNova.Core.Controladores;

namespace GeNova.Server.Engines.XP
{
    public class PontosDeExperiencia
    {
        #region Construtores
        public PontosDeExperiencia(BaseCreature criatura)
        {
            this._criatura = criatura;
        }
        #endregion

        #region Atributos
        BaseCreature _criatura;
        #endregion

        #region Propriedades
        public List<DamageEntry> DanosInfligidos
        {
            get { return _criatura.DamageEntries; }
        }
        public Mobile AssassinadoPor
        {
            get { return _criatura.LastKiller; }
        }

        public bool Domado
        {
            get { return _criatura.Controlled; }
        }
        public Mobile MestreDomador
        {
            get { return this._criatura.ControlMaster; }
        }

        public bool Invocado
        {
            get { return this._criatura.Summoned; }
        }
        public Mobile MestreBruxo
        {
            get { return this._criatura.SummonMaster; }
        }

        public bool CriaturaRessureta
        {
            get { return this._criatura.IsBonded; }
        }

        public int ExperienciaDaCriatura
        {
            get
            {
                double valor = 0.0;
                valor += (_criatura.HitsMaxSeed * 0.03);
                valor += (_criatura.Fame * 0.02);
                valor += (_criatura.RawStatTotal * 0.10);
                return Convert.ToInt32(valor);
            }
        }
        #endregion

        #region Métodos
        private int CalcularValorGanhoComExperiencia(int famaDoJogador)
        {
            double valorTotal = this.ExperienciaDaCriatura - (famaDoJogador * 0.05);
            if (valorTotal < 0)
                valorTotal = 0;
            return Convert.ToInt32(valorTotal);
        }

        public void DistribuirPontosDeExperiencia()
        {
            for (int contador = (this.DanosInfligidos.Count - 1); contador >= 0; --contador)
            {
                if (contador >= this.DanosInfligidos.Count)
                    continue;

                DamageEntry danoInfligido = (DamageEntry)this.DanosInfligidos[contador];

                if (danoInfligido.HasExpired)
                    continue;

                Mobile atacante = danoInfligido.Damager;

                if (atacante == null || atacante.Deleted || !atacante.Player)
                    continue;
                else if (atacante == null || atacante.Deleted)
                    continue;

                if (atacante is BaseCreature)
                {
                    BaseCreature criatura = (BaseCreature)atacante;
                    if (criatura.Controlled && criatura.ControlMaster != null)
                        atacante = criatura.ControlMaster;
                    else if (criatura.Summoned && criatura.SummonMaster != null)
                        atacante = criatura.SummonMaster;
                }
                else if (atacante is PlayerMobile)
                {
                    if (this.AssassinadoPor is BaseGuard)
                        atacante.SendMessage("Os guardas fizeram o trabalho sujo! Você não recebe pontos de experiência.");
                    else if ((this.Domado && this.MestreDomador != null) || (this.MestreDomador == atacante) || (this.Invocado && this.MestreBruxo != null) || this.CriaturaRessureta)
                    {
                        if (this.Invocado && this.MestreBruxo != null)
                            atacante.SendMessage("Você matou um monstro invocado, e não recebe pontos de experiência.");
                        else if (this.MestreDomador == atacante || this.CriaturaRessureta)
                            atacante.SendMessage("Você matou um animal domado e não recebe pontos de experiência.");
                    }
                    else
                    {
                        PlayerMobile jogadorAtacante = atacante as PlayerMobile;
                        int pontosExperiencia = this.CalcularValorGanhoComExperiencia(jogadorAtacante.Fame);

                        if (pontosExperiencia < 0)
                            pontosExperiencia = 0;
                        
                        jogadorAtacante.SendMessage("Você recebeu {0} pontos de experiência", pontosExperiencia);                        
                        ControladorODBC.ODBCConcederPontosXP(jogadorAtacante.Account.Username, jogadorAtacante.Name, pontosExperiencia);
                    }
                }
            }
        }
        #endregion
    }
}
