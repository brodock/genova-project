using System;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Collections.Generic;

namespace GeNova.Core.Controladores
{
    public class ControladorConexaoMysql
    {
        public ControladorConexaoMysql() { }

        #region Atributos de Configuração
        private string _servidor = "127.0.0.1";
        private string _bancoDeDados = "uo";
        private string _usuario = "root";
        private string _senha = "";
        #endregion

        public string RecuperarStringDeConexao()
        {
            StringBuilder retorno = new StringBuilder();
            retorno.AppendFormat("Server={0};", this._servidor);
            retorno.AppendFormat("Database={0};", this._bancoDeDados);
            retorno.AppendFormat("User ID={0};", this._usuario);
            retorno.AppendFormat("Password={0};", this._senha);
            return retorno.ToString();
        }

        public bool AtualizarDadosNaBase(StringBuilder querySql)
        {
            return ManipulandoDadosNaBase(querySql);
        }
        public bool InserirDadosNaBase(StringBuilder querySql)
        {
            return ManipulandoDadosNaBase(querySql);
        }
        public bool RemoverDadosNaBase(StringBuilder querySql)
        {
            return ManipulandoDadosNaBase(querySql);
        }
        private bool ManipulandoDadosNaBase(StringBuilder querySql)
        {
            bool executou = false;
            MySqlConnection conexaoSql = null;
            MySqlCommand comandoSql = null;
            try
            {
                conexaoSql = new MySqlConnection(this.RecuperarStringDeConexao());
                comandoSql = new MySqlCommand(querySql.ToString(), conexaoSql);
                conexaoSql.Open();
                executou = comandoSql.ExecuteNonQuery() > 0;
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(String.Format(@"Excessao Mysql (ControladorConexaoMysql.cs - INSERT,DELETE,UPDATE): {0}", ex.Message));
            }
            finally
            {
                if (conexaoSql != null)
                    conexaoSql.Close();
                if (comandoSql != null)
                    comandoSql.Dispose();
            }
            return executou;
        }

        public DataSet BuscarDadosNaBase(StringBuilder querySql)
        {
            return RecuperarDataSet(querySql);
        }
        private DataSet RecuperarDataSet(StringBuilder querySql)
        {
            MySqlConnection conexaoSql = null;
            MySqlDataAdapter adaptadorSql = null;
            DataSet dsRetorno = new DataSet();
            try
            {
                conexaoSql = new MySqlConnection(this.RecuperarStringDeConexao());
                adaptadorSql = new MySqlDataAdapter(querySql.ToString(), conexaoSql);
                conexaoSql.Open();
                adaptadorSql.Fill(dsRetorno);
            }
            catch (MySqlException ex)
            {
                Console.WriteLine(String.Format(@"Excessao Mysql (ControladorConexaoMysql.cs - SELECT): {0}", ex.Message));
            }
            finally
            {
                if (conexaoSql != null)
                    conexaoSql.Close();
                if (adaptadorSql != null)
                    adaptadorSql.Dispose();
            }
            return dsRetorno;
        }

        public static List<Dictionary<string, object>> RecuperarRegistrosDataSet(DataSet ds)
        {            
            List<Dictionary<string, object>> registros = new List<Dictionary<string, object>>();
            foreach (DataRow dsLinha in ds.Tables[0].Rows)
            {
                Dictionary<string, object> colunas = new Dictionary<string, object>();
                foreach (DataColumn dsColuna in ds.Tables[0].Columns)
                {
                    colunas.Add(dsColuna.ColumnName, dsLinha[dsColuna]);
                }
                registros.Add(colunas);
            }
            return registros;
        }
    }
}
