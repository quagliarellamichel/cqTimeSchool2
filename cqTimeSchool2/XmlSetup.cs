using cqTimeSchool2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace XmlConfigCqTimeSchool2 {

    [XmlRoot(ElementName = "Classe")]
    public class Classe {
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "OreFrontali")]
        public int OreFrontali { get; set; }
    }

    [XmlRoot(ElementName = "Professore")]
    public class Professore {

        [XmlElement(ElementName = "Classe")]
        public List<Classe> Classi { get; set; }

        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "Sigla")]
        public string Sigla { get; set; }
    }

    [XmlRoot(ElementName = "XML")]
    public class XML {

        public static XML ReadXML(string path) {
            XmlSerializer reader = new XmlSerializer(typeof(XML));
            var file = new StreamReader(path);
            XML r = (XML)reader.Deserialize(file);
            file.Close();
            return r;
        }

        public static WeekDays[] ParseStringToWeeks(string weeks) {
            weeks = weeks.Replace(" ", "");
            var tmp = weeks.Split(',');
            var res = new List<WeekDays>();

            foreach (var t in tmp) {
                //Console.WriteLine($"{t}");
                try {
                    res.Add((WeekDays)Enum.Parse(typeof(WeekDays), t));
                } catch { }
            }
            return res.ToArray();
        }

        public static SProfessore[] ParseProfessori(List<Professore> xmlProfessori) {
            var res = new List<SProfessore>();
            foreach (var p in xmlProfessori) {
                var tmpp = new SProfessore(p.Name, p.Sigla) {
                    Classi = p.Classi.Select(x => new SClasse(x.Name, x.OreFrontali)).ToArray(),
                };
                res.Add(tmpp);
            }
            return res.ToArray();
        }

        [XmlElement(ElementName = "Professore")]
        public List<Professore> Professori { get; set; }

        [XmlAttribute(AttributeName = "DayInWeek")]
        public int DayInWeek { get; set; }

        [XmlAttribute(AttributeName = "SetupWeeks")]
        public string SetupWeeks { get; set; }
    }

}


