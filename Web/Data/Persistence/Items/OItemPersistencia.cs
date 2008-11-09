using System;
using System.Collections.Generic;
using System.Text;

namespace Persistence.Items
{
    internal class OItemPersistencia
    {
        #region Atributos
        private string _nome;
        private object _valor;
        private Type _tipo;
        #endregion

        #region Propriedades
        public string Nome
        {
            get { return _nome; }
            set { _nome = value; }
        }

        public object Valor
        {
            get { return _valor; }
            set { _valor = value; }
        }

        public Type Tipo
        {
            get { return _tipo; }
            set { _tipo = value; }
        }       
        #endregion

        public OItemPersistencia(string nome, object valor, Type tipo)
        {
            this._nome = nome;
            this._valor = valor;
            this._tipo = tipo;
        }
    }
}
