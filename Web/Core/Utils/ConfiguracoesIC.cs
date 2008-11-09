using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Utils
{
    public abstract class ConfiguracoesIC
    {
        #region Nant
        public static string NAntPath
        {
            get { return ConfigurationManager.AppSettings["nantPath"]; }
        }

        public static string NAntBuildFilePath
        {
            get { return ConfigurationManager.AppSettings["nantBuildFilePath"]; }
        }
        #endregion

        #region Release
        public static string ReleasePath
        {
            get { return ConfigurationManager.AppSettings["releasePath"]; }
        }
        #endregion

        #region Version Control
        public static string XMLPath
        {
            get { return ConfigurationManager.AppSettings["XMLPath"]; }
        }

        public static string VersionValueDefault
        {
            get { return ConfigurationManager.AppSettings["versionValueDefault"]; }
        }
        #endregion
    }
}
