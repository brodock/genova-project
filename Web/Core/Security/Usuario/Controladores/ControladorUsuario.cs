using System;
using System.Collections.Generic;
using System.Text;
using Persistence.Utilitarios;
using Security.Criptografia;
using Security.Usuario.Enumeradores;
using Security.Usuario.Excessoes;
using Persistence.ControleTransacao;
using Utils;

namespace Security.Usuario.Controladores
{
    public abstract class ControladorUsuario
    {
        private static ObjUsuario _usuarioBase = new ObjUsuario();

        public static void Manter(int id, string login, string senha, TipoUsuario tipo, string avatar, ColecaoPersistencia colecao)
        {
            ObjUsuario usuario = new ObjUsuario();

            if (id > 0)
                usuario.Materializar(id);

            bool existeAlteracoes = false;

            if (!String.IsNullOrEmpty(login) && !usuario.Login.Equals(login))
            {
                usuario.Login = login;
                existeAlteracoes = true;
            }

            Criptografia.Criptografia criptografia = new CriptografiaMD5();
            string senhaCriptografada = criptografia.Criptografar(senha);
            if (!String.IsNullOrEmpty(senha) && !usuario.Senha.Equals(senhaCriptografada))
            {
                usuario.Senha = senhaCriptografada;
                existeAlteracoes = true;
            }

            if (!usuario.Tipo.Equals(tipo))
            {
                usuario.Tipo = tipo;
                existeAlteracoes = true;
            }

            if (!String.IsNullOrEmpty(avatar) && !usuario.Avatar.Equals(avatar))
            {
                usuario.Avatar = avatar;
                existeAlteracoes = true;
            }

            if (existeAlteracoes)
            {
                if (usuario.ID > 0)
                    colecao.AdicionarItem(usuario, Persistence.Enumeradores.EnumTipoTransacao.Alterar);
                else
                    colecao.AdicionarItem(usuario, Persistence.Enumeradores.EnumTipoTransacao.Incluir);
            }
        }

        public static void Excluir(string login, ColecaoPersistencia colecao)
        {
            ObjUsuario usuario = ControladorUsuario.GetUsuarioPorLogin(login);
            colecao.AdicionarItem(usuario, Persistence.Enumeradores.EnumTipoTransacao.Remover);
        }

        public static void Autenticar(string login, string senha)
        {
            // Previnindo Sql Injection
            SqlUtils.PrevencaoSqlInjection(ref login);
            SqlUtils.PrevencaoSqlInjection(ref senha);

            ObjUsuario usuario = new ObjUsuario();

            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT IdUsuario");
            query.AppendFormat("FROM {0}\n", _usuarioBase.Tabela);
            query.AppendFormat("WHERE 1=1 AND Login = '{0}'", login);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                int idUsuario = Conversoes.Inteiro32(leitor.GetValor("IdUsuario"));
                usuario.Materializar(idUsuario);
            }
            else
                throw new UsuarioNaoExisteException(Erros.MsgUsuarioInexistente);

            usuario.Autenticar(senha);
            UsuarioCorrenteSingleton.Istance.Usuario = usuario;
        }

        public static void Desconectar()
        {
            UsuarioCorrenteSingleton.Destroy();
        }

        #region  Outros Metodos
        public static ObjUsuario GetUsuarioPorLogin(string login)
        {
            StringBuilder query = new StringBuilder();
            query.AppendLine("SELECT IdUsuario");
            query.AppendFormat("FROM {0}\n", _usuarioBase.Tabela);
            query.AppendFormat("WHERE 1=1 AND Login = '{0}'", login);
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
            {
                int idUsuario = Conversoes.Inteiro32(leitor.GetValor("IdUsuario"));
                if (idUsuario > 0)
                    return new ObjUsuario(idUsuario);
            }
            throw new Exception(Erros.NaoEncontrado("Web User"));
        }
        #endregion

        #region Métodos Auxiliares
        public static int GetIdUsuarioLogado()
        {
            return UsuarioCorrenteSingleton.Istance.Usuario.ID;
        }
        public static string GetLoginUsuarioLogado()
        {
            return UsuarioCorrenteSingleton.Istance.Usuario.Login;
        }
        public static string GetTipoUsuarioLogado()
        {
            return UsuarioCorrenteSingleton.Istance.Usuario.Tipo.ToString();
        }
        public static string GetAvatarUsuarioLogado()
        {
            return UsuarioCorrenteSingleton.Istance.Usuario.AvatarCompleto.ToString();
        }

        public static bool IsUsuarioLogado()
        {
            return UsuarioCorrenteSingleton.Istance.Usuario.Autenticado;
        }
        public static bool IsAdministrador()
        {
            return UsuarioCorrenteSingleton.Istance.Usuario.Tipo == Security.Usuario.Enumeradores.TipoUsuario.Administrador;
        }

        /// <summary>
        /// Alterar senha usuário corrente (logado).
        /// </summary>        
        public static void AtualizarSenhaUsuarioLogado(string senhaAtual, string novaSenha, ColecaoPersistencia colecao)
        {
            if ((!string.IsNullOrEmpty(senhaAtual) && string.IsNullOrEmpty(novaSenha)) ||
                (!string.IsNullOrEmpty(novaSenha) && string.IsNullOrEmpty(senhaAtual)))
                throw new Usuario.Excessoes.AlterarSenhaDadosIncompletosException("Não foi possível Alterar a senha, dados incompletos.");

            if (!string.IsNullOrEmpty(senhaAtual) && !string.IsNullOrEmpty(novaSenha))
            {
                ObjUsuario usuario = UsuarioCorrenteSingleton.Istance.Usuario;

                Criptografia.Criptografia criptografia = new CriptografiaMD5();
                senhaAtual = criptografia.Criptografar(senhaAtual);

                if (senhaAtual.Equals(usuario.Senha))
                {
                    novaSenha = criptografia.Criptografar(novaSenha);
                    usuario.Senha = novaSenha;
                    colecao.AdicionarItem(usuario, Persistence.Enumeradores.EnumTipoTransacao.Alterar);                    
                }
                else
                    throw new Usuario.Excessoes.SenhaInvalidaException("A senha informada não confere.");
            }
        }

        /// <summary>
        /// Este método irá atualizar o avatar do usuário corrente (logado). Usado apenas na troca de Avatar.
        /// </summary>
        public static void AtualizarAvatarUsuarioLogado(string avatar, ColecaoPersistencia colecao)
        {
            if (!string.IsNullOrEmpty(avatar))
            {
                ObjUsuario usuario = UsuarioCorrenteSingleton.Istance.Usuario;
                usuario.Avatar = avatar;
                colecao.AdicionarItem(usuario, Persistence.Enumeradores.EnumTipoTransacao.Alterar);
            }
        }
        #endregion
    }
}
