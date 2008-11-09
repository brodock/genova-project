using System;
using System.Text;
using System.Data;
using SimpleVelocity.Adaptadores.Estrutura;

namespace SimpleVelocity.Adaptadores
{
    /// <summary>
    /// Adaptador responsável por recuperar itens de um DataRow.
    /// </summary>
    public class DataRowAdapter : EstruturaAdapter
    {
        public override object RecuperarItem(object item, int valor, object valorDefault)
        {
            DataRow linhaItem = (DataRow)item;
            return base.ItemRetorno(linhaItem[valor], valorDefault);
        }
        public override object RecuperarItem(object item, string valor, object valorDefault)
        {
            DataRow linhaItem = (DataRow)item;
            return base.ItemRetorno(linhaItem[valor], valorDefault);
        }
    }
}
