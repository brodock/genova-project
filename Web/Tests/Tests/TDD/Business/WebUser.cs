using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitlibrary;
using Persistence.ControleTransacao;
using Security.Usuario.Controladores;
using Security.Usuario.Enumeradores;
using CoreBusiness.Controller;

namespace GeNova.Web.Tests.TDD.Business
{
    public class WebUser : SequenceFixture
    {
        public bool Create(string login, string senha, string nome, string email, char sexo, DateTime dataNascimento)
        {
            try
            {
                ColecaoPersistencia colecao = new ColecaoPersistencia();

                /*Criando Usuário*/
                ControladorUsuario.Manter(0, login, senha, TipoUsuario.Usuario, string.Empty, colecao);
                colecao.Persistir();

                /*Criando Pessoa*/
                colecao.Limpar();
                int idUsuario = ControladorUsuario.GetUsuarioPorLogin(login).ID;
                ControladorPessoa.Manter(0, idUsuario, nome, email, sexo, dataNascimento, colecao);
                colecao.Persistir();

                return true;
            }
            catch { return false; /*Usuário já existe no sistema*/ }
        }

        public bool Exists(string login)
        {
            try
            {
                return ControladorUsuario.GetUsuarioPorLogin(login).ID > 0;
            }
            catch { return false; }
        }

        public bool Delete(string login)
        {
            try
            {
                ColecaoPersistencia colecao = new ColecaoPersistencia();
                ControladorUsuario.Excluir(login, colecao);
                colecao.Persistir();
                return true;
            }
            catch { return false; /*Não foi possível remover o usuário*/ }
        }
    }
}
