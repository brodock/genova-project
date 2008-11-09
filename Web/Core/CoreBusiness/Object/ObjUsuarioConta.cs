using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security;
using Security.Usuario;
using Utils;
using Persistence.Utilitarios;

namespace CoreBusiness.Object
{
    internal class ObjUsuarioConta : ModeloObjetoBase
    {
        #region Atributos e Propriedades

        public int ID { get; set; }
        public ObjUsuario Usuario { get; set; }
        public ObjShardAccount Conta { get; set; }
        public bool Materializado { get; private set; }

        #endregion

        public ObjUsuarioConta()
            : base("GeNova_Union_UsuarioConta", "IdUsuarioConta")
        {
            this.Reset();
        }

        public ObjUsuarioConta(int id, bool materializarClasses)
            : this()
        {
            this.Materializar(id, materializarClasses);
        }

        #region Métodos

        public void Materializar(int id, bool materializarClasses)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT {1}, IdGeNovaWebUsuario, IdGeNovaUsuario
            FROM {0}
            WHERE {1} = {2}
            ", this.Tabela, this.ChavePrimaria, id);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro32(leitor.GetValor("IdUsuarioConta"));
                this.Usuario.ID = Conversoes.Inteiro32(leitor.GetValor("IdGeNovaWebUsuario"));
                this.Conta.ID = Conversoes.Inteiro32(leitor.GetValor("IdGeNovaUsuario"));
                if (materializarClasses)
                {
                    this.Usuario.Materializar(this.Usuario.ID);
                    this.Conta.Materializar(this.Conta.ID);
                }
                this.Materializado = true;
            }
        }

        protected override void Reset()
        {
            this.ID = int.MinValue;
            this.Usuario = new ObjUsuario();
            this.Conta = new ObjShardAccount();
        }

        protected override void PreencherListaItems()
        {
            if (this.ItemsPersistencia.Count <= 0)
            {
                this.ItemsPersistencia.AdicionarItem("IdGeNovaWebUsuario", this.Usuario.ID, this.Usuario.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("IdGeNovaUsuario", this.Conta.ID, this.Conta.ID.GetType());
            }
        }

        protected override bool Validar()
        {
            if (this.Usuario.ID <= 0)
                throw new Exception(Erros.ObjetoInvalido("Web User"));

            if (this.Conta.ID <= 0)
                throw new Exception(Erros.ObjetoInvalido("Shard Account"));

            return true;
        }

        #endregion
    }
}
