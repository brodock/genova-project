using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBusiness.Object;
using Utils;
using Persistence.ControleTransacao;
using Persistence.Utilitarios;

namespace CoreBusiness.Controller
{
    public abstract class ControladorShardAccount
    {
        private static ObjShardAccount _shardAccountBase = new ObjShardAccount();

        /// <summary>
        /// Inclusion of registry does not believe the collection of persistence.
        /// </summary>
        public static void Manter(int id, string loginUsuario, string login, string senha, string CPF, string RG, bool doador, bool ativo, ColecaoPersistencia colecao)
        {
            ObjShardAccount shardAccount = new ObjShardAccount();

            if (id > 0)
                shardAccount.Materializar(id);

            bool existeAlteracao = false;

            if (!shardAccount.Login.Equals(login))
            {
                shardAccount.Login = login;
                existeAlteracao = true;
            }

            if (!shardAccount.Senha.Equals(senha))
            {
                shardAccount.Senha = senha;
                existeAlteracao = true;
            }

            if (!shardAccount.CPF.Equals(CPF))
            {
                shardAccount.CPF = CPF;
                existeAlteracao = true;
            }

            if (!shardAccount.RG.Equals(RG))
            {
                shardAccount.RG = RG;
                existeAlteracao = true;
            }

            if (!shardAccount.Doador.Equals(doador))
            {
                shardAccount.Doador = doador;
                existeAlteracao = true;
            }

            if (!shardAccount.Ativo.Equals(ativo))
            {
                shardAccount.Ativo = ativo;
                existeAlteracao = true;
            }

            if (existeAlteracao)
            {
                ObjPessoa pessoa = ControladorPessoa.GetPessoa(loginUsuario);
                shardAccount.NomeCompleto = pessoa.Nome;
                shardAccount.DataNascimento = pessoa.DataNascimento;
                shardAccount.Sexo = pessoa.Sexo;
                shardAccount.Email = pessoa.Email;
                shardAccount.DataCriacaoConta = DateTime.Now;

                if (shardAccount.ID > 0)
                    colecao.AdicionarItem(shardAccount, Persistence.Enumeradores.EnumTipoTransacao.Alterar);
                else
                {
                    shardAccount.Ativo = false;
                    bool inserted = shardAccount.Incluir();
                    if (inserted) ControladorUsuarioConta.LinkingWebAccountToShardAccount(pessoa.Usuario.ID, shardAccount.Login);
                }
            }
        }

        public static void Excluir(int id, ColecaoPersistencia colecao)
        {
            ObjShardAccount account = new ObjShardAccount(id);
            colecao.AdicionarItem(account, Persistence.Enumeradores.EnumTipoTransacao.Remover);
        }

        public static ObjShardAccount GetAccount(string login)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT {1}
            FROM {0}
            WHERE Login = '{2}'
            ", _shardAccountBase.Tabela, _shardAccountBase.ChavePrimaria, login);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                int id = Conversoes.Inteiro32(leitor.GetValor(_shardAccountBase.ChavePrimaria));
                return new ObjShardAccount(id);
            }
            throw new Exception(Erros.NaoEncontrado("Shard Account"));
        }

        public static LeitorFacade GetAccounts(string loginUsuario)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT T1.{1}, T1.Login, T1.Ativo
            FROM {0} T1
            INNER JOIN GeNova_Union_UsuarioConta T2 ON T1.IdUsuario = T2.IdGeNovaUsuario
            INNER JOIN GeNova_Web_Usuario T3 ON T3.IdUsuario = T2.IdGeNovaWebUsuario
            WHERE T3.Login = '{2}'
            ORDER BY Login ASC
            ", _shardAccountBase.Tabela, _shardAccountBase.ChavePrimaria, loginUsuario);
            return new LeitorFacade(query);
        }
    }
}
