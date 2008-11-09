using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils;
using Persistence.Utilitarios;

namespace CoreBusiness.Controller
{
    public class ControladorCreatures
    {
        public static LeitorFacade GetActiveCreatures()
        {
            StringBuilder query = new StringBuilder();
            query.Append(@"
            SELECT
            COALESCE(NomeTraduzido,Nome) AS Name,
            CASE
              WHEN COALESCE(TituloTraduzido,Titulo) = 'null'
              THEN '-'
              ELSE COALESCE(TituloTraduzido,Titulo)
            END AS Title
            FROM genova_traducaomobiles
            ORDER BY Name ASC
            ");
            return new LeitorFacade(query);
        }

        public static LeitorFacade GetCreatureLoots(string nomeCriatura, bool apenasAtivos)
        {
            string creatureName = nomeCriatura;
            SqlUtils.PrevencaoSqlInjection(ref nomeCriatura);

            if (nomeCriatura.Contains("%") || !creatureName.Equals(nomeCriatura))
                throw new Exception(Erros.NaoEncontrado("Creature"));

            StringBuilder query = new StringBuilder();
            query.AppendFormat(@"
            SELECT DISTINCT
            T1.Classe,
            T1.Item,
            T1.Quantidade,
            T1.Status
            FROM GeNova_LoteGenerico T1
            INNER JOIN GeNova_TraducaoMobiles T2 ON T1.Classe = T2.Classe
            WHERE 1=1
              AND (
                   T2.Nome LIKE '%{0}%'
                   OR T2.NomeTraduzido LIKE '%{0}%'
                  )
              {1}
            ",
             nomeCriatura,
             apenasAtivos ? "AND T1.Status = 'A'" : string.Empty
             );

            return new LeitorFacade(query);
        }
    }
}
