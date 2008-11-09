using System;
using System.Collections.Generic;
using System.Text;
using Utils;
using Persistence.Utilitarios;
using Persistence.ControleTransacao;

namespace Persistence.Items
{
    public class ListaItemsPersistencia
    {
        #region Atributos
        private string _tabela;
        private string _chavePrimaria;

        private Mysql _mysql;
        private List<OItemPersistencia> _listaItems;
        #endregion

        #region Propriedades
        public int Count
        {
            get { return this._listaItems.Count; }
        }
        #endregion

        public ListaItemsPersistencia(string nomeTabela, string chavePrimaria)
        {
            this._tabela = nomeTabela;
            this._chavePrimaria = chavePrimaria;

            this._mysql = new Mysql();
            this._listaItems = new List<OItemPersistencia>();
        }

        #region Métodos
        public void AdicionarItem(string nome, object valor, Type tipo)
        {
            AdicionarItem(nome, valor, tipo, false);
        }
        public void AdicionarItem(string nome, object valor, Type tipo, bool valorNuloQuando)
        {
            if (valorNuloQuando)
                this._listaItems.Add(new OItemPersistencia(nome, null, tipo));
            else
                this._listaItems.Add(new OItemPersistencia(nome, valor, tipo));
        }
        #endregion

        #region Métodos - SQL
        // Sem controle de Transação
        public bool Inserir() { return this.Inserir(null); }
        public bool Alterar() { return this.Alterar(null); }
        public bool Remover() { return this.Remover(null); }

        // Com controle de Transação
        public bool Inserir(ColecaoPersistencia colecao)
        {
            int cColunas = 0;
            int cValores = 0;

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO {0}\n", this._tabela);
            sql.AppendLine("(");
            foreach (OItemPersistencia item in this._listaItems)
            {
                cColunas++;
                sql.AppendFormat("{0}{1}\n", item.Nome, Outros.InserirVirgula(cColunas, this._listaItems.Count));
            }
            sql.AppendLine(")");
            sql.AppendLine("VALUES");
            sql.AppendLine("(");
            foreach (OItemPersistencia item in this._listaItems)
            {
                cValores++;
                if (item.Nome.Equals(this._chavePrimaria) || item.Valor == null)
                    sql.AppendFormat("NULL{0}\n", Outros.InserirVirgula(cValores, this._listaItems.Count));
                else if (Outros.Comparar(item.Tipo, typeof(string)) || Outros.Comparar(item.Tipo, typeof(char)))
                    sql.AppendFormat("'{0}'{1}\n", SqlUtils.PrevencaoInternaSqlInjection(item.Valor), Outros.InserirVirgula(cValores, this._listaItems.Count));
                else if (Outros.Comparar(item.Tipo, typeof(DateTime)))
                {
                    DateTime dataHora = Conversoes.DataHora(item.Valor);
                    sql.AppendFormat("'{0}'{1}\n", dataHora.ToString("yyyy-MM-dd HH:mm:ss"), Outros.InserirVirgula(cValores, this._listaItems.Count));
                }
                else
                    sql.AppendFormat("{0}{1}\n", SqlUtils.PrevencaoInternaSqlInjection(item.Valor), Outros.InserirVirgula(cValores, this._listaItems.Count));
            }
            sql.AppendLine(")");

            if (colecao == null)
                return this._mysql.Inserir(sql);
            else
                return this._mysql.Inserir(sql, colecao);
        }

        public bool Alterar(ColecaoPersistencia colecao)
        {
            int cItems = 0;

            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("UPDATE {0}\n", this._tabela);
            sql.AppendLine("SET");
            foreach (OItemPersistencia item in this._listaItems)
            {
                cItems++;
                if (item.Nome.Equals(this._chavePrimaria))
                    continue;

                if (item.Valor == null)
                    sql.AppendFormat("{0} = NULL{1}\n", item.Nome, Outros.InserirVirgula(cItems, this._listaItems.Count));
                else if (Outros.Comparar(item.Tipo, typeof(string)))
                    sql.AppendFormat("{0} = '{1}'{2}\n", item.Nome, SqlUtils.PrevencaoInternaSqlInjection(item.Valor), Outros.InserirVirgula(cItems, this._listaItems.Count));
                else if (Outros.Comparar(item.Tipo, typeof(DateTime)))
                {
                    DateTime dataHora = Conversoes.DataHora(item.Valor);
                    sql.AppendFormat("{0} = '{1}'{2}\n", item.Nome, dataHora.ToString("yyyy-MM-dd HH:mm:ss"), Outros.InserirVirgula(cItems, this._listaItems.Count));
                }
                else
                    sql.AppendFormat("{0} = {1}{2}\n", item.Nome, SqlUtils.PrevencaoInternaSqlInjection(item.Valor), Outros.InserirVirgula(cItems, this._listaItems.Count));
            }
            this.FecharPorIdTabela(sql);

            if (colecao == null)
                return this._mysql.Alterar(sql);
            else
                return this._mysql.Alterar(sql, colecao);
        }

        public bool Remover(ColecaoPersistencia colecao)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("DELETE FROM {0}\n", this._tabela);
            this.FecharPorIdTabela(sql);

            if (colecao == null)
                return this._mysql.Remover(sql);
            else
                return this._mysql.Remover(sql, colecao);
        }

        #region Métodos - SQL (Auxiliares)
        private void FecharPorIdTabela(StringBuilder sql)
        {
            foreach (OItemPersistencia item in this._listaItems)
            {
                if (item.Nome.Equals(this._chavePrimaria))
                {
                    sql.AppendFormat("WHERE {0} = {1}", this._chavePrimaria, item.Valor);
                    break;
                }
            }
        }
        #endregion

        #endregion
    }
}
