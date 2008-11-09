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
    public class ObjPessoa : ModeloObjetoBase
    {
        #region Atributos e Propriedades

        public int ID { get; set; }
        public ObjUsuario Usuario { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public char Sexo { get; set; }
        public DateTime DataNascimento { get; set; }
        public bool Materializado { get; protected set; }

        #endregion

        public ObjPessoa()
            : base("GeNova_Web_Pessoa", "IdPessoa")
        {
            this.Reset();
        }

        public ObjPessoa(int id)
            : this()
        {
            this.Materializar(id);
        }

        #region Métodos

        public void Materializar(int id)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT T1.{2}, T2.{3}, Nome, Email, Sexo, DataNascimento
            FROM {0} T1
            INNER JOIN {1} T2 on T1.IdUsuario = T2.IdUsuario
            WHERE 1=1 AND {2} = {4}
            ", this.Tabela, this.Usuario.Tabela, this.ChavePrimaria, this.Usuario.ChavePrimaria, id);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro32(leitor.GetValor(this.ChavePrimaria));

                int idUsuario = Conversoes.Inteiro32(leitor.GetValor("IdUsuario"));
                this.Usuario.Materializar(idUsuario);

                this.Nome = leitor.GetValor("Nome").ToString();
                this.Email = leitor.GetValor("Email").ToString();
                this.Sexo = Conversoes.Char(leitor.GetValor("Sexo"));
                this.DataNascimento = Conversoes.DataHora(leitor.GetValor("DataNascimento"));

                this.Materializado = true;
            }
        }

        protected override void Reset()
        {
            this.ID = int.MinValue;
            this.Usuario = new ObjUsuario();
            this.Nome = string.Empty;
            this.Email = string.Empty;
            this.Sexo = char.MinValue;
            this.DataNascimento = DateTime.MinValue;
            this.Materializado = false;
        }

        protected override void PreencherListaItems()
        {
            if (this.ItemsPersistencia.Count <= 0)
            {
                this.ItemsPersistencia.AdicionarItem("IdPessoa", this.ID, this.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("IdUsuario", this.Usuario.ID, this.Usuario.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("Nome", this.Nome, this.Nome.GetType());
                this.ItemsPersistencia.AdicionarItem("Email", this.Email, this.Email.GetType());
                this.ItemsPersistencia.AdicionarItem("Sexo", this.Sexo, this.Sexo.GetType());
                this.ItemsPersistencia.AdicionarItem("DataNascimento", this.DataNascimento, this.DataNascimento.GetType());
            }
        }

        protected override bool Validar()
        {
            if (this.Usuario.ID <= 0)
                throw new Exception(Erros.ObjetoInvalido("Web User"));

            if (string.IsNullOrEmpty(this.Nome))
                throw new Exception(Erros.ValorInvalido("Person - Web User", "Full Name"));

            if (string.IsNullOrEmpty(this.Email))
                throw new Exception(Erros.ValorInvalido("Person - Web User", "E-mail"));

            if (this.Sexo.Equals(char.MinValue))
                throw new Exception(Erros.ValorInvalido("Person - Web User", "Sex"));

            if (this.DataNascimento.Equals(DateTime.MinValue))
                throw new Exception(Erros.ValorInvalido("Person - Web User", "Birth date"));

            return true;
        }

        #endregion
    }
}
