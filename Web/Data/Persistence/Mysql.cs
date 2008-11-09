using System;
using System.Collections.Generic;
using System.Text;
using Persistence.Configuracoes;
using MySql.Data.MySqlClient;
using System.Data;
using Persistence.ControleTransacao;

namespace Persistence
{
    public class Mysql
    {
        internal static string GetConfiguracoes()
        {
            ConfiguracoesSingleton configuracoes = ConfiguracoesSingleton.Istance;
            return configuracoes.GetConfiguracoes().ToString();
        }

        // Sem Controle de Transação
        internal bool Inserir(StringBuilder querySql)
        {
            return ManipulandoDadosNaBase(querySql);
        }
        internal bool Alterar(StringBuilder querySql)
        {
            return ManipulandoDadosNaBase(querySql);
        }
        internal bool Remover(StringBuilder querySql)
        {
            return ManipulandoDadosNaBase(querySql);
        }

        // Com Controle de Transação
        internal bool Inserir(StringBuilder querySql, ColecaoPersistencia colecao)
        {
            return ManipulandoDadosNaBase(querySql, colecao);
        }
        internal bool Alterar(StringBuilder querySql, ColecaoPersistencia colecao)
        {
            return ManipulandoDadosNaBase(querySql, colecao);
        }
        internal bool Remover(StringBuilder querySql, ColecaoPersistencia colecao)
        {
            return ManipulandoDadosNaBase(querySql, colecao);
        }

        private bool ManipulandoDadosNaBase(StringBuilder querySql)
        {
            // retorna true em caso de sucesso.
            bool executou = false;

            MySqlTransaction transacaoSql = null;
            MySqlConnection conexaoSql = null;
            MySqlCommand comandoSql = null;

            try
            {
                // Abrindo conexão
                conexaoSql = new MySqlConnection(GetConfiguracoes());
                conexaoSql.Open();

                // recuperando instancia de MySqlCommand
                comandoSql = conexaoSql.CreateCommand();

                // Iniciando transacao                
                transacaoSql = conexaoSql.BeginTransaction();

                // Definindo conexao e transacao para MySqlCommand
                comandoSql.Connection = conexaoSql;
                comandoSql.Transaction = transacaoSql;

                // Definindo query para comandoSql
                comandoSql.CommandText = querySql.ToString();

                // executando query
                executou = comandoSql.ExecuteNonQuery() > 0;

                // integridade referencial, confirmando execucao.
                transacaoSql.Commit();
            }
            catch (Exception ex)
            {
                try
                {
                    // integridade referencial, desfazendo execucao.
                    transacaoSql.Rollback();
                }
                catch (MySqlException e)
                {
                    if (transacaoSql.Connection != null)
                        throw new Exception("Uma exceção do tipo " + e.GetType() + " foi encontrado ao tentar desfazer a transação.");
                }
                throw new Exception("Uma exceção do tipo " + ex.GetType() + " foi encontrado enquanto a query estava sendo processada.");
            }
            finally
            {
                if (conexaoSql != null)
                    conexaoSql.Close();
                if (comandoSql != null)
                    comandoSql.Dispose();
                if (transacaoSql != null)
                    transacaoSql.Dispose();
            }

            return executou;
        }
        private bool ManipulandoDadosNaBase(StringBuilder querySql, ColecaoPersistencia colecao)
        {
            // retorna true em caso de sucesso.
            bool executou = false;

            MySqlCommand comandoSql = null;

            try
            {
                // recuperando instancia de MySqlCommand
                comandoSql = colecao.TransacaoAtiva.Conexao.CreateCommand();

                // Definindo conexao e transacao para MySqlCommand
                comandoSql.Connection = colecao.TransacaoAtiva.Conexao;
                comandoSql.Transaction = colecao.TransacaoAtiva.Transacao;

                // Definindo query para comandoSql
                comandoSql.CommandText = querySql.ToString();

                // executando query
                executou = comandoSql.ExecuteNonQuery() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception("Uma exceção do tipo " + ex.GetType() + " foi encontrado enquanto a query estava sendo processada.");
            }
            finally
            {
                if (comandoSql != null)
                    comandoSql.Dispose();
            }

            return executou;
        }

        internal DataSet RecuperarDataSet(StringBuilder querySql)
        {
            DataSet dataSet = new DataSet();
            MySqlConnection conexaoSql = null;
            MySqlDataAdapter adaptadorSql = null;

            try
            {
                // Abrindo conexão
                conexaoSql = new MySqlConnection(GetConfiguracoes());
                conexaoSql.Open();

                // instanciando DataAdapter
                adaptadorSql = new MySqlDataAdapter(querySql.ToString(), conexaoSql);

                // executando query e preenchendo dataSet.
                adaptadorSql.Fill(dataSet);
            }
            catch (Exception ex)
            {
                throw new Exception("Uma exceção do tipo " + ex.GetType() + " foi encontrado enquanto a query estava sendo processada.");
            }
            finally
            {
                if (conexaoSql != null)
                    conexaoSql.Close();
                if (adaptadorSql != null)
                    adaptadorSql.Dispose();
            }

            return dataSet;
        }
    }
}
