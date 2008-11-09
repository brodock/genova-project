using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Utils
{
    public abstract class ConfiguracoesWeb
    {
        #region Configurações Banco de Dados
        public static string Servidor
        {
            get { return ConfigurationManager.AppSettings["Servidor"]; }
        }

        public static string BancoDeDados
        {
            get { return ConfigurationManager.AppSettings["BancoDeDados"]; }
        }

        public static string Usuario
        {
            get { return ConfigurationManager.AppSettings["Usuario"]; }
        }

        public static string Senha
        {
            get { return ConfigurationManager.AppSettings["Senha"]; }
        }
        #endregion

        #region Configurações XML
        public static string WebPathXml
        {
            get { return ConfigurationManager.AppSettings["WebPathXml"]; }
        }

        public static string WebPathXmlConfiguracoes
        {
            get { return ConfigurationManager.AppSettings["WebPathXmlConfiguracoes"]; }
        }
        #endregion

        #region Outras Configurações
        public static string PathSimpleVelocity
        {
            get { return ConfigurationManager.AppSettings["PathSimpleVelocity"]; }
        }

        public static string PathAvatars
        {
            get { return ConfigurationManager.AppSettings["PathAvatars"]; }
        }

        public static string WebPath
        {
            get { return ConfigurationManager.AppSettings["WebPath"]; }
        }

        public static string WebPathAvatars
        {
            get { return ConfigurationManager.AppSettings["WebPathAvatars"]; }
        }

        public static string ExibicaoPadrao
        {
            get { return ConfigurationManager.AppSettings["ExibicaoPadrao"]; }
        }

        public static string VirtualPathUploadAvatars
        {
            get { return ConfigurationManager.AppSettings["VirtualPathUploadAvatars"]; }
        }
        #endregion

        #region Outras Configurações
        
        #endregion
    }
}
