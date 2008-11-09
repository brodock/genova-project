using System;
using System.Collections.Generic;
using System.Text;
using Utils;

namespace Persistence.Configuracoes
{
    public sealed class ConfiguracoesSingleton
    {
        private string _servidor;
        private string _bancoDeDados;
        private string _usuario;
        private string _senha;

        private volatile static ConfiguracoesSingleton instance;
        private static object syncRoot = new Object();

        private ConfiguracoesSingleton()
        {
            this._servidor = ConfiguracoesWeb.Servidor;
            this._bancoDeDados = ConfiguracoesWeb.BancoDeDados;
            this._usuario = ConfiguracoesWeb.Usuario;
            this._senha = ConfiguracoesWeb.Senha;
        }

        public static ConfiguracoesSingleton Istance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ConfiguracoesSingleton();
                    }
                }
                return instance;
            }
        }

        public StringBuilder GetConfiguracoes()
        {
            StringBuilder retorno = new StringBuilder();
            retorno.AppendFormat("Server={0};", this._servidor);
            retorno.AppendFormat("Database={0};", this._bancoDeDados);
            retorno.AppendFormat("User ID={0};", this._usuario);
            retorno.AppendFormat("Password={0};", this._senha);
            return retorno;
        }
    }
}
