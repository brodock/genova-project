using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;

namespace Persistence.ControleTransacao
{
    internal class OTransacao
    {
        #region Atributos
        private MySqlConnection _conexao;
        private MySqlTransaction _transacao;

        private bool _conexaoFechada;
        #endregion

        #region Propriedades
        internal MySqlConnection Conexao
        {
            get { return _conexao; }
            set { _conexao = value; }
        }

        internal MySqlTransaction Transacao
        {
            get { return _transacao; }
            set { _transacao = value; }
        }
        #endregion

        public OTransacao()
        {
            this._conexaoFechada = false;
        }

        #region Métodos
        public void Iniciar()
        {
            this._conexao = new MySqlConnection(Mysql.GetConfiguracoes());
            this._conexao.Open();

            this._transacao = this._conexao.BeginTransaction();            
        }

        private void Fechar()
        {
            if (!_conexaoFechada)
            {
                this._conexao.Close();
                this._transacao.Dispose();
            }
        }
        #endregion

        #region Métodos para Controle da Transação
        public void Commit()
        {
            try
            {
                this._transacao.Commit();
            }
            catch (Exception)
            {
                this.Rollback();
            }
            finally
            {
                this.Fechar();
            }
        }

        public void Rollback()
        {
            try
            {
                this._transacao.Rollback();
            }
            finally
            {
                this.Fechar();
            }
        }
        #endregion
    }
}