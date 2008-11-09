using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security;
using Persistence.Utilitarios;
using Utils;
using CoreBusiness.Interface;

namespace CoreBusiness.Object
{
    internal class ObjStatusServidor : ModeloObjetoBase, IServerStatus
    {
        #region Atributos e Propriedades

        public int ID { get; private set; }
        public string Descricao { get; private set; }
        public string IP { get; private set; }
        public string Porta { get; private set; }
        public string ClienteRequerido { get; private set; }
        public string UsuariosOnline { get; private set; }
        public string TotalMobiles { get; private set; }
        public string TotalItens { get; private set; }
        public DateTime TempoOnline { get; private set; }
        public DateTime DataHoraIniciado { get; private set; }
        public bool Materializado { get; private set; }

        #endregion

        public ObjStatusServidor()
            : base("GeNova_StatusServidor", "IdWebStatus")
        {
            this.Reset();
            this.Materializar();
        }

        #region Métodos

        public void Materializar()
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT {1}, Descricao, IP, Porta, ClienteRequerido, UsuariosOnline, TotalMobiles, TotalItens, TempoOnline, DataHoraIniciado
            FROM {0}
            ORDER BY {1} DESC
            LIMIT 1
            ", this.Tabela, this.ChavePrimaria);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro32(leitor.GetValor(this.ChavePrimaria));
                this.Descricao = leitor.GetValor("Descricao").ToString();
                this.IP = leitor.GetValor("IP").ToString();
                this.Porta = leitor.GetValor("Porta").ToString();
                this.ClienteRequerido = leitor.GetValor("ClienteRequerido").ToString();
                this.UsuariosOnline = leitor.GetValor("UsuariosOnline").ToString();
                this.TotalMobiles = leitor.GetValor("TotalMobiles").ToString();
                this.TotalItens = leitor.GetValor("TotalItens").ToString();

                string strTempoOnline = leitor.GetValor("TempoOnline").ToString();
                strTempoOnline = strTempoOnline.Substring(0, (strTempoOnline.Length - 3));
                this.TempoOnline = Conversoes.DataHora(strTempoOnline);

                this.DataHoraIniciado = Conversoes.DataHora(leitor.GetValor("DataHoraIniciado"));
                this.Materializado = true;
            }
        }

        protected override void Reset()
        {
            this.ID = int.MinValue;
            this.Descricao = string.Empty;
            this.IP = string.Empty;
            this.Porta = string.Empty;
            this.ClienteRequerido = string.Empty;
            this.UsuariosOnline = string.Empty;
            this.TotalMobiles = string.Empty;
            this.TotalItens = string.Empty;
            this.TempoOnline = DateTime.MinValue;
            this.DataHoraIniciado = DateTime.MinValue;
            this.Materializado = false;
        }

        protected override void PreencherListaItems()
        {
            throw new NotImplementedException();
        }

        protected override bool Validar()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IStatusServidor Members

        public string GetDescription()
        {
            return Outros.StringDefault(this.Descricao);
        }

        public string GetIP()
        {
            return Outros.StringDefault(this.IP);
        }

        public string GetPort()
        {
            return Outros.StringDefault(this.Porta);
        }

        public string GetClientRequirement()
        {
            return Outros.StringDefault(this.ClienteRequerido);
        }

        public string GetOnlineUsers()
        {
            return Outros.StringDefault(this.UsuariosOnline);
        }

        public string GetTotalMobiles()
        {
            return Outros.StringDefault(this.TotalMobiles);
        }

        public string GetTotalItens()
        {
            return Outros.StringDefault(this.TotalItens);
        }

        public DateTime GetServerUptime()
        {
            return this.TempoOnline;
        }

        public DateTime GetLastInfoUpdated()
        {
            return this.DataHoraIniciado;
        }

        #endregion
    }
}
