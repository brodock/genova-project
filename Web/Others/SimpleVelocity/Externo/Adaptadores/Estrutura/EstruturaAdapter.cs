using System;
using System.Collections.Generic;
using System.Text;
using SimpleVelocity.Adaptadores.Interface;

namespace SimpleVelocity.Adaptadores.Estrutura
{
    public abstract class EstruturaAdapter : IAdapter
    {
        public object RecuperarItem(object item, int valor)
        {
            return RecuperarItem(item, valor, null);
        }

        public object RecuperarItem(object item, string valor)
        {
            return RecuperarItem(item, valor, null);
        }

        protected object ItemRetorno(object item, object valorDefault)
        {
            if (valorDefault == null)
                return item;

            return item.ToString() == string.Empty ? valorDefault : item;
        }

        public abstract object RecuperarItem(object item, int valor, object valorDefault);
        public abstract object RecuperarItem(object item, string valor, object valorDefaut);
    }
}
