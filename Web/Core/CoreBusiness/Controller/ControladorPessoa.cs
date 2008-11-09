using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBusiness.Object;
using Persistence.ControleTransacao;
using Security.Usuario.Controladores;
using Security.Usuario;
using Persistence.Utilitarios;
using Utils;

namespace CoreBusiness.Controller
{
    public abstract class ControladorPessoa
    {
        private static ObjPessoa _pessoaBase = new ObjPessoa();

        public static void Manter(int id, int idUsuario, string nome, string email, char sexo, DateTime dataNascimento, ColecaoPersistencia colecao)
        {
            ObjPessoa pessoa = new ObjPessoa();

            if (id > 0)
                pessoa.Materializar(id);

            bool existeAlteracoes = false;

            if (!pessoa.Usuario.ID.Equals(idUsuario))
            {
                pessoa.Usuario.ID = idUsuario;
                existeAlteracoes = true;
            }

            if (!pessoa.Nome.Equals(nome))
            {
                pessoa.Nome = nome;
                existeAlteracoes = true;
            }

            if (!pessoa.Email.Equals(email))
            {
                pessoa.Email = email;
                existeAlteracoes = true;
            }

            if (!pessoa.Sexo.Equals(sexo))
            {
                pessoa.Sexo = sexo;
                existeAlteracoes = true;
            }

            if (!pessoa.DataNascimento.Equals(dataNascimento))
            {
                pessoa.DataNascimento = dataNascimento;
                existeAlteracoes = true;
            }

            if (existeAlteracoes)
            {
                if (pessoa.ID > 0)
                    colecao.AdicionarItem(pessoa, Persistence.Enumeradores.EnumTipoTransacao.Alterar);
                else
                {
                    ObjUsuario usuario = new ObjUsuario(idUsuario);
                    pessoa.Usuario = usuario;

                    colecao.AdicionarItem(pessoa, Persistence.Enumeradores.EnumTipoTransacao.Incluir);
                }
            }
        }

        public static ObjPessoa GetPessoa(string loginUsuario)
        {
            ObjUsuario usuario = ControladorUsuario.GetUsuarioPorLogin(loginUsuario);

            StringBuilder query = new StringBuilder();
            query.AppendFormat("SELECT {0}\n", _pessoaBase.ChavePrimaria);
            query.AppendFormat("FROM {0}\n", _pessoaBase.Tabela);
            query.AppendFormat("WHERE 1=1 AND {0} = {1}", usuario.ChavePrimaria, usuario.ID);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                int idPessoa = Conversoes.Inteiro32(leitor.GetValor("IdPessoa"));
                if (idPessoa > 0)
                    return new ObjPessoa(idPessoa);
            }
            throw new Exception(Erros.NaoEncontrado("Person - Web Account"));
        }
    }
}
