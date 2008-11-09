using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Utils;

namespace ContinousIntegration
{
    public class VersionControl
    {
        private string _fileName = "VersionControl.xml";

        #region Enum
        private enum EnumVersionControlOperation
        {
            Increase = 1,
            Decrease = 2
        }
        #endregion

        #region Properties
        /// <summary>
        /// Recover your current build value version
        /// </summary>
        public string GetCurrentValue
        {
            get
            {
                string caminhoXML = this.GetFileVersionControl();
                XDocument xml = XDocument.Load(caminhoXML);

                var versions = from item in xml.Descendants("version")
                               where item.Attribute("active").Value == "true"
                               select new
                               {
                                   Current = item.Attribute("build")
                               };

                foreach (var version in versions)
                    return version.Current.Value;

                return ConfiguracoesIC.VersionValueDefault;
            }
        }

        /// <summary>
        /// Retrieves the new build value version
        /// </summary>
        public string GetValue
        {
            get { return this.GetCalculatedValue(EnumVersionControlOperation.Increase); }
        }
        #endregion

        #region Methods
        
        public void DecreaseVersionValue()
        {
            this.GetCalculatedValue(EnumVersionControlOperation.Decrease);
        }

        #region Intern
        private string GetCalculatedValue(EnumVersionControlOperation operation)
        {
            string build = string.Empty;

            string caminhoXML = this.GetFileVersionControl();
            XElement xml = XElement.Load(caminhoXML, LoadOptions.SetBaseUri | LoadOptions.SetLineInfo);
            IEnumerable<XElement> elements = xml.Elements();
            foreach (var item in elements.Where(e => e.Attribute("active").Value == "true"))
            {
                string attBuild = item.Attribute("build").Value;
                int version_4 = Conversoes.Inteiro32(attBuild.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[3]); // init 0

                switch (operation)
                {
                    case EnumVersionControlOperation.Increase:
                        {
                            version_4++;
                            break;
                        }
                    case EnumVersionControlOperation.Decrease:
                        {
                            if (version_4 > 0) 
                                version_4--;
                            
                            break;
                        }
                }

                string version_1_2 = attBuild.Substring(0, attBuild.LastIndexOf("."));
                build = string.Format("{0}.{1}", version_1_2, version_4);

                item.Attribute("build").Value = build;
            }
            xml.Save(this.GetFileVersionControl());

            return string.IsNullOrEmpty(build) ? ConfiguracoesIC.VersionValueDefault : build;
        }

        private string GetFileVersionControl()
        {
            string path = ConfiguracoesIC.XMLPath;
            path = Path.Combine(path, _fileName);
            return path;
        }
        #endregion

        #endregion
    }
}
