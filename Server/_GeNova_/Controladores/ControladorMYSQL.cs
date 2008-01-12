using System;
using System.Collections.Generic;
using System.Text;

namespace GeNova.Core.Controladores
{
    public abstract class ControladorMYSQL
    {
        public static string RecuperarDateTimeFormatado(DateTime dataHora)
        {
            return dataHora.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
