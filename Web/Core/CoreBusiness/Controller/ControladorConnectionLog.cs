using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBusiness.Object;
using Persistence.Utilitarios;
using Utils;

namespace CoreBusiness.Controller
{
    public abstract class ControladorConnectionLog
    {
        private static ObjConnectionLog _connectionLogBase = new ObjConnectionLog();

        public static LeitorFacade GetLogs(int idConta)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT {1}, Entrada, Saida, IP
            FROM {0}
            WHERE {1} = {2}
            ORDER BY 1 DESC
            ", _connectionLogBase.Tabela, _connectionLogBase.Conta.ChavePrimaria, idConta);
            return new LeitorFacade(query);
        }
    }
}
