using System;
using System.Text;
using System.Data;
using SimpleVelocity.Adaptadores.Estrutura;

namespace SimpleVelocity.Adaptadores
{
    /// <summary>
    /// Adaptador responsável por recuperar itens de um DataTableCollection. Retorno: DataTable.
    /// </summary>
    public class DataTableCollectionAdapter : EstruturaAdapter
    {        
        public override object RecuperarItem(object item, int valor, object valorDefault)
        {
            DataTableCollection linhaItem = (DataTableCollection)item;
            return base.ItemRetorno(linhaItem[valor], valorDefault);
        }

        public override object RecuperarItem(object item, string valor, object valorDefaut)
        {
            throw new InvalidOperationException();
        }
    }
}
