using System;
using System.IO;
using System.Xml;
using System.Collections;

using Server;
using Server.Scripts.Commands;
using Server.Commands.Generic;
using Server.Commands;
using System.Collections.Generic;

namespace Server.Mobiles
{
    public class SpawnerExporter
    {
        public static void Initialize()
        {
            TargetCommands.Register(new ExportSpawnerCommand());
            CommandSystem.Register("ImportSpawners", AccessLevel.Administrator, new CommandEventHandler(ImportSpawners_OnCommand));
        }

        public class ExportSpawnerCommand : BaseCommand
        {
            public ExportSpawnerCommand()
            {
                AccessLevel = AccessLevel.Administrator;
                Supports = CommandSupport.Area | CommandSupport.Region | CommandSupport.Global | CommandSupport.Multi | CommandSupport.Single;
                Commands = new string[] { "ExportSpawner" };
                ObjectTypes = ObjectTypes.Items;
                Usage = "ExportSpawner <filename>";
                Description = "Exports all Spawner objects to the specified filename.";
                ListOptimized = true;
            }

            public override void ExecuteList(CommandEventArgs e, ArrayList list)
            {
                string filename = e.GetString(0);

                ArrayList spawners = new ArrayList();

                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i] is ODBCSpawner)
                    {
                        ODBCSpawner spawner = (ODBCSpawner)list[i];
                        if (spawner != null && !spawner.Deleted && spawner.Map != Map.Internal && spawner.Parent == null)
                            spawners.Add(spawner);
                    }
                }

                AddResponse(String.Format("{0} spawners exported to Data/Spawners/{1}.", spawners.Count.ToString(), filename));

                ExportSpawners(spawners, filename);
            }

            public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
            {
                if (e.Arguments.Length >= 1)
                    return true;

                e.Mobile.SendMessage("Usage: " + Usage);
                return false;
            }

            private void ExportSpawners(ArrayList spawners, string filename)
            {
                if (spawners.Count == 0)
                    return;

                if (!Directory.Exists("Data/Spawners"))
                    Directory.CreateDirectory("Data/Spawners");

                string filePath = Path.Combine("Data/Spawners", filename);

                using (StreamWriter op = new StreamWriter(filePath))
                {
                    XmlTextWriter xml = new XmlTextWriter(op);

                    xml.Formatting = Formatting.Indented;
                    xml.IndentChar = '\t';
                    xml.Indentation = 1;

                    xml.WriteStartDocument(true);

                    xml.WriteStartElement("spawners");

                    xml.WriteAttributeString("count", spawners.Count.ToString());

                    foreach (ODBCSpawner spawner in spawners)
                        ExportSpawner(spawner, xml);

                    xml.WriteEndElement();

                    xml.Close();
                }
            }

            private void ExportSpawner(ODBCSpawner spawner, XmlTextWriter xml)
            {
                xml.WriteStartElement("spawner");

                xml.WriteStartElement("count");
                xml.WriteString(spawner.Count.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("group");
                xml.WriteString(spawner.Group.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("homerange");
                xml.WriteString(spawner.HomeRange.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("maxdelay");
                xml.WriteString(spawner.MaxDelay.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("mindelay");
                xml.WriteString(spawner.MinDelay.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("team");
                xml.WriteString(spawner.Team.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("creaturesname");
                foreach (string creatureName in spawner.CreaturesName)
                {
                    xml.WriteStartElement("creaturename");
                    xml.WriteString(creatureName);
                    xml.WriteEndElement();
                }
                xml.WriteEndElement();

                // Item properties

                xml.WriteStartElement("name");
                xml.WriteString(spawner.Name);
                xml.WriteEndElement();

                xml.WriteStartElement("location");
                xml.WriteString(spawner.Location.ToString());
                xml.WriteEndElement();

                xml.WriteStartElement("map");
                xml.WriteString(spawner.Map.ToString());
                xml.WriteEndElement();

                xml.WriteEndElement();
            }
        }

        [Usage("ImportSpawners")]
        [Description("Recreates Spawner items from the specified file.")]
        public static void ImportSpawners_OnCommand(CommandEventArgs e)
        {
            if (e.Arguments.Length >= 2)
            {
                string filename = e.GetString(0);
                string mapa = e.GetString(1);
                string tipo = e.GetString(2);

                string filePath = Path.Combine("Data/Spawners", filename);

                if (File.Exists(filePath))
                {
                    XmlDocument doc = new XmlDocument();
                    doc.Load(filePath);

                    XmlElement root = doc["spawners"];

                    int successes = 0, failures = 0;

                    foreach (XmlElement spawner in root.GetElementsByTagName("spawner"))
                    {
                        try
                        {
                            ImportSpawner(spawner, mapa, tipo);
                            successes++;
                        }
                        catch { failures++; }
                    }

                    e.Mobile.SendMessage("{0} spawners loaded successfully from {1}, {2} failures.", successes, filePath, failures);
                }
                else
                {
                    e.Mobile.SendMessage("File {0} does not exist.", filePath);
                }
            }
            else
            {
                e.Mobile.SendMessage("Usage: [ImportSpawners <filename> <map> <odbc/normal>");
            }
        }

        private static string GetText(XmlElement node, string defaultValue)
        {
            if (node == null)
                return defaultValue;

            return node.InnerText;
        }

        private static void ImportSpawner(XmlElement node, string mapa, string tipo)
        {
            int count = int.Parse(GetText(node["count"], "1"));
            int homeRange = int.Parse(GetText(node["homerange"], "4"));
            int team = int.Parse(GetText(node["team"], "0"));

            bool group = bool.Parse(GetText(node["group"], "False"));
            TimeSpan maxDelay = TimeSpan.Parse(GetText(node["maxdelay"], "10:00"));
            TimeSpan minDelay = TimeSpan.Parse(GetText(node["mindelay"], "05:00"));
            ArrayList creaturesName = LoadCreaturesName(node["creaturesname"]);

            string name = GetText(node["name"], "Spawner");
            Point3D location = Point3D.Parse(GetText(node["location"], "Error"));
            Map map = Map.Parse(GetText(node["map"], "Error"));

            if (map.Name == mapa)
            {

                if (tipo == "odbc")
                {

                    ODBCSpawner spawner = new ODBCSpawner(count, minDelay, maxDelay, team, homeRange, creaturesName);
                    spawner.Name = name;
                    spawner.MoveToWorld(location, map);
                    if (spawner.Map == Map.Internal)
                    {
                        spawner.Delete();
                        throw new Exception("Spawner created on Internal map.");
                    }
                    spawner.Respawn();
                }
                else
                {
                    // Genova: necessário converter creaturesName para list<string>.
                    List<string> listaCreaturesName = new List<string>();
                    listaCreaturesName.CopyTo(creaturesName.ToArray(typeof(string)) as string[]);
                    Spawner spawner = new Spawner(count, minDelay, maxDelay, team, homeRange, listaCreaturesName);
                    spawner.Name = name;
                    spawner.MoveToWorld(location, map);
                    if (spawner.Map == Map.Internal)
                    {
                        spawner.Delete();
                        throw new Exception("Spawner created on Internal map.");
                    }
                    spawner.Respawn();
                }
            }
        }

        private static ArrayList LoadCreaturesName(XmlElement node)
        {
            ArrayList names = new ArrayList();

            if (node != null)
            {
                foreach (XmlElement ele in node.GetElementsByTagName("creaturename"))
                {
                    if (ele != null)
                        names.Add(ele.InnerText);
                }
            }

            return names;
        }
    }
}
