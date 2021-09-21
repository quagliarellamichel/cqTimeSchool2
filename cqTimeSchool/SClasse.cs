using System;
using System.Collections.Generic;
using System.Linq;

namespace cqTimeSchool {
    public class SClasse {

        public string Classe { get; set; }
        public SWeek[] Weeks { get; set; } = Program.GenerateWeekEmpty();

        public int NProf { get; set; } = 0;
        public int TotOFPorfClasse { get; set; } = 0;
        public int NOre { get; set; } = 0;
        public double PrecOF => Program.TryDiv(TotOFPorfClasse, NOre);
        public double PrecComp => Program.TryDiv(GetOreFull(), NOre);
        public string ErrorProf { get; set; } = "";

        public SClasse(string classe) {
            Classe = classe;
            _professori = Program
                .Professori
                .Where(p => p.Classi.Any(c => c.Classe == this.Classe))
                .OrderByDescending(x => x.WeekFree.Length)
                .OrderByDescending(x => x.GetClasse(this).OreFrontali)
                .ToArray();
        }

        private SProfessore[] _professori { get; set; }
        public void Calcola() {
            for (int i = 0; i < _professori.Length; i++) {
                if (CheckOreComplete()) break;

                foreach (var p in _professori)
                    CalcProfessore(p);

                Console.WriteLine(RubaOra());
            }

            NProf = _professori.Count();
            TotOFPorfClasse = _professori.Sum(p => p.Classi.Where(c => c.Classe == this.Classe).Sum(c => c.OreFrontali));
            NOre = Weeks.Count() * (Weeks.FirstOrDefault()?.Ore.Length ?? 0);

            if (TotOFPorfClasse == NOre) ErrorProf = "OK";
            if (TotOFPorfClasse > NOre) ErrorProf = "TROPPI";
            if (TotOFPorfClasse < NOre) ErrorProf = "POCHI";
        }

        private void CalcProfessore(SProfessore p, int recurse = 0) {
            if (p.CalcOfMancantiClasse(this.Classe) == 0) return;

            foreach (var w in Weeks)
                CalcProfessoreWeek(p, w);

            if (p.CalcTotOFToSetInClasse(this) > 0) {
                /*(Weeks.Length * Weeks.First().Ore.Length)*/
                if (recurse > p.GetClasse(this.Classe).OreFrontali) return;
                CalcProfessore(p, ++recurse);
            }
        }

        private void CalcProfessoreWeek(SProfessore p, SWeek w) {
            //ci sono ancora ore da impostare?
            if (!w.Ore.Contains(null)) return;

            //ore frontali da impostare
            var ofToSet = p.CalcTotOFToSetInClasse(this);
            // il break c'è perchè la persona non deve più fare ore frontali quindi è inutile iterare nella settimana
            if (ofToSet <= 0) return;

            var res = w.Set(p, ofToSet);
        }


        private int[] GetIndexHole(string[] ore) {
            var tmp = new List<int>();
            for (int i = 0; i < ore.Length; i++) {
                if (ore[i] != null) continue;
                tmp.Add(i);
            }
            return tmp.ToArray();
        }

        private class IP {
            public IP(string prof, int index) {
                Prof = prof;
                Index = index;
            }

            /// <summary>
            /// professore
            /// </summary>
            public string Prof { get; set; }

            /// <summary>
            /// index
            /// </summary>
            public int Index { get; set; }
        }

        private IP[] GetFullIndexProf(string[] ore) {
            var tmp = new List<IP>();
            for (int i = 0; i < ore.Length; i++) {
                if (ore[i] == null) continue;
                tmp.Add(new IP(ore[i], i));
            }
            return tmp.ToArray();
        }


        private bool RubaOra() {
            Console.Write("Ruba Ora ");
            if (CheckOreComplete()) return false;

            var wsHole = this.Weeks.Where(x => x.Ore.Contains(null)).ToArray();
            foreach (var wHole in wsHole) {
                var indexsHole = GetIndexHole(wHole.Ore);
                foreach (var index in indexsHole) {
                    var g = GetFullIndexProf(wHole.Ore)
                        .GroupBy(x => x.Prof)
                        .Select(x => new {
                            Prof = x.First().Prof,
                            Count = x.Count(),
                            LastIndex = x.Last().Index
                        })
                        .OrderBy(x => x.Count)
                        .LastOrDefault();
                    if (g == null) continue;
                    wHole.Ore[g.LastIndex] = null;
                    SProfessore[] psTo = _professori.Where(x => x.Professore != g.Prof && x.CalcOfMancantiClasse(this.Classe) > 0).ToArray();
                    foreach (var pTo in psTo) {
                        if (wHole.Ore[g.LastIndex] != null) continue;
                        CalcProfessoreWeek(pTo, wHole);
                    }                    
                }
            }




            /*
            
            if (pTo == null) return false;

            var wHole = this.Weeks.FirstOrDefault(x =>  x.Ore.Contains(null));
            if (wHole == null) return false;

            var indexHole = wHole.Ore
                .Select((x, pos) => new { x, pos })
                .Where(x => x.x == null)
                .Select(x => x.pos)
                .ToArray()
                .First();

            var indexFull = wHole.Ore
                .Select((x, pos) => new { x, pos })
                .Where(x => x.x != null && x.x != "/"  && x.x != pTo.Sigla)
                .Select(x => x.pos)
                .ToArray()
                .Last();

            //Array.ForEach<int>(indexHole, Console.WriteLine);
            //var check = wHole.Ore[indexHole];

            var siglaFrom = wHole.Ore[indexFull];
            Console.Write($" {siglaFrom} {indexFull} {wHole.Week} {this.Classe}");

            var pFrom = _professori.First(x => x.Sigla == siglaFrom);
            var classFrom = pFrom.CalcOrarioWeek(wHole.Week).Ore[indexHole];

            this.Weeks.First(x => x.Week == wHole.Week).Ore[indexFull] = null;
            
            if (classFrom != null) {
                Program.Classi.First(x => x.Classe == classFrom).Weeks.First(x => x.Week == wHole.Week).Ore[indexHole] = null;
                Program.Classi.First(x => x.Classe == classFrom).Weeks.First(x => x.Week == wHole.Week).Ore[indexFull] = null;
            }
            

            //wHole.Ore[indexHole] = siglaFrom;
            //wHole.Ore[indexFull] = pTo.Sigla;            
            CalcProfessore(pTo);
            CalcProfessore(pFrom);
            */
            /*
            Array.ForEach(wHole.Ore, Console.WriteLine);
            Console.WriteLine($"{Classe} {indexHole} {wHole.Week} {pTo.Sigla} '{check}'");
            Console.ReadLine();
            */

            /*
            var oFrom = psFrom
                .Where(p => p.CalcOrario().Count(o => o.Week == wHole.Week && o.Ore.Contains(this.Classe)) > 0)
                //.Where(x => x.Week == wHole.Week && x.Ore.Contains(this.Classe))
                .LastOrDefault();
            if (oFrom == null) return false;
            */
            //debug
            //Array.ForEach(oFrom.Ore, Console.WriteLine);


            return true;
        }



        /// <summary>
        /// cerca buchi
        /// </summary>
        /// <returns></returns>
        private List<KVP> GetHoles() {
            SWeek[] weeksHole = this.Weeks.Where(x => x.IsHole()).ToArray();
            var holes = new List<KVP>();
            foreach (var wh in weeksHole)
                for (int i = 0; i < wh.Ore.Length; i++) {
                    if (!string.IsNullOrEmpty(wh.Ore[i])) continue;
                    //adesso la variabile i è l'indice da riempire con il professore disponibile
                    holes.Add(new KVP() {
                        Hole = wh,
                        Index = i
                    });
                }
            return holes;
        }

        public class KVP {
            public SWeek Hole { get; set; }
            public int Index { get; set; }
        }

        public int GetOreFull() {
            return GetOre().Count(o => !string.IsNullOrEmpty(o));
        }

        public int GetOreEmpty() {
            return GetOre().Count(o => string.IsNullOrEmpty(o));
        }

        public bool CheckOreComplete() {
            var nEmpty = Weeks.Count(x => x.Ore.Contains(null));
            return nEmpty == 0;
        }

        public SWeek GetWeek(WeekDays w) {
            return Weeks.FirstOrDefault(x => x.Week == w);
        }

        public string[] GetOre() {
            var tmp = new List<string>();
            foreach (var w in Weeks)
                tmp.AddRange(w.Ore);
            return tmp.ToArray();
        }

        public string[] GetOre(WeekDays week) {
            var tmp = new List<string>();
            foreach (var w in Weeks.Where(x => x.Week == week))
                tmp.AddRange(w.Ore);
            return tmp.ToArray();
        }
    }

}