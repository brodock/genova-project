using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security;
using Persistence.Utilitarios;
using Utils;

namespace CoreBusiness.Object
{
    public class ObjCharacterAccount : ModeloObjetoBase
    {
        #region Atributos e Propriedades

        public long ID { get; set; }
        public ObjShardAccount Conta { get; set; }
        public string Nome { get; set; }
        public DateTime DataCriacao { get; set; }
        public bool Confirmado { get; set; }
        public int PontosExperiencia { get; private set; }
        public bool Materializado { get; private set; }

        #endregion

        public ObjCharacterAccount()
            : base("Genova_Usuario_Personagens", "IdPersonagem")
        {
            this.Reset();
        }

        public ObjCharacterAccount(long id, bool materializarClasses)
            : this()
        {
            this.Materializar(id, materializarClasses);
        }

        #region Métodos

        public void Materializar(long id, bool materializarClasses)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT T1.{1}, T1.NomePersonagem, T1.DataCadastro, T1.Confirmado, T2.PontosXP
            FROM {0} T1
            LEFT JOIN GeNova_PontosDeExperiencia T2 ON T1.IdPersonagem = T2.IdPersonagem
            WHERE 1=1
              AND T1.{1} = {2}
            ", this.Tabela, this.ChavePrimaria, id);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro64(leitor.GetValor(this.ChavePrimaria));
                this.Conta.ID = Conversoes.Inteiro32(leitor.GetValor(this.Conta.ChavePrimaria));
                this.Nome = leitor.GetValor("NomePersonagem").ToString();
                this.DataCriacao = Conversoes.DataHora(leitor.GetValor("DataCadastro"));
                this.Confirmado = Conversoes.Booleano(leitor.GetValor("Confirmado"));
                this.PontosExperiencia = Conversoes.Inteiro32(leitor.GetValor("PontosXP").ToString());

                if (materializarClasses)
                    this.Conta.Materializar(this.Conta.ID);

                this.Materializado = true;
            }
        }

        protected override void Reset()
        {
            this.ID = long.MinValue; 
            this.Conta = new ObjShardAccount(); 
            this.Nome = string.Empty; 
            this.DataCriacao = DateTime.MinValue;
            this.Confirmado = false; 
            this.PontosExperiencia = int.MinValue;
            this.Materializado = false;
        }

        protected override void PreencherListaItems()
        {
            if (this.ItemsPersistencia.Count <= 0)
            {
                this.ItemsPersistencia.AdicionarItem("IdPersonagem", this.ID, this.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("IdUsuario", this.Conta.ID, this.Conta.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("NomePersonagem", this.Nome, this.Nome.GetType());
                this.ItemsPersistencia.AdicionarItem("DataCadastro", this.DataCriacao, this.DataCriacao.GetType());
                this.ItemsPersistencia.AdicionarItem("Confirmado", this.Confirmado, this.Confirmado.GetType());
            }
        }

        protected override bool Validar()
        {
            if (string.IsNullOrEmpty(this.Nome))
                throw new Exception(Erros.ValorInvalido("Character", "Name"));

            if (this.DataCriacao.Equals(DateTime.MinValue))
                throw new Exception(Erros.ValorInvalido("Character", "Data of Creation"));

            return true;
        }

        #endregion
    }
}
