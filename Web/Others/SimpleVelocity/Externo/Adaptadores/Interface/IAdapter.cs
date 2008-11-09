using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleVelocity.Adaptadores.Interface
{
    internal interface IAdapter
    {
        object RecuperarItem(object item, int valor);
        object RecuperarItem(object item, string valor);
    }
}
