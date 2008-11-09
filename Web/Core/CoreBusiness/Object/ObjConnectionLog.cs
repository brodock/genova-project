using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security;
using Utils;
using Persistence.Utilitarios;

namespace CoreBusiness.Object
{
    public class ObjConnectionLog : ModeloObjetoBase
    {
        #region Atributos e Propriedades

        public long ID { get; set; }
        public ObjShardAccount Conta { get; set; }
        public DateTime Entrada { get; set; }
        public DateTime Saida { get; set; }
        public string IP { get; set; }
        public bool Materializado { get; private set; }

        #endregion

        public ObjConnectionLog()
            : base("Registro_LogConexoes", "IdLogConexoes")
        {
            this.Reset();
        }

        public ObjConnectionLog(long id, bool materializarClasses)
            : this()
        {
            this.Materializar(id, materializarClasses);
        }

        #region Métodos

        public void Materializar(long id, bool materializarClasses)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT {2}, {3}, Entrada, Saida, IP 
            FROM {0} T1 
            INNER JOIN {1} T2 ON T1.{3} = T2.{3} 
            WHERE T1.{2} = {4}
            ", this.Tabela, this.Conta.Tabela, this.ChavePrimaria, this.Conta.ChavePrimaria, id);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro64(leitor.GetValor(this.ChavePrimaria));
                this.Conta.ID = Conversoes.Inteiro32(leitor.GetValor(this.Conta.ChavePrimaria));
                this.Entrada = Conversoes.DataHora(leitor.GetValor("Entrada"));
                this.Saida = Conversoes.DataHora(leitor.GetValor("Saida"));
                this.IP = leitor.GetValor("IP").ToString();

                if (materializarClasses)
                    this.Conta.Materializar(this.Conta.ID);

                this.Materializado = true;
            }
        }

        protected override void Reset()
        {
            this.ID = long.MinValue;
            this.Conta = new ObjShardAccount();
            this.Entrada = DateTime.MinValue;
            this.Saida = DateTime.MinValue;
            this.IP = string.Empty;
            this.Materializado = false;
        }

        protected override void PreencherListaItems()
        {
            if (this.ItemsPersistencia.Count <= 0)
            {
                this.ItemsPersistencia.AdicionarItem("IdLogConexoes", this.ID, this.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("IdUsuario", this.Conta.ID, this.Conta.ID.GetType(), this.Conta.ID <= 0);
                this.ItemsPersistencia.AdicionarItem("Entrada", this.Entrada, this.Entrada.GetType(), this.Entrada.Equals(DateTime.MinValue));
                this.ItemsPersistencia.AdicionarItem("Saida", this.Saida, this.Saida.GetType(), this.Saida.Equals(DateTime.MinValue));
                this.ItemsPersistencia.AdicionarItem("IP", this.IP, this.IP.GetType(), string.IsNullOrEmpty(this.IP));
            }
        }

        protected override bool Validar()
        {
            return true;
        }

        #endregion
    }
}
