using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using Utils.Ambiente;

namespace Utils
{
    public abstract class Sistema
    {
        public static CultureInfo Cultura
        {
            get { return CulturaSingleton.Istance.Cultura; }
        }
    }
}
