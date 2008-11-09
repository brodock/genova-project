using System;
using System.Collections.Generic;
using System.Text;
using Persistence.Interfaces;
using Persistence.Enumeradores;

namespace Persistence.ControleTransacao
{
    internal class OItemTransacao
    {
        #region Atributos
        private IItemPersistencia _objeto;
        private EnumTipoTransacao _tipoTransacao;
        #endregion

        #region Propriedades
        public IItemPersistencia Objeto
        {
            get { return _objeto; }
        }

        public EnumTipoTransacao Tipo
        {
            get { return _tipoTransacao; }
        }
        #endregion       

        public OItemTransacao(IItemPersistencia objeto, EnumTipoTransacao tipoTransacao)
        {
            this._objeto = objeto;
            this._tipoTransacao = tipoTransacao;
        }
    }
}
