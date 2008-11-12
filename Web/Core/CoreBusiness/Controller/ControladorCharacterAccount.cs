using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreBusiness.Object;
using Persistence.Utilitarios;

namespace CoreBusiness.Controller
{
    public abstract class ControladorCharacterAccount
    {
        private static ObjCharacterAccount _characterAccountBase = new ObjCharacterAccount();

        public static LeitorFacade GetCharactersAccount(int idConta)
        {
            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT T1.NomePersonagem, T1.DataCadastro, T1.Confirmado, COALESCE(T2.PontosXP,0) AS PontosXP
            FROM {0} T1
            LEFT JOIN GeNova_PontosDeExperiencia T2 ON T1.IdPersonagem = T2.IdPersonagem
            WHERE 1=1
              AND T1.{1} = {2}
            ORDER BY T1.DataCadastro DESC
            ", _characterAccountBase.Tabela, _characterAccountBase.Conta.ChavePrimaria, idConta);
            return new LeitorFacade(query);
        }
    }
}
