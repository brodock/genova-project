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
using System.Data;
using System.Xml;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using GeNova.Core.Utilitarios.XML;

namespace GeNova.Core.Controladores
{
    public class ControladorConexaoMysql
    {
        public ControladorConexaoMysql() { }

        #region Atributos de Configuração
        private string _servidor;
        private string _bancoDeDados;
        private string _usuario;
        private string _senha;
        #endregion

        #region Setar Atributos de Configuração
        private void SetAtributosConfiguracao()
        {
            XmlNode node;
            UtilitariosXML xmlUtil = new UtilitariosXML(CaminhosXML.FilePath_Configs_Mysql);
            
            // Set string: _servidor
            node = xmlUtil.GetSingleNode("server");
            this._servidor = xmlUtil.GetAttributeValue(node);

            // Set string: _bancoDeDados
            node = xmlUtil.GetSingleNode("dataBase");
            this._bancoDeDados = xmlUtil.GetAttributeValue(node);

            // Set string: _usuario
            node = xmlUtil.GetSingleNode("userid");
            this._usuario = xmlUtil.GetAttributeValue(node);

            // Set string: _senha
            node = xmlUtil.GetSingleNode("password");
            this._senha = xmlUtil.GetAttributeValue(node);
        }
        #endregion

        public string RecuperarStringDeConexao()
        {
            // set attributes.
            this.SetAtributosConfiguracao();

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
