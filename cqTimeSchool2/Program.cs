using Connect;
using System;
using System.IO;
using System.Linq;
using XmlConfigCqTimeSchool2;

namespace cqTimeSchool2 {
    public class Program {


        public static void Main(string[] args) {
            Console.WriteLine("cqTimeSchool v2.0");

            var files = Directory.GetFiles("./", "*.xml");
            if (files.Length == 0) {
                Console.WriteLine("nessun file da analizzare");
                return;
            }

            retry:
            Console.WriteLine("seleziona il file da analizzare");            
            files
                .Select((file, index) => new { file, index})
                .ToList()
                .ForEach(x => {
                    Console.WriteLine($"[{x.index}] => {x.file}");
                });

            Console.Write("-> ");
            var pos = Conv.ParseIntNull(Console.ReadLine(), null, 0, files.Length-1);
            Console.WriteLine();
            if (pos == null) goto retry;

            
            var xml = XML.ReadXML(files[(int)pos]);
            var c = new Calcolo(
                xml.DayInWeek
                , XML.ParseStringToWeeks(xml.SetupWeeks)
                , XML.ParseProfessori(xml.Professori)
                );

            c.Start(files[(int)pos]);
            Console.WriteLine("press enter to close");
            Console.ReadLine();
        }

    }
}