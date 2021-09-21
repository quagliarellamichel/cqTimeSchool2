using System.Linq;

namespace cqTimeSchool {
    public class SProfessore {
        public SProfessore(string professore, string sigla) {
            Professore = professore;
            Sigla = sigla;
        }

        public string Professore { get; }
        public string Sigla { get; }

        public PClasse[] Classi { get; set; } = new PClasse[0];
        public WeekDays[] WeekFree { get; set; } = new WeekDays[0];

        public SWeek[] CalcOrario() {
            var wp = Program.GenerateWeekEmpty();
            for (int i = 0; i < wp.Length; i++)
                wp[i] = CalcOrarioWeek(wp[i].Week);           
            
            return wp;
        }

        public SWeek CalcOrarioWeek(WeekDays week) {
            var wp = new SWeek(week, Program.NDay);
            foreach (var c in Program.Classi)
                for (int l = 0; l < wp.Ore.Length; l++) {
                    if (c.GetWeek(wp.Week).Ore[l] != Sigla) continue;
                    wp.Ore[l] = c.Classe;
                }

            if (WeekFree.Contains(week))
                wp.Ore = Enumerable.Repeat("/", wp.Ore.Length).ToArray();

            return wp;
        }

        public PClasse GetClasse(SClasse classe) {
            return this.Classi.FirstOrDefault(x => x.Classe == classe.Classe);
        }

        public PClasse GetClasse(string classe) {
            return this.Classi.FirstOrDefault(x => x.Classe == classe);
        }

        public int CalcTotOF() {
            return this.Classi.Sum(x => x.OreFrontali);
        }

        public int CalcTotHDisp() {
            var of = this.Classi.Sum(x => x.OreFrontali);
            return of - this.CalcOrario()
                .Sum(h => h.Ore.Where(o => !string.IsNullOrEmpty(o) && o != "/").Count());
        }

        public int CalcOfMancantiClasse(string classe) {
            return this.CalcOfClasse(classe) 
                 - this.CalcOrario().Sum(h => h.Ore.Where(o => o == classe && o != "/").Count());
        }

        public int CalcOfClasse(string classe) {
            return Classi.Where(x => x.Classe == classe).Sum(x => x.OreFrontali);
        }

        public int CalcTotHDisp(WeekDays week) {
            return CalcOrario()
                .Where(x => x.Week == week)
                .Sum(h => h.Ore.Where(o => string.IsNullOrEmpty(o)).Count());
        }

        public int CalcTotOFToSetInClasse(SClasse classe) {
            if (classe == null) return 0;
            var of = GetClasse(classe).OreFrontali;
            return of - classe.Weeks.Sum(x => x.Ore.Count(o => o == Sigla && o != "/"));
        }

        public string Check() {
            //var totNh = Program.Classi.Sum(x => x.Weeks.Length * x.Weeks.First().Ore.Count());
            var totHWeek = Program.Classi.Max(x => x.Weeks.Length * x.Weeks.First().Ore.Count());
            var h = this.CalcTotOF();
            if (h > totHWeek) {
                var diff = totHWeek - h;
                return $"[{totHWeek} - {h} = {diff}] ";
            } else
                return "OK";
        }
    }

}