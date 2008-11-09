using System;
using System.Collections.Generic;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;

namespace Persistence.Utilitarios
{
    public class LeitorFacade
    {
        #region Atributos - Somente Leitura
        private readonly int ValorDefault = -1;
        private readonly int ValorUnico = 0;
        #endregion

        #region Atributos
        private DataSet _dataSet;

        private int _tabelaAtual;
        private int _linhaAtual;
        #endregion

        #region Propriedades
        public int LinhaCorrente
        {
            get { return this._linhaAtual; }
        }
        public int TabelaCorrente
        {
            get { return this._tabelaAtual; }
        }

        private int QtdTabelas
        {
            get { return this._dataSet.Tables.Count - 1; }
        }
        private int QtdLinhas
        {
            get { return this._dataSet.Tables[_tabelaAtual].Rows.Count - 1; }
        }
        #endregion

        #region Construtores
        public LeitorFacade()
        {
            this.Reset();
            this._dataSet = null;
        }
        public LeitorFacade(StringBuilder query)
        {
            this.Reset();
            this.GetDados(query);
        }
        #endregion

        #region Métodos
        private bool ValidarExecucao()
        {
            return this._dataSet != null;
        }
        private void Reset()
        {
            this._linhaAtual = this.ValorDefault;
            this._tabelaAtual = this.ValorDefault;
        }
        public void GetDados(StringBuilder query)
        {
            Mysql mysql = new Mysql();
            this._dataSet = mysql.RecuperarDataSet(query);
        }
        public void Fechar()
        {
            if (ValidarExecucao())
                this._dataSet.Dispose();
        }
        #endregion

        #region Métodos - lerLinha e auxiliares deste.
        public bool LerLinha()
        {
            if (!ValidarExecucao())
                return false;

            if (!ValidarTabela())
                return false;
            else
                ReposicionarValoresInternos();

            if (!ValidarLinha())
                return false;

            this._linhaAtual++;
            return true;
        }
        private bool ValidarTabela()
        {
            if (this.QtdTabelas == this.ValorUnico)
                return true;
            else if (this._tabelaAtual < this.QtdTabelas)
                return true;

            return false;
        }
        private void ReposicionarValoresInternos()
        {
            if (this._tabelaAtual == this.ValorDefault)
            {
                this._tabelaAtual++;
            }
            else if (this._linhaAtual >= this.QtdLinhas && this._tabelaAtual < this.QtdTabelas)
            {
                this._tabelaAtual++;
                this._linhaAtual = this.ValorDefault;
            }
        }
        private bool ValidarLinha()
        {
            return !(this._linhaAtual >= this.QtdLinhas);
        }
        #endregion

        #region Métodos - Outros
        public DataSet GetDataSet()
        {
            if (!ValidarExecucao())
                return null;

            return this._dataSet;
        }
        #endregion

        #region Métodos - recuperação de valores
        public object GetValor(string coluna)
        {
            if (!ValidarExecucao())
                return null;

            return this._dataSet.Tables[_tabelaAtual].Rows[_linhaAtual][coluna];
        }
        public object GetValor(int coluna)
        {
            if (!ValidarExecucao())
                return null;

            return this._dataSet.Tables[_tabelaAtual].Rows[_linhaAtual][coluna];
        }
        #endregion
    }
}
