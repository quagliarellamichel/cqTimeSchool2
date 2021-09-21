using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTables;

namespace cqTimeSchool {
    public class Program {

        //configurazione
        public static int NDay { get; set; } = 6;
        //se vuoi configurare le settimane vai al file WeekDay ed aggiungi l'enum
        //configurazione

        public static List<SClasse> Classi { get; set; } = new List<SClasse>();
        public static List<SProfessore> Professori { get; set; } = new List<SProfessore>();

        public static void Main(string[] args) {
            Console.WriteLine("cqTimeSchool");

            /* 
             * inizializza professori
             */

            Professori.Add(
                new SProfessore("SCHEMBARI LUCIA", "SL") {
                    /*
                    WeekFree = new[]{
                        WeekDays.LU
                    },*/
                    Classi = new PClasse[]{
                        new PClasse("1A", 20),                        
                    },                    
                });

            Professori.Add(
                new SProfessore("OCCHIPINTI GIUSEPPA", "OG") {                    
                    Classi = new PClasse[]{
                        new PClasse("1B", 20),
                    },
                });

            Professori.Add(
                new SProfessore("FERRARO SALVATRICE", "FS") {
                    Classi = new PClasse[]{
                        new PClasse("1C", 20),
                    },
                });

            Professori.Add(
                new SProfessore("INGALLINESI BATTISTINA", "IB") {
                    Classi = new PClasse[]{
                        new PClasse("1D", 20),
                    },
                });

            Professori.Add(
                new SProfessore("ZISA GIUSEPPINA VALERIA", "ZG") {
                    Classi = new PClasse[]{
                        new PClasse("1E", 20),
                    },
                });

            Professori.Add(
                new SProfessore("GUCCIONE DANILA", "GD") {
                    Classi = new PClasse[]{
                        new PClasse("1A", 8),
                        new PClasse("1B", 6),
                        new PClasse("1C", 6),
                    },
                });

            Professori.Add(
                new SProfessore("FERRARO ANTONELLA", "FA") {
                    Classi = new PClasse[]{                        
                        new PClasse("1B", 2),
                        new PClasse("1C", 2),
                        new PClasse("1D", 8),
                        new PClasse("1E", 8),
                    },
                });

            Professori.Add(
                new SProfessore("BOCCHIERI CARMELO", "R1") {
                    Classi = new PClasse[]{                        
                        new PClasse("1B", 2),
                    },
                });

            Professori.Add(
                new SProfessore("GIRLANDO GIOVANNA", "R2") {
                    Classi = new PClasse[]{
                        new PClasse("1A", 2),
                        new PClasse("1E", 2),
                    },
                });

            Professori.Add(
                new SProfessore("GIRLANDO GIOVANNA", "R3") {
                    Classi = new PClasse[]{
                        new PClasse("1C", 2),
                        new PClasse("1D", 2),
                    },
                });


            /* 
             * inizializza classi
             

            var a = new SClasse("5A");
            var w = a.Weeks.FirstOrDefault(x => x.Week == WeekDays.VE);
            w.Ore[2] = "PI";
            w.Ore[4] = "FE";
            w.Ore[5] = "FE";
            Classi.Add(a);
            */

            Classi.Add(new SClasse("1A"));
            Classi.Add(new SClasse("1B"));
            Classi.Add(new SClasse("1C"));
            Classi.Add(new SClasse("1D"));
            Classi.Add(new SClasse("1E"));

            //fai il calcolo dell'assegnazione
            foreach (var p in Professori) p.Check();
            foreach (var c in Classi) c.Calcola();
            
            ConsoleWriteLineClasse();
            ConsoleWriteLineProfessori();

            /*
            var pOut = Professori.Select(p => new {
                SIGLA = p.Professore,
                H = p.CalcTotHDisp()
            }).Where(x => x.H > 0).ToArray();

            if (pOut.Count() > 0) {
                var oreOut = pOut.Sum(x => x.H);
                
                Console.WriteLine($"ERRORE NELLA ASSEGNAZIONE DELLE ORE");
                foreach (var o in pOut)
                    Console.WriteLine($"[{o.H} ore] {o.SIGLA}");
            }
            */
            Console.ReadLine();
        }

        public static SWeek[] GenerateWeekEmpty() {
            return new SWeek[] {
                new SWeek(WeekDays.LU, NDay),
                new SWeek(WeekDays.MA, NDay),
                new SWeek(WeekDays.ME, NDay),
                new SWeek(WeekDays.GI, NDay),
                new SWeek(WeekDays.VE, NDay),
            };
        }

        public static double TryDiv(double a, double b) {
            try {
                return a / b;
            } catch {
                return 0;
            }
        }

        private static void ConsoleWriteLineProfessori() {
            ConsoleTable
                .From(Professori.Select(p => {
                    var h = p.CalcOrario();

                    var lu = h.FirstOrDefault(z => z.Week == WeekDays.LU);
                    var ma = h.FirstOrDefault(z => z.Week == WeekDays.MA);
                    var me = h.FirstOrDefault(z => z.Week == WeekDays.ME);
                    var gi = h.FirstOrDefault(z => z.Week == WeekDays.GI);
                    var ve = h.FirstOrDefault(z => z.Week == WeekDays.VE);

                    var arrW = new[] { lu, ma, me, gi, ve }.Where(x => p.WeekFree.Contains(x.Week)).ToArray();
                    foreach (var w in arrW)
                        w.Ore = Enumerable.Repeat("/", w.Ore.Length).ToArray();
                    
                    var classi = "";
                    foreach (var c in p.Classi) {                        
                        //ore mancanti in classe
                        var om = p.CalcOfMancantiClasse(c.Classe)*-1;
                        if (om == 0) continue;
                        if (!string.IsNullOrEmpty(classi)) classi += " ";
                        classi += $"[{c.Classe}] {c.OreFrontali}|{om}";
                    }

                    if (string.IsNullOrEmpty(classi)) classi = "OK";
                    return new {
                        PROF = $"[{p.Sigla}] {p.Professore}",                        
                        CHECK = p.Check(),
                        CLASSI = classi,

                        LU1 = lu.Ore[0],
                        LU2 = lu.Ore[1],
                        LU3 = lu.Ore[2],
                        LU4 = lu.Ore[3],
                        LU5 = lu.Ore[4],
                        LU6 = lu.Ore[5],

                        MA1 = ma.Ore[0],
                        MA2 = ma.Ore[1],
                        MA3 = ma.Ore[2],
                        MA4 = ma.Ore[3],
                        MA5 = ma.Ore[4],
                        MA6 = ma.Ore[5],

                        ME1 = me.Ore[0],
                        ME2 = me.Ore[1],
                        ME3 = me.Ore[2],
                        ME4 = me.Ore[3],
                        ME5 = me.Ore[4],
                        ME6 = me.Ore[5],

                        GI1 = gi.Ore[0],
                        GI2 = gi.Ore[1],
                        GI3 = gi.Ore[2],
                        GI4 = gi.Ore[3],
                        GI5 = gi.Ore[4],
                        GI6 = gi.Ore[5],

                        VE1 = ve.Ore[0],
                        VE2 = ve.Ore[1],
                        VE3 = ve.Ore[2],
                        VE4 = ve.Ore[3],
                        VE5 = ve.Ore[4],
                        VE6 = ve.Ore[5]
                    };
                }))
                .Configure(o => {
                    o.NumberAlignment = Alignment.Right;
                    o.EnableCount = true;
                })
                .Write(Format.Alternative);
        }

        private static void ConsoleWriteLineClasse() {
            ConsoleTable
                .From(Classi.Select(x => {
                    var h = x.Weeks;

                    var lu = h.FirstOrDefault(z => z.Week == WeekDays.LU);
                    var ma = h.FirstOrDefault(z => z.Week == WeekDays.MA);
                    var me = h.FirstOrDefault(z => z.Week == WeekDays.ME);
                    var gi = h.FirstOrDefault(z => z.Week == WeekDays.GI);
                    var ve = h.FirstOrDefault(z => z.Week == WeekDays.VE);

                    var of = $"{x.TotOFPorfClasse}/{x.NOre}";
                    if (x.PrecOF != 1)
                        of += $" [{x.PrecOF:P0}]";

                    
                    var complete = "";
                    if (x.PrecComp != 1) {
                        var diffComp = x.GetOreFull() - x.NOre;
                        complete = $"{diffComp} [{x.PrecComp:P0}]";
                    }

                    if (string.IsNullOrEmpty(complete))
                        complete = "OK";

                    return new {
                        CLASSE = x.Classe,
                        COMPL = complete,

                        LU1 = lu.Ore[0],
                        LU2 = lu.Ore[1],
                        LU3 = lu.Ore[2],
                        LU4 = lu.Ore[3],
                        LU5 = lu.Ore[4],
                        LU6 = lu.Ore[5],

                        MA1 = ma.Ore[0],
                        MA2 = ma.Ore[1],
                        MA3 = ma.Ore[2],
                        MA4 = ma.Ore[3],
                        MA5 = ma.Ore[4],
                        MA6 = ma.Ore[5],

                        ME1 = me.Ore[0],
                        ME2 = me.Ore[1],
                        ME3 = me.Ore[2],
                        ME4 = me.Ore[3],
                        ME5 = me.Ore[4],
                        ME6 = me.Ore[5],

                        GI1 = gi.Ore[0],
                        GI2 = gi.Ore[1],
                        GI3 = gi.Ore[2],
                        GI4 = gi.Ore[3],
                        GI5 = gi.Ore[4],
                        GI6 = gi.Ore[5],

                        VE1 = ve.Ore[0],
                        VE2 = ve.Ore[1],
                        VE3 = ve.Ore[2],
                        VE4 = ve.Ore[3],
                        VE5 = ve.Ore[4],
                        VE6 = ve.Ore[5],

                        N_PROF = x.NProf,
                        OF = $"{of} {x.ErrorProf}",
                    };
                }))
                .Configure(o => o.NumberAlignment = Alignment.Right)
                .Write(Format.Alternative);
        }

    }

}