using System;
using System.Collections.Generic;
using System.Text;
using Server.Mobiles;
using GeNova.Core.Controladores;
using Server;

namespace GeNova.Server.Engines
{
    public abstract class TraducaoDeNomesMobiles
    {
        public static void AplicarAlteracoesCriatura(BaseCreature criatura, string caminhoObjetoCriatura, string tituloCriatura)
        {
            AplicarAlteracoesCriatura(criatura, caminhoObjetoCriatura, tituloCriatura, null, false);
        }
        public static void AplicarAlteracoesCriatura(BaseCreature criatura, string caminhoObjetoCriatura, string tituloCriatura, Map mapa, bool chanceParagon)
        {
            List<Dictionary<string, object>> registros = ControladorODBC.ODBCRecuperarInformacoesMobile(caminhoObjetoCriatura);
            if (registros.Count <= 0)
            {
                Console.WriteLine("** ODBCSpawner: NPC {0} nao encontrado! Adicionando na base de dados. **", caminhoObjetoCriatura);
                ControladorODBC.ODBCInserirInformacoesMobile(caminhoObjetoCriatura, tituloCriatura);
            }
            else
            {
                foreach (Dictionary<string, object> registro in registros)
                {
                    int idTraducaoMobiles = Convert.ToInt32(registro["IdTraducaoMobiles"].ToString());
                    string classe = registro["Classe"].ToString();
                    string nome = registro["Nome"].ToString();
                    string nomeTraduzido = registro["NomeTraduzido"].ToString();
                    string titulo = registro["Titulo"].ToString();
                    string tituloTraduzido = registro["TituloTraduzido"].ToString();
                    int status = Convert.ToInt32(registro["Status"].ToString());
                    int paragon = Convert.ToInt32(registro["Paragon"].ToString());
                    if (status == 1)
                    {
                        Console.WriteLine("** ODBCSpawner: Aplicando alteracoes para {0}. **", nome);
                        // Alterando nome
                        if (nomeTraduzido.Length > 0)
                            criatura.Name = nomeTraduzido;
                        // Alterando titulo
                        if (tituloTraduzido.Length > 0)
                            criatura.Title = tituloTraduzido;
                        // Calcular chance de ser Paragon
                        if (mapa != null)
                        {
                            if (paragon == 1 && (mapa == Map.Ilshenar && chanceParagon))
                                criatura.IsParagon = true;
                        }
                    }
                    else
                        Console.WriteLine("** ODBCSpawner: NPC {0} nao esta ativo. **", nome);
                }
            }
        }
    }
}
