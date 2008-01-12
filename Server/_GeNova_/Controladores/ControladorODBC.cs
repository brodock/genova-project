/***************************************************************************
 *   copyright            : (C) GeNova Project
 *   webSite              : http://code.google.com/p/genovaproject/
 *
 ***************************************************************************/

/***************************************************************************
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 ***************************************************************************/

using System;
using System.Text;
using System.Collections.Generic;
using Server;
using Server.Network;

namespace GeNova.Core.Controladores
{
    public abstract class ControladorODBC
    {
        #region ------------------------------------- CORE -------------------------------------

        #region NetState
        public static void ODBCDispose(Mobile from)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT COALESCE(U.IdUsuario,0) IdUsuario, COALESCE(LC.IdLogConexoes,0) IdLogConexoes ");
            querySql.Append("FROM GeNova_Usuario U ");
            querySql.Append("INNER JOIN GeNova_LogConexoes LC ON U.IdUsuario = LC.IdUsuario ");
            querySql.AppendFormat("WHERE U.Login = '{0}';", from.Account.Username);
            List<Dictionary<string, object>> registros = ControladorConexaoMysql.RecuperarRegistrosDataSet(controlador.BuscarDadosNaBase(querySql));
            foreach (Dictionary<string, object> linha in registros)
            {
                int idUsuario = Convert.ToInt32(linha["IdUsuario"].ToString());
                int idLogConexoes = Convert.ToInt32(linha["IdLogConexoes"].ToString());

                if (idUsuario > 0 && idLogConexoes > 0)
                {
                    DateTime logOut = DateTime.Now;
                    querySql = new StringBuilder();
                    querySql.AppendFormat("UPDATE GeNova_LogConexoes SET Saida = '{0}' WHERE IdUsuario = {1};", ControladorMYSQL.RecuperarDateTimeFormatado(logOut), idUsuario);
                    bool encontrou = controlador.AtualizarDadosNaBase(querySql);
                    if (encontrou)
                        Console.WriteLine("** Log de Conexao para {0}. **", from.Account.Username);

                    encontrou = false;
                    querySql = new StringBuilder();
                    querySql.Append("INSERT INTO Registro_LogConexoes (IdUsuario, Entrada, Saida, IP) ");
                    querySql.Append("SELECT IdUsuario, Entrada, Saida, IP ");
                    querySql.Append("FROM GeNova_LogConexoes LC ");
                    querySql.Append("WHERE 1=1 ");
                    querySql.AppendFormat("AND LC.IdUsuario = {0} ", idUsuario);
                    querySql.AppendFormat("AND LC.Saida = '{0}';", ControladorMYSQL.RecuperarDateTimeFormatado(logOut));
                    encontrou = controlador.InserirDadosNaBase(querySql);
                    if (encontrou)
                        Console.WriteLine("** Armazenando Registro de Conexao para {0}. **", from.Account.Username);
                }
            }
        }
        #endregion

        #region World
        public static void ODBCRemoverPersonagensNaoConfirmados()
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("DELETE FROM GeNova_Usuario_Personagens WHERE Confirmado IS NULL OR Confirmado IS FALSE;");
            bool executou = controlador.RemoverDadosNaBase(querySql);
            if (executou)
                Console.WriteLine("** Personagens nao confirmados deletados com sucesso. **");
        }
        public static void ODBCConfirmarPersonagensNaoConfirmados()
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("UPDATE GeNova_Usuario_Personagens SET Confirmado = TRUE WHERE Confirmado IS FALSE;");
            bool executou = controlador.RemoverDadosNaBase(querySql);
            if (executou)
                Console.WriteLine("** Personagens confirmados com sucesso. **");
        }
        #endregion

        #endregion

        #region ------------------------------------- SOURCE -------------------------------------

        #region ----- Classes Nativa -----
        #region AccountHandler
        public static void ODBCReceive(NetState estado, string usuario)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT COUNT(*) AS Quantidade FROM GeNova_LogConexoes;");
            bool temRegistro = Convert.ToInt32(controlador.BuscarDadosNaBase(querySql).Tables[0].Rows[0]["Quantidade"].ToString()) > 0;
            querySql = new StringBuilder();
            querySql.Append("SELECT COALESCE(U.IdUsuario,0) IdUsuario, COALESCE(LC.IdLogConexoes,0) IdLogConexoes ");
            querySql.Append("FROM GeNova_Usuario U ");
            querySql.Append("LEFT JOIN GeNova_LogConexoes LC ON U.IdUsuario = LC.IdUsuario ");
            querySql.AppendFormat("WHERE U.Login = '{0}';", usuario);
            List<Dictionary<string, object>> registros = ControladorConexaoMysql.RecuperarRegistrosDataSet(controlador.BuscarDadosNaBase(querySql));
            foreach (Dictionary<string, object> linha in registros)
            {
                int idUsuario = Convert.ToInt32(linha["IdUsuario"].ToString());
                int idLogConexoes = Convert.ToInt32(linha["IdLogConexoes"].ToString());
                if (temRegistro && (idUsuario > 0 && idLogConexoes > 0))
                {
                    querySql = new StringBuilder();
                    querySql.AppendFormat("UPDATE GeNova_LogConexoes SET Entrada = NOW(), Saida = NULL, IP = '{1}' WHERE IdUsuario = {0};", idUsuario, estado.Address.ToString());
                    bool encontrou = controlador.AtualizarDadosNaBase(querySql);
                    if (encontrou)
                        Console.WriteLine("** Atualizando Entrada, Saida e IP de {0}. **", usuario);
                }
                else
                {
                    querySql = new StringBuilder();
                    querySql.AppendFormat("INSERT INTO GeNova_LogConexoes (IdUsuario, Entrada, IP) VALUES ({0}, NOW(), '{1}');", idUsuario, estado.Address.ToString());
                    bool encontrou = controlador.InserirDadosNaBase(querySql);
                    if (encontrou)
                        Console.WriteLine("** Gravando log de Conexao para {0}. **", usuario);
                }
            }
        }
        #endregion

        #region PlayerMobile
        public static void ODBCDeleteName(string nomeUsuario, string nomePersonagem)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("DELETE FROM GeNova_Usuario_Personagens ");
            querySql.Append("WHERE 1=1 ");
            querySql.AppendFormat("AND IdUsuario in (SELECT IdUsuario FROM GeNova_Usuario WHERE Login = '{0}') ", nomeUsuario);
            querySql.AppendFormat("AND NomePersonagem = '{0}';", nomePersonagem);
            bool encontrou = controlador.RemoverDadosNaBase(querySql);
            if (encontrou)
                Console.WriteLine("** Deletado personagem {0}, conta {1}. **", nomePersonagem, nomeUsuario);
        }
        #endregion
        #endregion

        #region ----- Classes GeNova -----

        #region BaseCreatureGeNova
        public static List<Dictionary<string, object>> ODBCGiveLoot(string possivelNomeUm, string possivelNomeDois)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT * FROM GeNova_LoteGenerico LTC ");
            querySql.Append("WHERE 1=1 ");
            querySql.AppendFormat("AND (LTC.Classe = '{0}' OR LTC.Classe = '{1}') ", possivelNomeUm, possivelNomeDois);
            querySql.Append("AND Status = 'A';");
            List<Dictionary<string, object>> lista = ControladorConexaoMysql.RecuperarRegistrosDataSet(controlador.BuscarDadosNaBase(querySql));
            return lista;
        }
        #endregion

        #region PontosDeExperiencia
        public static void ODBCConcederPontosXP(string nomeUsuario, string nomePersonagem, int pontosExperiencia)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT COUNT(PE.IdPontosDeExperiencia) AS Quantidade, UP.IdPersonagem ");
            querySql.Append("FROM GeNova_Usuario U ");
            querySql.Append("INNER JOIN GeNova_Usuario_Personagens UP ON U.IdUsuario = UP.IdUsuario ");
            querySql.Append("LEFT JOIN GeNova_PontosDeExperiencia PE ON UP.IdPersonagem = PE.IdPersonagem ");
            querySql.Append("WHERE 1=1 ");
            querySql.AppendFormat("AND U.Login = '{0}' ", nomeUsuario);
            querySql.AppendFormat("AND UP.NomePersonagem = '{0}' ", nomePersonagem);
            querySql.Append("GROUP BY PE.IdPersonagem;");

            bool temRegistro = Convert.ToInt32(controlador.BuscarDadosNaBase(querySql).Tables[0].Rows[0]["Quantidade"].ToString()) > 0;
            int idPersonagem = Convert.ToInt32(controlador.BuscarDadosNaBase(querySql).Tables[0].Rows[0]["IdPersonagem"].ToString());

            if (idPersonagem > 0)
            {
                if (temRegistro)
                {
                    querySql = new StringBuilder();
                    querySql.Append("SELECT PontosXP ");
                    querySql.Append("FROM GeNova_PontosDeExperiencia ");
                    querySql.Append("WHERE 1=1 ");
                    querySql.AppendFormat("AND IdPersonagem = {0};", idPersonagem);
                    int totalPontosXP = Convert.ToInt32(controlador.BuscarDadosNaBase(querySql).Tables[0].Rows[0]["PontosXP"].ToString());

                    querySql = new StringBuilder();
                    querySql.Append("UPDATE GeNova_PontosDeExperiencia ");
                    querySql.AppendFormat("SET PontosXP = {0} ", (totalPontosXP + pontosExperiencia));
                    querySql.Append("WHERE 1=1 ");
                    querySql.AppendFormat("AND IdPersonagem = {0};", idPersonagem);
                    bool encontrou = controlador.AtualizarDadosNaBase(querySql);
                    if (encontrou)
                        Console.WriteLine("** Atualizando pontos experiencia personagem {0}, conta {1}. **", nomePersonagem, nomeUsuario);
                }
                else
                {
                    querySql = new StringBuilder();
                    querySql.AppendFormat("INSERT INTO GeNova_PontosDeExperiencia (IdPersonagem, PontosXP) VALUES({0},{1});", idPersonagem, pontosExperiencia);
                    bool encontrou = controlador.InserirDadosNaBase(querySql);
                    if (encontrou)
                        Console.WriteLine("** Inserindo pontos experiencia personagem {0}, conta {1}. **", nomePersonagem, nomeUsuario);
                }
            }
            else
                Console.WriteLine("** ERRO. Personagem nao persistido: {0}, conta {1}. **", nomePersonagem, nomeUsuario);
        }
        #endregion

        #region MensagensGlobal
        public static void ODBCProcessarMensagensGlobal()
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT CorMensagem, Mensagem ");
            querySql.Append("FROM GeNova_MensagensGlobal ");
            querySql.Append("WHERE Status = 'A';");
            List<Dictionary<string, object>> registros = ControladorConexaoMysql.RecuperarRegistrosDataSet(controlador.BuscarDadosNaBase(querySql));
            foreach (Dictionary<string, object> registro in registros)
            {
                int corMensagem = Convert.ToInt32(registro["CorMensagem"].ToString());
                string mensagem = registro["Mensagem"].ToString();
                World.Broadcast(corMensagem, false, mensagem);
            }
            Console.WriteLine("** Mensagens Global enviada com sucesso : {0}. **", DateTime.Now);
        }
        #endregion

        #region StatusServidor
        public static void ODBCProcessarStatusServidor(string tempoOnline)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("UPDATE GeNova_StatusServidor ");
            querySql.AppendFormat("SET UsuariosOnline = {0}, ", NetState.Instances.Count);
            querySql.AppendFormat("TotalMobiles = {0}, ", World.Mobiles.Count);
            querySql.AppendFormat("TotalItens = {0}, ", World.Items.Count);
            querySql.AppendFormat("TempoOnline = '{0}', ", tempoOnline);
            querySql.Append("DataHoraIniciado = NOW();");
            bool executou = controlador.AtualizarDadosNaBase(querySql);
            if (executou)
                Console.WriteLine("** Atualizando Status do Servidor. **");
        }
        #endregion

        #region CriacaoDeContas
        public static List<Dictionary<string, object>> ODBCRecuperarListaUsuariosNaoAtivos()
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT IdUsuario, Login, Senha, Ativo FROM GeNova_Usuario WHERE Ativo = false OR Ativo IS NULL;");
            return ControladorConexaoMysql.RecuperarRegistrosDataSet(controlador.BuscarDadosNaBase(querySql));
        }
        public static void ODBCAtivarContaUsuario(int idUsuario, string login)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.AppendFormat("UPDATE Genova_Usuario SET Ativo = true WHERE IdUsuario = {0};", idUsuario);
            bool executou = controlador.AtualizarDadosNaBase(querySql);
            if (executou)
                Console.WriteLine("** Conta {0} ativado(a) com sucesso **", login);
        }
        #endregion

        #region RegistroDePersonagem
        public static void ODBCRegistrarPersonagem(Mobile personagem, string nomePersonagem)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT COUNT(IdPersonagem) as Quantidade ");
            querySql.Append("FROM GeNova_Usuario_Personagens ");
            querySql.Append("WHERE 1=1 ");
            querySql.AppendFormat("AND NomePersonagem = '{0}' ", nomePersonagem);
            bool registros = Convert.ToInt32(controlador.BuscarDadosNaBase(querySql).Tables[0].Rows[0]["Quantidade"].ToString()) > 0;
            if (registros)
                personagem.SendMessage(60, String.Format("O personagem '{0}' já está cadastrado no sistema. Tente outro nome.", nomePersonagem));
            else
            {
                querySql = new StringBuilder();
                querySql.AppendFormat("SELECT IdUsuario FROM GeNova_Usuario WHERE Login = '{0}';", personagem.Account.Username);
                int idUsuario = Convert.ToInt32(controlador.BuscarDadosNaBase(querySql).Tables[0].Rows[0]["IdUsuario"].ToString());
                if (idUsuario > 0)
                {
                    querySql = new StringBuilder();
                    querySql.Append("INSERT INTO GeNova_Usuario_Personagens (IdUsuario, NomePersonagem, DataCadastro, Confirmado) ");
                    querySql.AppendFormat("VALUES ({0},'{1}',Now(), FALSE);", idUsuario, nomePersonagem);
                    bool executou = controlador.InserirDadosNaBase(querySql);
                    if (executou)
                    {
                        Console.WriteLine("** Personagem {0} cadastrado, conta {1}. **", nomePersonagem, personagem.Account.Username);
                        personagem.Name = nomePersonagem;
                        personagem.Frozen = false;
                        personagem.Hidden = false;
                        personagem.PlaySound(0x1E3); // som de NightSight
                        personagem.FixedEffect(0x376A, 10, 16, 1259, 0); // efeito de Resurrection com cor de fogo.
                        personagem.SendMessage(60, "Seu nome foi registrado com sucesso. Desfrute-o com honra e coragem.");
                    }
                }
            }
        }
        #endregion

        #region ODBCSpawner
        public static List<Dictionary<string, object>> ODBCRecuperarInformacoesMobile(string caminhoObjetoCriatura)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("SELECT  IdTraducaoMobiles, Classe, Nome, NomeTraduzido, ");
            querySql.Append("Titulo, TituloTraduzido, Status, 0 as Paragon ");
            querySql.Append("FROM GeNova_TraducaoMobiles ");
            querySql.AppendFormat("WHERE Classe = '{0}';", caminhoObjetoCriatura);
            return ControladorConexaoMysql.RecuperarRegistrosDataSet(controlador.BuscarDadosNaBase(querySql));
        }
        public static void ODBCInserirInformacoesMobile(string caminhoObjetoCriatura, string tituloCriatura)
        {
            ControladorConexaoMysql controlador = new ControladorConexaoMysql();
            StringBuilder querySql = new StringBuilder();
            querySql.Append("INSERT INTO GeNova_TraducaoMobiles (Classe, Nome, Titulo, Status) ");
            querySql.AppendFormat("VALUES ('{0}', SUBSTRING_INDEX('{0}','.',-1), '{1}', 0);", caminhoObjetoCriatura, tituloCriatura);
            bool executou = controlador.InserirDadosNaBase(querySql);
            if (executou)
                Console.WriteLine("** NPC {0} adicionado na base de dados. **", caminhoObjetoCriatura);
        }
        #endregion

        #endregion

        #endregion
    }
}
