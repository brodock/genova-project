using System;
using System.Collections.Generic;
using System.Text;
using SimpleVelocity.Adaptadores.Estrutura;
using System.Collections;

namespace SimpleVelocity.Adaptadores
{
    /// <summary>
    /// Adaptador responsável por recuperar itens de um DataRow.
    /// </summary>
    public class ListAdapter : EstruturaAdapter
    {
        public override object RecuperarItem(object item, int valor, object valorDefault)
        {
            ArrayList list = ArrayList.Adapter((IList)item);
            return base.ItemRetorno(list[valor], valorDefault);
        }

        public override object RecuperarItem(object item, string valor, object valorDefaut)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
