using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;

namespace Utils.Ambiente
{
    public class CulturaSingleton
    {
        #region Atributos
        private CultureInfo _cultura;

        private volatile static CulturaSingleton instance;
        private static object syncRoot = new Object();
        #endregion

        #region Propriedades
        internal CultureInfo Cultura
        {
            get { return this._cultura; }
        }
        #endregion

        private CulturaSingleton()
        {
            this._cultura = new CultureInfo("pt-BR", true);

            this._cultura.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            this._cultura.DateTimeFormat.ShortTimePattern = "HH:mm:ss";
            this._cultura.DateTimeFormat.FullDateTimePattern = "dd/MM/yyyy HH:mm:ss";
        }

        public static CulturaSingleton Istance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new CulturaSingleton();
                    }
                }
                return instance;
            }
        }
    }
}