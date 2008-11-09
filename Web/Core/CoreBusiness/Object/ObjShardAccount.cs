using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Security;
using Utils;
using Persistence.Utilitarios;

namespace CoreBusiness.Object
{
    public class ObjShardAccount : ModeloObjetoBase
    {
        #region Atributos e Propriedades

        public int ID { get; set; }
        public string Login { get; set; }
        public string Senha { get; set; }
        public string NomeCompleto { get; set; }
        public DateTime DataNascimento { get; set; }
        public char Sexo { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public string Email { get; set; }
        public DateTime DataCriacaoConta { get; set; }
        public bool Doador { get; set; }
        public bool Ativo { get; set; }
        public bool Materializado { get; private set; }

        #endregion

        public ObjShardAccount()
            : base("Genova_Usuario", "IdUsuario")
        {
            this.Reset();
        }

        public ObjShardAccount(int id)
            : this()
        {
            this.Materializar(id);
        }

        #region Métodos

        public void Materializar(int id)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT {1}, Login, Senha, NomeCompleto, DataNascimento, Sexo, CPF, RG, Email, DataCriacaoConta, Doador, Ativo
            FROM {0}
            WHERE {1} = {2}
            ", this.Tabela, this.ChavePrimaria, id);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                this.ID = Conversoes.Inteiro32(leitor.GetValor(this.ChavePrimaria));
                this.Login = leitor.GetValor("Login").ToString();
                this.Senha = leitor.GetValor("Senha").ToString();
                this.NomeCompleto = leitor.GetValor("NomeCompleto").ToString();
                this.DataNascimento = Conversoes.DataHora(leitor.GetValor("DataNascimento"));
                this.Sexo = Conversoes.Char(leitor.GetValor("Sexo"));
                this.CPF = leitor.GetValor("CPF").ToString();
                this.RG = leitor.GetValor("RG").ToString();
                this.Email = leitor.GetValor("Email").ToString();
                this.DataCriacaoConta = Conversoes.DataHora(leitor.GetValor("DataCriacaoConta"));
                this.Doador = Conversoes.Booleano(leitor.GetValor("Doador"));
                this.Ativo = Conversoes.Booleano(leitor.GetValor("Ativo"));
                this.Materializado = true;
            }
        }

        protected override void Reset()
        {
            this.ID = int.MinValue;
            this.Login = string.Empty;
            this.Senha = string.Empty;
            this.NomeCompleto = string.Empty;
            this.DataNascimento = DateTime.MinValue;
            this.Sexo = char.MinValue;
            this.CPF = string.Empty;
            this.RG = string.Empty;
            this.Email = string.Empty;
            this.DataCriacaoConta = DateTime.Now;
            this.Doador = false;
            this.Ativo = false;
            this.Materializado = false;
        }

        protected override void PreencherListaItems()
        {
            if (this.ItemsPersistencia.Count <= 0)
            {
                this.ItemsPersistencia.AdicionarItem("IdUsuario", this.ID, this.ID.GetType());
                this.ItemsPersistencia.AdicionarItem("Login", this.Login, this.Login.GetType());
                this.ItemsPersistencia.AdicionarItem("Senha", this.Senha, this.Senha.GetType());
                this.ItemsPersistencia.AdicionarItem("NomeCompleto", this.NomeCompleto, this.NomeCompleto.GetType());
                this.ItemsPersistencia.AdicionarItem("DataNascimento", this.DataNascimento, this.DataNascimento.GetType());
                this.ItemsPersistencia.AdicionarItem("Sexo", this.Sexo, this.Sexo.GetType());
                this.ItemsPersistencia.AdicionarItem("CPF", this.CPF, this.CPF.GetType(), string.IsNullOrEmpty(this.CPF));
                this.ItemsPersistencia.AdicionarItem("RG", this.RG, this.RG.GetType(), string.IsNullOrEmpty(this.RG));
                this.ItemsPersistencia.AdicionarItem("Email", this.Email, this.Email.GetType());
                this.ItemsPersistencia.AdicionarItem("DataCriacaoConta", this.DataCriacaoConta, this.DataCriacaoConta.GetType());
                this.ItemsPersistencia.AdicionarItem("Doador", this.Doador, this.Doador.GetType());
                this.ItemsPersistencia.AdicionarItem("Ativo", this.Ativo, this.Ativo.GetType());
            }
        }

        protected override bool Validar()
        {
            if (string.IsNullOrEmpty(this.Login))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "Login"));

            if (string.IsNullOrEmpty(this.Senha))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "Password"));

            if (string.IsNullOrEmpty(this.NomeCompleto))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "Full Name"));

            if (this.DataNascimento.Equals(DateTime.MinValue))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "Birth Date"));

            if (this.Sexo.Equals(char.MinValue))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "Sex"));

            if (string.IsNullOrEmpty(this.Email))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "E-mail"));

            if (this.DataCriacaoConta.Equals(DateTime.MinValue))
                throw new Exception(Erros.ValorInvalido("ShardAccount", "Date of the Account Creation"));

            return true;
        }

        #endregion
    }
}
