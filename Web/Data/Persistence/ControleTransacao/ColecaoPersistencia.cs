using System;
using System.Collections.Generic;
using System.Text;
using Persistence.Interfaces;
using Persistence.Enumeradores;

namespace Persistence.ControleTransacao
{
    public class ColecaoPersistencia
    {
        #region Atributos
        private OTransacao _transacao;
        private List<OItemTransacao> _listaItens;
        #endregion

        #region Propriedades
        internal OTransacao TransacaoAtiva
        {
            get { return _transacao; }
            set { _transacao = value; }
        }
        #endregion

        public ColecaoPersistencia()
        {
            this._transacao = new OTransacao();
            this._listaItens = new List<OItemTransacao>();
        }

        #region Métodos
        public void AdicionarItem(object item, EnumTipoTransacao tipoTransacao)
        {
            IItemPersistencia itemPersistencia = item as IItemPersistencia;

            OItemTransacao itemTransacao = new OItemTransacao(itemPersistencia, tipoTransacao);
            this._listaItens.Add(itemTransacao);
        }

        public void Limpar()
        {
            this._listaItens.Clear();
        }

        public void Persistir()
        {
            this._transacao.Iniciar();

            try
            {
                foreach (OItemTransacao item in this._listaItens)
                {
                    item.Objeto.DefinirTipoPersistencia(this);

                    switch (item.Tipo)
                    {
                        case EnumTipoTransacao.Incluir:
                            {
                                item.Objeto.Incluir();
                                break;
                            }
                        case EnumTipoTransacao.Alterar:
                            {
                                item.Objeto.Alterar();
                                break;
                            }
                        case EnumTipoTransacao.Remover:
                            {
                                item.Objeto.Excluir();
                                break;
                            }
                    }
                }

                this._transacao.Commit();
            }
            catch (Exception erro)
            {
                this._transacao.Rollback();
                throw new Exception(erro.Message);
            }
        }
        #endregion
    }
}
