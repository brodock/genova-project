using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using fitlibrary;
using CoreBusiness.Controller;
using Persistence.ControleTransacao;

namespace GeNova.Web.Tests.TDD.Business
{
    public class ShardUser : SequenceFixture
    {
        public bool Create(string loginUsuario, string loginConta, string senhaConta)
        {
            try
            {
                ColecaoPersistencia colecao = new ColecaoPersistencia();
                ControladorShardAccount.Manter(0, loginUsuario, loginConta, senhaConta, string.Empty, string.Empty, false, false, colecao);
                colecao.Persistir();
                return true;
            }
            catch { return false; /*Conta já existe*/ }
        }

        public bool Linked(string loginUsuario, string loginConta)
        {
            try
            {
                return ControladorUsuarioConta.ValidateTheExistenceOfLink(loginUsuario, loginConta);
            }
            catch { return false; /*Alguma falha no processamento*/ }
        }

        public bool Delete(string loginConta)
        {
            try
            {
                ColecaoPersistencia colecao = new ColecaoPersistencia();
                int id = ControladorShardAccount.GetAccount(loginConta).ID;
                ControladorShardAccount.Excluir(id, colecao);
                colecao.Persistir();
                return true;
            }
            catch { return false; /*Não foi possível remover a conta*/ }
        }
    }
}
