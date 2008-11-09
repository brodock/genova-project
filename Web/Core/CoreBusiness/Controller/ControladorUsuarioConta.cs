using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBusiness.Object;
using Security.Usuario.Controladores;
using Security.Usuario;
using Utils;
using Persistence.Utilitarios;

namespace CoreBusiness.Controller
{
    public abstract class ControladorUsuarioConta
    {
        private static ObjUsuarioConta _usuarioContaBase = new ObjUsuarioConta();

        public static void Manter(int id, int idUsuario, int idConta)
        {
            ObjUsuarioConta usuarioConta = new ObjUsuarioConta();

            if (id > 0)
                usuarioConta.Materializar(id, false);

            bool existeAlteracao = false;

            if (!usuarioConta.Usuario.ID.Equals(idUsuario))
            {
                usuarioConta.Usuario.ID = idUsuario;
                existeAlteracao = true;
            }

            if (!usuarioConta.Conta.ID.Equals(idConta))
            {
                usuarioConta.Conta.ID = idConta;
                existeAlteracao = true;
            }

            if (existeAlteracao)
            {
                if (id > 0)
                    usuarioConta.Alterar();
                else
                    usuarioConta.Incluir();
            }
        }

        public static void LinkingWebAccountToShardAccount(string loginUsuario, string loginShardAccount, string password)
        {
            ObjShardAccount conta = ControladorShardAccount.GetAccount(loginShardAccount);
            if (!conta.Senha.Equals(password))
                throw new Exception(Erros.MsgSenhaInvalida);

            bool isContaJaLinkada = ControladorUsuarioConta.ValidateTheExistenceOfLink(loginUsuario, loginShardAccount);
            if (isContaJaLinkada)
                throw new Exception("This account is already associated.");

            ObjUsuario usuario = ControladorUsuario.GetUsuarioPorLogin(loginUsuario);
            ControladorUsuarioConta.Manter(int.MinValue, usuario.ID, conta.ID);
        }

        internal static void LinkingWebAccountToShardAccount(int idUsuario, string loginShardAccount)
        {
            int idConta = ControladorShardAccount.GetAccount(loginShardAccount).ID;
            ControladorUsuarioConta.Manter(int.MinValue, idUsuario, idConta);
        }

        public static bool ValidateTheExistenceOfLink(string loginUsuario, string loginShardAccount)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT *
            FROM {0}
            WHERE 1=1
              AND IdGeNovaWebUsuario = (SELECT IdUsuario FROM GeNova_Web_Usuario WHERE Login = '{1}')
              AND IdGeNovaUsuario = (SELECT IdUsuario FROM GeNova_Usuario WHERE Login = '{2}')
            ", _usuarioContaBase.Tabela, loginUsuario, loginShardAccount);
            
            LeitorFacade leitor = new LeitorFacade(query);
            if (leitor.LerLinha())
                return true;
            
            return false;
        }
    }
}
