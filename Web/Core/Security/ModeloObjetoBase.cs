using System;
using System.Collections.Generic;
using System.Text;
using Persistence.Interfaces;
using Persistence.Items;
using Persistence.ControleTransacao;
using Persistence.Enumeradores;

namespace Security
{
    public abstract class ModeloObjetoBase : ICloneable, IItemPersistencia
    {
        #region Atributos
        protected string _tabela;
        protected string _chavePrimaria;        

        private ListaItemsPersistencia _itemsPersistencia;

        private EnumTipoPersistencia _tipoPersistencia;
        private ColecaoPersistencia _colecao;
        #endregion

        #region Propriedades
        public string Tabela
        {
            get { return _tabela; }
        }
        public string ChavePrimaria
        {
            get { return _chavePrimaria; }
        }
        protected ListaItemsPersistencia ItemsPersistencia
        {
            get { return _itemsPersistencia; }
            set { _itemsPersistencia = value; }
        }
        #endregion

        public ModeloObjetoBase(string tabela, string chavePrimaria)
        {
            this._tabela = tabela;
            this._chavePrimaria = chavePrimaria;

            this.ItemsPersistencia = new ListaItemsPersistencia(this._tabela, this._chavePrimaria);

            this._tipoPersistencia = EnumTipoPersistencia.Normal;
            this._colecao = null;
        }

        #region Métodos Abstratos
        protected abstract void Reset();
        protected abstract void PreencherListaItems();
        protected abstract bool Validar();
        #endregion

        #region IItemPersistencia Members
        
        /// <summary>
        /// Por padrão o tipo de persistência é Normal. Executando este método você irá utilizar persistência com coleção.
        /// </summary>
        public void DefinirTipoPersistencia(ColecaoPersistencia colecao)
        {
            this._colecao = colecao;
            this._tipoPersistencia = EnumTipoPersistencia.Colecao;
        }

        public bool Incluir()
        {
            if (this.Validar())
            {
                this.PreencherListaItems();
                if (this._tipoPersistencia == EnumTipoPersistencia.Normal)
                    return this.ItemsPersistencia.Inserir();
                else
                    return this.ItemsPersistencia.Inserir(this._colecao);
            }
            else
                return false;
        }
        public bool Alterar()
        {
            if (this.Validar())
            {
                this.PreencherListaItems();
                if (this._tipoPersistencia == EnumTipoPersistencia.Normal)
                    return this.ItemsPersistencia.Alterar();
                else
                    return this.ItemsPersistencia.Alterar(this._colecao);
            }
            else
                return false;
        }
        public bool Excluir()
        {
            if (this.Validar())
            {
                this.PreencherListaItems();
                if (this._tipoPersistencia == EnumTipoPersistencia.Normal)
                    return this.ItemsPersistencia.Remover();
                else
                    return this.ItemsPersistencia.Remover(this._colecao);
            }
            else
                return false;
        }

        #endregion

        #region ICloneable Members
        public object Clone()
        {
            return this.MemberwiseClone();
        }
        #endregion
    }
}
