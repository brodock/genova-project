using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Utils;

namespace ContinousIntegration
{
    public class Program
    {
        private static Process NAntProcess = new Process();

        public static void Main(string[] args)
        {
            RunNAnt();

            /*Prevent Error*/
            while (!NAntProcess.HasExited) { }

            if (IsNotSuccess())
                RevertVersionValueGenerated();

            Console.ReadKey(true);
        }

        #region Methods

        private static void RunNAnt()
        {
            NAntProcess.StartInfo.FileName = GetNAntFilePath();
            NAntProcess.StartInfo.Arguments = GetNAntArguments();
            NAntProcess.StartInfo.UseShellExecute = false;
            NAntProcess.Start();
        }

        private static string GetNAntArguments()
        {
            return string.Format("-buildfile:{0} -D:Version={1}", GetBuildFilePath(), GetVersion());
        }

        private static bool IsNotSuccess()
        {
            VersionControl version = new VersionControl();
            string fileName = string.Format("{0}-{1}.zip", GetSolutionName(), version.GetCurrentValue);
            string newFileReleasePath = Path.Combine(ConfiguracoesIC.ReleasePath, fileName);
            return !File.Exists(newFileReleasePath);
        }

        private static void RevertVersionValueGenerated()
        {
            VersionControl version = new VersionControl();
            version.DecreaseVersionValue();
        }

        #region NAnt Arguments
        private static string GetVersion()
        {
            VersionControl version = new VersionControl();
            return version.GetValue;
        }
        #endregion

        #region Configs
        private static string GetSolutionName()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            AssemblyProductAttribute info = AssemblyProductAttribute.GetCustomAttribute(asm, typeof(AssemblyProductAttribute)) as AssemblyProductAttribute;
            return info.Product;
        }
        private static string GetBuildFilePath()
        {
            return ConfiguracoesIC.NAntBuildFilePath;
        }
        private static string GetNAntFilePath()
        {
            string path = ConfiguracoesIC.NAntPath;
            path = Path.Combine(path, "NAnt.exe");
            return path;
        }
        #endregion

        #endregion
    }
}
