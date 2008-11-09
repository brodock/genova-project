using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBusiness.Object;
using CoreBusiness.Interface;

namespace CoreBusiness.Controller
{
    public abstract class ControladorStatusServidor
    {
        public static IServerStatus GetServerStatus()
        {
            ObjStatusServidor statusServidorBase = new ObjStatusServidor();

            IServerStatus ifacade = statusServidorBase as IServerStatus;
            return ifacade;
        }
    }
}
