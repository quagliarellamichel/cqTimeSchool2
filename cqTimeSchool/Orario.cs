using System.Linq;

namespace cqTimeSchool {
    public class SWeek {

        public SWeek(WeekDays week, int nOre) {
            Week = week;
            Ore = new string[nOre];
        }

        public WeekDays Week { get; set; }
        public string[] Ore { get; set; }

        public bool IsHole() {
            return Ore.Count(x => string.IsNullOrEmpty(x)) > 0;
        }


        public bool Set(SProfessore p, int ofToSet) {
            var sigla = p.Sigla;
            var oreDisp = Ore.Count(x => string.IsNullOrEmpty(x)); // sono tutte le ore vuote

            if (ofToSet <= 0) return false;
            if (oreDisp <= 0) return false;

            var wp = p.CalcOrarioWeek(this.Week);
            if (wp == null) return false;

            for (int i = 0; i < Ore.Length; i++) {
                if (!string.IsNullOrEmpty(wp.Ore[i])) continue;
                if (!string.IsNullOrEmpty(Ore[i])) continue;
                Ore[i] = sigla;
                return true;
            }

            return false;
        }

        /*
        public string H1 => TryGetOre(0);
        public string H2 => TryGetOre(1);
        public string H3 => TryGetOre(2);
        public string H4 => TryGetOre(3);
        public string H5 => TryGetOre(4);
        public string H6 => TryGetOre(5);

        private string TryGetOre(int pos) {
            try {
                return Ore[pos];
            } catch {
                return "";
            }
        }*/
    }

}